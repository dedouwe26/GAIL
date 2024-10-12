using System.Diagnostics;
using GAIL.Input;
using OxDED.Terminal;
using OxDED.Terminal.Logging;

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
            app = new GAIL.Application("Hello Triangle", 1000, 600, severity:Severity.Debug);

            Logger.LogDebug("Applying listeners.");

            // Adds listeners to all events.
            app.OnLoad+=Load;
            app.OnUpdate+=Update;
            app.OnStop+=Stop;

            Logger.LogDebug("Starting Application.");

            // Locks thread. And starts everything.
            app.Start();
        }
        public static void Load(GAIL.Application app) {
            Logger.LogInfo("Loading...");

            // Listens for a key press event.
            app.InputManager.OnKeyDown+=(Key key)=>{
                if (key == Key.Escape) {
                    app.Stop();
                }
            };
        }
        private static uint frameCount = 0;
        private static double timePassed = 0;
        public static void Update(GAIL.Application app, double deltaTime) {
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