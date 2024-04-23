using System.Runtime.InteropServices;
using GAIL.Core;
using GAIL.Graphics.Utils;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;

namespace GAIL.Graphics
{
    /// <summary>
    /// This handles all the graphics of GAIL.
    /// </summary>
    public class GraphicsManager : IManager {
        /// <summary>
        /// Vulkan Instance.
        /// </summary>
        public Instance instance;
        /// <summary>
        /// The vulkan devices utility, for custom usage.
        /// </summary>
        public Utils.Device? device;
        /// <summary>
        /// The vulkan window surface utility, for custom usage.
        /// </summary>
        public Surface? surface;

        /// <summary>
        /// The vulkan swapchain utility, for custom usage.
        /// </summary>
        public SwapChain? swapchain;
        /// <summary>
        /// Current MSAA size, use setMSAA to change it.
        /// </summary>
        public MSAA MSAAsize = MSAA.MSAAx1;

        /// <summary></summary>
        ~GraphicsManager() {
            Dispose();
        }

        /// <summary>
        /// Initializes the graphics manager.
        /// </summary>
        /// <param name="globals">The globals of this application.</param>
        /// <param name="appInfo">The application info for vulkan.</param>
        /// <exception cref="APIBackendException"></exception>
        public void Init(Application.Globals globals, AppInfo appInfo) {
            CreateInstance(appInfo);
            surface = new Surface(instance, globals.windowManager);
            device = new Utils.Device(instance, ref surface);
            swapchain = new SwapChain(instance, surface, device, globals.windowManager);

        }
        private void CreateInstance(AppInfo appInfo) {
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
                if (API.Vk.CreateInstance(createInfo, null, out instance) != Result.Success) {
                    throw new APIBackendException("Vulkan", "Failed to create Vulkan Instance!");
                };

                // Clears unmanaged resources.
                Marshal.FreeHGlobal((IntPtr)vkInfo.PApplicationName);
                Marshal.FreeHGlobal((IntPtr)vkInfo.PEngineName);
            }
        }

        /// <inheritdoc/>
        public void Dispose() {
            unsafe {
                swapchain!.Dispose();
                device!.Dispose();
                surface!.Dispose();
                API.Vk.DestroyInstance(instance, null);
                API.Vk.Dispose();
            }
            GC.SuppressFinalize(this);
        }
        public MSAA GetMaxMSAA() {
            throw new NotImplementedException();
        }
        public void SetMSAA() {
            throw new NotImplementedException();
        }
        public void Render3D(List<Model> models) {
            throw new NotImplementedException();
        }
        public void Render3DInstanced(Mesh mesh, List<IMaterial> materials) {
            throw new NotImplementedException();
        }
        public void Render2D(List<Model> models) {
            throw new NotImplementedException();
        }
        public void Render2DInstanced(Mesh mesh, List<IMaterial> materials) {
            throw new NotImplementedException();
        }
    }
}