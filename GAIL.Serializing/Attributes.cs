namespace GAIL.Serializing;

/// <summary>
/// An attribute to mark the info of a serializable or reducer. Must be on a field or property, static or not, public or not.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class SerializingInfoAttribute : Attribute { }