using System.Diagnostics.Contracts;

namespace GAIL.Serializing;

public interface ISerializer {
    [Pure]
    public SerializableInfo[] Format { get; }
    public ISerializable[] Serialize();
    public void Parse(ISerializable[] serializables);
}