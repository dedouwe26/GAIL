using GAIL.Core;
using GAIL.Window;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace GAIL.Graphics.Utils
{
    public class SwapChain : IDisposable {
        public struct SwapChainSupportDetails {
            public SurfaceCapabilitiesKHR Capabilities;
            public SurfaceFormatKHR[] Formats;
            public PresentModeKHR[] PresentModes;
        }
        public KhrSwapchain swapchainExtension;
        public SwapchainKHR swapchain;
        public Image[] images;
        public Format imageFormat;
        public Extent2D extent;
        private readonly Vk vk;
        private readonly Surface surface;
        private readonly WindowManager window;
        private readonly Device device;

        public SwapChain(Vk vk, Instance instance, Surface surface, Device device, WindowManager windowManager) {
            this.vk = vk;
            this.surface = surface;
            this.device = device;
            window = windowManager;

            SwapChainSupportDetails swapChainSupport = SwapChainSupport(device.physicalDevice);
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
            if (!vk.TryGetDeviceExtension(instance, device.logicalDevice, out swapchainExtension)) {
                throw new APIBackendException("Vulkan", "Failed to get VK_KHR_swapchain extension.");
            }
            unsafe {
                if (swapchainExtension.CreateSwapchain(device.logicalDevice, createInfo, null, out swapchain) != Result.Success) {
                    throw new APIBackendException("Vulkan", "Failed to create swapchain.");
                }
                swapchainExtension.GetSwapchainImages(device.logicalDevice, swapchain, ref imageCount, null);
                images = new Image[imageCount];
                fixed (Image* swapChainImagesPtr = images)
                {
                    swapchainExtension.GetSwapchainImages(device.logicalDevice, swapchain, ref imageCount, swapChainImagesPtr);
                }
            }
            imageFormat = surfaceFormat.Format;
        }

        public Extent2D CreateSwapExtent(SurfaceCapabilitiesKHR capabilities) {
            if (capabilities.CurrentExtent.Width != uint.MaxValue) {
                return capabilities.CurrentExtent;
            } else {
                unsafe {
                    window.glfw.GetFramebufferSize(window.Window, out int width, out int height);
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
        public SwapChainSupportDetails SwapChainSupport(PhysicalDevice physicalDevice) {
            SwapChainSupportDetails details = new();

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
    
        public void Dispose() {
            unsafe {
                swapchainExtension.DestroySwapchain(device.logicalDevice, swapchain, null);
            }
        }
    }
}