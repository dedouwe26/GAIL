using System.Reflection;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing;


/// <summary>
/// Represents a class that can be turned into serializables and can be created from serializables using streams.
/// </summary>
public interface IStreamReducer {
    /// <summary>
    /// Represents info for a stream reducer.
    /// </summary>
    /// <param name="Creator">The instance creator of the reducer.</param>
    public record Info (Func<Parser, IStreamReducer> Creator);

    /// <summary>
    /// Tries to get the reducer info of a reducer.
    /// </summary>
    /// <param name="reducer">The reducer of which to get the info from.</param>
    /// <returns>The info, if it is found, of that reducer.</returns>
    public static Info? TryGetInfo(IStreamReducer reducer) {
        try {
            PropertyInfo[] infos = reducer.GetType().GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (PropertyInfo info in infos) {
                if (info.GetCustomAttribute<SerializingInfoAttribute>() is not null) {
                    if (info.GetValue(reducer) is Info s) {
                        return s;
                    } else {
                        throw new InvalidOperationException($"IStreamReducer property {info.Name} did not return a IStreamReducer.Info.");
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
    public static Info? TryGetInfo<T>() where T : IStreamReducer, new() {
        return TryGetInfo(new T());
    }

    /// <summary>
    /// Creates a reducer info.
    /// </summary>
    /// <param name="creator">The creator that creates an empty instance.</param>
    /// <returns>A new reducer info.</returns>
    public static Info CreateInfo(Func<IStreamReducer> creator) {
        return new(parser => {
            IStreamReducer inst = creator();
            inst.Parse(parser);
            return inst;
        });
    }
    /// <summary>
    /// Creates a reducer info.
    /// </summary>
    /// <typeparam name="T">The type of the reducer.</typeparam>
    /// <returns>A new reducer info.</returns>
    public static Info CreateInfo<T>() where T : IStreamReducer, new() {
        return CreateInfo(() => new T());
    }

    /// <summary>
    /// Serializes this reducer into that serializer.
    /// </summary>
    /// <param name="serializer">The serializer to write the data to.</param>
    public void Serialize(Serializer serializer);
    /// <summary>
    /// Creates this reducer from serializables.
    /// </summary>
    /// <param name="parser">The parser to read the data from.</param>
    public void Parse(Parser parser);
}