using System.Collections.ObjectModel;
using GAIL.Serializing.Streams;
using GAIL.Storage.Members;

namespace GAIL.Storage.Parser;

/// <summary>
/// A serializer that can serialize the storage format (opposite of <see cref="StorageParser"/>).
/// </summary>
public class StorageSerializer : Serializer {
    /// <summary>
    /// Creates a new storage serializer.
    /// </summary>
    /// <inheritdoc/>
    public StorageSerializer(Stream output, bool shouldCloseStream = true) : base(output, shouldCloseStream) { }
    /// <summary>
    /// Creates a new storage serializer.
    /// </summary>
    /// <inheritdoc/>
    public StorageSerializer(bool shouldCloseStream = true) : base(shouldCloseStream) { }
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
        WriteChildren([.. list.Members.Cast<IMember>()], false);
        WriteByte((byte)MemberType.End);
    }
    /// <summary>
    /// Writes a member to the stream.
    /// </summary>
    /// <param name="member">The member to serialize.</param>
    /// <param name="hasKey">If the member has a key (no key in list).</param>
    /// <exception cref="InvalidOperationException">Field is not registered.</exception>
    public void WriteMember(IMember member, bool hasKey = true) {
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
    public void WriteChildren(List<IMember> children, bool hasKey = true) {
        foreach (IMember child in children) {
            if (child is IMember member) {
                WriteMember(member, hasKey);
            }
        }
    }
    /// <summary>
    /// Writes the children of a node to the stream.
    /// </summary>
    /// <param name="children">The children to write to the stream.</param>
    public void Serialize(ReadOnlyDictionary<string, IMember> children) {
        WriteChildren([.. children.Values]);
        WriteType(MemberType.End);
    }
}