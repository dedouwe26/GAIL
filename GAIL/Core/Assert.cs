using OxDED.Terminal.Logging;

namespace GAIL.Core;

/// <summary>
/// A simple asserting tool.
/// </summary>
public static class Assert {
    /// <summary>
    /// Tests if <paramref name="tested"/> is null.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="tested"/>.</typeparam>
    /// <param name="logger">The logger to log to when <paramref name="tested"/> is null.</param>
    /// <param name="tested">The object that is being tested for being null.</param>
    /// <param name="message">The message to log when <paramref name="tested"/> is null.</param>
    /// <returns>True if <paramref name="tested"/> is <b>not</b> null.</returns>
    public static T NotNull<T>(Logger logger, T? tested, string message) where T : notnull {
        if (tested is null) {
            logger.LogError(message);
            return false;
        }
        return true;
    }
}