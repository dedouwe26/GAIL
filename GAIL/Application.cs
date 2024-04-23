using GAIL.Audio;
using GAIL.Graphics;
using GAIL.Input;
using GAIL.Window;
using Silk.NET.OpenGL;

namespace GAIL
{
    /// <summary>
    /// The metadata of the application.
    /// </summary>
    public struct AppInfo
    {
        /// <summary>
        /// App name (UTF-8)
        /// </summary>
        public string AppName = "App";
        /// <summary>
        /// 0: Major  1: Minor  2: Patch.
        /// </summary>
        public uint[] AppVersion = [1, 0, 0];
        /// <summary>
        /// Engine name (UTF-8)
        /// </summary>
        public string EngineName = "GAIL";
        /// <summary>
        /// 0: Major  1: Minor  2: Patch.
        /// </summary>
        ///
        public uint[] EngineVersion = [1, 0, 0];

        public AppInfo() { }
    }



    /// <summary>
    /// The central part of GAIL. Includes all the Managers.
    /// </summary>
    public class Application : IDisposable
    {

        /// <summary>
        /// Stores all the managers for other managers.
        /// </summary>
        public struct Globals
        {
            public GraphicsManager graphicsManager;
            public AudioManager audioManager;
            public InputManager inputManager;
            public WindowManager windowManager;
        }

        /// <summary>
        /// Creates an 3D / 2D application.
        /// </summary>
        /// <param name="windowName">The name of the window.</param>
        /// <param name="width">The width of the window (in pixels).</param>
        /// <param name="height">The height of the window (in pixels).</param>
        /// <param name="appInfo">The app info used for vulkan (null for default).</param>
        /// <param name="audioDevice">The custom audio device if necessary</param>
        public Application(string windowName = "GAIL Window", int width = 1000, int height = 600, AppInfo? appInfo = null, string audioDevice = "") {
            globals = new() { windowManager = new WindowManager() };
            globals.windowManager.Init(windowName, width, height);
            globals.inputManager = new InputManager(globals);
            globals.inputManager.Init();
            globals.graphicsManager = new GraphicsManager();
            globals.graphicsManager.Init(globals, appInfo == null? new AppInfo() : appInfo.Value);
            globals.audioManager = new AudioManager();
            globals.audioManager.Init(audioDevice);
        }
        
        /// <summary>
        /// Starts the application, use after subscribing on events.
        /// </summary>
        public void Start() {
            // globals.inputManager.Init();

            OnLoad?.Invoke(this);

            double CurrentTime = 0;
            double lastTime = CurrentTime;
            unsafe
            {
                while (!globals.windowManager.ShouldClose) {
                    globals.windowManager.Update();
                    CurrentTime = WindowManager.Time;
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
        /// The manager for the graphics.
        /// </summary>
        public GraphicsManager GraphicsManager {get {return globals.graphicsManager;}}
        /// <summary>
        /// The manager for the audio.
        /// </summary>
        public AudioManager AudioManager {get {return globals.audioManager;}}
        /// <summary>
        /// The manager for the input.
        /// </summary>
        public InputManager InputManager {get {return globals.inputManager;}}
        /// <summary>
        /// The manager for the window.
        /// </summary>
        public WindowManager WindowManager {get {return globals.windowManager;}}

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
        public void Stop()
        {
            OnStop?.Invoke(this);
            globals.audioManager.Dispose();
            globals.graphicsManager.Dispose();
            globals.inputManager.Dispose();

            globals.windowManager.Dispose();
        }

        public void Dispose() {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}

