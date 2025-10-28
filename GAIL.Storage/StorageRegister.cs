using GAIL.Serializing;
using GAIL.Serializing.Streams;
using GAIL.Storage.Members;
using GAIL.Storage.Streams;

namespace GAIL.Storage;

/// <summary>
/// Represents a class that handles the creation of fields.
/// </summary>
public static class StorageRegister<TContent> where TContent : IField {
	private static IField.Info GenerateInfo<TValue>(MemberType type, ISerializable.Info baseInfo) where TValue : notnull {
		return BasicField<TValue>.CreateInfo(type, baseInfo);
	}
	private static readonly KeyValuePair<MemberType, IField.Info>[] registeredFields = [
		new(MemberType.Container, Container.Info),

		new(MemberType.Bool, GenerateInfo<bool>(MemberType.Bool, BoolSerializable.Info)),

		new(MemberType.Float, GenerateInfo<float>(MemberType.Float, FloatSerializable.Info)),
		new(MemberType.Double, GenerateInfo<double>(MemberType.Double, DoubleSerializable.Info)),

		new(MemberType.Byte, GenerateInfo<byte>(MemberType.Byte, ByteSerializable.Info)),
		new(MemberType.Short, GenerateInfo<short>(MemberType.Short, ShortSerializable.Info)),
		new(MemberType.Int, GenerateInfo<int>(MemberType.Int, IntSerializable.Info)),
		new(MemberType.Long, GenerateInfo<long>(MemberType.Long, LongSerializable.Info)),

		new(MemberType.SByte, GenerateInfo<sbyte>(MemberType.SByte, SByteSerializable.Info)),
		new(MemberType.UShort, GenerateInfo<ushort>(MemberType.UShort, UShortSerializable.Info)),
		new(MemberType.UInt, GenerateInfo<uint>(MemberType.UInt, UIntSerializable.Info)),
		new(MemberType.ULong, GenerateInfo<ulong>(MemberType.ULong, ULongSerializable.Info)),

		new(MemberType.Bytes, GenerateInfo<byte[]>(MemberType.Bytes, BytesSerializable.Info)),
		new(MemberType.String, GenerateInfo<string>(MemberType.String, StringSerializable.Info))
	];
	private static readonly KeyValuePair<MemberType, Func<IField.Info, IField.Info>>[] registeredDynamicFields = [
		new(MemberType.List, ListField<TContent>.CreateInfo)
	];
	/// <summary>
	/// Gets a static field info from a type.
	/// </summary>
	/// <param name="type">The type to use.</param>
	/// <returns>The field info corresponding to that type.</returns>
	/// <exception cref="ArgumentException">No matching field info exists.</exception>
	public static IField.Info GetStaticInfo(MemberType type) {
		try {
			return registeredFields.First((p) => p.Key == type).Value;
		} catch (InvalidOperationException e) {
			throw new ArgumentException("No matching field info exists for: "+type.ToString()+", is it a custom or dynamic field?", nameof(type), e);
		}
	}
	/// <summary>
	/// Gets the field info from a member type.
	/// </summary>
	/// <param name="type">The member type to use.</param>
	/// <param name="parser">The parser to read the additional types from.</param>
	/// <returns>The info corresponding to the type.</returns>
	/// <exception cref="ArgumentException">No matching field info exists.</exception>
	public static IField.Info GetInfo(Parser parser, MemberType type) {
		try {
			Func<IField.Info, IField.Info> creator = registeredDynamicFields.First((p) => p.Key == type).Value;
			return creator(GetInfo(parser, parser.ReadType()));
		} catch (InvalidOperationException) {
			return GetStaticInfo(type);
		}
	}
}