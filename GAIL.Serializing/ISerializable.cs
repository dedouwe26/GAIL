using System.Diagnostics.Contracts;

namespace GAIL.Serializing;

/// <summary>
/// Represents info for a serializable.
/// </summary>
/// <param name="FixedSize">The fixed size of the serializable.</param>
/// <param name="Creator">The instance creator of the serializable.</param>
public record SerializableInfo (uint? FixedSize, Func<byte[], ISerializable> Creator);

/// <summary>
/// Represents a class that can be turned into bytes and can be created from bytes.
/// </summary>
public interface ISerializable {
    /// <summary>
    /// Creates a serializable info.
    /// </summary>
    /// <param name="creator">The creator that creates an empty instance.</param>
    /// <returns>A new serializable info.</returns>
    public static SerializableInfo CreateInfo(Func<ISerializable> creator) {
        return new (creator().FixedSize, (byte[] raw) => {
            ISerializable inst = creator();
            inst.Parse(raw);
            return inst;
        });
    }

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
    /// <param name="data">The serialized bytes (from <see cref="Serialize"/> ).</param>
    public void Parse(byte[] data);
}
/// <summary>
/// Represents a class that turns <typeparamref name="T"/> into bytes and the other way around.
/// </summary>
/// <typeparam name="T">The type of the object to be serializable.</typeparam>
public interface ISerializer<T> : ISerializable {
    /// <summary>
    /// The value of the serializable.
    /// </summary>
    public T Value { get; set; }
}