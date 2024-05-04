using GAIL.Networking.Parser;

namespace GAIL.Networking;

/// <summary>
/// Represents a field in a <see cref="Packet"/>
/// </summary>
/// <typeparam name="T">The type of this field.</typeparam>
public abstract class Field<T> where T : notnull {
    /// <summary>
    /// Initializes a field.
    /// </summary>
    public Field() { }
    /// <summary>
    /// Creates a field from an value of type <typeparamref name="T"/>
    /// </summary>
    /// <param name="value">The value for this Field</param>
    public Field(T value) { Value = value; }
    /// <summary>
    /// Creates a field from the raw data.
    /// </summary>
    /// <param name="data">The raw data to create this field from.</param>
    public Field(byte[] data) { Value = Parse(data); }
    /// <summary>
    /// Creates the raw data from this Field.
    /// </summary>
    /// <returns>The raw data.</returns>
    public abstract byte[] Format();
    /// <summary>
    /// Creates a <typeparamref name="T"/> from the raw data.
    /// </summary>
    /// <param name="data">The raw data to convert.</param>
    /// <returns>The parsed <typeparamref name="T"/> object.</returns>
    public abstract T Parse(byte[] data);
    /// <summary>
    /// The value of this field.
    /// </summary>
    public T Value { get { return value!;} set { this.value = value; } }
    /// <summary>
    /// True if the value always has a fixed size.
    /// </summary>
    public virtual bool HasFixedSize { get { return FixedSize!=null; } }
    /// <summary>
    /// The fixed size of the value if it has one (in bytes).
    /// </summary>
    public abstract uint? FixedSize { get; }
    private T? value;
    /// <summary>
    /// The type of this field (type of the parser / formatter).
    /// </summary>
    public Type Type {get { return typeof(T); }}
    /// <summary>
    /// Casts a field into an object field.
    /// </summary>
    /// <param name="field">The object field to cast.</param>
    public static implicit operator Field<T>(Field<object> field) { return PacketParser.CreateFieldFromType<T>(field.Value); }
    /// <summary>
    /// Casts an object field into a field.
    /// </summary>
    /// <param name="field">The field to cast.</param>
    public static implicit operator Field<object>(Field<T> field) { return (Field<object>)field; }
}