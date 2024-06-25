
using System.Collections.ObjectModel;

namespace GAIL.Storage;

/// <summary>
/// Represents a node that has children.
/// </summary>
public interface IParentNode {
    /// <summary>
    /// The children of this node.
    /// </summary>
    public ReadOnlyDictionary<string, IChildNode> Children { get; }

    /// <summary>
    /// Gets the node of the corresponding ID. This is relative to this node.
    /// </summary>
    /// <param name="ID">The ID of the node to return (can contain dots).</param>
    /// <returns>The node if that node exists.</returns>
    public IChildNode? Get(string ID);
    /// <summary>
    /// Gets the node of the corresponding keys. This is relative to this node.
    /// </summary>
    /// <param name="keys">The list of keys for what path to take.</param>
    /// <returns>The node if that node exists.</returns>
    public IChildNode? Get(List<string> keys);

    /// <summary>
    /// Gets the node of the corresponding ID. This is relative to this node.
    /// </summary>
    /// <typeparam name="T">The type of the child node.</typeparam>
    /// <param name="ID">The ID of the node to return (can contain dots).</param>
    /// <returns>The node if that node exists.</returns>
    public T? Get<T>(string ID) where T : IChildNode;

    /// <summary>
    /// Gets the node of the corresponding ID. This is relative to this node.
    /// </summary>
    /// <typeparam name="T">The type of the child node.</typeparam>
    /// <param name="keys">The list of keys for what path to take.</param>
    /// <returns>The node if that node exists.</returns>
    public T? Get<T>(List<string> keys) where T : IChildNode;

    /// <summary>
    /// Adds a child to this node.
    /// </summary>
    /// <param name="node">The node to add as a child.</param>
    public void AddChild(IChildNode node);
    /// <summary>
    /// Removes a child from this node.
    /// </summary>
    /// <param name="child">The child node to remove.</param>
    /// <returns>True it was successful. Else if it didn't find the node in the list, it is false.</returns>
    public bool RemoveChild(IChildNode child);
}

/// <summary>
/// Represents a node that can have a parent.
/// </summary>
public interface IChildNode {
    /// <summary>
    /// The key of this node (cannot contain a dot).
    /// </summary>
    public string Key { get; }
    /// <summary>
    /// The parent of this node, null if it has no parent.
    /// </summary>
    public IParentNode? Parent { get; }

    /// <summary>
    /// Creates the total ID from the root (root.childnode.leafnode).
    /// </summary>
    /// <returns>The ID from the root.</returns>
    public string GetID();

    /// <summary>
    /// Removes the parent.
    /// </summary>
    public void ClearParent();
    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="parent">The new parent.</param>
    public void SetParent(IParentNode parent);
}

/// <summary>
/// This is a node of a node tree structure.
/// </summary>
public class Node : IParentNode, IChildNode {
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
    public virtual ReadOnlyDictionary<string, IChildNode> Children => children.AsReadOnly();
    private readonly Dictionary<string, IChildNode> children = [];

    /// <inheritdoc/>
    public string Key { get; private set; }

    /// <inheritdoc/>
    public IParentNode? Parent { get; private set; }

    /// <inheritdoc/>
    public virtual string GetID() {
        if (Parent == null) {
            return Key;
        }
        if (Parent is IChildNode parentChild) {
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
    public virtual void SetParent(IParentNode parent) {
        if (Parent!=null) {
            ClearParent();
        }
        Parent = parent;
        if (!parent.Children.ContainsKey(Key)) {
            parent.AddChild(this);
        }
    }

    /// <inheritdoc/>
    public virtual void AddChild(IChildNode node) {
        if (children.ContainsKey(node.Key)) { return; }
        if (node.Parent != null) { return; }
        children.Add(node.Key, node);
        if (node.Parent == this) {
            return;
        }
        node.SetParent(this);
    }

    /// <inheritdoc/>
    public virtual bool RemoveChild(IChildNode child) {
        if (!children.ContainsKey(child.Key)) { return false; }
        children.Remove(child.Key);
        if (child.Parent == this) {
            child.ClearParent();
        }
        return true;
    }

    /// <inheritdoc/>
    public virtual T? Get<T>(string ID) where T : IChildNode {
        return (T?)Get(ID);
    }
    /// <inheritdoc/>
    public virtual T? Get<T>(List<string> keys) where T : IChildNode {
        return (T?)Get(keys);
    }

    /// <inheritdoc/>
    public virtual IChildNode? Get(string ID) {
        return Get([.. ID.Split('.')]);
    }

    /// <inheritdoc/>
    public virtual IChildNode? Get(List<string> keys) {
        if (children.Count < 1) {
            return null;
        }
        IChildNode? child = children!.GetValueOrDefault(keys[0]);
        if (child == null) {
            return null;
        }
        keys.RemoveAt(0);
        if (keys.Count > 0) {
            return child;
        }
        if (child is IParentNode childParent) {
            return childParent.Get(keys);
        }
        return null;
    }
}

/// <summary>
/// This is a child node of a node tree structure.
/// </summary>
public class ChildNode : IChildNode {
    /// <summary>
    /// Creates a node with a key, without a parent.
    /// </summary>
    /// <param name="key">The key of this node.</param>
    /// <exception cref="InvalidOperationException">Key cannot contain a dot.</exception>
    public ChildNode(string key)  {
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
    public ChildNode(string key, IParentNode parent) : this(key) {
        SetParent(parent);
    }

    /// <inheritdoc/>
    public string Key { get; private set; }

    /// <inheritdoc/>
    public IParentNode? Parent { get; private set; }

    /// <inheritdoc/>
    public virtual string GetID() {
        if (Parent == null) {
            return Key;
        }
        if (Parent is IChildNode parentChild) {
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
    public virtual void SetParent(IParentNode parent) {
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
/// This is a node of a node tree structure.
/// </summary>
public class ParentNode : IParentNode {
    /// <inheritdoc/>
    public virtual ReadOnlyDictionary<string, IChildNode> Children => children.AsReadOnly();
    private readonly Dictionary<string, IChildNode> children = [];

    /// <inheritdoc/>
    public virtual void AddChild(IChildNode node) {
        if (children.ContainsKey(node.Key)) { return; }
        if (node.Parent != null) { return; }
        children.Add(node.Key, node);
        if (node.Parent == this) {
            return;
        }
        node.SetParent(this);
    }

    /// <inheritdoc/>
    public virtual bool RemoveChild(IChildNode child) {
        if (!children.ContainsKey(child.Key)) { return false; }
        children.Remove(child.Key);
        if (child.Parent == this) {
            child.ClearParent();
        }
        return true;
    }
    /// <inheritdoc/>
    public virtual T? Get<T>(string ID) where T : IChildNode {
        return (T?)Get(ID);
    }
    /// <inheritdoc/>
    public virtual T? Get<T>(List<string> keys) where T : IChildNode {
        return (T?)Get(keys);
    }

    /// <inheritdoc/>
    public virtual IChildNode? Get(string ID) {
        return Get([.. ID.Split('.')]);
    }

    /// <inheritdoc/>
    public virtual IChildNode? Get(List<string> keys) {
        if (children.Count < 1) {
            return null;
        }
        IChildNode? child = children!.GetValueOrDefault(keys[0]);
        if (child == null) {
            return null;
        }
        keys.RemoveAt(0);
        if (keys.Count > 0) {
            return child;
        }
        if (child is IParentNode childParent) {
            return childParent.Get(keys);
        }
        return null;
    }
}