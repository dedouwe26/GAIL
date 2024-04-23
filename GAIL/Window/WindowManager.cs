using GAIL.Core;
using Silk.NET.GLFW;

namespace GAIL.Window
{
    /// <summary>
    /// Handles everything for the window (GLFW).
    /// </summary>
    public class WindowManager : IManager {

        /// <summary>
        /// Creates a window manager.
        /// </summary>
        /// <exception cref="APIBackendException"></exception>
        public WindowManager() {
            API.Glfw.SetErrorCallback((ErrorCode error, string description) => throw new APIBackendException("GLFW", $"({error}): {description}"));
        }

        /// <summary></summary>
        ~WindowManager() {
            Dispose();
        }

        /// <summary>
        /// Initializes the window manager.
        /// </summary>
        /// <param name="windowName">The window name.</param>
        /// <param name="width">The width of the window (horizontal).</param>
        /// <param name="height">The height of the window (vertical).</param>
        /// <exception cref="APIBackendException"></exception>
        public void Init(string windowName, int width, int height) {
            unsafe {
                API.Glfw.SetErrorCallback((ErrorCode error, string description) => {
                    throw new APIBackendException("GLFW", error.ToString()+": "+description);
                });
            }

            if (!API.Glfw.Init())
            {
                throw new APIBackendException("GLFW", "initialization failed!");
            }

            API.Glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);

            unsafe {
                Window = API.Glfw.CreateWindow(width, height, windowName, null, null);
            }
            if (Window.IsNull) {
                throw new APIBackendException("GLFW", "window creation failed!");
            }
        }

        /// <summary>
        /// The GLFW window instance for custom usage.
        /// </summary>
        public Pointer<WindowHandle> Window { get; private set; } = Pointer<WindowHandle>.FromNull();

        /// <summary>
        /// Updates GLFW (polls events).
        /// </summary>
        public void Update() {
            API.Glfw.PollEvents();
        }

        /// <summary>
        /// True if the window should close.
        /// </summary>
        public bool ShouldClose{get{
            unsafe {
                return API.Glfw.WindowShouldClose(Window);
            }
        }}

        /// <summary>
        /// The current time on the GLFW Timer.
        /// </summary>
        public static double Time{ get { return API.Glfw.GetTime(); } }

        /// <inheritdoc/>
        public void Dispose() {
            unsafe {
                API.Glfw.DestroyWindow(Window);
            }
            API.Glfw.Terminate();
            GC.SuppressFinalize(this);
        }
    }
}