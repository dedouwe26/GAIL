using System.Collections.ObjectModel;

namespace GAIL.Storage.Parser;


public static class StorageRegister {
    private static readonly List<Field> registeredFields = [];
    public static ReadOnlyCollection<Field> RegisteredFields => registeredFields.AsReadOnly();
    public static bool IsFieldRegistered(Field field) {
        return registeredFields.Contains(field);
    }
    public static byte GetFieldTypeID(Field field) {
        return (byte)registeredFields.FindIndex((Field a) => a == field);
    }
}