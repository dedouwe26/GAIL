using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using GAIL.Networking.Parser;
using GAIL.Serializing.Formatters;
using OxDED.Terminal;
using OxDED.Terminal.Logging;
using OxDED.Terminal.Logging.Targets;
using IFormatter = GAIL.Serializing.Formatters.IFormatter;

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
    /// <param name="enableLogging">If the client should start logging.</param>
    /// <param name="logger">The logger to use.</param>
    /// <returns>The created client, if it didn't fail.</returns>
    public static ClientContainer? Create(IPEndPoint server, IPEndPoint? local = null, bool enableLogging = false, Logger? logger = null) {
        ClientContainer client;
        try {
            client = new(server, local, enableLogging, logger);
        } catch (SocketException) {
            return null;
        }
        
        return client;
    }
    /// <summary>
    /// The server end point (IP address and port).
    /// </summary>
    public readonly IPEndPoint Server;
    /// <summary>
    /// The local end point or the end point the server sees (IP address and port, for different port or different IP address).
    /// </summary>
    public readonly IPEndPoint IP;
    private Thread? listenThread;
    private NetworkSerializer? serializer;
    private NetworkSerializer Serializer { get {
        if (serializer == null) {
            serializer = new(NetworkStream!);
        }
        return serializer;
    } }
    /// <summary>
    /// The logger of this client.
    /// Set it using <see cref="SetLogger"/>
    /// </summary>
    public Logger? Logger { get; private set; }
    /// <summary>
    /// The tcp client back-end.
    /// </summary>
    public readonly TcpClient tcpClient;
    /// <summary>
    /// The stream to write to.
    /// </summary>
    public NetworkStream? NetworkStream {get; private set;}

    /// <summary>
    /// The formatter used to encode / decode all packets.
    /// </summary>
    public IFormatter GlobalFormatter = new DefaultFormatter();

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
    /// An event for when an exception is thrown (only IOException, SocketException).
    /// </summary>
    public event ExceptionCallback? OnException;
    /// <summary>
    /// If the connection is closed.
    /// </summary>
    public bool Closed {get; private set;}
    /// <exception cref="SocketException"/>
    private ClientContainer(IPEndPoint server, IPEndPoint? local, bool enableLogging = false, Logger? logger = null) {
        Server = server;
        Closed = true;

        if (enableLogging) {
            SetLogger(logger);
        }

        try {
            if (local == null) {
                tcpClient = new TcpClient();
            } else {
                tcpClient = new TcpClient(local);
            }

            IP = (tcpClient.Client.LocalEndPoint! as IPEndPoint)!;
            
        } catch (SocketException e) {
            Logger?.LogFatal("Unable to initiate the socket:");
            Logger?.LogException(e, Severity.Fatal);
            throw;
        }
    }

    /// <summary>
    /// Disposes the client.
    /// </summary>
    ~ClientContainer() {
        Dispose();
    }
    /// <summary>
    /// Sets the logger of this client.
    /// </summary>
    /// <param name="logger">The logger for this client (on default only contains a terminal target).</param>
    /// <param name="disable">If it should disable the logging.</param>
    public void SetLogger(Logger? logger = null, bool disable = false) {
        Logger = logger ?? new Logger(
            name: "Networking Client",
            severity: Severity.Info,
            targets: [
                new TerminalTarget()
            ]
        );
        int index = Logger.GetTargetIndex<TerminalTarget>();
        if (index > -1) {
            Logger.GetTarget<TerminalTarget>(index)!.Format = "<{0}>: ("+Color.DarkBlue.ToForegroundANSI()+"{2}"+ANSI.Styles.ResetAll+")[{5}"+ANSI.Styles.Bold+"{3}"+ANSI.Styles.ResetAll+"] : {5}{4}"+ANSI.Styles.ResetAll;
            Logger.GetTarget<TerminalTarget>(index)!.NameFormat =  "{0} - {1}";
        }
        index = Logger.GetTargetIndex<FileTarget>();
        if (index > -1) {
            Logger.GetTarget<FileTarget>(index)!.Format = "<{0}>: ({2})[{3}] : {4}";
            Logger.GetTarget<FileTarget>(index)!.NameFormat =  "{0} - {1}";
        }
    }

    /// <summary>
    /// Starts listening (asynchronous).
    /// </summary>
    /// <returns>True if connected successfully.</returns>
    public async ValueTask<bool> StartAsync() {
        try {
            await tcpClient.ConnectAsync(Server);
        } catch (SocketException e) {
            Logger?.LogFatal("Unable to connect to server: '"+e.Message+"'.");
            OnException?.Invoke(e);
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
        } catch (SocketException e) {
            Logger?.LogFatal("Unable to connect to server:");
            Logger?.LogException(e, Severity.Fatal);
            OnException?.Invoke(e);
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
        NetworkParser parser = new(NetworkStream!);
        try {
            if (!parser.Parse(GlobalFormatter, () => Closed, p => {
                OnPacket?.Invoke(this, p);
                if (p is DisconnectPacket dp) {
                    OnDisconnect?.Invoke(this, true, dp.AdditionalData);
                    Dispose();
                    return true;
                }
                return false;
            })) {
                Logger?.LogFatal("Unable to start reading from network stream.");
                OnException?.Invoke(new InvalidOperationException("Unable to start reading from network stream."));
            }
        } catch (IOException e) {
            if (Closed) {
                return;
            }
            Logger?.LogError("Could not read from network stream:");
            Logger?.LogException(e, Severity.Error);
            OnException?.Invoke(e);
        }
        
    }
    /// <summary>
    /// Sends a packet to the server.
    /// </summary>
    /// <param name="packet">The packet to send to the server.</param>
    /// <returns>True if it succeeded, false if the client is closed or the client could not send the packet.</returns>
    public bool SendPacket(Packet packet) {
        if (Closed) { return false; }
        try {
            Serializer.WritePacket(packet, GlobalFormatter);
        } catch (IOException e) {
            Logger?.LogError("Could not send packet:");
            Logger?.LogException(e, Severity.Error);
            OnException?.Invoke(e);
            return false;
        }
        NetworkStream!.Flush();
        OnPacketSent?.Invoke(this, packet);
        return true;
    }

    /// <summary>
    /// Sends a packet to the server (asynchronous).
    /// </summary>
    /// <param name="packet">The packet to send to the server.</param>
    /// <returns>True if it succeeded, false if the client is closed or the client could not send the packet.</returns>
    public async ValueTask<bool> SendPacketAsync(Packet packet) {
        if (Closed) { return false; }
        try {
            Serializer.WritePacket(packet, GlobalFormatter); // TODO?: this isnt really async.
        } catch (IOException e) {
            Logger?.LogError("Could not send packet:");
            Logger?.LogException(e, Severity.Error);
            OnException?.Invoke(e);
            return false;
        }
        await NetworkStream!.FlushAsync();
        OnPacketSent?.Invoke(this, packet);
        return true;
    }

    /// <summary>
    /// Disconnects from the server and disposes everything.
    /// </summary>
    /// <param name="additionalData">The optional additional data.</param>
    /// <returns>True if it succeeded, false if the client is closed.</returns>
    public bool Stop(byte[]? additionalData = null) {
        if (Closed) { return false; }
        additionalData ??= [];
        OnDisconnect?.Invoke(this, false, additionalData);
        SendPacket(new DisconnectPacket(additionalData));
        Dispose();
        return true;
    }
    /// <summary>
    /// Disconnects from the server and disposes everything (asynchronous).
    /// </summary>
    /// <param name="additionalData">The optional additional data send to the server.</param>
    /// <returns>True if it succeeded, false if the client is closed.</returns>
    public async ValueTask<bool> StopAsync(byte[]? additionalData = null) {
        if (Closed) { return false; }
        Logger?.LogDebug("Stopping");
        additionalData ??= [];
        OnDisconnect?.Invoke(this, false, additionalData);
        await SendPacketAsync(new DisconnectPacket(additionalData));
        Dispose();
        return true;
    }
    /// <inheritdoc/>
    /// <remarks>
    /// <b>Please use </b><see cref="Stop"/><para/>
    /// <b>Will not call the ClientContainer.OnDisconnect event and will not send a </b><see cref="DisconnectPacket"/>.<para/>
    /// Disconnects from the server and disposes everything.
    /// </remarks>
    public void Dispose() {
        if (Closed) { return; }
        Logger?.LogDebug("Disposing");
        Closed = true;
        NetworkStream!.Close();
        tcpClient.Close();
        OnStop?.Invoke(this);
        GC.SuppressFinalize(this);
    }
}