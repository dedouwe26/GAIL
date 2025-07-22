using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Networking.Streams;

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

    private void Encode(IFormatter? formatter = null) {
        byte[] result = [.. (BaseStream as MemoryStream)!.ToArray()];
        BaseStream.SetLength(0);
        if (formatter != null) result = formatter.Encode(result);
        
        OutStream.Write(new IntSerializable(result.Length).Serialize());
        OutStream.Write(result);
    }
    
    /// <summary>
    /// Serializes a packet into raw data.
    /// </summary>
    /// <param name="packet">The packet to format.</param>
    /// <param name="formatter">The formatter used globally (multiple packets).</param>
    /// <exception cref="InvalidOperationException"/>
    public void WritePacket(Packet packet, IFormatter? formatter = null) {
        uint ID = NetworkRegister.GetPacketID(packet) ?? throw new InvalidOperationException($"{packet.GetType().Name} packet is not registered");
        WriteUInt(ID);
        WriteStreamReducer(packet, NetworkRegister.GetPacketFormatter(ID));
        Encode(formatter);
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