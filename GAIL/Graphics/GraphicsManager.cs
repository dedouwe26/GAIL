using System.Runtime.InteropServices;
using GAIL.Core;
using GAIL.Graphics.Utils;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace GAIL.Graphics
{
    /// <summary>
    /// This handles all the graphics of GAIL.
    /// </summary>
    public class GraphicsManager : IManager {

        /// <summary>
        /// The Vulkan API instance for custom usage.
        /// </summary>
        public readonly Vk vk;
        /// <summary>
        /// Vulkan Instance.
        /// </summary>
        public Instance instance;
        /// <summary>
        /// The vulkan devices, for custom usage.
        /// </summary>
        public Utils.Device? device;
        /// <summary>
        /// The vulkan window surface, for custom usage.
        /// </summary>
        public Surface? surface;
        public SwapChain? swapchain;
        /// <summary>
        /// Current MSAA size, use setMSAA to change it.
        /// </summary>
        public MSAA MSAAsize = MSAA.MSAAx1;

        public static readonly string[] ValidationLayers = ["VK_LAYER_KHRONOS_validation"];

        public GraphicsManager() {
            vk = Vk.GetApi();
        }

        ~GraphicsManager() {
            Dispose();
        }

        /// <summary>
        /// Initializes the graphics manager.
        /// </summary>
        /// <param name="appInfo">The application info for vulkan.</param>
        /// <exception cref="APIBackendException"></exception>
        public void Init(Application.Globals globals, AppInfo appInfo) {
            CreateInstance(appInfo);
            surface = new Surface(vk, instance, globals.windowManager);
            device = new Utils.Device(vk, instance, ref surface);
            swapchain = new SwapChain(vk, instance, surface, device, globals.windowManager);

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
                if (vk.CreateInstance(createInfo, null, out instance) != Result.Success) {
                    throw new APIBackendException("Vulkan", "Failed to create Vulkan Instance!");
                };

                // Clears unmanaged resources.
                Marshal.FreeHGlobal((IntPtr)vkInfo.PApplicationName);
                Marshal.FreeHGlobal((IntPtr)vkInfo.PEngineName);
            }
        }


        public void Dispose() {
            unsafe {
                swapchain!.Dispose();
                device!.Dispose();
                surface!.Dispose();
                vk.DestroyInstance(instance, null);
                vk.Dispose();
            }
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