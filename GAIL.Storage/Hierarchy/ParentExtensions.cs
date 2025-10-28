using System.Diagnostics.CodeAnalysis;
using GAIL.Serializing;
using GAIL.Storage.Members;

namespace GAIL.Storage.Hierarchy;

/// <summary>
/// Adds more methods to parent for quality of life. 
/// </summary>
public static class ParentExtensions {
	/// <summary>
	/// Gets the node of the corresponding ID. This is relative to this node.
	/// </summary>
	/// <typeparam name="T">The type of the child node.</typeparam>
	/// <param name="parent">The parent to start from.</param>
	/// <param name="id">The list of id for what path to take.</param>
	/// <returns>The node if that node exists.</returns>
	public static T? Get<T>(this IParentNode parent, IList<string> id) where T : IChildNode {
		return (T?)parent.Get(id);
	}
	/// <summary>
	/// Gets the value of the field.
	/// </summary>
	/// <typeparam name="T">The type of the value (example: byte, string, bool).</typeparam>
	/// <param name="parent">The parent to start from.</param>
	/// <param name="id">The relative path from this node to the field.</param>
	/// <returns>The value of that field if it exists.</returns>
	/// <exception cref="InvalidCastException">The field does not support an interface for accessing its value</exception>
	public static T? GetValueOrDefault<T>(this IParentNode parent, IList<string> id) {
		IChildNode? child = parent.Get(id);
        if (child == null) return default;
		ISerializable<T> value = (child as ISerializable<T>) ?? throw new InvalidCastException("The field does not support an interface for accessing its value");
		return value.Value;
	}
	/// <summary>
	/// Gets the value of the field.
	/// </summary>
	/// <typeparam name="T">The type of the value (example: byte, string, bool).</typeparam>
	/// <param name="parent">The parent to start from.</param>
	/// <param name="id">The relative path from this node to the field.</param>
	/// <param name="value">The value of that field.</param>
	/// <returns>Whether it exists.</returns>
	/// <exception cref="InvalidCastException">The field does not support an interface for accessing its value</exception>
	public static bool TryGetValue<T>(this IParentNode parent, IList<string> id, [NotNullWhen(true)] out T? value) {
		IChildNode? child = parent.Get(id);
        if (child == null) {
			value = default;
			return false;
		}
		ISerializable<T> serializable = (child as ISerializable<T>) ?? throw new InvalidCastException("The field does not support an interface for accessing its value");
		value = serializable.Value!;
		return true;
	}
	/// <summary>
	/// Keeps creating containers until the path is reached (top to bottom, less efficient, better checks).
	/// </summary>
	/// <param name="parent">The parent to start from.</param>
	/// <param name="path">The path to create.</param>
	/// <returns>The parent at that path.</returns>
	/// <exception cref="InvalidOperationException">Found a conflicting child node.</exception>
	public static IParentNode Create(this IParentNode parent, IList<string> path) {
		if (path.Count <= 0) {
			return parent; 
		}
		if (parent.Children.TryGetValue(path[0], out IChildNode? child)) {
			if (child is IParentNode childParent) {
				path.RemoveAt(0);
				return Create(childParent, path);
			} else {
				throw new InvalidOperationException("Found a conflicting child node at "+child.ID);
			}
		} else {
			Container newParent = new(path[0]);
			parent.AddChild(newParent);
			path.RemoveAt(0);
			return Create(newParent, path);
		}
	}
	/// <summary>
	/// Keeps creating containers until the path is reached (bottom to top, efficient, worse checks).
	/// </summary>
	/// <param name="parent">The parent of that path.</param>
	/// <param name="path">The path to create.</param>
	/// <returns>The container created at that path.</returns>
	public static Container CreateReverse(this IParentNode parent, IList<string> path) {
		Container container = new(path[^1]);
		if (path.Count <= 1) {
			container.SetParent(parent);
		} else {
			path.RemoveAt(path.Count-1);
			container.SetParent(CreateReverse(parent, path));
		}
		return container;
	}
}