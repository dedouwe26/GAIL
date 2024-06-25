using GAIL.Serializing.Streams;

namespace GAIL.Storage.Parser;

public static class StorageParser {
    public static bool IsFieldRegistered(Field field) {

    }
    public static byte GetFieldTypeID(Field field) {

    }
    public static void WriteField(Serializer serializer, Field field) {
        // TODO: check if field is registered.
        serializer.WriteByte(GetFieldTypeID(field));
        serializer.WriteString(field.Key);
        serializer.WriteSerializable(field);
    }
    public static void WriteContainer(Serializer serializer, Container container) {
        serializer.WriteString(container.Key);
        serializer.Write
    }
}