namespace GAIL.Core
{
    /// <summary>
    /// A class that wraps around a pointer.
    /// </summary>
    /// <typeparam name="T">The type to what it points.</typeparam>
    /// <param name="pointer">The pointer to make it from.</param>
    public unsafe class Pointer<T>(T* pointer) where T : unmanaged {
        private T* pointer = pointer;
        /// <summary>
        /// Gets the value of the pointer.
        /// </summary>
        /// <returns>The value of the pointer.</returns>
        public T GetValue() {
            return *pointer;
        }
        /// <summary>
        /// Gets the pointer itself.
        /// </summary>
        /// <returns>The pointer of this <see cref="Pointer<T>">.</returns>
        public T* GetPointer() {
            return pointer;
        }
        /// <summary>
        /// Sets a new value to the pointer.
        /// </summary>
        /// <param name="value">The new value</param>
        public void SetValue(T value) {
            pointer = &value;
        }
        /// <summary>
        /// Checks if pointer is null.
        /// </summary>
        public bool IsNull{get {
            return pointer == null;
        }}
        /// <summary>
        /// Creates a new Pointer with the type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the new pointer.</typeparam>
        /// <param name="value">The value of the new pointer.</param>
        /// <returns>The new pointer.</returns>
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
}