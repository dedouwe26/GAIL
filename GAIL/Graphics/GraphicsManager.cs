using System.Runtime.InteropServices;
using GAIL.Core;
using GAIL.Graphics.Renderer;
using OxDED.Terminal.Logging;
using Silk.NET.GLFW;

namespace GAIL.Graphics
{
    /// <summary>
    /// Handles all the graphics of GAIL.
    /// </summary>
    public class GraphicsManager : IManager {
        /// <summary>
        /// The renderer of the graphics manager.
        /// </summary>
        public VulkanRenderer? renderer;

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
            Logger.LogDebug("Initalizing Graphics.");

            renderer = new VulkanRenderer(Logger, globals, appInfo);
        }

        /// <inheritdoc/>
        public void Dispose() {
            renderer?.Dispose();
        }
    }
}