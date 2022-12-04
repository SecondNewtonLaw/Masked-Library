namespace Masked.Sys.Dangerous.Unsafe;
public partial class MemoryUnsafe
{
    /// <summary>
    /// A Pointer that holds the Type of the object it points to
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct TypedPointer<T>
    {
        /// <summary>
        /// The real pointer.
        /// </summary>
        private readonly IntPtr _pointer;
        /// <summary>
        /// Build a struct of type <see cref="TypedPointer{T}"/>
        /// </summary>
        /// <param name="pointer">The <see cref="IntPtr"/> that should be wrapped.</param>
        public TypedPointer(IntPtr pointer)
        {
            _pointer = pointer;
        }
        /// <summary>
        /// Obtains the <see cref="IntPtr"/> that points towards to the underlying value.
        /// </summary>
        /// <returns>An <see cref="IntPtr"/> which represents the address of the object this pointed in memory</returns>
        public IntPtr GetIntPointer()
        {
            return _pointer;
        }
        /// <summary>
        /// Obtains the <see cref="System.Void"/>* that points towards the underlying value.
        /// </summary>
        /// <returns>A <see cref="System.Void"/>* that points to the address of the object this pointed in memory.</returns>
        public unsafe void* GetVoidPointer()
        {
            return _pointer.ToPointer();
        }
    }
}