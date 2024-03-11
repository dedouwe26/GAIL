using GAIL.Core;
using Silk.NET.GLFW;

namespace GAIL.Window
{
    public class WindowManager : IManager {
        public WindowManager(string windowName, int width, int height) {
            glfw = Glfw.GetApi();
            glfw.SetErrorCallback((ErrorCode error, string description) => throw new APIBackendException("GLFW", $"({error}): {description}"));
            if (!glfw.Init())
            {
                throw new APIBackendException("GLFW", "initialization failed!");
            }

            glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);

            Silk.NET.GLFW.Monitor monitor = new();
            WindowHandle handle = new();
            unsafe {
                window = glfw.CreateWindow(width, height, windowName, Pointer<Silk.NET.GLFW.Monitor>.From(monitor), Pointer<WindowHandle>.From(handle));

                if (window.GetPointer()==null) {
                    throw new APIBackendException("GLFW", "window creation failed!");
                }
            }
        }
        ~WindowManager() {
            Dispose();
        }

        /// <summary>
        /// The GLFW window instance for custom usage.
        /// </summary>
        public readonly Pointer<WindowHandle> window;
        /// <summary>
        /// The GLFW API instance for custom usage.
        /// </summary>
        public readonly Glfw glfw;

        public void Update() {
            glfw.PollEvents();
        }

        public bool ShouldClose{get{
            unsafe {
                return glfw.WindowShouldClose(window);
            }
        }}

        public double Time{get{
            return glfw.GetTime();
        }}

        public void Dispose() {
            Glfw glfw = Glfw.GetApi();
            unsafe {
                glfw.DestroyWindow(window);
            }
            glfw.Terminate();
        }
    }
}