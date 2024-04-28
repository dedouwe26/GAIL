using System.Net;
using System.Net.Sockets;

namespace GAIL.Networking.Server;

/// <summary>
/// Represents a server-side connection to identifiy the client.
/// </summary>
public class Connection : IDisposable {
    /// <summary>
    /// Creates an ID from the IP end point.
    /// </summary>
    /// <param name="ip">The IP end point to create an ID from.</param>
    /// <returns>The created ID.</returns>
    public static byte[] CreateID(IPEndPoint ip) {
        return [.. ip.Address.GetAddressBytes(), .. BitConverter.GetBytes(Convert.ToUInt16(ip.Port))];
    }

    /// <summary>
    /// The 6-byte-long ID from the IP and port of the client.
    /// </summary>
    public readonly byte[] ID;
    /// <summary>
    /// The IP end point of the client (IP address and port).
    /// </summary>
    public readonly IPEndPoint IP;
    /// <summary>
    /// The server-side client instance.
    /// </summary>
    public readonly TcpClient TcpClient;
    /// <summary>
    /// The stream to read and write from.
    /// </summary>
    public readonly NetworkStream Stream;
    /// <summary>
    /// The user-set data.
    /// </summary>
    public object? data = null;
    internal Connection(TcpClient client) {
        if (client.Client.RemoteEndPoint == null) {
            throw new ArgumentException("Client is not connected", nameof(client));
        }
        IP = (client.Client.RemoteEndPoint as IPEndPoint)!;
        ID = CreateID(IP);
        TcpClient = client;
        Stream = client.GetStream();
    }

    /// <summary>
    /// Disposes the connection.
    /// </summary>
    ~Connection() {
        Dispose();
    }

    /// <summary>
    /// Changes the user-set data.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="newData">The new data to set.</param>
    public void SetData<T>(T newData) {
        data = newData;
    }

    /// <summary>
    /// Gets the user-set data from this connection.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <returns>The user-set data.</returns>
    public T? GetData<T>() where T : notnull {
        if (data == null) { return default; }
        return (T)data!;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Destroys the server-side tcp client and stream. Please use <see cref="ServerContainer.Disconnect(Connection)"/> to disconnect from a client.
    /// </remarks>
    public void Dispose() {
        Stream.Close();
        TcpClient.Close();
    }
}