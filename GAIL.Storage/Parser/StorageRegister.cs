using System.Collections.Immutable;
using System.Collections.ObjectModel;
using GAIL.Storage.Members;

namespace GAIL.Storage.Parser;

/// <summary>
/// A class that handles and creates members.
/// </summary>
public static class StorageRegister {
    private static readonly Dictionary<MemberType,FieldInfo> registeredFields = new() {
        [MemberType.Bool] = BoolField.Info,

        [MemberType.Float] = FloatField.Info,
        [MemberType.Double] = DoubleField.Info,

        [MemberType.Byte] = ByteField.Info,
        [MemberType.Short] = ShortField.Info,
        [MemberType.Int] = IntField.Info,
        [MemberType.Long] = LongField.Info,

        [MemberType.SByte] = SByteField.Info,
        [MemberType.UShort] = UShortField.Info,
        [MemberType.UInt] = UIntField.Info,
        [MemberType.ULong] = ULongField.Info,

        [MemberType.Bytes] = BytesField.Info,
        [MemberType.String] = StringField.Info
    };
    /// <summary>
    /// All the registered fields, with there type and there field info.
    /// </summary>
    public static ReadOnlyDictionary<MemberType,FieldInfo> RegisteredFields => registeredFields.AsReadOnly();
    /// <summary>
    /// Checks if the member is registered.
    /// </summary>
    /// <param name="member">The member to check.</param>
    /// <returns>True if the member is registered, otherwise false.</returns>
    public static bool IsMemberRegistered(IMember member) {
        if (member is Container) {
            return true;
        } else if (member is List) {
            return true;
        } else if (member is BoolField) {
            return true;
        } else if (member is FloatField || member is DoubleField) {
            return true;
        } else if (member is ByteField || member is ShortField || member is IntField || member is LongField) {
            return true;
        } else if (member is SByteField || member is UShortField || member is UIntField || member is ULongField) {
            return true;
        } else if (member is BytesField) {
            return true;
        } else if (member is StringField) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Gets the type from a type ID.
    /// </summary>
    /// <param name="ID">The type ID to convert to a type.</param>
    /// <returns>The member type of the type ID.</returns>
    public static MemberType GetType(byte ID) {
        return (MemberType)ID;
    }
    /// <summary>
    /// Gets the type ID from a type.
    /// </summary>
    /// <param name="type">The type to convert to a type ID.</param>
    /// <returns>The type ID of the member type.</returns>
    public static byte GetTypeID(MemberType type) {
        return (byte)type;
    }
    /// <summary>
    /// Gets the fixed size of a field.
    /// </summary>
    /// <param name="type">The type of the field (must be a type of a field).</param>
    /// <returns>The fixed size of the corresponding field.</returns>
    public static uint? GetFixedSize(MemberType type) {
        return registeredFields[type].FixedSize;
    }
    /// <summary>
    /// Creates a field from a key, type and the raw bytes.
    /// </summary>
    /// <param name="key">The key of the field, used for the node.</param>
    /// <param name="type">The member type of the field. Used to create the field.</param>
    /// <param name="raw">The raw data for the field to parse.</param>
    /// <returns>The newly created field.</returns>
    public static Field CreateField(string key, MemberType type, byte[] raw) {
        return registeredFields[type].Creator(raw, key);
    }
}