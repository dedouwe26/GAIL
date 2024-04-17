using System.Net;
using GAIL.Networking;
using GAIL.Networking.Server;

namespace examples.Packets.Client;

class Program {

    public static async void Main(string[] args) {
        ServerContainer server = await NetworkManager.CreateServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3002));

        server.OnPacket+=OnPacket;
        server.OnConnect+=OnConnect;
    }

    private static void OnConnect(ServerContainer server, Connection connection) {
        throw new NotImplementedException();
    }

    private static void OnPacket(ServerContainer server, Connection connection, Packet packet) {
        throw new NotImplementedException();
    }
}