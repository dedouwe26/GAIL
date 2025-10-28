using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Members;

namespace GAIL.Storage.Streams;

/// <summary>
/// Adds methods to read storage specific data.
/// </summary>
public static class ParserExtensions {
	/// <summary>
	/// Reads the type from the parser.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <returns>The read member type.</returns>
	public static MemberType ReadType(this Parser parser) {
		return (MemberType)parser.ReadByte();
	}
	/// <summary>
	/// Retrieves the field info from the written member types.
	/// </summary>
	/// <param name="parser">The parser to read the member types from.</param>
	/// <returns>The retrieved field info.</returns>
	public static IField.Info ReadFieldInfo<TContent>(this Parser parser) where TContent : IField {
		return StorageRegister<TContent>.GetInfo(parser, ReadType(parser));
	}
	/// <summary>
	/// Reads any valid member from the stream.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <param name="info">The field info used for reading.</param>
	/// <param name="hasKey">True if there is a key to read.</param>
	/// <returns>The new parsed member.</returns>
	public static IField ReadMember(this Parser parser, IField.Info info, bool hasKey = true) {
		return info.FieldCreator(parser, hasKey, null);
	}
	/// <summary>
	/// Reads multiple members.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <param name="hasKey">If it should read id.</param>
	/// <returns>A list of parsed members.</returns>
	public static List<IField> ReadChildren(this Parser parser, bool hasKey = true) {
		IField? member;
		try {
			member = ReadMember(parser, ReadFieldInfo<IField>(parser), hasKey);
		} catch (EndOfStreamException) {
			return [];
		}
		if (member==null) {
			return [];
		} else {
			return [member, .. ReadChildren(parser, hasKey)];
		}
	}
}