using System.Diagnostics.Contracts;
using System.Reflection;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing;

public interface ISerializer {
    public static SerializableInfo? TryGetInfo(ISerializer serializable) {
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
    public static SerializableInfo? TryGetInfo<T>() where T : ISerializer, new() {
        return TryGetInfo(new T());
    }
    public static SerializableInfo CreateInfo(Func<ISerializer> creator) {
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
            ISerializer inst = creator();
            foreach (SerializableInfo info in format) {
                Parser parser = new(raw);
                parser.ReadSerializable()
                ISerializable serializable = info.Creator();
                serializable.Parse(raw);
            }
        });
    }
    public static SerializableInfo CreateInfo<T>() where T : ISerializer, new() {
        return CreateInfo(() => new T());
    }
    
    [Pure]
    public SerializableInfo[] Format { get; }
    public ISerializable[] Serialize();
    public void Parse(ISerializable[] serializables);
}