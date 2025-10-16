using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using LambdaKit.Logging;

namespace GAIL.Storage;

/// <summary>
/// Represents a common layout for a storage file.
/// </summary>
public abstract class BaseStorage : ParentNode, ISerializable {
    /// <summary>
    /// The ID of the storage logger.
    /// </summary>
    public const string LoggerID = "GAIL.Storage.BaseStorage";
    private static Logger? logger;
    /// <summary>
    /// The logger for the storage systems.
    /// </summary>
    public static Logger Logger { get {
        if (logger == null) {
            try {
                logger = new Logger("Storage", LoggerID);
            } catch (ArgumentException) {
                logger = Loggers.Get(LoggerID) ?? new Logger("Storage");
            }
        }
        return logger;
    } }
    /// <summary>
    /// This is the formatter used for this storage file.
    /// </summary>
    public IFormatter? Formatter { get; set; }

    /// <summary>
    /// Creates a new storage.
    /// </summary>
    /// <param name="formatter">The formatter to use for encoding and decoding .</param>
    public BaseStorage(IFormatter? formatter = null) { Formatter = formatter; }
    /// <summary>
    /// Loads the storage from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="shouldCloseStream">Whether it should close the stream afterwards.</param>
    /// <returns>True if it succeeded.</returns>
    public virtual bool Load(Stream stream, bool shouldCloseStream = true) {
        try {
            using Serializer parser = new(stream, shouldCloseStream);
            Parse(parser, Formatter);
        } catch (Exception e) {
            Logger.LogError("Failed to load storage file:");
            Logger.LogException(e);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Loads the storage from a file.
    /// </summary>
    /// <param name="filePath">The path of the file.</param>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="PathTooLongException"/>
    /// <returns>True if it succeeded.</returns>
    public virtual bool Load(string filePath) {
        using FileStream fs = new(Path.GetFullPath(filePath), FileMode.Open, FileAccess.Read);
        return Load(fs);
    }

    /// <summary>
    /// Saves the storage to a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="shouldCloseStream">Whether it should close the stream afterwards.</param>
    /// <returns>True if it succeeded.</returns>
    public virtual bool Save(Stream stream, bool shouldCloseStream = true) {
        try {
            using Serializer serializer = new(stream, shouldCloseStream);
            Serialize(serializer, Formatter);
        } catch (Exception e) {
            Logger.LogError("Failed to save storage file:");
            Logger.LogException(e);
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Loads the storage from a file.
    /// </summary>
    /// <param name="filePath">The path of the file (does not need to exist).</param>
    /// <returns>True if it succeeded.</returns>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="PathTooLongException"/>
    /// <exception cref="UnauthorizedAccessException"/>
    /// <exception cref="IOException"/>
    public virtual bool Save(string filePath) {
        using FileStream fs = new(Path.GetFullPath(filePath), FileMode.Create, FileAccess.Write);
        return Save(fs);
    }

	/// <inheritdoc/>
	public abstract void Serialize(Serializer serializer, IFormatter? formatter = null);

	/// <inheritdoc/>
	public abstract void Parse(Parser parser, IFormatter? formatter = null);
}