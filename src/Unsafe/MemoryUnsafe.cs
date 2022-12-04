using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Masked.Sys.Extensions;
using SysUnsafe = System.Runtime.CompilerServices.Unsafe;

namespace Masked.Sys.Dangerous.Unsafe;
/// <summary>
/// This class is not meant for real consumption, just to play with C#'s Unsafe functionality, due to this, it's <b>Undocumented</b>.
/// </summary>
public partial class MemoryUnsafe
{
    internal static class MemoryKeeper
    {
        internal static volatile ConcurrentDictionary<int, GCHandle> pointer_keeper = new();
    }
    public static void PinManagedObject<T>(T obj) where T : notnull
    {
        if (!MemoryKeeper.pointer_keeper.ContainsKey(obj!.GetHashCode()))
            throw new AlreadyPinnedException("That object has been ALREADY pinned in memory!");

        GCHandle handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
        int key = obj!.GetHashCode();
        MemoryKeeper.pointer_keeper.AddOrUpdate(key, keyI => keyI == key ? handle : GCHandle.FromIntPtr(IntPtr.Zero), (_, val) => val);
    }
    public static void UnpinManagedObject<T>(T obj) where T : notnull
    {
        int target = obj!.GetHashCode();

        if (!MemoryKeeper.pointer_keeper.Remove(target, out GCHandle handle))
            throw new HandleNotFound("Could not find the Handle that corresponds to the given object.");

        if (!handle.IsAllocated)
            return;
        handle.Free();
    }
    /// <summary>
    /// Converts a Managed Pointer into an Unsafe pointer (Uses the System.Runtime.CompilerServices.Unsafe.AsPointer{T}(ref {T} value) to work)
    /// </summary>
    /// <typeparam name="T">Anything that is not null.</typeparam>
    /// <param name="obj">The object in question.</param>
    /// <returns>A unspecified type and unmanaged pointer.</returns>
    public unsafe static void* GetPointerToManagedObject<T>(ref T obj) where T : notnull
    {
        return SysUnsafe.AsPointer(ref obj); // Magic.
    }
    /// <summary>
    /// Converts an Unsafe pointer into a Managed object (Uses the System.Runtime.CompilerServices.Unsafe.AsRef{T}(void* source) to work)
    /// </summary>
    /// <typeparam name="T">Anything that is not null.</typeparam>
    /// <param name="obj">The object in question.</param>
    /// <returns>A managed pointer.</returns>
    public unsafe static void GetManagedObjectFromPointer<T>(void* ptr, out T obj) where T : notnull
    {
        obj = SysUnsafe.AsRef<T>(ptr); // Magic.
    }
}