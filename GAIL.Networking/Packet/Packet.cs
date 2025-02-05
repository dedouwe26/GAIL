using GAIL.Networking.Parser;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;

namespace GAIL.Networking;

/// <summary>
/// Packet is an abstract class for parsing and formatting a packet.
/// </summary>
public abstract class Packet {
    /// <summary>
    /// The formatter used for encoding / decoding this packet.
    /// </summary>
    [PacketFormatter]
    public virtual IFormatter Formatter => new DefaultFormatter();
    /// <summary>
    /// Creates a packet (add own data here).
    /// </summary>
    public Packet() {}
    
    /// <summary>
    /// The ID of this packet.
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    public uint ID { get => NetworkRegister.GetPacketID(this) ?? throw new InvalidOperationException($"{GetType().Name} packet is not registered"); }
}