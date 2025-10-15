using System.Collections.ObjectModel;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Members;

namespace GAIL.Storage.Streams;

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
    /// Writes a container to the stream.
    /// </summary>
    /// <param name="container">The container to serialize.</param>
    public void WriteContainer(Container container) {
        WriteChildren([.. container.Children.Values]);
    }
    /// <summary>
    /// Writes a field to the stream.
    /// </summary>
    /// <param name="field">The field to serialize.</param>
    public void WriteField(IField field) {
        WriteSerializable(field);
    }
    /// <summary>
    /// Writes a list to the stream.
    /// </summary>
    /// <param name="list">The list to serialize.</param>
    public void WriteList(List list) {
        WriteChildren([.. list.Members.Cast<IChildNode>()], false);
    }
    /// <summary>
    /// Writes a member to the stream.
    /// </summary>
    /// <param name="member">The member to serialize.</param>
    /// <param name="hasKey">If the member has a key (no key in list).</param>
    /// <exception cref="InvalidOperationException">Field is not registered.</exception>
    public void WriteMember(IChildNode member, bool hasKey = true) {
        if (hasKey) {
            WriteString(member.Key);
        }

        if (member is IField field) {
            WriteField(field);
        } else if (member is Container container) {
            WriteContainer(container);
        } else if (member is List list) {
            WriteList(list);
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
    public void Serialize(Dictionary<string, IChildNode> children, IFormatter? formatter = null) {
        WriteChildren([.. children.Values]);

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