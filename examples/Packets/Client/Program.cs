using System.Net;
using System.Runtime.Serialization;
using examples.Packets.Shared;
using GAIL.Networking;
using GAIL.Networking.Client;
using GAIL.Networking.Parser;
using GAIL.Serializing;
using OxDED.Terminal;

namespace examples.Packets.Client;

class Program {
    public static async Task Main(string[] args) {
        // Registers all three packets.
        Shared.Packets.RegisterPackets();

        // Ask for port for the client to connect on.
        Terminal.Write("Port of client: ");
        string port = Terminal.ReadLine()!;

        // Creating a client.
        ClientContainer? client = ClientContainer.Create(
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3003), // The server endpoint.
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(port)), // The local endpoint (where the client is listening).
            true // Enables logging.
        );

        // Connection failed, so return.
        if (client == null) {
            return;
        }

        // Listen to events.
        client.OnPacket+=OnPacket;
        client.OnConnect+=OnConnect;
        client.OnStop+=OnStop;

        // Don't forget to start the client.
        await client.StartAsync();
    }

    private static void OnStop(ClientContainer client) {
        // Clears whole screen
        Terminal.WriteLine("Stopped.");
    }
    private static async ValueTask ReadMessage(ClientContainer client) {
        // Send a new message back.
        Terminal.Write("Message: ");
        string message = Terminal.ReadLine()!;
        if (message.StartsWith("/exit ")) {
            // Sends custom data.
            // Field is just for dealing with strings and endianness,
            // you could also use BitConverter.
            await client.StopAsync(new StringSerializable(message[6..]).Serialize());
        } else {
            await client.SendPacketAsync(new MessagePacket(message));
        }
    }
    private static async void OnConnect(ClientContainer client) {
        // Send register packet.
        Terminal.Write("Connected. Enter name: ");
        string name = Terminal.ReadLine()!;
        // Sends a register packet with the name.
        await client.SendPacketAsync(new RegisterPacket(name));
        // Usage
        Terminal.WriteLine("Enter /exit ... to exit with text");

        await ReadMessage(client);
    }

    private static async void OnPacket(ClientContainer client, Packet packet) {
        // Check if packet is a message packet.
        if (packet is NameMessagePacket messagePacket) {
            // Append message to messages.
            Terminal.WriteLine($"<{messagePacket.name}> : {messagePacket.message}");

            await ReadMessage(client);
        }
    }
}