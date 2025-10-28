using System.Reflection;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Networking;

/// <summary>
/// Packet is an abstract class for parsing and formatting a packet.
/// </summary>
public abstract class Packet : ISerializable {
    /// <summary>
    /// Describes a field in a packet.
    /// </summary>
    /// <param name="Property">The property of the field.</param>
    /// <param name="Info">The serializable info of the field.</param>
    public record class FieldInfo(PropertyInfo Property, ISerializable.Info Info);

#pragma warning disable IL2075
    /// <summary>
    /// Describes a packet.
    /// </summary>
    public readonly struct Info {
        private static ConstructorInfo? GetConstructor(ConstructorInfo[] constructors) {
            foreach (ConstructorInfo constructor in constructors) {
                if (constructor.GetParameters().Length > 0) {
                    throw new ArgumentException($"Constructor {constructor.Name} in {constructor.ReflectedType?.Name ?? "packet"} cannot have parameters");
                }
                return constructor;
            }
            return null;
        }
        /// <summary>
		/// Creates the info from a packet. The constructor must make parser-ready packet.
		/// </summary>
		/// <returns>The created packet info.</returns>
        public static Info Create(Packet packet) {
			Type type = packet.GetType();
			string name = type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
			ConstructorInfo? c;
			try {
                c = GetConstructor(type.GetConstructors());
            } catch (Exception e) {
                throw new ArgumentException($"Failed at the getting constructor of packet ({name})", e);
            }
            if (c == null) {
                throw new ArgumentException($"Could not find the constructor of packet ({name})");
            }
			return new(() => (Packet)c.Invoke(null), type);
		}
        /// <summary>
		/// Creates the info from a packet type. The constructor must make parser-ready packet.
		/// </summary>
		/// <typeparam name="T">The packet type.</typeparam>
		/// <returns>The created packet info.</returns>
        public static Info Create<T>() where T : Packet, new() {
			return new(() => new T(), typeof(T));
		}
        private readonly Func<Packet> constructor;
        /// <summary>
		/// The type of the packet used for equality checks.
		/// </summary>
		public readonly Type type;
		/// <summary>
		/// Creates a new packet info.
		/// </summary>
		/// <param name="constructor">The constructor of the packet (must make an parser-ready packet).</param>
		/// <param name="type">The type used for equality checks.</param>
		public Info(Func<Packet> constructor, Type type) {
            this.constructor = constructor;
			this.type = type;
		}
        /// <summary>
        /// Creates a packet from the parser (the creator of a packet).
        /// </summary>
        /// <param name="parser">The parser from which to read the packet.</param>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>The parsed packet.</returns>
        public Packet Create(Parser parser, IFormatter? formatter) {
			Packet packet = constructor();
			packet.Parse(parser, formatter);
            return packet;
        }
        /// <summary>
		/// Converts this packet info to a serializable info.
		/// </summary>
		/// <returns>The created serializable info.</returns>
		public ISerializable.Info<Packet> Convert() => new(Create);
	}
#pragma warning restore IL2075

    /// <summary>
    /// The formatter used for encoding / decoding this packet.
    /// </summary>
    public virtual IFormatter? Formatter => null;
    /// <summary>
    /// Creates a packet (add own data here).
    /// </summary>
    public Packet() { }

    private uint? id;
    /// <summary>
    /// The ID of this packet.
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    public uint ID { get {
        id ??= NetworkRegister.GetPacketID(this) ?? throw new InvalidOperationException($"Packet {GetType().Name} is not registered");
        return id.Value;
    } }

    /// <summary>
	/// A simplified serialize method, that serializes this packet. 
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	public abstract void Serialize(Serializer serializer);

    /// <summary>
	/// A simplified parse method, that parses this packet. 
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	public abstract void Parse(Parser parser);

	/// <summary>
	/// Encodes data written to the stream using the packet formatter and the global formatter.
	/// </summary>
	/// <param name="consumer">he consumer that writes the data that needs encoding.</param>
	/// <param name="serializer">The serializer to write to.</param>
	/// <param name="formatter">The formatter to use aside from <see cref="Formatter"/>.</param>
	protected void Encode(Action<Serializer> consumer, Serializer serializer, IFormatter? formatter) {
        if (formatter != null) {
			serializer.Encode((s) => Encode(consumer, s, null), formatter);
		} else {
            if (Formatter != null) {
				serializer.Encode((s) => consumer(s), Formatter);
			} else {
				consumer(serializer);
			}
        }
    }
    /// <summary>
    /// Decodes data read from the stream using the packet formatter and the global formatter.
    /// </summary>
    /// <param name="consumer">he consumer that reads the data that needs encoding.</param>
    /// <param name="parser">The parser to read from.</param>
    /// <param name="formatter">The formatter to use aside from <see cref="Formatter"/>.</param>
    protected void Decode(Action<Parser> consumer, Parser parser, IFormatter? formatter) {
        if (formatter != null) {
			parser.Decode((p) => Decode(consumer, p, null), formatter);
		} else {
            if (Formatter != null) {
				parser.Decode((p) => consumer(p), Formatter);
			} else {
				consumer(parser);
			}
        }
    }

    /// <inheritdoc/>
    public void Serialize(Serializer serializer, IFormatter? formatter = null) {
        Encode(Serialize, serializer, formatter);
    }

    /// <inheritdoc/>
    public void Parse(Parser parser, IFormatter? formatter = null) {
        Decode(Parse, parser, formatter);
    }
}