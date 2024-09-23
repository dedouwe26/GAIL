namespace GAIL.Core.File
{
    /// <summary>
    /// An interface for an Encoder.
    /// </summary>
    /// <typeparam name="T">The input type to encode.</typeparam>
    public interface IEncoder<T> : IDisposable {
        /// <summary>
        /// Writes to a file.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="obj">The object to encode</param>
        public void Write(Stream stream, T obj);
    }
}