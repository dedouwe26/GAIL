using GAIL.Core;
using GAIL.Window;
using OxDED.Terminal.Logging;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace GAIL.Graphics.Renderer.Vulkan
{
    public class SwapChain : IDisposable {
        public struct SupportDetails {
            public SurfaceCapabilitiesKHR Capabilities;
            public SurfaceFormatKHR[] Formats;
            public PresentModeKHR[] PresentModes;
        }
        /// <summary>
        /// If this class is already disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }
        public bool AreFramebuffersDisposed { get; private set; }
        public KhrSwapchain extension;
        public SwapchainKHR swapchain;
        public Image[] images;
        public ImageView[] imageViews { get; private set; }
        public Framebuffer[]? frameBuffers { get; private set; }
        public Format imageFormat { get; private set; }
        public Extent2D extent { get; private set; }
        private readonly Surface surface;
        private readonly WindowManager window;
        private readonly Device device;
        private readonly Logger Logger;

        public SwapChain(VulkanRenderer renderer, WindowManager windowManager) {
            Logger = renderer.Logger;
            surface = renderer.surface;
            device = renderer.device;
            window = windowManager;

            CreateSwapChain(renderer.instance!);

            imageViews = CreateImageViews();
        }

        public uint? AcquireNextImage(VulkanRenderer renderer) {
            uint index = default;
            Result result = extension.AcquireNextImage(device.logicalDevice, swapchain, ulong.MaxValue, renderer.Syncronization.imageAvailable[renderer.CurrentFrame], default, ref index);
            
            if (result == Result.ErrorOutOfDateKhr) {
                return null;
            } else if (result != Result.SuboptimalKhr) {
                _ = Utils.Check(result, Logger, "Failed to acquire next image", true);
            }
            
            return index;
        }

        public ImageView[] CreateImageViews() {
            Logger.LogDebug("Creating Vulkan image views.");
            imageViews = new ImageView[images.Length];

            for (int i = 0; i < imageViews.Length; i++) {
                ImageViewCreateInfo createInfo = new() {
                    SType = StructureType.ImageViewCreateInfo,
                    Image = images[i],
                    ViewType = ImageViewType.Type2D,
                    Format = imageFormat,
                    Components = {
                        R = ComponentSwizzle.Identity,
                        G = ComponentSwizzle.Identity,
                        B = ComponentSwizzle.Identity,
                        A = ComponentSwizzle.Identity,
                    },
                    SubresourceRange = {
                        AspectMask = ImageAspectFlags.ColorBit,
                        BaseMipLevel = 0,
                        LevelCount = 1,
                        BaseArrayLayer = 0,
                        LayerCount = 1,
                    }
                };

                unsafe {
                    _ = Utils.Check(API.Vk.CreateImageView(device.logicalDevice, createInfo, Allocator.allocatorPtr, out imageViews[i]), Logger, "Failed to create image view", true);
                }
            }
            return imageViews;
        }

        public void CreateSwapChain(Instance instance) {
            
            SupportDetails swapChainSupport = device.CheckSwapChainSupport(device.physicalDevice);
            SurfaceFormatKHR surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
            PresentModeKHR presentMode = ChoosePresentMode(swapChainSupport.PresentModes);
            extent = CreateSwapExtent(swapChainSupport.Capabilities);

            uint imageCount = swapChainSupport.Capabilities.MinImageCount + 1;
            if (swapChainSupport.Capabilities.MaxImageCount > 0 && imageCount > swapChainSupport.Capabilities.MaxImageCount) {
                imageCount = swapChainSupport.Capabilities.MaxImageCount;
            }

            SwapchainCreateInfoKHR createInfo = new() {
                SType = StructureType.SwapchainCreateInfoKhr,
                Surface = surface.surface,

                MinImageCount = imageCount,
                ImageFormat = surfaceFormat.Format,
                ImageColorSpace = surfaceFormat.ColorSpace,
                ImageExtent = extent,
                ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.ColorAttachmentBit,

                PreTransform = swapChainSupport.Capabilities.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = presentMode,
                Clipped = true,
                OldSwapchain = default
            };

            QueueFamilyIndices indices = device.FindQueueFamilies(device.physicalDevice);
            unsafe {
                uint* queueFamilyIndices = stackalloc[] { indices.GraphicsFamily!.Value, indices.PresentFamily!.Value };

                if (indices.GraphicsFamily != indices.PresentFamily) {
                    createInfo = createInfo with {
                        ImageSharingMode = SharingMode.Concurrent,
                        QueueFamilyIndexCount = 2,
                        PQueueFamilyIndices = queueFamilyIndices
                    };
                } else {
                    createInfo.ImageSharingMode = SharingMode.Exclusive;
                }

            }
            if (!API.Vk.TryGetDeviceExtension(instance, device.logicalDevice, out extension)) {
                Logger.LogFatal("Vulkan: Failed to get Swapchain extension!");
                throw new APIBackendException("Vulkan", "Failed to get VK_KHR_swapchain extension.");
            }
            unsafe {
                _ = Utils.Check(extension.CreateSwapchain(device.logicalDevice, createInfo, Allocator.allocatorPtr, out swapchain), Logger, "Failed to create swapchain", true);

                Utils.GetArray((Pointer<Image> ptr, ref uint count) => { // NOTE: There is only a minimal image count specified.
                    return extension.GetSwapchainImages(device.logicalDevice, swapchain, ref count, ptr);
                }, out images, Logger, "SwapchainImages", true);
            }
            imageFormat = surfaceFormat.Format;
        }

        public Extent2D CreateSwapExtent(SurfaceCapabilitiesKHR capabilities) {
            if (capabilities.CurrentExtent.Width != uint.MaxValue) {
                return capabilities.CurrentExtent;
            } else {
                unsafe {
                    API.Glfw.GetFramebufferSize(window.Window, out int width, out int height);
                    return new() {
                        Width = Math.Clamp((uint)width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width),
                        Height = Math.Clamp((uint)height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height)
                    };
                }
            }
        }
        public PresentModeKHR ChoosePresentMode(PresentModeKHR[] presentModes) {
            foreach (var presentMode in presentModes) {
                if (presentMode == PresentModeKHR.MailboxKhr) {
                    return presentMode;
                }
            }
            return PresentModeKHR.FifoKhr;
        }
        public SurfaceFormatKHR ChooseSwapSurfaceFormat(SurfaceFormatKHR[] surfaceFormats) {
            foreach (SurfaceFormatKHR availableFormat in surfaceFormats) {
                if (availableFormat.Format == Format.B8G8R8A8Srgb && availableFormat.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr) {
                    return availableFormat;
                }
            }
            return surfaceFormats[0];
        }
    
        public void CreateFramebuffers(RenderPass renderPass) {
            if (!AreFramebuffersDisposed) DisposeFramebuffers();

            frameBuffers = new Framebuffer[imageViews.Length];

            Logger.LogDebug("Creating Framebuffers.");

            for (int i = 0; i < imageViews.Length; i++) {
                ImageView imageView = imageViews[i];
                FramebufferCreateInfo createInfo = new() {
                    SType = StructureType.FramebufferCreateInfo,

                    RenderPass = renderPass.renderPass,
                    AttachmentCount = 1,
                    PAttachments = Pointer<ImageView>.From(ref imageView),
                    Width = extent.Width,
                    Height = extent.Height,
                    Layers = 1
                };

                unsafe {
                    _ = Utils.Check(API.Vk.CreateFramebuffer(device.logicalDevice, Pointer<FramebufferCreateInfo>.From(ref createInfo), Allocator.allocatorPtr, Pointer<Framebuffer>.From(ref frameBuffers[i])), Logger, "Failed to create framebuffer", true);
                }
            }

            AreFramebuffersDisposed = false;
        }
        public void DisposeFramebuffers() {
            if (AreFramebuffersDisposed) { return; }

            foreach (Framebuffer framebuffer in frameBuffers!) {
                unsafe {
                    API.Vk.DestroyFramebuffer(device.logicalDevice, framebuffer, Allocator.allocatorPtr);
                }
            }

            AreFramebuffersDisposed = true;
        }
        /// <inheritdoc/>
        public void Dispose() {
            if (IsDisposed) { return; }

            DisposeFramebuffers();

            foreach (ImageView imageView in imageViews) {
                unsafe {
                    API.Vk.DestroyImageView(device.logicalDevice, imageView, Allocator.allocatorPtr);
                }
            }
            unsafe {
                extension.DestroySwapchain(device.logicalDevice, swapchain, Allocator.allocatorPtr);
            }
            IsDisposed = true;
            
            GC.SuppressFinalize(this);
        }
    }
}