namespace GAIL.Core
{
    /// <summary>
    /// This is an exception that is thrown when the file format is wrong.
    /// </summary>
    /// <param name="path">The path of that file.</param>
    /// <param name="message">The thing that went wrong.</param>
    [Serializable]
    public class FileFormatException(string path, string message) : Exception($"GAIL: Wrong fileformat: {path}: {message}") { }
}