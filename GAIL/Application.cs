using GAIL.Audio;
using GAIL.Graphics;
using GAIL.Input;
using GAIL.Window;
using OxDED.Terminal;
using OxDED.Terminal.Logging;

namespace GAIL
{
    /// <summary>
    /// The metadata of the application.
    /// </summary>
    public struct AppInfo {
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

        /// <summary></summary>
        public AppInfo() { }
    }

    /// <summary>
    /// The central part of GAIL. Includes all the Managers.
    /// </summary>
    public class Application : IDisposable {

        /// <summary>
        /// Stores all managers and loggers for this application.
        /// </summary>
        public struct Globals {
            /// <summary>
            /// The graphics manager of this application.
            /// </summary>
            public GraphicsManager graphicsManager;
            /// <summary>
            /// The audio manager of this application.
            /// </summary>
            public AudioManager audioManager;
            /// <summary>
            /// The input manager of this application.
            /// </summary>
            public InputManager inputManager;
            /// <summary>
            /// The window manager of this application.
            /// </summary>
            public WindowManager windowManager;
            /// <summary>
            /// The main logger for this application.
            /// </summary>
            public Logger logger;
        }
        /// <summary>
        /// A boolean to indicate if the application has stopped.
        /// </summary>
        public bool hasStopped { get; private set; }

        /// <summary>
        /// Creates a GAIL application.
        /// </summary>
        /// <param name="windowTitle">The name of the window (also used for the <paramref name="loggerID"/>).</param>
        /// <param name="width">The width of the window (in pixels).</param>
        /// <param name="height">The height of the window (in pixels).</param>
        /// <param name="loggerID">The ID for the logger (default: GAIL.App.{windowTitle replace ' ' with '_'}).</param>
        /// <param name="appInfo">The app info used for vulkan (null for default).</param>
        /// <param name="audioDevice">The custom audio device if necessary (default: empty).</param>
        /// <param name="logLevel">The log level of all the loggers in this application (default: Info).</param>
        /// <param name="logTargets">All the log targets for all the loggers in this application (default: TerminalTarget).</param>
        public Application(string windowTitle = "GAIL Window", int width = 1000, int height = 600, AppInfo? appInfo = null, string audioDevice = "", string? loggerID = null, Severity logLevel = Severity.Info, Dictionary<Type, ITarget>? logTargets = null) {  
            hasStopped = true;
            
            globals = new() {
                logger = new Logger(loggerID??"GAIL.App."+windowTitle.Replace(' ', '_'), "GAIL", logLevel, logTargets??new(){[typeof(TerminalTarget)] = new TerminalTarget()})
            };
            if (globals.logger.HasTarget<TerminalTarget>()) {
                globals.logger.GetTarget<TerminalTarget>().Format = "<{0}>: ("+Color.DarkBlue.ToForegroundANSI()+"{2}"+ANSI.Styles.ResetAll+")[{5}"+ANSI.Styles.Bold+"{3}"+ANSI.Styles.ResetAll+"] : {5}{4}"+ANSI.Styles.ResetAll;
                globals.logger.GetTarget<TerminalTarget>().NameFormat =  "{0} - {1}";
            }
            if (globals.logger.HasTarget<FileTarget>()) {
                globals.logger.GetTarget<FileTarget>().Format = "<{0}>: ({2})[{3}] : {4}";
                globals.logger.GetTarget<FileTarget>().NameFormat =  "{0} - {1}";
            }

            globals.logger.LogDebug("Initializing all managers.");

            globals.windowManager = new WindowManager(globals.logger.CreateSubLogger("Window", "Window", logLevel));
            globals.windowManager.Init(windowTitle, width, height);

            globals.inputManager = new InputManager(globals, globals.logger.CreateSubLogger("Input", "Input", logLevel));
            globals.inputManager.Init();

            globals.graphicsManager = new GraphicsManager(globals.logger.CreateSubLogger("Graphics", "Graphics", logLevel));
            globals.graphicsManager.Init(globals, appInfo == null? new AppInfo() : appInfo.Value);

            globals.audioManager = new AudioManager(globals.logger.CreateSubLogger("Audio", "Audio", logLevel));
            globals.audioManager.Init(audioDevice);
        }

        /// <summary>
        /// Starts the application, use after subscribing on events.
        /// </summary>
        public void Start() {
            hasStopped = false;

            Logger.LogInfo("Starting...");
            OnLoad?.Invoke(this);

            double CurrentTime; // Could also be WindowManager.Time
            double lastTime = WindowManager.Time;

            while (!hasStopped) {
                CurrentTime = WindowManager.Time;

                double deltaTime = CurrentTime - lastTime;

                globals.windowManager.Update();

                if (hasStopped) { break; }
                if (globals.windowManager.ShouldClose) { break; }
            
                OnUpdate?.Invoke(this, deltaTime);
                
                lastTime = CurrentTime;
                
            }

            Dispose();
        }
        /// <summary>
        /// Disposes this application.
        /// </summary>
        ~Application() {
            Dispose();
        }
        /// <summary>
        /// The Globals of this Application, contains all the managers.
        /// </summary>
        public readonly Globals globals;
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
        /// The logger of this application.
        /// </summary>
        public Logger Logger {get {return globals.logger;}}

        /// <summary>
        /// The update event (calls every frame).
        /// </summary>
        public event UpdateCallback? OnUpdate;
        /// <summary>
        /// The load event (calls at the start).
        /// </summary>
        public event LoadCallback? OnLoad;
        /// <summary>
        /// The stop event (calls at disposal).
        /// </summary>
        public event StopCallback? OnStop;

        /// <summary>
        /// Stops the application (see: <see cref="Dispose"/>).
        /// </summary>
        public void Stop() {
            Dispose();
        }

        /// <inheritdoc/>
        /// <remarks>Stops the application (don't use this application after disposal).</remarks>
        public void Dispose() {
            if (hasStopped) {return;}

            hasStopped = true;

            Logger.LogInfo("Stopping...");
            OnStop?.Invoke(this);

            globals.audioManager.Dispose();
            globals.graphicsManager.Dispose();
            globals.inputManager.Dispose();
            globals.windowManager.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

