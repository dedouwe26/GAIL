using System.Collections;
using GAIL.Serializing;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Streams;

namespace GAIL.Storage.Members;

/// <summary>
/// A list can contain more members without id.
/// </summary>
public class ListField<T> : WrapperField<IEnumerable<T>>, IField<IEnumerable<T>>, IEnumerable<T> where T : IField {
	/// <summary>
	/// A utility to create a list field.
	/// </summary>
	/// <param name="key">The key to use.</param>
	/// <param name="contentType">The member type of the content, this CANNOT be a type of a dynamic field like ListField.</param>
	/// <param name="values">The values to start with.</param>
	/// <returns>A new list field.</returns>
	public static ListField<T> Create(string key, MemberType contentType, IEnumerable<T> values) {
		return new(key, values, StorageRegister<T>.GetStaticInfo(contentType));
	}
	
	/// <summary>
	/// A utility to create a list field.
	/// </summary>
	/// <param name="key">The key to use.</param>
	/// <param name="parent">The parent to use.</param>
	/// <param name="contentType">The member type of the content, this CANNOT be a type of a dynamic field like ListField.</param>
	/// <param name="values">The values to start with.</param>
	/// <returns>A new list field.</returns>
	public static ListField<T> Create(string key, IParentNode parent, MemberType contentType, IEnumerable<T> values) {
		return new(key, parent, values, StorageRegister<T>.GetStaticInfo(contentType));
	}
	/// <summary>
	/// The info for a list field.
	/// </summary>
	public static IField.Info CreateInfo(IField.Info contentInfo) {
		return IField.CreateInfo(() => new ListField<T>(contentInfo), (s) => WriteType(s, contentInfo));
	}

	/// <inheritdoc/>
	public IEnumerator<T> GetEnumerator() => Value.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	private readonly IField.Info ContentInfo;

	private ListField(IField.Info contentInfo) : base(
		ListSerializable<T>.CreateInfo(contentInfo.Convert(false))
	) { ContentInfo = contentInfo; }
	/// <summary>
	/// Creates serializer-ready list field.
	/// </summary>
	/// <param name="key">The key of the list field.</param>
	/// <param name="values">The values to initially use.</param>
	/// <param name="contentInfo">The info of the content.</param>
	public ListField(string key, IEnumerable<T> values, IField.Info contentInfo) : base(key, new ListSerializable<T>(values)) { ContentInfo = contentInfo; }
	/// <summary>
	/// Creates a serializer-ready list field.
	/// </summary>
	/// <param name="key">The key of the list field.</param>
	/// <param name="values">The values to initially use.</param>
	/// <param name="parent">The parent of this list.</param>
	/// <param name="contentInfo">The info of the content.</param>
	public ListField(string key, IParentNode parent, IEnumerable<T> values, IField.Info contentInfo) : base(key, new ListSerializable<T>(values), parent) { ContentInfo = contentInfo; }

	private static void WriteType(Serializer serializer, IField.Info contentInfo) {
		serializer.WriteType(MemberType.List);
		contentInfo.WriteType(serializer);
	}
	/// <inheritdoc/>
	public override void WriteType(Serializer serializer) {
		WriteType(serializer, ContentInfo);
	}
}