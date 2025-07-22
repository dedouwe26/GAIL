using System.Net;
using examples.Packets.Shared;
using GAIL.Networking;
using GAIL.Networking.Server;
using GAIL.Serializing;
using LambdaKit.Terminal;

namespace examples.Packets.Server;

class Program {
    private static readonly string serverName = new StyleBuilder().Foreground((StandardColor)StandardColor.Colors.Blue).Text("System").ResetForeground().ToString();
    public static void Main() {
        // Registers all three packets.
        Shared.Packets.RegisterPackets();

        ServerContainer server = ServerContainer.Create(
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3003), // The endpoint to listen on.
            true // Enables logging.
        );

        // Listening to events
        server.OnPacket+=OnPacket;
        server.OnStop+=OnStop;
        server.OnDisconnect+=OnDisconnect;

        // Applies a formatter for all packets (note that is must happen both on the client and the server).
        // server.GlobalFormatter = new GZipFormatter();

        // Stop when key pressed.
        Terminal.OnKeyPress+=async (key, ch, alt, shift, control) => {
            await server.StopAsync();
        };

        // Don't forget to start the server.
        Terminal.WriteLine("Started listening...", new Style{ ForegroundColor = (StandardColor)StandardColor.Colors.Green});

        // Starts listening (non-thread blocking).
        server.Start();

        Terminal.ListenForKeys = true; // Thread blocking.
    }

    private static void OnDisconnect(ServerContainer server, Connection connection, bool byClient, byte[] additionalData) {
        string text;
        // If the client has been kicked by this server.
        if (!byClient) {
            text = connection.GetData<string>() + " has been kicked.";
            Terminal.WriteLine(text);
            
            // Notifies all the other clients.
            server.BroadcastPacket(new NameMessagePacket(serverName, text));
            return;
        }
        StringSerializable msg = new("");
        msg.Parse(additionalData);
        text = connection.GetData<string>() + " left with message: '" + msg.Value + "'.";
        Terminal.WriteLine(text);

        // Notifies all the other clients.
        server.BroadcastPacket(new NameMessagePacket(serverName, text));
    }

    private static void OnStop(ServerContainer server) {
        Terminal.WriteLine("Stopped.");
        Terminal.ListenForKeys = false;
    }
    
    private static void OnPacket(ServerContainer server, Connection connection, Packet packet) {
        if (packet is RegisterPacket registerPacket) {
            // Checks if there already is a name set (registered).
            if (connection.GetData<string>() == null) {
                // Sets the data of the connection.
                connection.SetData(registerPacket.name);

                string text = registerPacket.name + " joined.";
                Terminal.WriteLine(text);

                // Notifies all the other clients.
                server.BroadcastPacket(new NameMessagePacket(serverName, text));
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