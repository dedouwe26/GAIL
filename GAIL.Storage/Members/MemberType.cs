namespace GAIL.Storage.Members;

/// <summary>
/// Represents the type of a member.
/// </summary>
public enum MemberType : byte {
    /// <summary>
    /// The end of a container-like member (does not have a corresponding member).
    /// </summary>
    End,
    /// <summary>
    /// A type for <see cref="Members.Container"/> (container-like, can contain more members).
    /// </summary>
    Container,
    /// <summary>
    /// A type for <see cref="Members.List"/> (container-like, can contain more members).
    /// </summary>
    List,
    /// <summary>
    /// A type for <see cref="BoolField"/>.
    /// </summary>
    Bool,
    /// <summary>
    /// A type for <see cref="FloatField"/>.
    /// </summary>
    Float,
    /// <summary>
    /// A type for <see cref="DoubleField"/>.
    /// </summary>
    Double,
    /// <summary>
    /// A type for <see cref="ByteField"/>.
    /// </summary>
    Byte,
    /// <summary>
    /// A type for <see cref="ShortField"/>.
    /// </summary>
    Short,
    /// <summary>
    /// A type for <see cref="IntField"/>.
    /// </summary>
    Int,
    /// <summary>
    /// A type for <see cref="LongField"/>.
    /// </summary>
    Long,
    /// <summary>
    /// A type for <see cref="SByteField"/>.
    /// </summary>
    SByte,
    /// <summary>
    /// A type for <see cref="UShortField"/>.
    /// </summary>
    UShort,
    /// <summary>
    /// A type for <see cref="UIntField"/>.
    /// </summary>
    UInt,
    /// <summary>
    /// A type for <see cref="ULongField"/>.
    /// </summary>
    ULong,
    /// <summary>
    /// A type for <see cref="BytesField"/>.
    /// </summary>
    Bytes,
    /// <summary>
    /// A type for <see cref="StringField"/>.
    /// </summary>
    String,
}