using GAIL.Serializing.Formatters;

namespace GAIL.Serializing.Streams;

/// <summary>
/// A reader for reading serializables (opposite of <see cref="Serializer"/>).
/// </summary>
public class Parser : IDisposable {
    /// <summary>
    /// Creates a new parser.
    /// </summary>
    /// <param name="input">The stream to read from.</param>
    /// <param name="shouldCloseStream">If the stream should be closed when disposing.</param>
    /// <exception cref="InvalidOperationException">The input stream does not support reading.</exception>
    public Parser(Stream input, bool shouldCloseStream = true) {
        if (!input.CanRead) { throw new InvalidOperationException("The input stream does not support reading"); }

        BaseStream = input;
        ShouldCloseStream = shouldCloseStream;
    }
    /// <summary>
    /// Creates a new parser from raw bytes.
    /// </summary>
    /// <param name="input">The raw bytes to parse.</param>
    /// <param name="shouldCloseStream">If the stream should be closed when disposing.</param>
    public Parser(byte[] input, bool shouldCloseStream = true) : this(new MemoryStream(input), shouldCloseStream) { }

    /// <summary>
    /// True if the stream should be closed when disposing.
    /// </summary>
    public bool ShouldCloseStream;
    /// <summary>
    /// True if the underlying stream is disposed.
    /// </summary>
    public bool Disposed { get; private set; }

    /// <summary>
    /// The stream to read the data from.
    /// </summary>
    public Stream BaseStream { get; protected set; }

    /// <summary>
    /// Reads raw bytes from the stream.
    /// </summary>
    /// <param name="size">The size of the array or the count of the bytes to read.</param>
    /// <returns>The raw bytes that have been read.</returns>
    /// <exception cref="EndOfStreamException">Could not read all bytes.</exception>
    public virtual byte[] Read(uint size) {
        byte[] raw = new byte[size];

        int offset = 0;
        int toRead = Convert.ToInt32(size);
        int read;
        while (toRead > 0 && (read = BaseStream.Read(raw, offset, toRead)) > 0) {
            toRead -= read;
            offset += read;
        }
        if (toRead > 0) throw new EndOfStreamException();
        return raw;
    }

    /// <summary>
    /// Reads raw bytes from the stream with a fixed size.
    /// </summary>
    /// <param name="size">The fixed size to read from the stream.</param>
    /// <param name="formatter">The formatter used to decode the raw data.</param>
    /// <returns>The raw bytes that have been read.</returns>
    /// <exception cref="EndOfStreamException">Could not read all bytes.</exception>
    public virtual byte[] Read(uint? size, IFormatter? formatter = null) {
        size ??= ReadUInt(formatter);
        return formatter!=null ? formatter.Decode(Read(size.Value)) : Read(size.Value);
    }

    /// <summary>
    /// Reads a serializable from the stream.
    /// </summary>
    /// <param name="info">The info of the serializable on how to create and read the serializable.</param>
    /// <param name="formatter">The formatter used to decode the raw data.</param>
    /// <returns>The parsed serializable.</returns>
    public virtual ISerializable ReadSerializable(SerializableInfo info, IFormatter? formatter = null) {
        byte[] raw = Read(info.FixedSize, formatter);

        return info.Creator(raw);
    }
    /// <summary>
    /// Reads a serializable from the stream.
    /// </summary>
    /// <typeparam name="T">The type of the serializable.</typeparam>
    /// <param name="info">The info of the serializable on how to create and read the serializable.</param>
    /// <param name="formatter">The formatter used to decode the raw data.</param>
    /// <returns>The parsed serializable.</returns>
    /// <exception cref="InvalidCastException">The serializable is not of type T.</exception>
    public virtual T ReadSerializable<T>(SerializableInfo info, IFormatter? formatter = null) where T : ISerializable {
        if (ReadSerializable(info, formatter) is T serializable) {
            return serializable;
        }
        throw new InvalidCastException("The serializable is not of type T");
    }

    public virtual ISerializer ReadSerializer
    /// <summary>
    /// Reads an unsigned integer from the stream.
    /// </summary>
    /// <param name="formatter">The formatter used to decode the raw data.</param>
    /// <returns>The parsed unsigned integer.</returns>
    /// <exception cref="InvalidCastException">The serializable is not an unsigned integer.</exception>
    public virtual uint ReadUInt(IFormatter? formatter = null) {
        return ReadSerializable<UIntSerializable>(UIntSerializable.Info).Value;
    }
    /// <summary>
    /// Reads an integer from the stream.
    /// </summary>
    /// <param name="formatter">The formatter used to decode the raw data.</param>
    /// <returns>The parsed integer.</returns>
    /// <exception cref="InvalidCastException">The serializable is not an integer.</exception>
    public virtual int ReadInt(IFormatter? formatter = null) {
        return ReadSerializable<IntSerializable>(IntSerializable.Info).Value;
    }
    /// <summary>
    /// Reads a byte from the stream.
    /// </summary>
    /// <param name="formatter">The formatter used to decode the raw data.</param>
    /// <returns>The parsed byte.</returns>
    /// <exception cref="InvalidCastException">The serializable is not a byte.</exception>
    public virtual byte ReadByte(IFormatter? formatter = null) {
        return ReadSerializable<ByteSerializable>(ByteSerializable.Info).Value;
    }
    /// <summary>
    /// Reads an UTF-8 string from the stream.
    /// </summary>
    /// <param name="formatter">The formatter used to decode the raw data.</param>
    /// <returns>The parsed string.</returns> 
    /// <exception cref="InvalidCastException">The serializable is not a string.</exception>
    public virtual string ReadString(IFormatter? formatter = null) {
        return ReadSerializable<StringSerializable>(StringSerializable.Info).Value;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Disposes the underlying stream (if enabled).
    /// </remarks>
    public virtual void Dispose() {
        if (Disposed) { return; }

        Disposed = true;

        if (!ShouldCloseStream) { return; }
        
        BaseStream.Close();

        GC.SuppressFinalize(this);
    }
}