
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Storage.Members;

namespace GAIL.Storage.Parser;

/// <summary>
/// A parser that can parse the storage format (opposite of <see cref="StorageSerializer"/>).
/// </summary>
public class StorageParser : Serializing.Streams.Parser {
    /// <summary>
    /// The stream to read from while formatting.
    /// </summary>
    public Stream InStream { get; private set; }

    /// <summary>
    /// Creates a new storage parser.
    /// </summary>
    /// <inheritdoc/>
    public StorageParser(Stream input, bool shouldCloseStream = true) : base(new MemoryStream(), shouldCloseStream) { InStream = input; }
    /// <summary>
    /// Creates a new storage parser.
    /// </summary>
    /// <inheritdoc/>
    public StorageParser(byte[] input, bool shouldCloseStream = true) : base(new MemoryStream(), shouldCloseStream) { InStream = new MemoryStream(input); }

    /// <summary>
    /// Reads a member type from the stream.
    /// </summary>
    /// <returns>The read member type.</returns>
    protected virtual MemberType ReadType() {
        return StorageRegister.GetType(ReadByte());
    }
    /// <summary>
    /// Reads a field from the stream.
    /// </summary>
    /// <param name="key">The key of the field.</param>
    /// <param name="type">The type of the field.</param>
    /// <returns>A new parsed field.</returns>
    protected virtual Field ReadField(string key, MemberType type) {
        byte[] raw = Read(StorageRegister.GetFixedSize(type));
        return StorageRegister.CreateField(key, type, raw);
    }
    /// <summary>
    /// Reads a list from the stream.
    /// </summary>
    /// <param name="key">The key of the list.</param>
    /// <returns>A new parsed list.</returns>
    protected virtual List ReadList(string key) {
        List<IMember> children = ReadMembers(false);

        return new List(key, children);
    }
    /// <summary>
    /// Reads a container from the stream.
    /// </summary>
    /// <param name="key">The key of the container.</param>
    /// <returns>A new parsed container.</returns>
    protected virtual Container ReadContainer(string key) {
        return new Container(key, ReadMembers().ToDictionary(static x => x.Key, static x => x));
    }
    /// <summary>
    /// Reads any valid member from the stream.
    /// </summary>
    /// <param name="hasKey">True if there is a key to read.</param>
    /// <returns>The new parsed member, null if it is an end.</returns>
    public virtual IMember? ReadMember(bool hasKey = true) {
        MemberType type = ReadType();
        if (type == MemberType.End) {
            return null;
        }

        string key = "";
        if (hasKey) {
            key = ReadString();
        }

        if (type == MemberType.Container) {
            return ReadContainer(key);
        } else if (type == MemberType.List) {
            return ReadList(key);
        } else {
            return ReadField(key, type);
        }
    }
    /// <summary>
    /// Reads multiple members.
    /// </summary>
    /// <param name="hasKey">If it should read keys.</param>
    /// <returns>A list of parsed members.</returns>
    public virtual List<IMember> ReadMembers(bool hasKey = true) {
        IMember? member;
        try {
            member = ReadMember(hasKey);
        } catch (EndOfStreamException) {
            return [];
        }
        if (member==null) {
            return [];
        } else {
            return [member, .. ReadMembers(hasKey)];
        }
    }
    /// <summary>
    /// Applies the formatter to make it parsable (should call at the beginning).
    /// </summary>
    /// <param name="formatter">The formatter to use for decoding.</param>
    public void Decode(IFormatter formatter) {
        byte[] raw = new byte[4];
        InStream.Read(raw);
        IntSerializable @int = new(default);
        @int.Parse(raw);
        raw = new byte[@int.Value];
        InStream.Read(raw);
        BaseStream = new MemoryStream(formatter.Decode(raw));
    }
    /// <summary>
    /// Parses the stream.
    /// </summary>
    /// <returns>A dictionary containing the key and child.</returns>
    /// <param name="formatter">The formatter to use for decoding.</param>
    public Dictionary<string, IMember> Parse(IFormatter formatter) {
        Decode(formatter);

        return ReadMembers().ToDictionary(static x => x.Key, static x => x);
    }

    /// <inheritdoc/>
    public override void Dispose() {
        if (Disposed) { return; }

        if (!ShouldCloseStream) { return; }
        
        BaseStream.Close();

        GC.SuppressFinalize(this);

        base.Dispose();
    }
}