using GAIL.Input;

namespace examples.HelloTriangle
{
    public class Program {
        static GAIL.Application? app;
        public static void Main(string[] args) {
            // Initializes the application.
            app = new GAIL.Application("Hello Triangle");

            // Adds listeners to all events.
            app.OnLoad+=Load;
            app.OnUpdate+=Update;
            app.OnStop+=Stop;

            // Locks thread. And starts everything.
            app.Start();
        }
        public static void Load(GAIL.Application app) {
            Console.WriteLine("Loading...");
            // Listens for a key press event.
            app.InputManager.OnKeyDown+=(Key key)=>{
                if (key == Key.Escape) {
                    app.Stop();
                }
            };
        }
        public static void Update(GAIL.Application app, double deltaTime) { }
        public static void Stop(GAIL.Application app) {
            Console.WriteLine("Stopping...");
        }
    }
}