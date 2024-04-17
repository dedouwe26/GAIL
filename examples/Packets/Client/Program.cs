using System.Net;
using GAIL.Networking;
using GAIL.Networking.Client;

namespace examples.Packets.Client;

class Program {

    public static async void Main(string[] args) {
        ClientContainer client = await NetworkManager.CreateClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3002));

        client.OnPacket+=OnPacket;
        client.OnConnect+=OnConnect;
    }

    private static void OnConnect(ClientContainer client) {
        
    }

    private static void OnPacket(ClientContainer client, Packet packet) {
        
    }
}