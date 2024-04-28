using System.Net;
using System.Net.Sockets;
using GAIL.Networking.Parser;

namespace GAIL.Networking.Client;

/// <summary>
/// This is a client that creates a connection to the server.
/// </summary>
public class ClientContainer : IDisposable {
    /// <summary>
    /// Creates a client.
    /// </summary>
    /// <param name="server">The server end point (IP address and port).</param>
    /// <param name="local">The local end point (for different port, etc).</param>
    /// <returns>The created client.</returns>
    public static ClientContainer Create(IPEndPoint server, IPEndPoint? local = null) {
        return new(server, local);
    }
    /// <summary>
    /// The server end point (IP address and port).
    /// </summary>
    public readonly IPEndPoint Server;
    /// <summary>
    /// The local end point or the end point the server sees (IP address and port, for different port or different IP address).
    /// </summary>
    public readonly IPEndPoint? IP;
    private Thread? listenThread;
    /// <summary>
    /// The tcp client back-end.
    /// </summary>
    public readonly TcpClient tcpClient;
    /// <summary>
    /// The stream to write to.
    /// </summary>
    public NetworkStream? NetworkStream {get; private set;}
    /// <summary>
    /// An event that is called when a packet is received.
    /// </summary>
    public event PacketCallback? OnPacket;
    /// <summary>
    /// An event that is called when a connection is established.
    /// </summary>
    public event ConnectCallback? OnConnect;
    /// <summary>
    /// An event that is called when the client has stopped.
    /// </summary>
    public event StopCallback? OnStop;
    /// <summary>
    /// An event that is called when the client is disconnecting.
    /// </summary>
    public event DisconnectCallback? OnDisconnect;
    /// <summary>
    /// An event that is called when a packet is sent.
    /// </summary>
    public event PacketSentCallback? OnPacketSent;
    /// <summary>
    /// If the connection is closed.
    /// </summary>
    public bool Closed {get; private set;}
    private ClientContainer(IPEndPoint server, IPEndPoint? local) {
        Server = server;
        Closed = true;
        if (local == null) {
            tcpClient = new TcpClient();
        } else {
            tcpClient = new TcpClient(local);
        }
        IP = tcpClient.Client.LocalEndPoint as IPEndPoint;
    }

    /// <summary>
    /// Disposes the client.
    /// </summary>
    ~ClientContainer() {
        Dispose();
    }

    /// <summary>
    /// Starts listening.
    /// </summary>
    public async Task Start() {
        await tcpClient.ConnectAsync(Server);
        OnConnect?.Invoke(this);
        NetworkStream = tcpClient.GetStream();
        Closed = false;
        listenThread = new Thread(Listen);
        listenThread.Start();
    }
    private void Listen() {
        List<byte> data = [];
        while (!Closed) {
            int i;
            NetworkStream.Read
            if ((i=NetworkStream!.ReadByte())==-1) {
                break;
            }
            byte b = Convert.ToByte(i);
            if (b == PacketParser.NewPacket && data[^1]!=PacketParser.NewPacket) {
            }
        }
    }
    /// <summary>
    /// Sends a packet to the server.
    /// </summary>
    /// <param name="packet">The packet to send to the server.</param>
    public void SendPacket(Packet packet) {
        OnPacketSent?.Invoke(this, packet);
    }

    /// <summary>
    /// Disconnects from the server and disposes everything.
    /// </summary>
    public void Stop() {
        OnDisconnect?.Invoke(this, false);
        Dispose();
    }
    /// <inheritdoc/>
    /// <remarks>
    /// Disconnects from the server and disposes everything, <b>will not call the ClientContainer.OnDisconnect event</b>.
    /// </remarks>
    public void Dispose() {
        Closed = true;
        NetworkStream!.Close();
        tcpClient.Close();
        OnStop?.Invoke(this);
        GC.SuppressFinalize(this);
    }
}