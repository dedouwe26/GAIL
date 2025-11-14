using GAIL.Core;
using GAIL.Graphics.Material;
using GAIL.Graphics.Renderer;
using GAIL.Graphics.Renderer.Layer;
using GAIL.Graphics.Renderer.Vulkan.Layer;
using LambdaKit.Logging;
using VulkanRenderer = GAIL.Graphics.Renderer.Vulkan.Renderer;

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
        /// <param name="initialSettings">The inital graphical settings to start with. This is useful to set for when you change settings on startup. The default defaults to the default settings.</param> // What the...
        /// <exception cref="APIBackendException"></exception>
        public void Initialize(Application.Globals globals, AppInfo appInfo, RendererSettings<IBackendLayer>? initialSettings = null) {
            Logger.LogDebug("Initalizing Graphics.");

			initialSettings ??= new();

            // TODO: Convert initial settings.
            // NOTE: How does one create the initial settings??
            //       Including layers??

			Renderer = new VulkanRenderer(LoggerFactory.CreateSublogger(Logger, "Renderer", "renderer"), globals, initialSettings, appInfo);

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
        /// <exception cref="NullReferenceException"/>
        public IRasterizationLayer CreateRasterizationLayer(RasterizationLayerSettings settings) {
            if (Renderer == null) {
                Logger.LogError("Renderer is not initialized.");
                throw new NullReferenceException("Renderer is not initialized.");
            }
            
            return Renderer.CreateRasterizationLayer(settings) ?? throw new Exception("Failed to make a rasterization layer");
        }
        /// <summary>
        /// Creates a shader.
        /// </summary>
        /// <param name="requiredAttributes">The attributes required by the following vertex shader.</param>
        /// <param name="requiredUniforms">The uniforms required by the following shaders.</param>
        /// <param name="vertexShader">The vertex shader in SPIR-V byte code (per-vertex).</param>
        /// <param name="fragmentShader">The fragment shader in SPIR-V byte code (per-vertex).</param>
        /// <param name="geometryShader">The geometry shader in SPIR-V byte code.</param>
        /// <returns>The created shader.</returns>
        /// <exception cref="NullReferenceException"></exception>
        public IShader CreateShader(FormatInfo[] requiredAttributes, FormatInfo[] requiredUniforms, byte[] vertexShader, byte[]? fragmentShader = null, byte[]? geometryShader = null) {
            if (Renderer == null) {
                Logger.LogError("Renderer is not initialized.");
                throw new NullReferenceException("Renderer is not initialized");
            }

            return Renderer.CreateShader(requiredAttributes, requiredUniforms, vertexShader, fragmentShader, geometryShader) ?? throw new Exception("Failed to make a shader");
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