using GAIL.Serializing;

namespace GAIL.Storage;

/// <summary>
/// Represents a field that can convert data.
/// </summary>
public abstract class Field : ISerializable {
    /// <summary>
    /// Creates a field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <exception cref="InvalidOperationException"/>
    protected Field(string key) {
        if (key.Contains('.')) {
            throw new InvalidOperationException("Invalid key, cannot contain dots");
        }
        Key = key;
    }
    /// <summary>
    /// The key of this field.
    /// </summary>
    /// <value></value>
    public string Key { get; private set; }

    /// <inheritdoc/>
    public abstract void Parse(byte[] data);

    /// <inheritdoc/>
    public abstract byte[] Serialize();
}