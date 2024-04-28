using System.Diagnostics;
using GAIL.Input;
using OxDED.Terminal.Logging;

namespace examples.HelloTriangle
{
    public class Program {
        static GAIL.Application? app;
        static Dictionary<Type, ITarget> targets = new(){[typeof(TerminalTarget)] = new TerminalTarget()};
        static readonly Logger Logger = new("examples.HelloTriangle", "Hello Triangle", Severity.Trace, targets);
        public static void Main(string[] args) {
            Logger.LogDebug("Creating Application instance.");

            // Initializes the application.
            app = new GAIL.Application("Hello Triangle", 1000, 600, null, "", null, Severity.Trace, targets);

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
        public static void Update(GAIL.Application app, double deltaTime) { }
        public static void Stop(GAIL.Application app) {
            Logger.LogInfo("Stopping...");
        }
    }
}