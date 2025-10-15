using System.Diagnostics.Contracts;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing;

/// <summary>
/// Represents a class that can be turned into bytes and can be created from bytes.
/// </summary>
public interface IRawSerializable : ISerializable {
    /// <summary>
    /// Determines whether the return value of <see cref="Serialize"/> has a fixed length and if so what the length is (in bytes). Must always be the same.
    /// </summary>
    [Pure]
    public uint? FixedSize { get; }
    /// <summary>
    /// Turns this class into bytes.
    /// </summary>
    /// <returns>The serialized bytes.</returns>
    public byte[] Serialize();
    /// <summary>
    /// Creates this class from bytes.
    /// </summary>
    /// <param name="data">The serialized bytes (from <see cref="Serialize"/>).</param>
    public void Parse(byte[] data);
}
/// <summary>
/// Represents a class that turns <typeparamref name="T"/> into bytes and the other way around.
/// </summary>
/// <typeparam name="T">The type of the object to be serializable.</typeparam>
public interface IRawSerializable<T> : IRawSerializable {
    /// <summary>
    /// The value of the serializable.
    /// </summary>
    public T Value { get; set; }
}

/// <summary>
/// A default implementation of the raw serializable interface.
/// </summary>
public abstract class RawSerializable : IRawSerializable {
    /// <inheritdoc/>
    [Pure]
    public abstract uint? FixedSize { get; }

    /// <inheritdoc/>
    public abstract byte[] Serialize();
    /// <inheritdoc/>
    public abstract void Parse(byte[] data);

    /// <inheritdoc/>
    public void Serialize(Serializer serializer, IFormatter? formatter = null) {
        byte[] raw = Serialize();

        if (FixedSize == null && formatter == null) {
            serializer.WriteUInt((uint)raw.Length, null); // NOTE: No formatter, because that could make it longer than 4 bytes.
        } else if (raw.Length != FixedSize && FixedSize != null) {
            throw new InvalidOperationException("Fixed size doesn't match the actual size");
        }
        serializer.Write(raw, formatter);
    }
    /// <inheritdoc/>
    public void Parse(Parser parser, IFormatter? formatter = null) {
        Parse(parser.Read(FixedSize, formatter));
    }
}
/// <summary>
/// A default implementation of the raw serializable interface.
/// </summary>
/// <typeparam name="T">The type of the object to be serializable.</typeparam>
public abstract class RawSerializable<T> : RawSerializable {
    /// <summary>
    /// The value of the serializable.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Creates a new raw serializable from a value.
    /// </summary>
    /// <param name="value">The value to use.</param>
    public RawSerializable(T value) {
        Value = value;
    }
}