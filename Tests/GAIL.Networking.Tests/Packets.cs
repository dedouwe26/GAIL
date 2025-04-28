namespace GAIL.Networking.Tests;

public class TestPacket : Packet {
    
}

public class FormattedPacket : Packet {
    
}

public static class Packets {
    public static void RegisterPackets() {
        if (!NetworkRegister.IsPacketRegistered<TestPacket>()) {
            NetworkRegister.RegisterPacket<TestPacket>();
            NetworkRegister.RegisterPacket<FormattedPacket>();
        }
    }
}