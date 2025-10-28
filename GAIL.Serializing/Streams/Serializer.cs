using GAIL.Serializing.Formatters;

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
	/// Creates a new serializer with a memory stream.
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
	/// <param name="buffer">The data to write to the stream.</param>
	/// <param name="formatter">The formatter used to encode the raw data (<paramref name="buffer"/>).</param>
	public virtual void Write(byte[] buffer, IFormatter? formatter = null) {
		if (formatter != null) {
			buffer = formatter.Encode(buffer);
			WriteUInt((uint)buffer.Length);
		}
		BaseStream.Write(buffer);
	}

	/// <summary>
	/// Encodes the data given.
	/// </summary>
	/// <param name="consumer">The consumer that writes the data that needs encoding.</param>
	/// <param name="formatter">The formatter used for encoding.</param>
	public virtual void Encode(Action<Serializer> consumer, IFormatter formatter) {
		using Serializer serializer = new();
		consumer(serializer);
		Write((serializer.BaseStream as MemoryStream)!.ToArray(), formatter);
	}

	/// <summary>
	/// Writes a serializable to the stream.
	/// </summary>
	/// <param name="serializable">The serializable to write to the stream.</param>
	/// <param name="formatter">The formatter used to encode the raw data.</param>
	/// <exception cref="InvalidOperationException">Fixed size doesn't match the actual size.</exception>
	public virtual void WriteSerializable(ISerializable serializable, IFormatter? formatter = null) {
		serializable.Serialize(this, formatter);
	}

	/// <summary>
	/// Writes a unsigned integer to the stream.
	/// </summary>
	/// <param name="value">The unsigned integer to write.</param>
	/// <param name="formatter">The formatter used to encode the raw data.</param>
	public virtual void WriteUInt(uint value, IFormatter? formatter = null) {
		WriteSerializable(new UIntSerializable(value), formatter);
	}
	/// <summary>
	/// Writes a byte to the stream.
	/// </summary>
	/// <param name="value">The byte to write.</param>
	/// <param name="formatter">The formatter used to encode the raw data.</param>
	public virtual void WriteByte(byte value, IFormatter? formatter = null) {
		WriteSerializable(new ByteSerializable(value), formatter);
	}
	/// <summary>
	/// Writes an integer to the stream.
	/// </summary>
	/// <param name="value">The integer to write.</param>
	/// <param name="formatter">The formatter used to encode the raw data.</param>
	public virtual void WriteInt(int value, IFormatter? formatter = null) {
		WriteSerializable(new IntSerializable(value), formatter);
	}
	/// <summary>
	/// Writes a string to the stream as UTF-8.
	/// </summary>
	/// <param name="value">The string to write.</param>
	/// <param name="formatter">The formatter used to encode the raw data.</param>
	public virtual void WriteString(string value, IFormatter? formatter = null) {
		WriteSerializable(new StringSerializable(value), formatter);
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Disposes the underlying stream (if enabled).
	/// </remarks>
	public virtual void Dispose() {
		if (Disposed) { return; }

		Disposed = true;

		if (ShouldCloseStream) BaseStream.Close();

		GC.SuppressFinalize(this);
	}
}