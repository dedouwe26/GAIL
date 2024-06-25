
namespace GAIL.Serializing.Streams;

/// <summary>
/// A writer for writing serializables (opposite of <see cref="Parser"/>).
/// </summary>
public class Serializer : IDisposable {
    /// <summary>
    /// Creates a new serializer.
    /// </summary>
    /// <param name="output">The stream to write to.</param>
    /// <param name="shouldCloseStream">If the the stream should be closed when disposing.</param>
    /// <exception cref="InvalidOperationException">The output stream does not support writing.</exception>
    /// 
    public Serializer(Stream output, bool shouldCloseStream = true) {
        if (!output.CanWrite) { throw new InvalidOperationException("The output stream does not support writing"); }
        
        BaseStream = output;
        ShouldCloseStream = shouldCloseStream;
    }
    /// <summary>
    /// Creates a new serializer.
    /// </summary>
    /// <param name="shouldCloseStream">If the the stream should be closed when disposing.</param>
    public Serializer(bool shouldCloseStream = true) : this(new MemoryStream(), shouldCloseStream) { }

    /// <summary>
    /// True if the the stream should be closed when disposing.
    /// </summary>
    public bool ShouldCloseStream;
    /// <summary>
    /// True if the underlying stream is disposed.
    /// </summary>
    public bool Disposed { get; private set; }

    /// <summary>
    /// The stream to write the data to.
    /// </summary>
    public Stream BaseStream { get; private set; }

    /// <summary>
    /// Writes data to the stream.
    /// </summary>
    /// <param name="buffer"></param>
    public virtual void Write(ReadOnlySpan<byte> buffer) {
        BaseStream.Write(buffer);
    }

    /// <summary>
    /// Writes a serializable to the stream.
    /// </summary>
    /// <param name="serializable">The serializable to write to the stream.</param>
    /// <exception cref="InvalidOperationException">Fixed size doesn't match the actual size.</exception>
    public virtual void WriteSerializable(ISerializable serializable) {
        byte[] raw = serializable.Serialize();
        if (serializable.FixedSize == null) {
            WriteUInt(Convert.ToUInt32(raw.Length));
        } else if (raw.Length != serializable.FixedSize) {
            throw new InvalidOperationException("Fixed size doesn't match the actual size");
        }
        Write(raw);
    }
    /// <summary>
    /// Writes a unsigned integer to the stream.
    /// </summary>
    /// <param name="value">The unsigned integer to write.</param>
    public virtual void WriteUInt(uint value) {
        WriteSerializable(new UIntSerializable(value));
    }
    /// <summary>
    /// Writes a byte to the stream.
    /// </summary>
    /// <param name="value">The byte to write.</param>
    public virtual void WriteByte(byte value) {
        WriteSerializable(new ByteSerializable(value));
    }
    /// <summary>
    /// Writes an integer to the stream.
    /// </summary>
    /// <param name="value">The integer to write.</param>
    public virtual void WriteInt(int value) {
        WriteSerializable(new IntSerializable(value));
    }
    /// <summary>
    /// Writes a string to the stream as UTF-8.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public virtual void WriteString(string value) {
        WriteSerializable(new StringSerializable(value));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Disposes the underlying stream (if enabled).
    /// </remarks>
    public virtual void Dispose() {
        if (Disposed || !ShouldCloseStream) { return; }

        BaseStream.Close();

        Disposed = true;
        
        GC.SuppressFinalize(this);
    }
}