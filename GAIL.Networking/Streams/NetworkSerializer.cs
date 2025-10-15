using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Networking.Streams;

/// <summary>
/// A serializer that can serialize the network format (opposite of: <see cref="NetworkParser"/>).
/// </summary>
public class NetworkSerializer : Serializer {
    /// <summary>
    /// Creates a new network serializer.
    /// </summary>
    /// <inheritdoc/>
    public NetworkSerializer(Stream output, bool shouldCloseStream = false) : base(output, shouldCloseStream) { }
    /// <summary>
    /// Creates a new network serializer.
    /// </summary>
    /// <inheritdoc/>
    public NetworkSerializer(bool shouldCloseStream = true) : base(shouldCloseStream) { }
    
    private static void WritePacket(Serializer serializer, Packet packet) {
        uint ID = NetworkRegister.GetPacketID(packet) ?? throw new InvalidOperationException($"{packet.GetType().Name} packet is not registered");
        serializer.WriteUInt(ID);
        serializer.WriteSerializable(packet, null);
    }
    /// <summary>
    /// Serializes a packet into raw data.
    /// </summary>
    /// <param name="packet">The packet to format.</param>
    /// <param name="formatter">The formatter used globally (multiple packets).</param>
    /// <exception cref="InvalidOperationException"/>
    public void WritePacket(Packet packet, IFormatter? formatter = null) {
        if (formatter != null) {
            Encode((s) => {
                WritePacket(s, packet);
            }, formatter);
        } else {
            WritePacket(this, packet);
        }
        
    }
    /// <inheritdoc/>
    public override void Dispose() {
        if (Disposed) { return; }
        
        base.Dispose();
        
        GC.SuppressFinalize(this);
    }
}