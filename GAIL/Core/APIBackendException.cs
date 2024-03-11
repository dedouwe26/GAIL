namespace GAIL.Core
{
    /// <summary>
    /// This is an exception that is thrown when the one of the API backends throws an error.
    /// </summary>
    /// <param name="api">In which backend the error was thrown.</param>
    /// <param name="message">What went wrong.</param>
    [Serializable]
    public class APIBackendException(string api, string message) : Exception($"GAIL: {api}: {message}") { }
}