using System.Diagnostics.Contracts;
using System.Reflection;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing;


/// <summary>
/// Represents a class that can be turned into raw serializables and can be created from serializables.
/// </summary>
public interface IRawReducer : ISerializable {
    /// <summary>
    /// The raw serializables of which this raw reducer is made of.
    /// </summary>
    [Pure]
    public Info[] Format { get; }
    /// <summary>
    /// Serializes this reducer into raw serializables.
    /// </summary>
    /// <returns>The serialized reducer.</returns>
    public IRawSerializable[] Serialize();
    /// <summary>
    /// Creates this reducer from raw serializables.
    /// </summary>
    /// <param name="serializables">The raw serializables to parse the data from.</param>
    public void Parse(IRawSerializable[] serializables);
}
/// <summary>
/// A default implementation of a raw reducer.
/// </summary>
public abstract class RawReducer : IRawReducer {
    /// <summary>
    /// The raw serializables of which this raw reducer is made of.
    /// </summary>
    [Pure]
    public abstract ISerializable.Info[] Format { get; }
    /// <summary>
    /// Serializes this reducer into raw serializables.
    /// </summary>
    /// <returns>The serialized reducer.</returns>
    public abstract IRawSerializable[] Serialize();
    /// <summary>
    /// Creates this reducer from raw serializables.
    /// </summary>
    /// <param name="serializables">The raw serializables to parse the data from.</param>
    public abstract void Parse(IRawSerializable[] serializables);

    /// <inheritdoc/>
    public void Serialize(Serializer serializer, IFormatter? formatter = null) {
        if (Format.Length < 1) return;

        if (formatter != null) {
            serializer.Encode((s) => {
                Serialize(s, null);
            }, formatter);
        } else {
            foreach (IRawSerializable serializable in Serialize()) {
                serializer.WriteSerializable(serializable);
            }
        }
    }
    /// <inheritdoc/>
    public void Parse(Parser parser, IFormatter? formatter = null) {
        if (Format.Length < 1) {
            Parse([]);
            return;
        }

        if (formatter != null) {
            parser.Decode((p) => {
                Parse(p, null);
            }, formatter);
        } else {
            IRawSerializable[] serializables = new IRawSerializable[Format.Length];
            for (int i = 0; i < Format.Length; i++) {
                try {
                    serializables[i] = parser.ReadSerializable<IRawSerializable>(Format[i], null);
                } catch (InvalidCastException e) {
                    throw new InvalidDataException("The format of an raw reducer doesn't only contain raw serializables", e);
                }
            }
            Parse(serializables);
        }
    }
}