using GAIL.Core;
using GAIL.Graphics.Material;
using GAIL.Graphics.Renderer;
using GAIL.Graphics.Renderer.Layer;
using GAIL.Graphics.Renderer.Vulkan;
using LambdaKit.Assertion;
using LambdaKit.Logging;

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
        /// Creates a graphics manager. Use <see cref="Initialize"/> to initialize the manager.
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
        /// <param name="maxFramesInFlight">The default max frames used.</param>
        /// <param name="clearValue"></param>
        /// <exception cref="APIBackendException"></exception>
        public void Initialize(Application.Globals globals, ref AppInfo appInfo, uint maxFramesInFlight = 2, Color? clearValue = null) {
            Logger.LogDebug("Initalizing Graphics.");
            
            RendererSettings<IVulkanLayer> settings = new() {
                MaxFramesInFlight = maxFramesInFlight,
                ClearValue = clearValue ?? new Color(0, 0, 0, 0)
            };

            Renderer = new VulkanRenderer(Logger, globals, ref settings, ref appInfo);

            IsDisposed = false;

            globals.windowManager.OnFramebufferResize += (width, height) => {
                Renderer.Resize(width, height);
            };
        }
        /// <summary>
        /// Updates the graphics on the screen.
        /// </summary>
        public void Update() {
            Renderer!.Render();
        }

        /// <summary>
        /// Creates a rasterization layer.
        /// </summary>
        /// <param name="settings">The default settings of the layer.</param>
        /// <returns>The created rasterization layer, if it succeeded in creating a rasterization layer.</returns>
        public IRasterizationLayer CreateRasterizationLayer(RasterizationLayerSettings settings) {
            if (Renderer == null) {
                Logger.LogError("Renderer is not initialized.");
                return default;
            }
            
            return Renderer.CreateRasterizationLayer(settings);
        }
        public IShader CreateShader(byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null) {
            if (Renderer == null) {
                Logger.LogError("Renderer is not initialized.");
                return default;
            }

            Assert.Throws(() => Renderer.CreateShader(vertexShader, fragmentShader, geometryShader))
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