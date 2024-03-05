using Silk.NET.GLFW;
using Silk.NET.Windowing;

namespace GAIL
{
    // /// <summary>
    // /// The metadata of the application.
    // /// </summary>
    // public struct AppInfo
    // {
    //     /// <summary>
    //     /// App name (UTF-8)
    //     /// </summary>
    //     string AppName = "App";
    //     /// <summary>
    //     /// 0: Major  1: Minor  2: Patch.
    //     /// </summary>
    //     uint[] AppVersion = { 1, 0, 0 };
    //     /// <summary>
    //     /// Engine name (UTF-8)
    //     /// </summary>
    //     string EngineName = "GAIL";
    //     /// <summary>
    //     /// 0: Major  1: Minor  2: Patch.
    //     /// </summary>
    //     ///
    //     uint[] EngineVersion = { 1, 0, 0 };

    //     public AppInfo() { }
    // }
    /// <summary>
    /// The update callback.
    /// </summary>
    public delegate void UpdateCallback(Application app, double deltaTime);
    /// <summary>
    /// The load callback.
    /// </summary>
    public delegate void LoadCallback(Application app);
    /// <summary>
    /// The stop callback.
    /// </summary>
    public delegate void StopCallback(Application app);


    /// <summary>
    /// The central part of GAIL. Includes all the Managers.
    /// </summary>
    public class Application
    {
        public unsafe Application(string windowName = "GAIL Window", int width = 1000, int height = 600) {
            Glfw glfw = Glfw.GetApi();
            glfw.SetErrorCallback((ErrorCode error, string description) => throw new APIBackendException("GLFW", $"({error}): {description}"));
            if (!glfw.Init())
            {
                throw new APIBackendException("GLFW", "initialization failed!");
            }

            glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);

            Silk.NET.GLFW.Monitor monitor = new();
            WindowHandle handle = new();
            window = glfw.CreateWindow(width, height, windowName, Pointer<Silk.NET.GLFW.Monitor>.From(monitor), Pointer<WindowHandle>.From(handle));

            if (window.GetPointer()==null) {
                throw new APIBackendException("GLFW", "window creation failed!");
            }

            graphicsManager = new();
            inputManager = new();
            audioManager = new();

            LoadEvent?.Invoke(this);

            double CurrentTime = 0;
            double lastTime = CurrentTime;
            while (!glfw.WindowShouldClose(window)) {
                glfw.PollEvents();
                CurrentTime = glfw.GetTime();
                UpdateEvent?.Invoke(this, CurrentTime - lastTime);
                lastTime = CurrentTime;
            }
            Stop();

        }
        ~Application()
        {
            Stop();
        }
        /// <summary>
        /// The graphics manager.
        /// </summary>
        public GraphicsManager graphicsManager;
        /// <summary>
        /// The input manager.
        /// </summary>
        public InputManager inputManager;
        /// <summary>
        /// The audio manager.
        /// </summary>
        public AudioManager audioManager;
        /// <summary>
        /// The update event, with delta time (in seconds): CurrentTime - PreviousFrameTime (calls every frame).
        /// </summary>
        public event UpdateCallback? UpdateEvent;
        /// <summary>
        /// The load event (calls at the start).
        /// </summary>
        public event LoadCallback? LoadEvent;
        /// <summary>
        /// The stop event (calls at close).
        /// </summary>
        public event StopCallback? StopEvent;
        /// <summary>
        /// The Silk.NET window instance for custom usage.
        /// </summary>
        public readonly Pointer<WindowHandle> window;
        /// <summary>
        /// Stops the application (some things might break if used certain functions after).
        /// </summary>
        public unsafe void Stop() {
            StopEvent?.Invoke(this);
            Glfw glfw = Glfw.GetApi();
            glfw.DestroyWindow(window);
            glfw.Terminate();
        }
    }
}
