using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Members;
using GAIL.Storage.Streams;

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
		} else {
        	LookupTable = parser.ReadSerializable(LookupTable.Info);
		}
		// NOTE: Not parsing the following, because of the lookup table.
    }
	private void WriteParent(Serializer serializer, IParentNode parent) {
		foreach (IChildNode c in parent.Children.Values) {
			if (c is IParentNode p) {
				WriteParent(serializer, p);
			} else if (c is IField field) {
				serializer.WriteMember(field, false);
			}
		}
	}
	/// <inheritdoc/>
	public override void Serialize(Serializer serializer, IFormatter? formatter = null) {
		if (formatter != null) {
			serializer.Encode((s) => {
				Serialize(s, null);
			}, formatter);
		} else {
			serializer.WriteSerializable(LookupTable);
			WriteParent(serializer, this);
		}
    }
}