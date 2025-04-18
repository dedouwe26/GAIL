﻿using System.Net;
using examples.Packets.Shared;
using GAIL.Networking;
using GAIL.Networking.Client;
using GAIL.Serializing;
using OxDED.Terminal;

// Registers all three packets.
Packets.RegisterPackets();

// Ask for port for the client to connect on.
Terminal.Write("Port of client: ");
int port = int.Parse(Terminal.ReadLine()!);

// Creating a client.
ClientContainer? client = ClientContainer.Create(
    new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3003), // The server endpoint.
    new IPEndPoint(IPAddress.Parse("127.0.0.1"), port), // The local endpoint (where the client is listening).
    true // Enables logging.
);

// Connection failed, so quit.
if (client == null) {
    Terminal.WriteLine("Connection failed");
    return;
}

// Listen to events.
client.OnPacket+=OnPacket;
client.OnConnect+=OnConnect;
client.OnPacketSent+=OnPacketSent;
client.OnStop+=OnStop;

Terminal.OnKeyPress += async (key, keyChar, alt, shift, control) => {
    // Sends a packet when a key has been pressed.
    
    if (control && key == ConsoleKey.X) { // Stopping when CTRL+X has been pressed.
        Terminal.WriteLine("Stopping...");
        await client.StopAsync(new StringSerializable("Bye!").Serialize());
    } else {
        client.SendPacket(new MessagePacket("test"));
    }
};

Terminal.ListenForKeys = true;

// Applies a formatter for all packets (note that is must happen both on the client and the server).
// server.GlobalFormatter = new GZipFormatter();

// Don't forget to start the client.
await client.StartAsync();


static void OnStop(ClientContainer client) {
    Terminal.WriteLine("Stopped.");
    Terminal.ListenForKeys = false;
}

// NOTE: Could also use OnDisconnect.
static void OnPacketSent(ClientContainer client, Packet packet) {
    if (packet is DisconnectPacket) {
        Terminal.WriteLine("Disconnecting...");
    }
}

static async void OnConnect(ClientContainer client) {
    // Send register packet.
    Terminal.Write("Connected. Enter name: ");
    string name = Terminal.ReadLine()!;
    // Sends a register packet with the name.
    await client.SendPacketAsync(new RegisterPacket(name));
}

static void OnPacket(ClientContainer client, Packet packet) {
    // Check if packet is a message packet.
    if (packet is NameMessagePacket messagePacket) {
        // Append message to messages.
        Terminal.WriteLine($"<{messagePacket.name}> : {messagePacket.message}");
    }
}