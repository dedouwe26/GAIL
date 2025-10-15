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

    public Dictionary<string, uint> lookup;

    public LookupTable(Dictionary<string, uint> lookupDictionary) {
        lookup = lookupDictionary;
    }
    public LookupTable() : this([]) { }
    public LookupTable(LookupStorage storage) : this(GenerateLookupTable(storage)) { }

    /// <inheritdoc/>
    public void Parse(Parser parser, IFormatter? formatter = null) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Serialize(Serializer serializer, IFormatter? formatter = null) {
        throw new NotImplementedException();
    }
}