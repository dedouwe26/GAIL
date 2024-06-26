using System.Collections.ObjectModel;
using GAIL.Serializing.Streams;

namespace GAIL.Storage.Parser;

/// <summary>
/// A serializer that can serialize the storage format.
/// </summary>
public class StorageSerializer : Serializer {
    /// <inheritdoc/>
    /// <summary>
    /// Creates a new storage serializer.
    /// </summary>
    public StorageSerializer(Stream output, bool shouldCloseStream = true) : base(output, shouldCloseStream) { }
    /// <summary>
    /// Writes a field to the base stream.
    /// </summary>
    /// <param name="field">The field to write.</param>
    /// <exception cref="InvalidOperationException"><paramref name="field"/> is not registered.</exception>
    public void WriteField(Field field) {
        if (!StorageRegister.IsFieldRegistered(field)) { throw new InvalidOperationException("Field is not registered"); }
        WriteByte(StorageRegister.GetFieldTypeID(field));
        WriteString(field.Key);
        WriteSerializable(field);
    }
    private void WriteChild(IChildNode child) {
        if (child is Field subField) {
            WriteField(subField);
        } else if (child is Container subContainer) {
            WriteContainer(subContainer);
        }
    }
    /// <summary>
    /// Writes a dictionary of children to the stream.
    /// </summary>
    /// <param name="children">The children to write to the stream.</param>
    public void WriteChildren(List<IChildNode> children) {
        foreach (IChildNode child in children) {
            WriteChild(child);
        }
    }
    /// <summary>
    /// Writes a container to the stream.
    /// </summary>
    /// <param name="container">The container to write to the stream.</param>
    public void WriteContainer(Container container) {
        WriteString(container.Key);
        WriteChildren([.. container.Children.Values]);
    }
    /// <summary>
    /// Writes the children of a node to the stream.
    /// </summary>
    /// <param name="children">The children to write to the stream.</param>
    public void Serialize(ReadOnlyDictionary<string, IChildNode> children) {
        WriteChildren([.. children.Values]);
    }
}