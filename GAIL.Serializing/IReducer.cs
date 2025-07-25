using System.Diagnostics.Contracts;
using System.Reflection;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing;


/// <summary>
/// Represents a class that can be turned into serializables and can be created from serializables.
/// </summary>
public interface IReducer {
    /// <summary>
    /// Represents info for a reducer.
    /// </summary>
    /// <param name="Format">The format of the reducer.</param>
    /// <param name="Creator">The instance creator of the reducer.</param>
    public record Info (ISerializable.Info[] Format, Func<ISerializable[], IReducer> Creator);

    /// <summary>
    /// Tries to get the reducer info of a reducer.
    /// </summary>
    /// <param name="reducer">The reducer of which to get the info from.</param>
    /// <returns>The info, if it is found, of that reducer.</returns>
    public static Info? TryGetInfo(IReducer reducer) {
        try {
            PropertyInfo[] infos = reducer.GetType().GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (PropertyInfo info in infos) {
                if (info.GetCustomAttribute<SerializingInfoAttribute>() is not null) {
                    if (info.GetValue(reducer) is Info s) {
                        return s;
                    } else {
                        throw new InvalidOperationException($"IReducer property {info.Name} did not return a IReducer.Info.");
                    }
                }
            }
        } catch (Exception) {
            return null;
        }
        return null;
    }
    /// <summary>
    /// Tries to get the reducer info of a reducer.
    /// </summary>
    /// <typeparam name="T">The reducer type of which to get the info from.</typeparam>
    /// <returns>The info, if it is found, of that reducer.</returns>
    public static Info? TryGetInfo<T>() where T : IReducer, new() {
        return TryGetInfo(new T());
    }

    /// <summary>
    /// Creates a reducer info.
    /// </summary>
    /// <param name="creator">The creator that creates an empty instance.</param>
    /// <returns>A new reducer info.</returns>
    public static Info CreateInfo(Func<IReducer> creator) {
        return new(creator().Format, serializables => {
            IReducer inst = creator();
            inst.Parse(serializables);
            return inst;
        });
    }
    /// <summary>
    /// Creates a reducer info.
    /// </summary>
    /// <typeparam name="T">The type of the reducer.</typeparam>
    /// <returns>A new reducer info.</returns>
    public static Info CreateInfo<T>() where T : IReducer, new() {
        return CreateInfo(() => new T());
    }
    
    
    /// <summary>
    /// The serializables of which this reducer is made of.
    /// </summary>
    [Pure]
    public ISerializable.Info[] Format { get; }
    /// <summary>
    /// Serializes this reducer into serializables.
    /// </summary>
    /// <returns></returns>
    public ISerializable[] Serialize();
    /// <summary>
    /// Creates this reducer from serializables.
    /// </summary>
    /// <param name="serializables">The serializables to parse the data from.</param>
    public void Parse(ISerializable[] serializables);
}