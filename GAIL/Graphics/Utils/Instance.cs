using System.Runtime.InteropServices;
using GAIL.Core;
using OxDED.Terminal.Logging;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;

namespace GAIL.Graphics.Utils;

/// <summary>
/// A vulkan utility for the vulkan instance.
/// </summary>
public class Instance : IDisposable {
    /// <summary>
    /// Vulkan Instance.
    /// </summary>
    public Silk.NET.Vulkan.Instance instance;
    public Instance(Logger logger, AppInfo appInfo) {
        logger.LogDebug("Creating Vulkan Instance.");
        unsafe {
            // Creates application info from AppInfo.
            ApplicationInfo vkInfo = new() {
                SType = StructureType.ApplicationInfo,
                PApplicationName = (byte*)Marshal.StringToHGlobalAnsi(appInfo.AppName),
                ApplicationVersion = Vk.MakeVersion(appInfo.AppVersion[0], appInfo.AppVersion[1], appInfo.AppVersion[2]),
                PEngineName = (byte*)Marshal.StringToHGlobalAnsi(appInfo.EngineName),
                EngineVersion = Vk.MakeVersion(appInfo.EngineVersion[0], appInfo.EngineVersion[1], appInfo.EngineVersion[2]),
                ApiVersion = Vk.Version11
            };

            // Glfw required extensions.
            Glfw glfw = Glfw.GetApi();
            byte** extensions = glfw.GetRequiredInstanceExtensions(out uint extensionCount);
            glfw.Dispose();

            // Instance create info with extensions.
            InstanceCreateInfo createInfo = new()
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &vkInfo,
                EnabledExtensionCount = extensionCount,
                PpEnabledExtensionNames = extensions,
                // Sets no validation layers.
                EnabledLayerCount = 0,
                PNext = null
            };

            // Creates instance.
            Result r;
            if ((r=API.Vk.CreateInstance(createInfo, null, out instance)) != Result.Success) {
                logger.LogFatal("Vulkan: Failed to create Vulkan Instance: "+r.ToString());
                throw new APIBackendException("Vulkan", "Failed to create Vulkan Instance: "+r.ToString());
            };

            // Clears unmanaged resources.
            Marshal.FreeHGlobal((IntPtr)vkInfo.PApplicationName);
            Marshal.FreeHGlobal((IntPtr)vkInfo.PEngineName);
        }
    }

    /// <inheritdoc/>
    public unsafe void Dispose() {
        API.Vk.DestroyInstance(instance, null);
        API.Vk.Dispose();
    }
}