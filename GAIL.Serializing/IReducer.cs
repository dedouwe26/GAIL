using System.Diagnostics.Contracts;
using System.Reflection;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing;


/// <summary>
/// Represents a class that can be turned into serializables and can be created from serializables using streams.
/// </summary>
public interface IReducer : ISerializable {
	/// <summary>
	/// The serializables of which this reducer is made of.
	/// </summary>
	[Pure]
	public Info[] Format { get; }
	/// <summary>
	/// Serializes this reducer into serializables.
	/// </summary>
	/// <returns>The serialized reducer.</returns>
	public ISerializable[] Serialize();
	/// <summary>
	/// Creates this reducer from serializables.
	/// </summary>
	/// <param name="serializables">The serializables to parse the data from.</param>
	public void Parse(ISerializable[] serializables);
}

/// <summary>
/// A default implementation of a reducer.
/// </summary>
public abstract class Reducer : IReducer {
	/// <summary>
	/// The serializables of which this reducer is made of.
	/// </summary>
	[Pure]
	public abstract ISerializable.Info[] Format { get; }
	/// <summary>
	/// Serializes this reducer into serializables.
	/// </summary>
	/// <returns>The serialized reducer.</returns>
	public abstract ISerializable[] Serialize();
	/// <summary>
	/// Creates this reducer from serializables.
	/// </summary>
	/// <param name="serializables">The serializables to parse the data from.</param>
	public abstract void Parse(ISerializable[] serializables);

	/// <inheritdoc/>
	public void Serialize(Serializer serializer, IFormatter? formatter = null) {
		if (Format.Length < 1) return;

		if (formatter != null) {
			serializer.Encode((s) => {
				Serialize(s, null);
			}, formatter);
		} else {
			foreach (ISerializable serializable in Serialize()) {
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
			ISerializable[] serializables = new ISerializable[Format.Length];
			for (int i = 0; i < Format.Length; i++) {
				ISerializable.Info info = Format[i];
				serializables[i] = parser.ReadSerializable(Format[i]);
			}
			Parse(serializables);
		}
	}
}