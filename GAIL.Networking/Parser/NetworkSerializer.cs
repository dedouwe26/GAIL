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
    /// <param name="packetFormatter">The formatter used for this specific packet.</param>
    public void Encode(IFormatter globalFormatter, IFormatter packetFormatter) {
        byte[] result = globalFormatter.Encode(packetFormatter.Encode((BaseStream as MemoryStream)!.ToArray()));
        OutStream.Write(new IntSerializable(result.Length).Serialize());
        OutStream.Write(result);
    }
    
    /// <summary>
    /// Serializes a packet into raw data.
    /// </summary>
    /// <param name="packet">The packet to format.</param>
    /// <param name="globalFormatter">The formatter used for global purposes (multiple packets).</param>
    /// <param name="packetFormatter">The formatter used for this specific packet.</param>
    public void WritePacket(Packet packet, IFormatter globalFormatter, IFormatter packetFormatter) {
        WriteUInt(NetworkRegister.GetPacketID(packet));

        foreach (ISerializable field in packet.GetFields()) {
            WriteSerializable(field);
        }

        Encode(globalFormatter, packetFormatter);
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