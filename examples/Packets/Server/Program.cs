using System.Net;
using examples.Packets.Shared;
using GAIL.Networking;
using GAIL.Networking.Server;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using OxDED.Terminal;

namespace examples.Packets.Server;

class Program {
    private static readonly ManualResetEvent mre = new(false);
    public static void Main(string[] args) {
        // Registers all three packets.
        Shared.Packets.RegisterPackets();

        ServerContainer server = ServerContainer.Create(
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3003), // The endpoint to listen on.
            true // Enables logging.
        );

        // Listening to events
        server.OnPacket+=OnPacket;
        server.OnDisconnect+=OnDisconnect;

        // Applies a formatter for all packets (note that is must happen both on the client and the server).
        // server.GlobalFormatter = new GZipFormatter();

        // Stop when key pressed.
        Terminal.OnKeyPress+=async (ConsoleKey key, char ch, bool alt, bool shift, bool control) => {
            await server.StopAsync();
            mre.Set();
        };

        // Don't forget to start the server.
        Terminal.WriteLine("Started listening...", new Style{ ForegroundColor = Color.Green});

        // Starts listening (non-thread blocking).
        server.Start();

        Terminal.ListenForKeys = true;

        mre.WaitOne(); // Block the thread.
    }

    private static void OnDisconnect(ServerContainer server, Connection connection, bool byClient, byte[] additionalData) {
        StringSerializable ss = new(string.Empty);
        ss.Parse(additionalData);
        Terminal.WriteLine(connection.GetData<string>() + " left with message: " + ss.Value);
    }
    
    private static void OnPacket(ServerContainer server, Connection connection, Packet packet) {
        if (packet is RegisterPacket registerPacket) {

            // Checks if there already is a name set (registered).
            if (connection.GetData<string>() == null) {
                // Sets the data of the connection.
                connection.SetData(registerPacket.name);
                Terminal.WriteLine(registerPacket.name + " joined.");
            }
        } else if (packet is MessagePacket messagePacket) {
            // Ensure that the client is registered.
            if (connection.data == null) { return; }

            Terminal.WriteLine($"<{connection.GetData<string>()}> : {messagePacket.message}");

            // Send a NameMessagePacket back to all the clients.
            server.BroadcastPacket(new NameMessagePacket(connection.GetData<string>()!, messagePacket.message));
        }
    }
}