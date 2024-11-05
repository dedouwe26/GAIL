using System.Runtime.InteropServices;
using GAIL.Core;
using OxDED.Terminal.Logging;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Renderer.Vulkan;

/// <summary>
/// A vulkan utility for the vulkan instance.
/// </summary>
public class Instance : IDisposable {
    /// <summary>
    /// If this class is already disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }
    /// <summary>
    /// Vulkan Instance.
    /// </summary>
    public readonly Silk.NET.Vulkan.Instance instance;
    public Instance(VulkanRenderer renderer, ref AppInfo appInfo) {
        renderer.Logger.LogDebug("Creating Vulkan Instance.");
        unsafe {
            // Creates application info from AppInfo.
            ApplicationInfo vkInfo = new() {
                SType = StructureType.ApplicationInfo,
                PApplicationName = (byte*)Marshal.StringToHGlobalAnsi(appInfo.AppName),
                ApplicationVersion = Vk.MakeVersion(appInfo.AppVersion[0], appInfo.AppVersion[1], appInfo.AppVersion[2]),
                PEngineName = (byte*)Marshal.StringToHGlobalAnsi(appInfo.EngineName),
                EngineVersion = Vk.MakeVersion(appInfo.EngineVersion[0], appInfo.EngineVersion[1], appInfo.EngineVersion[2]),
                ApiVersion = Vk.Version13
            };

            // Glfw required extensions.
            byte** extensions = API.Glfw.GetRequiredInstanceExtensions(out uint extensionCount);

            // Instance create info with extensions.
            InstanceCreateInfo createInfo = new()
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = Pointer<ApplicationInfo>.From(ref vkInfo),
                EnabledExtensionCount = extensionCount,
                PpEnabledExtensionNames = extensions,
                // Sets no validation layers.
                EnabledLayerCount = 0,
                PNext = null
            };

            // Creates instance.
            _ = Utils.Check(API.Vk.CreateInstance(in createInfo, Allocator.allocatorPtr, out instance), renderer.Logger, "Failed to create a Vulkan Instance", true);
            // NOTE: not checking return value, because fatal is turned on so an exception will be thrown.

            // Clears unmanaged resources.
            Marshal.FreeHGlobal((IntPtr)vkInfo.PApplicationName);
            Marshal.FreeHGlobal((IntPtr)vkInfo.PEngineName);
        }
    }

    /// <inheritdoc/>
    public unsafe void Dispose() {
        if (IsDisposed) { return; }

        API.Vk.DestroyInstance(instance, Allocator.allocatorPtr);

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }

    ///
    public static implicit operator Silk.NET.Vulkan.Instance(Instance instance) {
        return instance.instance;
    }
}