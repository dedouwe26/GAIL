using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;

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
    /// Tries to get the serializable info of a serializable.
    /// </summary>
    /// <param name="serializable">The serializable of which to get the info from.</param>
    /// <returns>The info, if it is found, of that serializable.</returns>
    public static SerializableInfo? TryGetInfo(ISerializable serializable) {
        try {
            PropertyInfo[] infos = serializable.GetType().GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (PropertyInfo info in infos) {
                if (info.GetCustomAttribute<SerializableInfoAttribute>() is not null) {
                    if (info.GetValue(serializable) is SerializableInfo s) {
                        return s;
                    } else {
                        throw new InvalidOperationException($"SerializableInfo property {info.Name} did not return a SerializableInfo.");
                    }
                }
            }
        } catch (Exception) {
            return null;
        }
        return null;
    }
    /// <summary>
    /// Tries to get the serializable info of a serializable.
    /// </summary>
    /// <typeparam name="T">The serializable type of which to get the info from.</typeparam>
    /// <returns>The info, if it is found, of that serializable.</returns>
    public static SerializableInfo? TryGetInfo<T>() where T : ISerializable, new() {
        return TryGetInfo(new T());
    }
    /// <summary>
    /// Creates a serializable info.
    /// </summary>
    /// <param name="creator">The creator that creates an empty instance.</param>
    /// <returns>A new serializable info.</returns>
    public static SerializableInfo CreateInfo(Func<ISerializable> creator) {
        return new (creator().FixedSize, raw => {
            ISerializable inst = creator();
            inst.Parse(raw);
            return inst;
        });
    }
    /// <summary>
    /// Creates a serializable info.
    /// </summary>
    /// <typeparam name="T">The type of the serializable.</typeparam>
    /// <returns>A new serializable info.</returns>
    public static SerializableInfo CreateInfo<T>() where T : ISerializable, new() {
        return CreateInfo(() => new T());
    }
    // /// <summary>
    // /// The info of this serializable.
    // /// </summary>
    // public SerializableInfo Info { get; }

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
public interface ISerializable<T> : ISerializable {
    /// <summary>
    /// The value of the serializable.
    /// </summary>
    public T Value { get; set; }
}