using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;

namespace GAIL.Storage;

/// <summary>
/// A file containing data and a lookup table.
/// </summary>
public class LookupStorage : BaseStorage {
    public LookupTable LookupTable { get; private set; }
	/// <inheritdoc/>
	public override void Parse(Parser parser, IFormatter? formatter = null) {
		if (formatter != null) {
			parser.Decode((p) => {
				Parse(p, null);
			}, formatter);
		}
        LookupTable = parser.ReadSerializable(LookupTable.Info);
    }

	private void WriteChild(Serializer serializer, IChildNode child) {
		if (child is IParentNode parent) {
			foreach (IChildNode c in parent.Children.Values) {
				WriteChild(serializer, c);
			}
		} else if (child is ISerializable serializable) {
			serializer.WriteSerializable(serializable);
		}
	}
	/// <inheritdoc/>
	public override void Serialize(Serializer serializer, IFormatter? formatter = null) {
		if (formatter != null) {
			serializer.Encode((s) => {
				Serialize(s, null);
			}, formatter);
		}
        serializer.WriteSerializable(LookupTable);
		foreach (IChildNode child in children.Values) {
			
		}
    }
}