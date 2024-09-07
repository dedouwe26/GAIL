using System.Runtime.InteropServices;
using GAIL.Core;
using GAIL.Graphics.Utils;
using OxDED.Terminal.Logging;
using Silk.NET.GLFW;

namespace GAIL.Graphics
{
    /// <summary>
    /// Handles all the graphics of GAIL.
    /// </summary>
    public class GraphicsManager : IManager {
        
        /// <summary>
        /// The vulkan devices utility, for custom usage.
        /// </summary>
        public Device? device;
        /// <summary>
        /// The vulkan window surface utility, for custom usage.
        /// </summary>
        public Surface? surface;

        /// <summary>
        /// The vulkan swapchain utility, for custom usage.
        /// </summary>
        public SwapChain? swapchain;
        /// <summary>
        /// The vulkan instance utility, for custom usage.
        /// </summary>
        public Instance? instance;
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
            instance = new Instance(Logger, appInfo);
            surface = new Surface(instance, Logger, globals.windowManager);
            device = new Device(instance, Logger, ref surface);
            swapchain = new SwapChain(instance, Logger, surface, device, globals.windowManager);

        }

        /// <inheritdoc/>
        public void Dispose() {
            Logger.LogDebug("Disposing Vulkan.");
            unsafe {
                swapchain!.Dispose();
                device!.Dispose();
                surface!.Dispose();
                instance!.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}