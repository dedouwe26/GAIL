using System.Net;
using GAIL.Networking.Client;
using GAIL.Networking.Server;

namespace GAIL.Networking;

public static class NetworkManager {
    public static async Task<ClientContainer> CreateClient(string host, int port) {
        IPAddress[] addresses = Dns.GetHostAddresses(host);
        if (addresses.Length == 0) { throw new ArgumentException("Could not get IP address.", nameof(host)); }
        return await ClientContainer.Create(new IPEndPoint(addresses[0], port));
    }
    public static async Task<ClientContainer> CreateClient(IPEndPoint server) {
        return await ClientContainer.Create(server);
    }
    public static async Task<ServerContainer> CreateServer(string host, int port) {
        IPAddress[] addresses = Dns.GetHostAddresses(host);
        if (addresses.Length == 0) { throw new ArgumentException("Could not get IP address.", nameof(host)); }
        return await ServerContainer.Create(new IPEndPoint(addresses[0], port));
    }
    public static async Task<ServerContainer> CreateServer(IPEndPoint server) {
        return await ServerContainer.Create(server);
    }
}