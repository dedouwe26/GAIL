using System.Net;
using System.Net.Sockets;

namespace GAIL.Networking.Client;

public class ClientContainer : IDisposable {
    public static async Task<ClientContainer> Create(IPEndPoint ip) {
        ClientContainer client = new();
        await client.tcpClient.ConnectAsync(ip);
        client.OnConnect?.Invoke(client);
        client.NetworkStream = client.tcpClient.GetStream();
        client.readThread = new Thread(client.Listen);
        client.readThread.Start();
        return client;
    }
    private Thread? readThread;
    public readonly TcpClient tcpClient;
    public NetworkStream? NetworkStream {get; private set;}
    public event PacketCallback? OnPacket;
    public event ConnectCallback? OnConnect;
    public event StopCallback? OnStop;
    public event PacketSentCallback? OnPacketSent;
    public bool Closed {get; private set;}
    private ClientContainer() {
        Closed = false;
        tcpClient = new TcpClient();
    }
    private void Listen() {
        while (!Closed) {
            NetworkStream!.ReadByte();
        }
    }
    public void SendPacket(Packet packet) {
        OnPacketSent?.Invoke(this, packet);
    }

    public void Stop() {
        Dispose();
    }
    public void Dispose() {
        Closed = true;
        OnStop?.Invoke(this);
        NetworkStream!.Close();
        tcpClient.Close();
        GC.SuppressFinalize(this);
    }
}