namespace GAIL.Core.File
{
    /// <summary>
    /// An interface for a Decoder.
    /// </summary>
    /// <typeparam name="T">The output type from decoding.</typeparam>
    public interface IDecoder<T> : IDisposable {
        /// <summary>
        /// Parses a file.
        /// </summary>
        /// <param name="stream">The stream to parse.</param>
        /// <returns>The parsed object.</returns>
        public T Parse(Stream stream);
    }
}