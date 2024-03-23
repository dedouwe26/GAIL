namespace examples.FromFiles
{
    public class Program {
        static GAIL.Application? app;
        public static void Main(string[] args) {
            // Initializes the application.
            app = new GAIL.Application("From files");

            // Adds listeners to all events
            app.OnLoad+=Load;
            app.OnUpdate+=Update;
            app.OnStop+=Stop;

            app.Start();
        }

        public static void Load(GAIL.Application app) {
            
        }
        public static void Update(GAIL.Application app, double deltaTime) {

        }
        public static void Stop(GAIL.Application app) {

        }
    }
}