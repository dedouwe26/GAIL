namespace GAIL.Networking;

/// <summary>
/// Represents any field in a <see cref="Packet"/>.
/// </summary>
public abstract class Field {
    /// <summary>
    /// Initializes a field.
    /// </summary>
    public Field() { }
    /// <summary>
    /// Creates a field from an object.
    /// </summary>
    /// <param name="value">The value for this Field</param>
    public Field(object value) { BaseValue = value; }
    /// <summary>
    /// Creates a field from raw data.
    /// </summary>
    /// <param name="data">The raw data to create this field from.</param>
    public Field(RawData data) { BaseValue = BaseParse(data.data); }
    /// <summary>
    /// Creates the raw data from this Field.
    /// </summary>
    /// <returns>The raw data.</returns>
    public abstract byte[] Format();
    /// <summary>
    /// Creates an object from the raw data.
    /// </summary>
    /// <param name="data">The raw data to convert.</param>
    /// <returns>The parsed object.</returns>
    public abstract object BaseParse(byte[] data);
    /// <summary>
    /// The value of this field.
    /// </summary>
    public object BaseValue { get { return baseValue!;} set { baseValue = value; } }
    /// <summary>
    /// True if the value always has a fixed size.
    /// </summary>
    public virtual bool HasFixedSize { get { return FixedSize!=null; } }
    /// <summary>
    /// The fixed size of the value if it has one (in bytes).
    /// </summary>
    public abstract uint? FixedSize { get; }
    /// <summary>
    /// The value of this field.
    /// </summary>
    protected object? baseValue;
    /// <summary>
    /// The type of this field (type of the parser / formatter).
    /// </summary>
    public Type BaseType {get { return BaseValue.GetType(); }}
}

/// <summary>
/// Represents raw data for fields.
/// </summary>
public struct RawData {
    /// <summary>
    /// The raw data.
    /// </summary>
    public byte[] data;
}

/// <summary>
/// Represents a field in a <see cref="Packet"/>
/// </summary>
/// <typeparam name="T">The type of this field.</typeparam>
public abstract class Field<T> : Field where T : notnull {
    /// <summary>
    /// Initializes a field.
    /// </summary>
    public Field() { }
    /// <summary>
    /// Creates a field from a value of type <typeparamref name="T"/>
    /// </summary>
    /// <param name="value">The value for this Field</param>
    public Field(T value) : base(value) { }
    /// <summary>
    /// Creates a field from raw data.
    /// </summary>
    /// <param name="data">The raw data to create this field from.</param>
    public Field(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override object BaseParse(byte[] data) {
        return Parse(data);
    }
    /// <summary>
    /// Creates a <typeparamref name="T"/> from the raw data.
    /// </summary>
    /// <param name="data">The raw data to convert.</param>
    /// <returns>The parsed <typeparamref name="T"/> object.</returns>
    public abstract T Parse(byte[] data);
    /// <summary>
    /// The value of this field.
    /// </summary>
    public T Value { get { return (T)baseValue!;} set { baseValue = value; } }
    /// <summary>
    /// The type of this field (type of the parser / formatter).
    /// </summary>
    public Type Type {get { return typeof(T); }}
}