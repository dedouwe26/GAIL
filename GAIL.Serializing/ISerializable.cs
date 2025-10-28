using System.Reflection;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing;

/// <summary>
/// Represents a class that can be turned into bytes and can be created from bytes.
/// </summary>
public interface ISerializable {
	/// <summary>
	/// Represents info for a serializable.
	/// </summary>
	/// <param name="Creator">The instance creator of the serializable.</param>
	public record Info (Func<Parser, IFormatter?, ISerializable> Creator);
	/// <summary>
	/// Represents info for a serializable.
	/// </summary>
	/// <typeparam name="T">The type of the serializable.</typeparam>
	/// <param name="TypedCreator">The typed instance creator of the serializable.</param>
	public record Info<T> (Func<Parser, IFormatter?, T> TypedCreator) : Info((p, f) => TypedCreator(p, f)) where T : ISerializable;
	
	/// <summary>
	/// Creates a serializable info.
	/// </summary>
	/// <param name="instantiator">The instantiator of the serializable.</param>
	/// <returns>A new serializable info.</returns>
	public static Info CreateInfo(Func<ISerializable> instantiator) {
		return new ((p, f) => {
			ISerializable inst = instantiator();
			inst.Parse(p, f);
			return inst;
		});
	}
	/// <summary>
	/// Creates a serializable info.
	/// </summary>
	/// <typeparam name="T">The type of the serializable.</typeparam>
	/// <returns>A new serializable info.</returns>
	public static Info<T> CreateInfo<T>() where T : ISerializable, new() {
		return new ((p, f) => {
			T inst = new();
			inst.Parse(p, f);
			return inst;
		});
	}

	/// <summary>
	/// Turns this class into bytes.
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	/// <param name="formatter">The formatter to use.</param>
	public void Serialize(Serializer serializer, IFormatter? formatter = null);
	/// <summary>
	/// Creates this class from bytes.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <param name="formatter">The formatter to use.</param>
	public void Parse(Parser parser, IFormatter? formatter = null);
}
/// <summary>
/// Represents a class that turns <typeparamref name="T"/> into bytes and the other way around.
/// </summary>
/// <typeparam name="T">The type of the object to be serializable.</typeparam>
public interface ISerializable<T> : ISerializable {
	/// <summary>
	/// The value of the serializable.
	/// </summary>
	public T Value { get; set; }
}