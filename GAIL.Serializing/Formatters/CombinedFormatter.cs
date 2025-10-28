namespace GAIL.Serializing.Formatters;

/// <summary>
/// Represents a formatter that combines multiple formatters into one.
/// </summary>
public class CombinedFormatter : IFormatter
{
	private readonly IFormatter[] formatters;
	/// <summary>
	/// Creates a new combined formatter.
	/// </summary>
	/// <param name="formatters">The formatters to use (the first formatter starts while encoding and the first one ends while decoding).</param>
	public CombinedFormatter(IFormatter[] formatters) {
		this.formatters = formatters;
	}
	/// <inheritdoc/>
	public byte[] Decode(byte[] encoded) {
		foreach (IFormatter formatter in formatters.Reverse()) {
			encoded = formatter.Decode(encoded);
		}
		return encoded;
	}

	/// <inheritdoc/>
	public byte[] Encode(byte[] original) {
		foreach (IFormatter formatter in formatters) {
			original = formatter.Encode(original);
		}
		return original;
	}
}