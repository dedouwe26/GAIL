using System.Diagnostics.Contracts;
using System.Reflection;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing;

/// <summary>
/// Represents info for a serializer.
/// </summary>
/// <param name="FixedSize">The fixed size of the serializer.</param>
/// <param name="Creator">The instance creator of the serializer.</param>
public record SerializerInfo (uint? FixedSize, Func<byte[], ISerializer> Creator);

public interface ISerializer {
    public static SerializerInfo? TryGetInfo(ISerializer serializable) {
        try {
            PropertyInfo[] infos = serializable.GetType().GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (PropertyInfo info in infos) {
                if (info.GetCustomAttribute<SerializingInfoAttribute>() is not null) {
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
    public static SerializerInfo? TryGetInfo<T>() where T : ISerializer, new() {
        return TryGetInfo(new T());
    }
    public static SerializerInfo CreateInfo(Func<ISerializer> creator) {
        SerializableInfo[] format = creator().Format;
        uint? fixedSize = 0;
        foreach (SerializableInfo info in format) {
            if (info.FixedSize is not null) {
                fixedSize += info.FixedSize;
            } else {
                fixedSize = null;
                break;
            }
        }
        return new (fixedSize, raw => {
            Parser parser = new(raw);
            
            ISerializable[] serializables = new ISerializable[format.Length];
            for (int i = 0; i < format.Length; i++) {
                serializables[i] = parser.ReadSerializable(format[i]);
            }
            ISerializer inst = creator();
            inst.Parse(serializables);
            return inst;
        });
    }
    public static SerializerInfo CreateInfo<T>() where T : ISerializer, new() {
        return CreateInfo(() => new T());
    }
    
    [Pure]
    public SerializableInfo[] Format { get; }
    public ISerializable[] Serialize();
    public void Parse(ISerializable[] serializables);
}