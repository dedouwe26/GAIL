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
    /// Starts listening (asynchronous).
    /// </summary>
    /// <returns>True if connected successfully.</returns>
    public async ValueTask<bool> StartAsync() {
        try {
            await tcpClient.ConnectAsync(Server);
        } catch (SocketException) {
            return false;
        }
        NetworkStream = tcpClient.GetStream();
        Closed = false;
        OnConnect?.Invoke(this);
        listenThread = new Thread(Listen);
        listenThread.Start();
        return true;
    }
    /// <summary>
    /// Starts listening.
    /// </summary>
    /// <returns>True if connected successfully.</returns>
    public bool Start() {
        try {
            tcpClient.Connect(Server);
        } catch (SocketException) {
            return false;
        }
        NetworkStream = tcpClient.GetStream();
        Closed = false;
        OnConnect?.Invoke(this);
        listenThread = new Thread(Listen);
        listenThread.Start();
        return true;
    }
    private void Listen() {
        PacketParser.Parse(NetworkStream!, () => Closed, (Packet p) => {
            OnPacket?.Invoke(this, p);
            if (p is DisconnectPacket) {
                OnDisconnect?.Invoke(this, true, (p as DisconnectPacket)!.AdditionalData);
                Dispose();
                return true;
            }
            return false;
        });
    }
    /// <summary>
    /// Sends a packet to the server.
    /// </summary>
    /// <param name="packet">The packet to send to the server.</param>
    public void SendPacket(Packet packet) {
        NetworkStream!.Write(PacketParser.FormatPacket(packet));
        NetworkStream.Flush();
        OnPacketSent?.Invoke(this, packet);
    }

    /// <summary>
    /// Sends a packet to the server (asynchronous).
    /// </summary>
    /// <param name="packet">The packet to send to the server.</param>
    public async ValueTask SendPacketAsync(Packet packet) {
        await NetworkStream!.WriteAsync(PacketParser.FormatPacket(packet));
        await NetworkStream.FlushAsync();
        OnPacketSent?.Invoke(this, packet);
    }

    /// <summary>
    /// Disconnects from the server and disposes everything.
    /// </summary>
    /// <param name="additionalData">The optional additional data.</param>
    public void Stop(byte[]? additionalData = null) {
        additionalData ??= [];
        OnDisconnect?.Invoke(this, false, additionalData);
        SendPacket(new DisconnectPacket(additionalData));
        Dispose();
    }
    /// <summary>
    /// Disconnects from the server and disposes everything (asynchronous).
    /// </summary>
    /// <param name="additionalData">The optional additional data send to the server.</param>
    public async ValueTask StopAsync(byte[]? additionalData = null) {
        additionalData ??= [];
        OnDisconnect?.Invoke(this, false, additionalData);
        await SendPacketAsync(new DisconnectPacket(additionalData ?? []));
        Dispose();
    }
    /// <inheritdoc/>
    /// <remarks>
    /// <b>&gt;&gt;&gt; Please use </b><see cref="Stop"/> &lt;&lt;&lt;<para/>
    /// <b>&gt;&gt;&gt; Will not call the ClientContainer.OnDisconnect event and will not send a </b><see cref="DisconnectPacket"/>.<para/>
    /// Disconnects from the server and disposes everything.
    /// </remarks>
    public void Dispose() {
        Closed = true;
        NetworkStream!.Close();
        tcpClient.Close();
        OnStop?.Invoke(this);
        GC.SuppressFinalize(this);
    }
}