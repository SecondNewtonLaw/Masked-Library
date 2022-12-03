namespace Masked.Sys.Dangerous.Unsafe;
public partial class MemoryUnsafe
{
    public struct TypedPointer<T>
    {
        private readonly IntPtr _pointer;
        public TypedPointer(IntPtr pointer)
        {
            _pointer = pointer;
        }
        public IntPtr GetIntPointer()
        {
            return _pointer;
        }
        public unsafe void* GetVoidPointer()
        {
            return _pointer.ToPointer();
        }
    }
}