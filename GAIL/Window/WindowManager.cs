using GAIL.Core;
using Silk.NET.GLFW;

namespace GAIL.Window
{
    public class WindowManager : IManager {
        public WindowManager() {
            glfw = Glfw.GetApi();
            glfw.SetErrorCallback((ErrorCode error, string description) => throw new APIBackendException("GLFW", $"({error}): {description}"));
            
        }
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
                glfw.SetErrorCallback((ErrorCode error, string description) => {
                    throw new APIBackendException("GLFW", error.ToString()+": "+description);
                });
            }

            if (!glfw.Init())
            {
                throw new APIBackendException("GLFW", "initialization failed!");
            }

            glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);

            unsafe {
                Window = glfw.CreateWindow(width, height, windowName, null, null);
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
        /// The GLFW API instance for custom usage.
        /// </summary>
        public readonly Glfw glfw;

        public void Update() {
            glfw.PollEvents();
        }

        public bool ShouldClose{get{
            unsafe {
                return glfw.WindowShouldClose(Window);
            }
        }}

        public double Time{get{
            return glfw.GetTime();
        }}

        public void Dispose() {
            unsafe {
                glfw.DestroyWindow(Window);
            }
            glfw.Terminate();
            glfw.Dispose();
        }
    }
}