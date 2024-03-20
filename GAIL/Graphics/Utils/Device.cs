using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GAIL.Core;
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
        private readonly Vk vk;


        public Device(Vk vk, Instance instance, ref Surface surface) {
            this.vk = vk;
            this.surface = surface;

            foreach (PhysicalDevice device in GetPhysicalDevices(instance)) {
                if (IsDeviceSuitable(device)) {
                    physicalDevice = device;
                }
            }
            CreateLogicalDevice();
        }
        public void CreateLogicalDevice() {
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
                    EnabledExtensionCount = 0,
                    EnabledLayerCount = 0
                };
                if (vk.CreateDevice(physicalDevice, in createInfo, null, out logicalDevice)!=Result.Success) {
                    throw new APIBackendException("Vulkan", "Failed to create logical device.");
                }
                vk.GetDeviceQueue(logicalDevice, indices.GraphicsFamily!.Value, 0, out graphicsQueue);
                vk.GetDeviceQueue(logicalDevice, indices.PresentFamily!.Value, 0, out presentQueue);

            }
        }

        /// <param name="vk">Vulkan API instance.</param>
        /// <param name="instance">Vulkan instance.</param>
        /// <exception cref="APIBackendException"></exception>
        public PhysicalDevice[] GetPhysicalDevices(Instance instance) {
            uint deviceCount = 0;
            unsafe {
                vk.EnumeratePhysicalDevices(instance, ref deviceCount, null);
            }
            if (deviceCount == 0) {
                throw new APIBackendException("Vulkan", "Failed to find GPU with Vulkan support.");
            }
            PhysicalDevice[] devices = new PhysicalDevice[deviceCount];
            unsafe {
                fixed (PhysicalDevice* devicesPtr = devices) {
                    vk.EnumeratePhysicalDevices(instance, ref deviceCount, devicesPtr);
                }
            }
            return devices;
        }
        /// <param name="vk">Vulkan API instance.</param>
        /// <param name="device">What device.</param>
        public bool IsDeviceSuitable(PhysicalDevice device) {

            bool extensionsSupported = CheckDeviceExtensionsSupport(device);

            bool swapChainAdequate = false;
            
            if (extensionsSupported) {
                SwapChain.SwapChainSupportDetails swapChainSupport = CheckSwapChainSupport(device);
                swapChainAdequate = swapChainSupport.Formats.Length != 0 && swapChainSupport.PresentModes.Length != 0;
            }

            return FindQueueFamilies(device).IsComplete() && extensionsSupported && swapChainAdequate;

        }

        public SwapChain.SwapChainSupportDetails CheckSwapChainSupport(PhysicalDevice device) {
            var details = new SwapChain.SwapChainSupportDetails();

            unsafe {
                surface.surfaceExtension.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface.surface, out details.Capabilities);

                uint formatCount = 0;
                surface.surfaceExtension.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface.surface, ref formatCount, null);

                if (formatCount != 0)
                {
                    details.Formats = new SurfaceFormatKHR[formatCount];
                    fixed (SurfaceFormatKHR* formatsPtr = details.Formats)
                    {
                        surface.surfaceExtension.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface.surface, ref formatCount, formatsPtr);
                    }
                }
                else
                {
                    details.Formats = Array.Empty<SurfaceFormatKHR>();
                }

                uint presentModeCount = 0;
                surface.surfaceExtension.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface.surface, ref presentModeCount, null);

                if (presentModeCount != 0)
                {
                    details.PresentModes = new PresentModeKHR[presentModeCount];
                    fixed (PresentModeKHR* formatsPtr = details.PresentModes)
                    {
                        surface.surfaceExtension.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface.surface, ref presentModeCount, formatsPtr);
                    }

                }
                else
                {
                    details.PresentModes = Array.Empty<PresentModeKHR>();
                }
            }
            

            return details;
        }

        public bool CheckDeviceExtensionsSupport(PhysicalDevice device) {
            uint extentionsCount = 0;
            unsafe {
                vk.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, null);

                ExtensionProperties[] availableExtensions = new ExtensionProperties[extentionsCount];
                fixed (ExtensionProperties* availableExtensionsPtr = availableExtensions)
                {
                    vk.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, availableExtensionsPtr);
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
                vk.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilyCount, null);

                QueueFamilyProperties[] queueFamilies = new QueueFamilyProperties[queueFamilyCount];

                fixed (QueueFamilyProperties* queueFamiliesPtr = queueFamilies) {
                    vk!.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilyCount, queueFamiliesPtr);
                }

                for (uint i = 0; i < queueFamilies.Length; i++)
                {
                    if (queueFamilies[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit)) {
                        indices.GraphicsFamily = i;
                    }
                    
                    if (surface.surfaceExtension.GetPhysicalDeviceSurfaceSupport(device, i, surface.surface, out Bool32 presentSupport) != Result.Success) {
                        throw new APIBackendException("Vulkan", "Unable to get surface support.");
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

        public void Dispose() {
            unsafe {
                vk.DestroyDevice(logicalDevice, null);
            }
        }
    }
}