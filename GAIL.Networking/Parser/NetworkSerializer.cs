using GAIL.Serializing;
using GAIL.Serializing.Streams;

namespace GAIL.Networking.Parser;

/// <summary>
/// A serializer that can serialize the network format (opposite of: <see cref="NetworkParser"/>).
/// </summary>
public class NetworkSerializer : Serializer {
    /// <summary>
    /// Creates a new network serializer.
    /// </summary>
    /// <inheritdoc/>
    public NetworkSerializer(Stream input, bool shouldCloseStream = false) : base(input, shouldCloseStream) { }
    /// <summary>
    /// Creates a new network serializer.
    /// </summary>
    /// <inheritdoc/>
    public NetworkSerializer(bool shouldCloseStream = false) : base(shouldCloseStream) { }
    /// <summary>
    /// Serializes a packet into raw data.
    /// </summary>
    /// <param name="packet">The packet to format.</param>
    public void WritePacket(Packet packet) {
        WriteUInt(NetworkRegister.GetPacketID(packet));

        foreach (ISerializable field in packet.GetFields()) {
            WriteSerializable(field);
        }
    }
}