using System.Net;
using examples.Packets.Shared;
using GAIL.Networking;
using GAIL.Networking.Parser;
using GAIL.Networking.Server;
using OxDED.Terminal;

namespace examples.Packets.Server;

class Program {
    public static void Main(string[] args) {
        // Registers all three packets.
        Shared.Packets.RegisterPackets();

        ServerContainer server = NetworkManager.CreateServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3003));

        // Listening to events
        server.OnPacket+=OnPacket;
        server.OnDisconnect+=OnDisconnect;

        // Don't forget to start the server.
        Terminal.WriteLine("Started listening...", new Style{ ForegroundColor = Color.Green});
        server.Start();
    }

    private static void OnDisconnect(ServerContainer server, Connection connection, bool byClient, byte[] additionalData) {
        Terminal.WriteLine(connection.GetData<string>() + " left.");
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