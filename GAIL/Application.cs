using GAIL.Audio;
using GAIL.Graphics;
using GAIL.Input;
using GAIL.Window;
using OxDED.Terminal;
using OxDED.Terminal.Logging;
using OxDED.Terminal.Logging.Targets;

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
        /// A boolean to indicate if the application has stopped and will be able to resume.
        /// </summary>
        public bool HasStopped { get; set; }
        /// <summary>
        /// A boolean to indicate if the application is disposed and it is not able to be run again.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Creates a GAIL application.
        /// </summary>
        /// <param name="logger">The optional logger to use.</param>
        /// <param name="severity">The severity to use if the logger isn't specified (defaults to Info).</param>
        public Application(Logger? logger = null, Severity severity = Severity.Info) {  
            HasStopped = true;
            IsDisposed = true;
            
            globals = new() {
                logger = logger ?? new Logger(name:"GAIL", severity:severity, targets:new(){[typeof(TerminalTarget)] = new TerminalTarget()})
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

            globals.windowManager = new WindowManager(globals.logger.CreateSubLogger("Window", "Window", false, globals.logger.logLevel));

            globals.inputManager = new InputManager(globals, globals.logger.CreateSubLogger("Input", "Input", false, globals.logger.logLevel));

            globals.graphicsManager = new GraphicsManager(globals.logger.CreateSubLogger("Graphics", "Graphics", false, globals.logger.logLevel));

            globals.audioManager = new AudioManager(globals.logger.CreateSubLogger("Audio", "Audio", false, globals.logger.logLevel));
        }
        /// <summary>
        /// Initializes the application after the settings have been applied.
        /// </summary>
        /// <param name="windowSettings">The settings of the window (default: "GAIL Window", 1000, 600).</param>
        /// <param name="graphicsSettings">The settings for the renderer (default: 2, (0, 0, 0, 0)).</param>
        /// <param name="appInfo">The app info used for vulkan (null for default).</param>
        /// <param name="audioDevice">The custom audio device if necessary (defaults to empty).</param>
        public void Initialize(AppInfo? appInfo = null, (string windowTitle, int width, int height)? windowSettings = default, (uint maxFramesInFlight, Core.Color clearValue)? graphicsSettings = default, string audioDevice = "") {
            globals.windowManager.Initialize(windowSettings?.windowTitle ?? "GAIL Window", windowSettings?.width ?? 1000, windowSettings?.height ?? 600);
            
            globals.inputManager.Initialize();

            AppInfo appInfoRef = appInfo ?? new AppInfo();
            globals.graphicsManager.Initialize(globals, ref appInfoRef, graphicsSettings?.maxFramesInFlight ?? 2, graphicsSettings?.clearValue);

            globals.audioManager.Initialize(audioDevice);
        }

        /// <summary>
        /// Starts the application, use after subscribing on events.
        /// </summary>
        public void Start() {
            if (!IsDisposed) {
                Logger.LogError("Cannot start the application if it is disposed or initialized.");
                return;
            }

            HasStopped = false;

            Logger.LogInfo("Starting...");
            OnLoad?.Invoke(this);

            double CurrentTime; // Could also be WindowManager.Time
            double lastTime = WindowManager.Time;

            while (!HasStopped) {
                CurrentTime = WindowManager.Time;

                double deltaTime = CurrentTime - lastTime;

                globals.windowManager.Update();

                if (HasStopped) { break; }
                if (globals.windowManager.ShouldClose) { break; }
            
                OnUpdate?.Invoke(this, deltaTime);

                globals.graphicsManager.Update();
                
                lastTime = CurrentTime;
                
            }

            HasStopped = true;

            Logger.LogInfo("Stopped.");
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
            if (IsDisposed) {return;}

            Logger.LogInfo("Disposing...");
            OnStop?.Invoke(this);

            globals.audioManager.Dispose();
            globals.graphicsManager.Dispose();
            globals.windowManager.Dispose();

            IsDisposed = true;

            Logger.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

