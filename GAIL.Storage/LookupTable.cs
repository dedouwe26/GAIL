using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Members;
using GAIL.Storage.Streams;

namespace GAIL.Storage;

public class LookupTable {

	public Dictionary<string, IChildNode> lookup;

	public LookupTable() {
		lookup = [];
	}

	private static KeyValuePair<string, uint> ReadPair(Parser p) {
		return new(p.ReadString(), p.ReadUInt());
	}
	public void Parse(Parser parser, IFormatter? formatter = null) {
		if (formatter != null) {
			parser.Decode((p) => {
				Parse(p, null);
			}, formatter);
		} else {
			Lookup = [];
			MemberType type;
			do {
				type = parser.ReadType();
				if (type == MemberType.LookupTable) {
					((IDictionary<string, uint>)Lookup).Add(ReadPair(parser));
				} else if (type != MemberType.End) {
					throw new InvalidDataException("A lookup table cannot contain any other type than LookupTable and End");
				}
			} while (type!=MemberType.End);
		}
	}
	public readonly KeyValuePair<string[], uint>[] lookup;
	public LookupStorage() {
		lookup = [];
	}
	public void WriteEntry(Serializer serializer, string[] id, uint location) {
		serializer.WriteSerializable()
	}
	public void WriteEnd(Serializer serializer) {
		serializer.WriteType(MemberType.End);
	}
}