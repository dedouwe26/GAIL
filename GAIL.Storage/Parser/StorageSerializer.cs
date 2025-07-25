using System.Collections.ObjectModel;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Members;

namespace GAIL.Storage.Parser;

/// <summary>
/// A serializer that can serialize the storage format (opposite of <see cref="StorageParser"/>).
/// </summary>
public class StorageSerializer : Serializer {
    /// <summary>
    /// The stream to write to when done formatting.
    /// </summary>
    public Stream OutStream { get; private set; }
    /// <summary>
    /// Creates a new storage serializer.
    /// </summary>
    /// <inheritdoc/>
    public StorageSerializer(Stream output, bool shouldCloseStream = true) : base(shouldCloseStream) { OutStream = output; }
    /// <summary>
    /// Creates a new storage serializer.
    /// </summary>
    public StorageSerializer(bool shouldCloseStream = true) : base(shouldCloseStream) { OutStream = new MemoryStream(); }
    /// <summary>
    /// Writes a member type to the stream.
    /// </summary>
    /// <param name="type">The type to write.</param>
    protected void WriteType(MemberType type) {
        WriteByte(StorageRegister.GetTypeID(type));
    }
    /// <summary>
    /// Writes a container to the stream.
    /// </summary>
    /// <param name="container">The container to serialize.</param>
    protected void WriteContainer(Container container) {
        WriteChildren([.. container.Children.Values]);
        WriteByte((byte)MemberType.End);
    }
    /// <summary>
    /// Writes a field to the stream.
    /// </summary>
    /// <param name="field">The field to serialize.</param>
    protected void WriteField(Field field) {
        WriteSerializable(field);
    }
    /// <summary>
    /// Writes a list to the stream.
    /// </summary>
    /// <param name="list">The list to serialize.</param>
    protected void WriteList(List list) {
        WriteChildren([.. list.Members.Cast<IChildNode>()], false);
        WriteByte((byte)MemberType.End);
    }
    /// <summary>
    /// Writes a member to the stream.
    /// </summary>
    /// <param name="member">The member to serialize.</param>
    /// <param name="hasKey">If the member has a key (no key in list).</param>
    /// <exception cref="InvalidOperationException">Field is not registered.</exception>
    public void WriteMember(IChildNode member, bool hasKey = true) {
        if (!StorageRegister.IsMemberRegistered(member)) { throw new InvalidOperationException("Field is not registered"); }

        WriteType(member.Type);
        if (hasKey) {
            WriteString(member.Key);
        }

        if (member is Field subField) {
            WriteField(subField);
        } else if (member is Container subContainer) {
            WriteContainer(subContainer);
        } else if (member is List subList) {
            WriteList(subList);
        }
    }
    /// <summary>
    /// Writes a dictionary of children to the stream.
    /// </summary>
    /// <param name="children">The children to write to the stream.</param>
    /// <param name="hasKey">True if it should serialize the keys from the <paramref name="children"/>.</param>
    public void WriteChildren(List<IChildNode> children, bool hasKey = true) {
        foreach (IChildNode child in children) {
            if (child is IChildNode member) {
                WriteMember(member, hasKey);
            }
        }
    }

    /// <summary>
    /// Applies the formatter (should call at the end).
    /// </summary>
    /// <param name="formatter">The formatter to apply.</param>
    public void Encode(IFormatter? formatter = null) {
        byte[] result = [.. (BaseStream as MemoryStream)!.ToArray()];
        BaseStream.SetLength(0);
        if (formatter != null) result = formatter.Encode(result);
        
        OutStream.Write(new IntSerializable(result.Length).Serialize());
        OutStream.Write(result);
    }

    /// <summary>
    /// Writes the children of a node to the stream and formats them.
    /// </summary>
    /// <param name="children">The children to write to the stream.</param>
    /// <param name="formatter">The formatter to use for encoding.</param>
    public void Serialize(ReadOnlyDictionary<string, IChildNode> children, IFormatter? formatter = null) {
        WriteChildren([.. children.Values]);
        WriteType(MemberType.End);

        Encode(formatter);
    }
    /// <inheritdoc/>
    public override void Dispose() {
        if (Disposed) { return; }

        if (!ShouldCloseStream) { return; }

        OutStream.Close();
        
        base.Dispose();
        
        GC.SuppressFinalize(this);
    }
}