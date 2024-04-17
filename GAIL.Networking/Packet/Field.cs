using GAIL.Networking.Parser;

namespace GAIL.Networking;

public abstract class Field<T> where T : notnull {
    public Field(T value) { Value = value; }
    public Field(byte[] data) { Value = Parse(data); }
    public abstract FormatData Format();
    public abstract T Parse(byte[] data);
    public T Value;
    public Type Type {get { return typeof(T); }}
}