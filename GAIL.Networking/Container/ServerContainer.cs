using System.Net;
using System.Net.Sockets;

namespace GAIL.Networking.Server;

public class ServerContainer : IDisposable {
    public TcpListener tcpListener;
    public event PacketCallback? OnPacket;
    public event StartCallback? OnStart;
    public event ConnectCallback? OnConnect;
    public event StopCallback? OnStop;
    public event PacketSentCallback? OnPacketSent;
    public ServerContainer(IPEndPoint ip) {
        tcpListener = new TcpListener(ip);
    }
    public void SendPacket(Packet packet) {
        
    }

    public void Dispose() {
        tcpListener.Stop();
        GC.SuppressFinalize(this);
    }
}