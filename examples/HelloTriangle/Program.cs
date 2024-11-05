using System.Diagnostics;
using GAIL.Input;
using OxDED.Terminal;
using OxDED.Terminal.Logging;
using Silk.NET.Vulkan;

namespace examples.HelloTriangle
{
    public class Program {
        static GAIL.Application? app;
        static readonly Logger Logger = new(
            "examples.HelloTriangle",
            "Hello Triangle",
            Severity.Trace,
            new(){
                [typeof(TerminalTarget)] = new TerminalTarget()
            }
        );

        public static void Main(string[] args) {
            Logger.LogDebug("Creating Application instance.");

            // Initializes the application.
            app = new GAIL.Application("Hello Triangle", severity:Severity.Debug);

            Logger.LogDebug("Applying listeners.");

            // Adds listeners to all events.
            app.OnLoad+=Load;
            app.OnUpdate+=Update;
            app.OnStop+=Stop;
            app.Initialize(windowSettings:("Hello Triangle", 1000, 600));

            Logger.LogDebug("Starting Application.");

            // Locks thread (until the it has stopped). And starts everything.
            app.Start();

            app.Stop();
        }
        public static void Load(GAIL.Application app) {
            Logger.LogInfo("Loading...");

            // Listens for a key press event.
            app.InputManager.OnKeyDown+=(Key key)=>{
                if (key == Key.Escape) {
                    app.Stop();
                }
            };

            // Add listeners for graphics settings.
            app.InputManager.OnKeyDown += (Key key) => {
                if (key == Key.Equals) {
                    app.GraphicsManager.Settings.MaxFramesInFlight++;

                    Logger.LogDebug("Max frames in flight is set to: "+app.GraphicsManager.Settings.MaxFramesInFlight);
                } else if (key == Key.Minus) {
                    app.GraphicsManager.Settings.MaxFramesInFlight--;
                    
                    Logger.LogDebug("Max frames in flight is set to: "+app.GraphicsManager.Settings.MaxFramesInFlight);
                }

                else if (key == Key.Space) {
                    app.GraphicsManager.Settings.ShouldRender = !app.GraphicsManager.Settings.ShouldRender;

                    Logger.LogDebug("Should render is set to: "+app.GraphicsManager.Settings.ShouldRender);
                }
            };
        }
        private static uint frameCount = 0;
        private static double timePassed = 0;
        public static void Update(GAIL.Application app, double deltaTime) {
            // Counts the amount of updates (frameCount) in one second (timepassed == 1).
            timePassed += deltaTime;
            frameCount++;

            if (timePassed >= 1) {
                Logger.LogInfo("FPS: "+frameCount);
                frameCount = 0;
                timePassed = 0;
            }
        }
        public static void Stop(GAIL.Application app) {
            Logger.LogInfo("Stopping...");
        }
    }
}