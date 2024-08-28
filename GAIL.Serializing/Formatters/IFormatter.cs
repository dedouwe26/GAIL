namespace GAIL.Serializing.Formatters;

/// <summary>
/// Can replace bytes for compressing or encryption.
/// </summary>
public interface IFormatter {
    /// <summary>
    /// Can encode the bytes.
    /// </summary>
    /// <param name="original">The original state of the bytes.</param>
    /// <returns>The encoded state of the bytes.</returns>
    public byte[] Encode(byte[] original);
    /// <summary>
    /// Can decode the bytes.
    /// </summary>
    /// <param name="encoded">The encoded state of the bytes.</param>
    /// <returns>The original state of the bytes.</returns>
    public byte[] Decode(byte[] encoded);
}