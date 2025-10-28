namespace GAIL.Storage.Members;

/// <summary>
/// Represents a type of the member.
/// </summary>
public enum MemberType : byte {
	/// <summary>
	/// Represents the end of a member or field.
	/// </summary>
	End,
	/// <summary>
	/// The type of a container.
	/// </summary>
	Container,
	/// <summary>
	/// The primary type of a list field.
	/// </summary>
	List,
	/// <summary>
	/// The type of a lookup table entry.
	/// </summary>
	LookupTable,
	/// <summary>
	/// The type of a basic boolean field.
	/// </summary>
	Bool,
	/// <summary>
	/// The type of a basic float field.
	/// </summary>
	Float,
	/// <summary>
	/// The type of a basic double field.
	/// </summary>
	Double,
	/// <summary>
	/// The type of a basic byte field.
	/// </summary>
	Byte,
	/// <summary>
	/// The type of a basic short field.
	/// </summary>
	Short,
	/// <summary>
	/// The type of a basic int field.
	/// </summary>
	Int,
	/// <summary>
	/// The type of a basic long field.
	/// </summary>
	Long,
	/// <summary>
	/// The type of a basic sbyte field.
	/// </summary>
	SByte,
	/// <summary>
	/// The type of a basic ushort field.
	/// </summary>
	UShort,
	/// <summary>
	/// The type of a basic uint field.
	/// </summary>
	UInt,
	/// <summary>
	/// The type of a basic ulong field.
	/// </summary>
	ULong,
	/// <summary>
	/// The type of a basic bytes field.
	/// </summary>
	Bytes,
	/// <summary>
	/// The type of a basic string field.
	/// </summary>
	String,
	/// <summary>
	/// The type used for a custom field.
	/// </summary>
	Custom
}