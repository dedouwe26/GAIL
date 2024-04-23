using System.Net;
using System.Net.Sockets;
using GAIL.Networking.Parser;

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
        List<byte> data = [];
        while (!Closed) {
            int i;
            if ((i=NetworkStream!.ReadByte())==-1) {
                break;
            }
            byte b = Convert.ToByte(i);
            if (b == PacketParser.NewPacket && data[^1]!=PacketParser.NewPacket) {
            }
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