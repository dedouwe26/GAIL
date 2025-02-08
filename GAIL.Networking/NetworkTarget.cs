using GAIL.Networking.Client;
using GAIL.Networking.Server;
using OxDED.Terminal.Logging;
using OxDED.Terminal.Logging.Targets;

namespace GAIL.Networking;

/// <summary>
/// A logger target that logs to the client or server.
/// </summary>
public class NetworkTarget : FormattedTarget {
    private Action<LogPacket> sendMethod;

    private bool enabled;
    /// <summary>
    /// Whether this target still sends packets.
    /// </summary>
    public bool Enabled { get => enabled; private set {
        if (!value) {
            sendMethod = _ => { };
            enabled = value;
        }
    } }

    /// <summary>
    /// Creates a new network target that logs to the server.
    /// </summary>
    /// <param name="client">The client that logs to the server.</param>
    public NetworkTarget(ClientContainer client) {
        sendMethod = log => {
            if (!client.SendPacket(log)) {
                Enabled = false;
            }
        };
    }
    /// <summary>
    /// Creates a new network target that logs to a client.
    /// </summary>
    /// <param name="server">The server that logs to the client.</param>
    /// <param name="connection">The client to log to</param>
    public NetworkTarget(ServerContainer server, Connection connection) {
        sendMethod = log => {
            if(!server.SendPacket(log, connection)) {
                Enabled = false;
            }
        };
    }
    /// <summary>
    /// Creates a new network target that logs to all clients.
    /// </summary>
    /// <param name="server">The servers that log to all clients.</param>
    public NetworkTarget(ServerContainer server) {
        sendMethod = log => {
            if (!server.BroadcastPacket(log)) {
                Enabled = false;
            }
        };
    }

    /// <inheritdoc/>
    public override void Write<T>(Severity severity, DateTime time, Logger logger, T? text) where T : default {
        sendMethod.Invoke(new LogPacket(severity, time, logger.ID ?? "", GetName(logger), text?.ToString() ?? ""));
    }
    /// <inheritdoc/>
    public override void Dispose() {
        Enabled = false;

        GC.SuppressFinalize(this);
    }
}