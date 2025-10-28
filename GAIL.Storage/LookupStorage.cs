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
public class LookupStorage : BaseStorage, IDisposable {
	/// <summary>
	/// The lookup table of this lookup storage.
	/// </summary>
	public LookupTable LookupTable { get; private set; } = new();
	private Parser? parser;
	/// <inheritdoc/>
	public override bool Load(Stream stream, bool shouldCloseStream = true) {
		try {
			MemoryStream stream2 = new();
			stream.CopyTo(stream2);
			stream.Dispose();
			stream2.Position = 0;
			Parse(new(stream2, shouldCloseStream), Formatter);
		} catch (Exception e) {
			Logger.LogError("Failed to load storage file:");
			Logger.LogException(e);
			return false;
		}
		return true;
	}
	/// <inheritdoc/>
	public override void Parse(Parser parser, IFormatter? formatter = null) {
		if (formatter != null) {
			parser.Decode((p) => {
				Parse(p, null);
			}, formatter);
		} else {
			this.parser = parser;
			LookupTable.Parse(parser, null);
		}
		// NOTE: Not parsing the following, because of the lookup table.
	}
	

	/// <summary>
	/// Gets a field or reads a custom field from the lookup storage.
	/// </summary>
	/// <typeparam name="T">The custom field's type.</typeparam>
	/// <param name="id">The id to use.</param>
	/// <param name="info">The field info of the custom field.</param>
	/// <returns>The custom field.</returns>
	/// <exception cref="InvalidCastException"></exception>
	/// <exception cref="InvalidDataException"></exception>
	public T? GetCustom<T>(List<string> id, IField.Info info) {
		IChildNode? child = GetCustom(id, info);
		if (child == null) return default;
		ISerializable<T> value = (child as ISerializable<T>) ?? throw new InvalidCastException("The field does not support an interface for accessing its value");
		return value.Value;
	}

	/// <summary>
	/// Gets a field or reads a custom field from the lookup storage.
	/// </summary>
	/// <param name="id">The id to use.</param>
	/// <param name="info">The field info of the custom field.</param>
	/// <returns>The custom field.</returns>
	/// <exception cref="InvalidDataException"></exception>
	public IChildNode? GetCustom(List<string> id, IField.Info info) {
		IChildNode? node = base.Get(id);
		if (node == null) {
			if (parser == null) return null;
			uint? position = LookupTable.GetPosition([.. id]);
			if (position == null) return null;
			try {
				parser.BaseStream.Position = position.Value;
			} catch (IOException e) {
				throw new InvalidDataException("Invalid position in lookup table", e);
			}
			if (parser.ReadType() != MemberType.Custom) {
				throw new InvalidDataException("The read member type is not a custom field");
			}
			try {
				node = parser.ReadMember(info, false);
			} catch (EndOfStreamException e) {
				throw new InvalidDataException("Corrupted field ended before it finished reading", e);
			}
			node.Key = id[^1];
			id.RemoveAt(id.Count-1);
			node.SetParent(ParentExtensions.CreateReverse(this, id)); // Using reverse, because we know it is a safe path.
			return node;
		} else {
			return node;
		}
	}
	/// <summary>
	/// Gets the node of the corresponding id. This is relative to this node.
	/// </summary>
	/// <param name="id">The list of id for what path to take.</param>
	/// <returns>The node if that node exists.</returns>
	public IChildNode? Get<T>(IList<string> id) where T : IField {
		IChildNode? node = base.Get([.. id]);
		if (node == null) {
			if (parser == null) return default;
			uint? position = LookupTable.GetPosition([.. id]);
			if (position == null) return default;
			try {
				parser.BaseStream.Position = position.Value;
			} catch (IOException e) {
				throw new InvalidDataException("Invalid position in lookup table", e);
			}
			try {
				node = parser.ReadMember(parser.ReadFieldInfo<T>(), false);
			} catch (EndOfStreamException e) {
				throw new InvalidDataException("Corrupted entry ended before it finished reading", e);
			}
			node.Key = id[^1];
			if (id.Count > 1) {
				id.RemoveAt(id.Count-1);
				node.SetParent(ParentExtensions.CreateReverse(this, id)); // Using reverse, because we know it is a safe path.
			} else {
				node.SetParent(this);
			}
			return node;
		} else {
			return node;
		}
	}
	/// <inheritdoc/>
	public override IChildNode? Get(IList<string> id) {
		return Get<IField>(id);
	}
	private static void WriteParent(Serializer content, Serializer lookup, IParentNode parent) {
		foreach (IChildNode c in parent.Children.Values) {
			if (c is IParentNode p) {
				WriteParent(content, lookup, p);
			} else if (c is IField field) {
				LookupTable.WriteEntry(lookup, field.ID, Convert.ToUInt32(content.BaseStream.Position));
				content.WriteMember(field, false);
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
			using Serializer content = new();
			WriteParent(content, serializer, this);
			LookupTable.WriteEnd(serializer);
			serializer.Write((content.BaseStream as MemoryStream)!.ToArray());
		}
	}

	/// <inheritdoc/>
	public void Dispose() {
		parser?.Dispose();
		GC.SuppressFinalize(this);
	}
}