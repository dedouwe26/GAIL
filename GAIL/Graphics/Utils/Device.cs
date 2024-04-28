using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GAIL.Core;
using OxDED.Terminal.Logging;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace GAIL.Graphics.Utils
{
    public struct QueueFamilyIndices {
        public uint? GraphicsFamily { get; set; }
        public uint? PresentFamily { get; set; }
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

        public Device(Instance instance, Logger logger, ref Surface surface) {
            Logger = logger;
            this.surface = surface;

            Logger.LogDebug("Searching for Vulkan physical device.");
            foreach (PhysicalDevice device in GetPhysicalDevices(instance)) {
                if (IsDeviceSuitable(device)) {
                    physicalDevice = device;
                }
            }
            CreateLogicalDevice();
        }
        public void CreateLogicalDevice() {
            Logger.LogDebug("Creating Vulkan logical device");
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
                Result r;
                if ((r = API.Vk.CreateDevice(physicalDevice, in createInfo, null, out logicalDevice))!=Result.Success) {
                    Logger.LogFatal("Vulkan: Failed to create logical device: "+r.ToString());
                    throw new APIBackendException("Vulkan", "Failed to create logical device: "+r.ToString());
                }
                API.Vk.GetDeviceQueue(logicalDevice, indices.GraphicsFamily!.Value, 0, out graphicsQueue);
                API.Vk.GetDeviceQueue(logicalDevice, indices.PresentFamily!.Value, 0, out presentQueue);

                SilkMarshal.Free((nint)createInfo.PpEnabledExtensionNames);
            }
        }

        /// <param name="instance">Vulkan instance.</param>
        /// <exception cref="APIBackendException"></exception>
        public PhysicalDevice[] GetPhysicalDevices(Instance instance) {
            uint deviceCount = 0;
            unsafe {
                API.Vk.EnumeratePhysicalDevices(instance, ref deviceCount, null);
            }
            if (deviceCount == 0) {
                Logger.LogFatal("Vulkan: Failed to find GPU with Vulkan support!");
                throw new APIBackendException("Vulkan", "Failed to find GPU with Vulkan support.");
            }
            PhysicalDevice[] devices = new PhysicalDevice[deviceCount];
            unsafe {
                fixed (PhysicalDevice* devicesPtr = devices) {
                    API.Vk.EnumeratePhysicalDevices(instance, ref deviceCount, devicesPtr);
                }
            }
            return devices;
        }
        /// <param name="device">What device.</param>
        public bool IsDeviceSuitable(PhysicalDevice device) {

            bool extensionsSupported = CheckDeviceExtensionsSupport(device);

            bool swapChainAdequate = false;
            
            if (extensionsSupported) {
                SwapChain.SupportDetails swapChainSupport = CheckSwapChainSupport(device);
                swapChainAdequate = swapChainSupport.Formats.Length != 0 && swapChainSupport.PresentModes.Length != 0;
            }

            return FindQueueFamilies(device).IsComplete() && extensionsSupported && swapChainAdequate;

        }

        public SwapChain.SupportDetails CheckSwapChainSupport(PhysicalDevice device) {
            var details = new SwapChain.SupportDetails();

            unsafe {
                surface.surfaceExtension.GetPhysicalDeviceSurfaceCapabilities(device, surface.surface, out details.Capabilities);

                uint formatCount = 0;
                surface.surfaceExtension.GetPhysicalDeviceSurfaceFormats(device, surface.surface, ref formatCount, null);

                if (formatCount != 0) {
                    details.Formats = new SurfaceFormatKHR[formatCount];
                    fixed (SurfaceFormatKHR* formatsPtr = details.Formats) {
                        surface.surfaceExtension.GetPhysicalDeviceSurfaceFormats(device, surface.surface, ref formatCount, formatsPtr);
                    }
                }
                else {
                    details.Formats = [];
                }
                uint presentModeCount = 0;
                surface.surfaceExtension.GetPhysicalDeviceSurfacePresentModes(device, surface.surface, ref presentModeCount, null);
                if (presentModeCount != 0) {
                    details.PresentModes = new PresentModeKHR[presentModeCount];
                    fixed (PresentModeKHR* formatsPtr = details.PresentModes) {
                        surface.surfaceExtension.GetPhysicalDeviceSurfacePresentModes(device, surface.surface, ref presentModeCount, formatsPtr);
                    }
                }
                else {
                    details.PresentModes = [];
                }
            }

            return details;
        }

        public bool CheckDeviceExtensionsSupport(PhysicalDevice device) {
            uint extentionsCount = 0;
            unsafe {
                API.Vk.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, null);

                ExtensionProperties[] availableExtensions = new ExtensionProperties[extentionsCount];
                fixed (ExtensionProperties* availableExtensionsPtr = availableExtensions)
                {
                    API.Vk.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, availableExtensionsPtr);
                }

                HashSet<string?> availableExtensionNames = availableExtensions.Select(extension => Marshal.PtrToStringAnsi((IntPtr)extension.ExtensionName)).ToHashSet();

                return deviceExtensions.All(availableExtensionNames.Contains);
            }
            
        }

        /// <summary>
        /// Finds queue families.
        /// </summary>
        /// <param name="vk">Vulkan API instance.</param>
        /// <param name="device">What device.</param>
        public QueueFamilyIndices FindQueueFamilies(PhysicalDevice device) {
            QueueFamilyIndices indices = new();

            unsafe {
                uint queueFamilyCount = 0;
                API.Vk.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilyCount, null);

                QueueFamilyProperties[] queueFamilies = new QueueFamilyProperties[queueFamilyCount];

                fixed (QueueFamilyProperties* queueFamiliesPtr = queueFamilies) {
                    API.Vk.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilyCount, queueFamiliesPtr);
                }

                for (uint i = 0; i < queueFamilies.Length; i++)
                {
                    if (queueFamilies[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit)) {
                        indices.GraphicsFamily = i;
                    }
                    Result r;
                    if ((r=surface.surfaceExtension.GetPhysicalDeviceSurfaceSupport(device, i, surface.surface, out Bool32 presentSupport)) != Result.Success) {
                        Logger.LogFatal("Vulkan: Unable to get surface support: "+r.ToString());
                        throw new APIBackendException("Vulkan", "Unable to get surface support: "+r.ToString());
                    }
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