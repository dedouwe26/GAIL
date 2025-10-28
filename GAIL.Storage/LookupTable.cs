using System.Collections.ObjectModel;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Members;
using GAIL.Storage.Streams;

namespace GAIL.Storage;

/// <summary>
/// Represents a table of the lookup table in which the relative position of all fields is stored.
/// </summary>
public class LookupTable {
	private readonly Dictionary<string[], uint> lookup = [];
	private uint offset;
	/// <summary>
	/// The parsed lookup table. This is empty if it hasn't been parsed yet.
	/// </summary>
	public ReadOnlyDictionary<string[], uint> Lookup { get => new(lookup); }
	private static KeyValuePair<string[], uint> ReadPair(Parser p) {
		return new([.. p.ReadSerializable(ListSerializable<StringSerializable>.CreateInfo(StringSerializable.Info)).Value.Select(s => s.Value)], p.ReadUInt());
	}
	/// <summary>
	/// Gets the relative position of that field.
	/// </summary>
	/// <param name="id">The ID of that field.</param>
	/// <returns>The relative position of that field.</returns>
	public uint? GetPosition(string[] id) {
		try {
			return lookup.First((kvp) => kvp.Key.SequenceEqual(id)).Value + offset;
		} catch (InvalidOperationException) {
			return null;
		}
	}
	/// <summary>
	/// Reads the entire lookup table.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <param name="formatter"></param>
	/// <exception cref="InvalidDataException">A lookup table cannot contain any other type than LookupTable and End.</exception>
	public void Parse(Parser parser, IFormatter? formatter = null) {
		if (formatter != null) {
			parser.Decode((p) => {
				Parse(p, null);
			}, formatter);
		} else {
			MemberType type;
			do {
				type = parser.ReadType();
				if (type == MemberType.LookupTable) {
					((IDictionary<string[], uint>)lookup).Add(ReadPair(parser));
				} else if (type != MemberType.End) {
					throw new InvalidDataException("A lookup table cannot contain any other type than LookupTable and End");
				}
			} while (type!=MemberType.End);
			offset = Convert.ToUInt32(parser.BaseStream.Position);
		}
	}
	/// <summary>
	/// Writes an entry of a lookup table.
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	/// <param name="id">The ID of that entry.</param>
	/// <param name="position">The position after this lookup table (the first byte after the lookup table is position 0).</param>
	public static void WriteEntry(Serializer serializer, string[] id, uint position) {
		serializer.WriteType(MemberType.LookupTable);
		serializer.WriteSerializable(new ListSerializable<StringSerializable>(id.Select((s) => new StringSerializable(s))));
		serializer.WriteUInt(position);
	}
	/// <summary>
	/// Signals the end of this lookup table.
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	public static void WriteEnd(Serializer serializer) {
		serializer.WriteType(MemberType.End);
	}
}