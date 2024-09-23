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
        public KhrSwapchain extension;
        public SwapchainKHR swapchain;
        public Image[] images;
        public ImageView[] imageViews;
        public Format imageFormat;
        public Extent2D extent;
        private readonly Surface surface;
        private readonly WindowManager window;
        private readonly Device device;
        private readonly Logger Logger;

        public SwapChain(VulkanRenderer renderer, WindowManager windowManager) {
            Logger = renderer.Logger;
            surface = renderer.surface!;
            device = renderer.device!;
            window = windowManager;

            (KhrSwapchain swapchainExtension, SwapchainKHR swapchain, Image[] images) = CreateSwapChain(renderer.instance!);
            
            extension = swapchainExtension;
            this.swapchain = swapchain;
            this.images = images;

            imageViews = CreateImageViews();
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
                    Result r;
                    if ((r = API.Vk.CreateImageView(device.logicalDevice, createInfo, null, out imageViews[i])) != Result.Success) {
                        Logger.LogDebug("Vulkan: Failed to create image view: "+r.ToString());
                        throw new APIBackendException("Vulkan", "Failed to create image view: "+r.ToString());
                    }
                }
            }
            return imageViews;
        }

        public (KhrSwapchain swapchainExtension, SwapchainKHR swapchain, Image[] images) CreateSwapChain(Instance instance) {
            
            SupportDetails swapChainSupport = SwapChainSupport(device.physicalDevice);
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
            if (!API.Vk.TryGetDeviceExtension(instance, device.logicalDevice, out KhrSwapchain swapchainExtension)) {
                Logger.LogFatal("Vulkan: Failed to get Swapchain extension!");
                throw new APIBackendException("Vulkan", "Failed to get VK_KHR_swapchain extension.");
            }
            Image[] images;
            unsafe {
                Result r;
                if ((r=swapchainExtension.CreateSwapchain(device.logicalDevice, createInfo, null, out SwapchainKHR swapchain)) != Result.Success) {
                    Logger.LogFatal("Vulkan: Failed to create swapchain: "+r.ToString());
                    throw new APIBackendException("Vulkan", "Failed to create swapchain: "+r.ToString());
                }
                swapchainExtension.GetSwapchainImages(device.logicalDevice, swapchain, ref imageCount, null);
                images = new Image[imageCount];
                fixed (Image* swapChainImagesPtr = images)
                {
                    swapchainExtension.GetSwapchainImages(device.logicalDevice, swapchain, ref imageCount, swapChainImagesPtr);
                }
            }
            imageFormat = surfaceFormat.Format;
            return (swapchainExtension, swapchain, images);
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
        public SupportDetails SwapChainSupport(PhysicalDevice physicalDevice) {
            SupportDetails details = new();

            surface.surfaceExtension.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface.surface, out details.Capabilities);

            unsafe {
                uint formatCount = 0;
                surface.surfaceExtension.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface.surface, ref formatCount, null);

                if (formatCount != 0) {
                    details.Formats = new SurfaceFormatKHR[formatCount];
                    fixed (SurfaceFormatKHR* formatsPtr = details.Formats) {
                        surface.surfaceExtension.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface.surface, ref formatCount, formatsPtr);
                    }
                } else {
                    details.Formats = [];
                }

                uint presentModeCount = 0;
                surface.surfaceExtension.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface.surface, ref presentModeCount, null);
                if (presentModeCount != 0) {
                    details.PresentModes = new PresentModeKHR[formatCount];
                    fixed (PresentModeKHR* modesPtr = details.PresentModes) {
                        surface.surfaceExtension.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface.surface, ref presentModeCount, modesPtr);
                    }
                } else {
                    details.PresentModes = [];
                }

                return details;
            }
            
        }
    
        /// <inheritdoc/>
        public void Dispose() {
            foreach (ImageView imageView in imageViews) {
                unsafe {
                    API.Vk.DestroyImageView(device.logicalDevice, imageView, null);
                }
            }
            unsafe {
                extension.DestroySwapchain(device.logicalDevice, swapchain, null);
            }
            GC.SuppressFinalize(this);
        }
    }
}