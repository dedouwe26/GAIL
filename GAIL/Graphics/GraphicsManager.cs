using System.Runtime.InteropServices;
using GAIL.Core;
using GAIL.Graphics.Utils;
using OxDED.Terminal.Logging;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;

namespace GAIL.Graphics
{
    /// <summary>
    /// Handles all the graphics of GAIL.
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

        /// <summary>
        /// The logger corresponding to the graphics part of the application.
        /// </summary>
        public readonly Logger Logger;

        /// <summary>
        /// Creates a graphics manager. Use <see cref="Init"/> to initialize the manager.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public GraphicsManager(Logger logger) {
            Logger = logger;
        }

        /// <summary>
        /// Disposes this graphics manager.
        /// </summary>
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
            surface = new Surface(instance, Logger, globals.windowManager);
            device = new Utils.Device(instance, Logger, ref surface);
            swapchain = new SwapChain(instance, Logger, surface, device, globals.windowManager);

        }
        private void CreateInstance(AppInfo appInfo) {
            Logger.LogDebug("Creating Vulkan Instance.");
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
                    Logger.LogFatal("Vulkan: Failed to create Vulkan Instance: "+r.ToString());
                    throw new APIBackendException("Vulkan", "Failed to create Vulkan Instance: "+r.ToString());
                };

                // Clears unmanaged resources.
                Marshal.FreeHGlobal((IntPtr)vkInfo.PApplicationName);
                Marshal.FreeHGlobal((IntPtr)vkInfo.PEngineName);
            }
        }

        /// <inheritdoc/>
        public void Dispose() {
            Logger.LogDebug("Disposing Vulkan.");
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