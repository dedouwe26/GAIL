using System.Net;
using GAIL.Networking.Client;
using GAIL.Networking.Server;

namespace GAIL.Networking;

/// <summary>
/// The manager for networking.
/// </summary>
public static class NetworkManager {
    /// <summary>
    /// Creates a client.
    /// </summary>
    /// <param name="server">The server end point (IP address and port).</param>
    /// <param name="local">The local end point (for different port, etc).</param>
    /// <returns>The created client, if it didn't fail.</returns>
    public static ClientContainer? CreateClient(IPEndPoint server, IPEndPoint? local = null) {
        return ClientContainer.Create(server, local);
    }
    /// <summary>
    /// Creates a client.
    /// </summary>
    /// <param name="host">The hostname of the server.</param>
    /// <param name="port">The port of the server.</param>
    /// <param name="local">The local end point (for different port, etc).</param>
    /// <returns>The created client, if it didn't fail.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static ClientContainer? CreateClient(string host, int port, IPEndPoint? local = null) {
        IPAddress[] addresses = Dns.GetHostAddresses(host);
        if (addresses.Length < 1) { throw new ArgumentException("Could not get IP address", nameof(host)); }
        return CreateClient(new IPEndPoint(addresses[0], port), local);
    }
    /// <summary>
    /// Creates a server.
    /// </summary>
    /// <param name="server">The IP end point to listen for (IP address and port).</param>
    /// <returns>The created server.</returns>
    public static ServerContainer CreateServer(IPEndPoint server) {
        return ServerContainer.Create(server);
    }
    /// <summary>
    /// Creates a server.
    /// </summary>
    /// <param name="host">The hostname to listen for.</param>
    /// <param name="port">The port to listen for.</param>
    /// <returns>The created server.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static ServerContainer CreateServer(string host, int port) {
        IPAddress[] addresses = Dns.GetHostAddresses(host);
        if (addresses.Length == 0) { throw new ArgumentException("Could not get IP address.", nameof(host)); }
        return CreateServer(new IPEndPoint(addresses[0], port));
    }
}