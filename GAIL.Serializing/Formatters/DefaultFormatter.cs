namespace GAIL.Serializing.Formatters;

/// <summary>
/// A formatter that doesn't apply any effects or formats.
/// </summary>
public class DefaultFormatter : IFormatter {
    /// <inheritdoc/>
    public byte[] Decode(byte[] encoded) {
        return encoded;
    }

    /// <inheritdoc/>
    public byte[] Encode(byte[] original) {
        return original;
    }
}