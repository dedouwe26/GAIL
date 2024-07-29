
using System.Collections.ObjectModel;
using GAIL.Storage.Members;
using GAIL.Storage.Parser;

namespace GAIL.Storage;

/// <summary>
/// Represents a node that has children.
/// </summary>
public interface IParentNode {
    /// <summary>
    /// The children of this node.
    /// </summary>
    public ReadOnlyDictionary<string, IMember> Children { get; }

    /// <summary>
    /// Gets the node of the corresponding ID. This is relative to this node.
    /// </summary>
    /// <param name="ID">The ID of the node to return (can contain dots).</param>
    /// <returns>The node if that node exists.</returns>
    public IMember? Get(string ID);
    /// <summary>
    /// Gets the node of the corresponding keys. This is relative to this node.
    /// </summary>
    /// <param name="keys">The list of keys for what path to take.</param>
    /// <returns>The node if that node exists.</returns>
    public IMember? Get(List<string> keys);

    /// <summary>
    /// Gets the node of the corresponding ID. This is relative to this node.
    /// </summary>
    /// <typeparam name="T">The type of the child node.</typeparam>
    /// <param name="ID">The ID of the node to return (can contain dots).</param>
    /// <returns>The node if that node exists.</returns>
    public T? Get<T>(string ID) where T : IMember;

    /// <summary>
    /// Gets the node of the corresponding ID. This is relative to this node.
    /// </summary>
    /// <typeparam name="T">The type of the child node.</typeparam>
    /// <param name="keys">The list of keys for what path to take.</param>
    /// <returns>The node if that node exists.</returns>
    public T? Get<T>(List<string> keys) where T : IMember;

    /// <summary>
    /// Adds a child to this node.
    /// </summary>
    /// <param name="member">The node to add as a child.</param>
    public void AddChild(IMember member);
    /// <summary>
    /// Removes a child from this node.
    /// </summary>
    /// <param name="child">The child node to remove.</param>
    /// <returns>True it was successful. Else if it didn't find the node in the list, it is false.</returns>
    public bool RemoveChild(IMember child);
}

/// <summary>
/// This can contain members and is a child of a member.
/// </summary>
public abstract class Node : IParentNode, IMember {
    /// <summary>
    /// Creates a node with a key, without a parent.
    /// </summary>
    /// <param name="key">The key of this node.</param>
    /// <exception cref="InvalidOperationException">Key cannot contain a dot.</exception>
    public Node(string key)  {
        if (key.Contains('.')) {
            throw new InvalidOperationException("Invalid key, cannot contain dots");
        }
        Key = key;
    }
    /// <summary>
    /// Create a node with a key, value and a parent.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public Node(string key, IParentNode parent) : this(key) {
        SetParent(parent);
    }

    /// <inheritdoc/>
    public virtual ReadOnlyDictionary<string, IMember> Children => children.AsReadOnly();
    /// <summary>
    /// The children of this node.
    /// </summary>
    protected Dictionary<string, IMember> children = [];

    /// <inheritdoc/>
    public string Key { get; private set; }

    /// <inheritdoc/>
    public IParentNode? Parent { get; private set; }
    
    /// <inheritdoc/>
    public abstract MemberType Type { get; }

    /// <inheritdoc/>
    public virtual string GetID() {
        if (Parent == null) {
            return Key;
        }
        if (Parent is IMember parentChild) {
            string? parentID = parentChild.GetID();
            return parentID+'.'+Key;
        }
        return Key;
    }

    /// <inheritdoc/>
    public virtual void ClearParent() {
        IParentNode? previousParent = Parent;
        Parent = null;
        if (previousParent?.Children.ContainsKey(Key) ?? false) {
            previousParent.RemoveChild(this);
        }
        
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="parent"/> is not a valid member.</exception>
    public virtual void SetParent(IParentNode parent) {
        if (parent is IMember member) {
            if (!StorageRegister.IsMemberRegistered(member)) {
                throw new ArgumentException("Parent is not a valid member", nameof(parent));
            }
        } else if (parent is not Storage) {
            throw new ArgumentException("Parent is not a valid member", nameof(parent));
        }
        
        if (Parent!=null) {
            ClearParent();
        }
        Parent = parent;
        if (!parent.Children.ContainsKey(Key)) {
            parent.AddChild(this);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="member"/> is not registered.</exception>
    public virtual void AddChild(IMember member) {
        if (!StorageRegister.IsMemberRegistered(member)) {
            throw new ArgumentException("Member is not registered", nameof(member));
        }
        if (children.ContainsKey(member.Key)) { return; }

        children.Add(member.Key, member);

        if (member.Parent != this) {
            member.SetParent(this);
        }
    }

    /// <inheritdoc/>
    public virtual bool RemoveChild(IMember child) {
        if (!children.ContainsKey(child.Key)) { return false; }
        children.Remove(child.Key);
        if (child.Parent == this) {
            child.ClearParent();
        }
        return true;
    }

    /// <inheritdoc/>
    public virtual T? Get<T>(string ID) where T : IMember {
        return (T?)Get(ID);
    }
    /// <inheritdoc/>
    public virtual T? Get<T>(List<string> keys) where T : IMember {
        return (T?)Get(keys);
    }

    /// <inheritdoc/>
    public virtual IMember? Get(string ID) {
        return Get([.. ID.Split('.')]);
    }

    /// <inheritdoc/>
    public virtual IMember? Get(List<string> keys) {
        if (children.Count < 1) {
            return null;
        }
        IMember? child = children!.GetValueOrDefault(keys[0]);
        if (child == null) {
            return null;
        }
        keys.RemoveAt(0);
        if (keys.Count <= 0) {
            return child;
        }
        if (child is IParentNode childParent) {
            return childParent.Get(keys);
        }
        return null;
    }
}

/// <summary>
/// This is a default implementation of <see cref="IMember"/>.
/// </summary>
public abstract class Member : IMember {
    /// <summary>
    /// Creates a node with a key, without a parent.
    /// </summary>
    /// <param name="key">The key of this node.</param>
    /// <exception cref="InvalidOperationException">Key cannot contain a dot.</exception>
    public Member(string key)  {
        if (key.Contains('.')) {
            throw new InvalidOperationException("Invalid key, cannot contain dots");
        }
        Key = key;
    }
    /// <summary>
    /// Create a node with a key, value and a parent.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public Member(string key, IParentNode parent) : this(key) {
        SetParent(parent);
    }

    /// <inheritdoc/>
    public string Key { get; private set; }

    /// <inheritdoc/>
    public IParentNode? Parent { get; private set; }
    
    /// <inheritdoc/>
    public abstract MemberType Type { get; }

    /// <inheritdoc/>
    public virtual string GetID() {
        if (Parent == null) {
            return Key;
        }
        if (Parent is IMember parentChild) {
            string? parentID = parentChild.GetID();
            return parentID+'.'+Key;
        }
        return Key;
    }

    /// <inheritdoc/>
    public virtual void ClearParent() {
        IParentNode? previousParent = Parent;
        Parent = null;
        if (previousParent?.Children.ContainsKey(Key) ?? false) {
            previousParent.RemoveChild(this);
        }
        
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="parent"/> is not a valid member.</exception>
    public virtual void SetParent(IParentNode parent) {
        if (parent is IMember member) {
            if (!StorageRegister.IsMemberRegistered(member)) {
                throw new ArgumentException("Parent is not a valid member", nameof(parent));
            }
        } else if (parent is not Storage) {
            throw new ArgumentException("Parent is not a valid member", nameof(parent));
        }
    
        if (Parent!=null) {
            ClearParent();
        }
        Parent = parent;
        if (!parent.Children.ContainsKey(Key)) {
            parent.AddChild(this);
        }
    }
}

/// <summary>
/// This is a parent of a node tree structure.
/// </summary>
public class ParentNode : IParentNode {
    /// <inheritdoc/>
    public virtual ReadOnlyDictionary<string, IMember> Children => children.AsReadOnly();
    /// <summary>
    /// The children of this node.
    /// </summary>
    protected Dictionary<string, IMember> children = [];

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="member"/> is not registered.</exception>
    public virtual void AddChild(IMember member) {
        if (!StorageRegister.IsMemberRegistered(member)) {
            throw new ArgumentException("Member is not registered", nameof(member));
        }
        if (children.ContainsKey(member.Key)) { return; }
        
        children.Add(member.Key, member);

        if (member.Parent != this) {
            member.SetParent(this);
        }
    }

    /// <inheritdoc/>
    public virtual bool RemoveChild(IMember child) {
        if (!children.ContainsKey(child.Key)) { return false; }
        children.Remove(child.Key);
        if (child.Parent == this) {
            child.ClearParent();
        }
        return true;
    }
    /// <inheritdoc/>
    public virtual T? Get<T>(string ID) where T : IMember {
        return (T?)Get(ID);
    }
    /// <inheritdoc/>
    public virtual T? Get<T>(List<string> keys) where T : IMember {
        return (T?)Get(keys);
    }

    /// <inheritdoc/>
    public virtual IMember? Get(string ID) {
        return Get([.. ID.Split('.')]);
    }

    /// <inheritdoc/>
    public virtual IMember? Get(List<string> keys) {
        if (children.Count < 1) {
            return null;
        }
        IMember? child = children!.GetValueOrDefault(keys[0]);
        if (child == null) {
            return null;
        }
        keys.RemoveAt(0);
        if (keys.Count <= 0) {
            return child;
        }
        if (child is IParentNode childParent) {
            return childParent.Get(keys);
        }
        return null;
    }
}