using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace GAIL.Core
{
    /// <summary>
    /// A class that wraps around a pointer.
    /// </summary>
    /// <typeparam name="T">The type to what it points.</typeparam>
    public unsafe struct Pointer<T> where T : unmanaged {
        private T* pointer;

        /// <summary>
        /// Creates a pointer.
        /// </summary>
        /// <param name="pointer">The pointer to make it from.</param>
        public Pointer(T* pointer) {
            this.pointer = pointer;
        }

        /// <summary>
        /// Gets the value of the pointer.
        /// </summary>
        /// <returns>The value of the pointer.</returns>
        public readonly T GetValue() {
            return *pointer;
        }
        /// <summary>
        /// Gets the pointer itself.
        /// </summary>
        /// <returns>The actual pointer.</returns>
        public readonly T* GetPointer() {
            return pointer;
        }
        /// <summary>
        /// Sets a new value to the pointer.
        /// </summary>
        /// <param name="value">The new value</param>
        public void SetValue(ref T value) {
            fixed (T* ptr = &value) {
                pointer = ptr;
            }
        }
        /// <summary>
        /// Checks if pointer is null.
        /// </summary>
        public readonly bool IsNull {
            get {
                return pointer == null;
            }
        }
        /// <summary>
        /// Creates a new Pointer from a reference with the type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="reference">The reference of the new pointer.</param>
        /// <returns>The new pointer.</returns>
        public static Pointer<T> From(ref T reference) {
            fixed (T* ptr = &reference) {
                return new Pointer<T>(ptr);
            }
            
        }
        /// <summary>
        /// Creates a null pointer.
        /// </summary>
        /// <returns>An pointer to null.</returns>
        public static Pointer<T> FromNull() {
            return new Pointer<T>((T*)null);
        }
        ///
        public static implicit operator T(Pointer<T> p) {
            return p.GetValue();
        }
        ///
        public static implicit operator T*(Pointer<T> p) {
            return p.GetPointer();
        }
        ///
        public static implicit operator Pointer<T>(T* p) {
            return new Pointer<T>(p);
        }
    }
}