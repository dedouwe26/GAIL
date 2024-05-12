using System.Net;
using examples.Packets.Shared;
using GAIL.Networking;
using GAIL.Networking.Client;
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
        ClientContainer client = NetworkManager.CreateClient(
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3003), // The server endpoint.
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(port)) // The local endpoint (where the client is listening).
        );

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

    private static async void OnConnect(ClientContainer client) {
        // Send register packet.
        Terminal.Write("Connected. Enter name: ");
        string name = Terminal.ReadLine()!;
        await client.SendPacketAsync(new RegisterPacket(name));

        // Send a new message back.
        Terminal.Write("Message: ");
        await client.SendPacketAsync(new MessagePacket(Terminal.ReadLine()!));
    }

    private static void OnPacket(ClientContainer client, Packet packet) {
        // Check if packet is a message packet.
        if (packet is NameMessagePacket messagePacket) {
            // Append message to messages.
            Terminal.WriteLine($"<{messagePacket.name}> : {messagePacket.message}");

            // Send a new message back.
            Terminal.Write("Message: ");
            client.SendPacket(new MessagePacket(Terminal.ReadLine()!));
        }
    }
}