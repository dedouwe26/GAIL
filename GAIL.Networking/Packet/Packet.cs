using GAIL.Serializing;
using GAIL.Serializing.Formatters;

namespace GAIL.Networking;

/// <summary>
/// Packet is an abstract class for parsing and formatting a packet.
/// </summary>
public abstract class Packet : IReducer {
    /// <summary>
    /// The formatter used for encoding / decoding this packet.
    /// </summary>
    public virtual IFormatter? Formatter => null;
    /// <summary>
    /// Creates a packet (add own data here).
    /// </summary>
    public Packet() {}

    private uint? id;
    /// <summary>
    /// The ID of this packet.
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    public uint ID { get {
        id ??= NetworkRegister.GetPacketID(this) ?? throw new InvalidOperationException($"Packet {GetType().Name} is not registered");
        return id.Value;
    } }

    /// <summary>
    /// Gets called before serializing the packet.
    /// </summary>
    protected virtual void OnSerialize() { }
    /// <summary>
    /// Gets called after parsing the packet.
    /// </summary>
    protected virtual void OnParse() { }

    /// <inheritdoc/>
    public SerializableInfo[] Format => NetworkRegister.GetPacketFormat(ID);

    /// <inheritdoc/>
    public ISerializable[] Serialize() {
        OnSerialize();
        return NetworkRegister.DestructPacket(this);
    }
    /// <inheritdoc/>
    public void Parse(ISerializable[] serializables) {
        NetworkRegister.SetFields(this, serializables);
        OnParse();
    }
}