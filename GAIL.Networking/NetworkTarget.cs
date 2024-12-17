using GAIL.Networking.Client;
using GAIL.Networking.Server;
using OxDED.Terminal.Logging;

namespace GAIL.Networking;

public class NetworkTarget : ITarget {
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

    public void Write<T>(Severity severity, DateTime time, Logger _, T? text) {
        sendMethod.Invoke(new LogPacket());
        FileTarget
    }
    public void Dispose() {
        
    }
}