using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Storage;

/// <summary>
/// A file containing data and a lookup table.
/// </summary>
public class LookupStorage : BaseStorage {
    public LookupTable lookupTable;
    /// <inheritdoc/>
    public override void Parse(Parser parser, IFormatter? formatter = null) {
        lookupTable = parser.ReadSerializable(LookupTable.Info);
    }

    /// <inheritdoc/>
    public override void Serialize(Serializer serializer, IFormatter? formatter = null) {
    }
}