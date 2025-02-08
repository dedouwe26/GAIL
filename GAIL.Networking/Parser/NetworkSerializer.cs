using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Networking.Parser;

/// <summary>
/// A serializer that can serialize the network format (opposite of: <see cref="NetworkParser"/>).
/// </summary>
public class NetworkSerializer : Serializer {
    /// <summary>
    /// The stream to write to when done formatting.
    /// </summary>
    public Stream OutStream { get; private set; }
    /// <summary>
    /// Creates a new network serializer.
    /// </summary>
    /// <inheritdoc/>
    public NetworkSerializer(Stream output, bool shouldCloseStream = false) : base(shouldCloseStream) { OutStream = output; }
    /// <summary>
    /// Creates a new network serializer.
    /// </summary>
    /// <inheritdoc/>
    public NetworkSerializer(bool shouldCloseStream = false) : base(shouldCloseStream) { OutStream = new MemoryStream(); }

    /// <summary>
    /// Applies all the formatters (should call at the end).
    /// </summary>
    /// <param name="globalFormatter">The formatter used for global purposes (multiple packets).</param>
    /// <param name="packetID">The ID of the packet (only encoded by the global encoder).</param>
    /// <param name="packetFormatter">The formatter used for this specific packet.</param>
    public void Encode(IFormatter globalFormatter, uint packetID, IFormatter packetFormatter) {
        byte[] result = globalFormatter.Encode([
            .. new UIntSerializable(packetID).Serialize(),
            .. packetFormatter.Encode(
                (BaseStream as MemoryStream)!.ToArray()
            )
        ]);
        
        OutStream.Write(new IntSerializable(result.Length).Serialize());
        OutStream.Write(result);
    }
    
    /// <summary>
    /// Serializes a packet into raw data.
    /// </summary>
    /// <param name="packet">The packet to format.</param>
    /// <param name="globalFormatter">The formatter used for global purposes (multiple packets).</param>
    /// <exception cref="InvalidOperationException"/>
    public void WritePacket(Packet packet, IFormatter globalFormatter) {
        foreach (ISerializable field in NetworkRegister.DestructPacket(packet)) {
            WriteSerializable(field);
        }
        uint ID = NetworkRegister.GetPacketID(packet) ?? throw new InvalidOperationException($"{packet.GetType().Name} packet is not registered");
        Encode(globalFormatter, ID, NetworkRegister.GetPacketFormatter(ID));
    }
    /// <inheritdoc/>
    public override void Dispose() {
        if (Disposed) { return; }

        if (!ShouldCloseStream) { return; }

        OutStream.Close();
        
        base.Dispose();
        
        GC.SuppressFinalize(this);
    }
}