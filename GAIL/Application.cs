using GAIL.Audio;
using GAIL.Graphics;
using GAIL.Input;
using GAIL.Window;

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
    /// The central part of GAIL. Includes all the Managers.
    /// </summary>
    public class Application : IDisposable {

        /// <summary>
        /// Stores all the managers for other managers.
        /// </summary>
        public struct Globals {
            public GraphicsManager graphicsManager;
            public AudioManager audioManager;
            public InputManager inputManager;
            public WindowManager windowManager;
        }

        /// <param name="windowName">The name of the window.</param>
        /// <param name="width">The width of the window (in pixels).</param>
        /// <param name="height">The height of the window (in pixels).</param>
        /// <exception cref="APIBackendException">GLFW: Initialization, window creation.</exception>
        public Application(string windowName = "GAIL Window", int width = 1000, int height = 600) {
            globals = new Globals{
                graphicsManager = new GraphicsManager(),
                audioManager = new AudioManager(),
                inputManager = new InputManager(globals),
                windowManager = new WindowManager(windowName, width, height)
            };

            OnLoad?.Invoke(this);

            double CurrentTime = 0;
            double lastTime = CurrentTime;
            unsafe {
                while (!globals.windowManager.ShouldClose) {
                    
                    CurrentTime = globals.windowManager.Time;
                    OnUpdate?.Invoke(this, CurrentTime - lastTime);
                    lastTime = CurrentTime;
                }
            }
            
            Stop();

        }
        ~Application()
        {
            Stop();
        }
        /// <summary>
        /// The Globals of this Application, contains all the managers.
        /// </summary>
        public Globals globals;

        /// <summary>
        /// The update event, with delta time (in seconds): CurrentTime - PreviousFrameTime (calls every frame).
        /// </summary>
        public event UpdateCallback? OnUpdate;
        /// <summary>
        /// The load event (calls at the start).
        /// </summary>
        public event LoadCallback? OnLoad;
        /// <summary>
        /// The stop event (calls at close).
        /// </summary>
        public event StopCallback? OnStop;
        
        /// <summary>
        /// Stops the application (some things might break if used certain functions after).
        /// </summary>
        public void Stop() {
            OnStop?.Invoke(this);
            globals.audioManager.Dispose();
            globals.graphicsManager.Dispose();
            globals.inputManager.Dispose();
            
            globals.windowManager.Dispose();
        }

        public void Dispose() {
            Stop();
        }
    }
}
