using GAIL.Networking.Client;
using GAIL.Networking.Server;
using OxDED.Terminal.Logging;
using OxDED.Terminal.Logging.Targets;

namespace GAIL.Networking;

public class NetworkTarget : FormattedTarget {
    private Action<LogPacket> sendMethod;

    public NetworkTarget(ClientContainer client) {
        sendMethod = (LogPacket log) => {
            if (!client.SendPacket(log)) {
                sendMethod = (LogPacket _) => {};
            }
        };
    }
    public NetworkTarget(ServerContainer server, Connection connection) {
        sendMethod = (LogPacket log) => {
            if(!server.SendPacket(log, connection)) {
                sendMethod = (LogPacket _) => {};
            }
        };
    }
    public NetworkTarget(ServerContainer server) {
        sendMethod = (LogPacket log) => {
            if (!server.BroadcastPacket(log)) {
                sendMethod = (LogPacket _) => {};
            }
        };
    }

    public override void Write<T>(Severity severity, DateTime time, Logger logger, T? text) where T : default {
        sendMethod.Invoke(new LogPacket(severity, time, logger.ID ?? "-", GetName(logger), text?.ToString() ?? ""));
    }
    public override void Dispose() {
        
    }
}