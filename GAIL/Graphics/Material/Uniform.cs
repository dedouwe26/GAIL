namespace GAIL.Graphics.Material;

/// <summary>
/// Represents a value that is sent to the shader.
/// </summary>
public abstract class Uniform {
    /// <summary>
    /// The info about the format of this uniform.
    /// </summary>
    public FormatInfo info;
    /// <summary>
    /// The value of this uniform.
    /// </summary>
    public byte[] value;

    /// <summary>
    /// Creates a new Uniform
    /// </summary>
    /// <param name="info">The info about the format of this uniform.</param>
    /// <param name="value">The value of this uniform.</param>
    public Uniform(FormatInfo info, byte[] value) {
        this.info = info;
        this.value = value;
    }
}