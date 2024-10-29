using System.Runtime.InteropServices;
using GAIL.Core;
using GAIL.Graphics.Layer;
using GAIL.Graphics.Renderer;
using GAIL.Graphics.Renderer.Layer;
using OxDED.Terminal.Logging;
using Silk.NET.GLFW;

namespace GAIL.Graphics
{
    /// <summary>
    /// Handles all the graphics of GAIL.
    /// </summary>
    public class GraphicsManager : IDisposable {
        /// <summary>
        /// If this manager is already disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// The renderer of the graphics manager.
        /// </summary>
        public VulkanRenderer? Renderer { get; private set; }
        /// <summary>
        /// The settings of the renderer.
        /// </summary>
        public IRendererSettings Settings { get => Renderer!.Settings; }

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
        /// Initializes the graphics manager.
        /// </summary>
        /// <param name="globals">The globals of this application.</param>
        /// <param name="appInfo">The application info for vulkan.</param>
        /// <exception cref="APIBackendException"></exception>
        public void Init(Application.Globals globals, AppInfo appInfo) {
            Logger.LogDebug("Initalizing Graphics.");

            Renderer = new VulkanRenderer(Logger, globals, appInfo);

            globals.windowManager.OnFramebufferResize += (int width, int height) => {
                Renderer.Resize(width, height);
            };
        }
        /// <summary>
        /// Updates the graphics on the screen.
        /// </summary>
        public void Update() {
            Renderer!.Render();
        }
        public bool CreateBackendLayer<TBackend>(out TBackend? layer) where TBackend : IBackendLayer {
            if (typeof(TBackend) == typeof(IRasterizationLayer)) {
                // TODO: Initialize layers.

                return true;
            }
            layer = default;
            return false;
        }
        public bool AddLayer<TBackend>(ILayer<TBackend> layer) where TBackend : IBackendLayer {
            if (!CreateBackendLayer(out TBackend? backendLayer)) return false;
            if (backendLayer == null) return false;
            
        }

        /// <inheritdoc/>
        public void Dispose() {
            if (IsDisposed) { return; }

            Renderer?.Dispose();

            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}