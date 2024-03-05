namespace GAIL
{
    public unsafe class Pointer<T>(T* pointer) where T : unmanaged {
        private T* pointer = pointer;

        public T GetValue() {
            return *pointer;
        }
        public T* GetPointer() {
            return pointer;
        }
        public void SetValue(T value) {
            pointer = &value;
        }
        public static Pointer<TValue> From<TValue>(TValue value) where TValue : unmanaged {
            return new Pointer<TValue>(&value);
        }
        public static implicit operator T(Pointer<T> p) {
            return p.GetValue();
        }
        public static implicit operator T*(Pointer<T> p) {
            return p.GetPointer();
        }
        public static implicit operator Pointer<T>(T* p) {
            return new Pointer<T>(p);
        }
    }
    /// <summary>
    /// This is an exception that is thrown when the one of the API backends throws an error.
    /// </summary>
    /// <param name="api">In which backend the error was thrown.</param>
    /// <param name="message">What went wrong.</param>
    [Serializable]
    public class APIBackendException(string api, string message) : Exception($"GAIL: {api}: {message}") { }
    /// <summary>
    /// This is an exception that is thrown when the file format is wrong.
    /// </summary>
    /// <param name="path">The path of that file.</param>
    /// <param name="message">The thing that went wrong.</param>
    [Serializable]
    public class FileFormatException(string path, string message) : Exception($"GAIL: Wrong fileformat: {path}: {message}") { }
}