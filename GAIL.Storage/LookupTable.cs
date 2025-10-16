using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Storage;

public class LookupTable : ISerializable {
    private static ISerializable.Info<LookupTable>? info;
    public static ISerializable.Info<LookupTable> Info { get {
        info ??= ISerializable.CreateInfo<LookupTable>();
        return info;
    } }

    private static Dictionary<string, uint> GenerateLookupTable(LookupStorage storage) {

    }

    public readonly Dictionary<string, uint> lookup;

    public LookupTable(Dictionary<string, uint> lookupDictionary) {
        lookup = lookupDictionary;
    }
    public LookupTable() : this([]) { }
    public LookupTable(LookupStorage storage) : this(GenerateLookupTable(storage)) { }

    private static KeyValuePair<string, uint> ReadPair(Parser p) {
		return new(p.ReadString(), p.ReadUInt());
	}
    /// <inheritdoc/>
    public void Parse(Parser parser, IFormatter? formatter = null) {
        if (formatter != null) {
			parser.Decode((p) => {
				ReadPair(p);
			}, formatter);
		} else {
			ReadPair(p);
        }
    }

    /// <inheritdoc/>
    public void Serialize(Serializer serializer, IFormatter? formatter = null) {
        throw new NotImplementedException();
    }
}