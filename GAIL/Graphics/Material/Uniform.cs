namespace GAIL.Graphics.Material;

/// <summary>
/// Represents a value that is sent to the shader.
/// </summary>
public abstract class Uniform {
    /// <summary>
    /// The type of this uniform.
    /// </summary>
    public AttributeType Type;
    /// <summary>
    /// The value of this uniform.
    /// </summary>
    public byte[] Value;

    /// <summary>
    /// Creates a new Uniform
    /// </summary>
    /// <param name="type">The type of this uniform.</param>
    /// <param name="value">The value of this uniform.</param>
    public Uniform(AttributeType type, byte[] value) {
        Type = type;
        Value = value;
    }
}