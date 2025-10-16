using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Streams;

namespace GAIL.Storage;

/// <summary>
/// Represents a file containing data.
/// </summary>
public sealed class SimpleStorage : BaseStorage, IParentNode {
    /// <inheritdoc/>
    public SimpleStorage(IFormatter? formatter = null) : base(formatter) { }

	/// <inheritdoc/>
	public override void Parse(Parser parser, IFormatter? formatter = null) {
		if (formatter != null) {
			parser.Decode((p) => {
				Parse(p, null);
			}, formatter);
		} else {
			children = parser.ReadChildren().ToDictionary(static x => x.Key, static x => x);
		}
	}

	/// <inheritdoc/>
	public override void Serialize(Serializer serializer, IFormatter? formatter = null) {
		if (formatter != null) {
			serializer.Encode((s) => {
				Serialize(s, null);
			}, formatter);
		} else {
            serializer.WriteChildren([.. children.Values]);
        }
	}
}