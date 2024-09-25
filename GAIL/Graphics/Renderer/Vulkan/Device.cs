using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GAIL.Core;
using OxDED.Terminal.Logging;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace GAIL.Graphics.Renderer.Vulkan
{
    public struct QueueFamilyIndices {
        public uint? GraphicsFamily { get; set; } // Can render?
        public uint? PresentFamily { get; set; } // Can present to the surface
        public readonly bool IsComplete() {
            return GraphicsFamily.HasValue && PresentFamily.HasValue;
        }
    }

    /// <summary>
    /// A wrapper around the vulkan logical and physical devices.
    /// </summary>
    public class Device : IDisposable {
        public static readonly string[] deviceExtensions = [KhrSwapchain.ExtensionName];
        public PhysicalDevice physicalDevice;
        public Silk.NET.Vulkan.Device logicalDevice;
        public Queue graphicsQueue;
        public Queue presentQueue;
        private readonly Surface surface;
        private readonly Logger Logger;

        public Device(VulkanRenderer renderer, ref Surface surface) {
            
            Logger = renderer.Logger;
            this.surface = surface;

            bool oneDeviceSuitable = false;
            Logger.LogDebug("Searching for suitable Vulkan physical device.");
            foreach (PhysicalDevice device in GetPhysicalDevices(renderer.instance!)) {
                if (IsDeviceSuitable(device)) {
                    physicalDevice = device;
                    oneDeviceSuitable = true;
                    CreateLogicalDevice();
                    break;
                }
            }

            if (!oneDeviceSuitable) {
                Logger.LogFatal("Vulkan: No suitable physical device found.");
                throw new APIBackendException("Vulkan", "No suitable physical device found");
            }
        }
        public void CreateLogicalDevice() {
            Logger.LogDebug("Creating Vulkan logical device.");
            QueueFamilyIndices indices = FindQueueFamilies(physicalDevice);

            uint[] uniqueQueueFamilies = [ indices.GraphicsFamily!.Value, indices.PresentFamily!.Value];
            uniqueQueueFamilies = uniqueQueueFamilies.Distinct().ToArray();

            unsafe {
                using GlobalMemory mem = GlobalMemory.Allocate(uniqueQueueFamilies.Length * sizeof(DeviceCreateInfo));
                DeviceQueueCreateInfo* queueCreateInfos = (DeviceQueueCreateInfo*)Unsafe.AsPointer(ref mem.GetPinnableReference());

                float queuePriority = 1f;
                for (int i = 0; i < uniqueQueueFamilies.Length; i++) {
                    queueCreateInfos[i] = new DeviceQueueCreateInfo() {
                        SType = StructureType.DeviceQueueCreateInfo,
                        QueueFamilyIndex = uniqueQueueFamilies[i],
                        QueueCount = 1,
                        PQueuePriorities = &queuePriority
                    };
                }

                PhysicalDeviceFeatures deviceFeatures = new();

                DeviceCreateInfo createInfo = new() {
                    SType = StructureType.DeviceCreateInfo,
                    QueueCreateInfoCount = (uint)uniqueQueueFamilies.Length,
                    PQueueCreateInfos = queueCreateInfos,
                    PEnabledFeatures = &deviceFeatures,
                    EnabledExtensionCount = (uint)deviceExtensions.Length,
                    PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(deviceExtensions),
                    EnabledLayerCount = 0
                };
                
                _ = Utils.Check(API.Vk.CreateDevice(physicalDevice, in createInfo, null, out logicalDevice), Logger, "Failed to create logical device", true);
                // NOTE: Could try other device.

                API.Vk.GetDeviceQueue(logicalDevice, indices.GraphicsFamily!.Value, 0, out graphicsQueue);
                API.Vk.GetDeviceQueue(logicalDevice, indices.PresentFamily!.Value, 0, out presentQueue);

                SilkMarshal.Free((nint)createInfo.PpEnabledExtensionNames);
            }
        }

        /// <param name="instance">Vulkan instance.</param>
        /// <exception cref="APIBackendException"></exception>
        public PhysicalDevice[] GetPhysicalDevices(Instance instance) {
            Utils.GetArray((Pointer<PhysicalDevice> pointer, ref uint count) => {
                unsafe {
                    return API.Vk.EnumeratePhysicalDevices(instance, ref count, pointer);
                }
            }, out PhysicalDevice[] devices, Logger, "PhysicalDevices", true);

            if (devices.Length == 0) {
                Logger.LogFatal("Vulkan: Failed to find GPU with Vulkan support!");
                throw new APIBackendException("Vulkan", "Failed to find GPU with Vulkan support.");
            }

            return devices;
        }
        /// <param name="device">What device.</param>
        public bool IsDeviceSuitable(PhysicalDevice device) {

            bool extensionsSupported = CheckDeviceExtensionsSupport(device);

            bool swapChainSupported = false;
            
            if (extensionsSupported) {
                SwapChain.SupportDetails swapChainSupport = CheckSwapChainSupport(device);
                swapChainSupported = swapChainSupport.Formats.Length != 0 && swapChainSupport.PresentModes.Length != 0;
            }

            return FindQueueFamilies(device).IsComplete() && extensionsSupported && swapChainSupported;

        }

        public SwapChain.SupportDetails CheckSwapChainSupport(PhysicalDevice device) {
            var details = new SwapChain.SupportDetails();

            unsafe {
                _ = Utils.Check(surface.surfaceExtension.GetPhysicalDeviceSurfaceCapabilities(device, surface.surface, out details.Capabilities), Logger, "Failed to get PhysicalDevice surface capabilities", true);

                // Surface formats
                Utils.GetArray((Pointer<SurfaceFormatKHR> pointer, Pointer<uint> count) => {
                    return surface.surfaceExtension.GetPhysicalDeviceSurfaceFormats(device, surface.surface, count, pointer);
                }, out details.Formats, Logger, "PhysicalDeviceSurfaceFormats", true);

                // Surface present modes
                Utils.GetArray((Pointer<PresentModeKHR> pointer, Pointer<uint> count) => {
                    return surface.surfaceExtension.GetPhysicalDeviceSurfacePresentModes(device, surface.surface, count, pointer);
                }, out details.PresentModes, Logger, "PhysicalDeviceSurfacePresentModes", true);
            }

            return details;
        }

        /// <summary>
        /// Checks if a physical device can support all the extensions.
        /// </summary>
        /// <param name="device">The device to check.</param>
        /// <returns>If the device is supported.</returns>
        public bool CheckDeviceExtensionsSupport(PhysicalDevice device) {
            // Get all the extensions supported by the GPU.
            if (!Utils.GetArray((Pointer<ExtensionProperties> pointer, ref uint count) => {
                unsafe {
                    return API.Vk.EnumerateDeviceExtensionProperties(device, (byte*)null, ref count, pointer);
                }
            }, out ExtensionProperties[] availableExtensions, Logger, "DeviceExtensionProperties", false)) {
                return false;
            }

            // Check if all the extensions are supported.
            unsafe {
                HashSet<string?> availableExtensionNames = availableExtensions.Select(extension => Marshal.PtrToStringAnsi((IntPtr)extension.ExtensionName)).ToHashSet();

                return deviceExtensions.All(availableExtensionNames.Contains);
            }
        }

        /// <summary>
        /// Finds queue families.
        /// </summary>
        /// <param name="device">What device.</param>
        public QueueFamilyIndices FindQueueFamilies(PhysicalDevice device) {
            QueueFamilyIndices indices = new();

            unsafe {
                Utils.GetArray((Pointer<QueueFamilyProperties> pointer, ref uint count) => {
                    API.Vk.GetPhysicalDeviceQueueFamilyProperties(device, ref count, pointer);
                    return Result.Success; // See here...
                }, out QueueFamilyProperties[] queueFamilies, Logger, string.Empty, true); // string.Empty because Result is always success.

                for (uint i = 0; i < queueFamilies.Length; i++)
                {
                    if (queueFamilies[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit)) {
                        indices.GraphicsFamily = i;
                    }
                    
                    Utils.Check(surface.surfaceExtension.GetPhysicalDeviceSurfaceSupport(device, i, surface.surface, out Bool32 presentSupport), Logger, "Unable get physical device surface support", true);
                    
                    if (presentSupport) {
                        indices.PresentFamily = i;
                    }

                    if (indices.IsComplete()) {
                        break;
                    }
                }
                return indices;
            }
        }

        /// <inheritdoc/>
        public void Dispose() {
            unsafe {
                API.Vk.DestroyDevice(logicalDevice, null);
            }
            GC.SuppressFinalize(this);
        }
    }
}