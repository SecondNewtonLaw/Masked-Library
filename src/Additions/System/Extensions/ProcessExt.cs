using System.Diagnostics;

namespace Masked.Sys.Extensions;

/// <summary>
/// Extensions for the <see cref="System.Diagnostics.Process"/> class.
/// </summary>
public static class ProcessExt
{
    /// <summary>
    /// Obtains the processes Currently Loaded Unmanaged Libraries.
    /// </summary>
    /// <param name="proc">Process Instance</param>
    /// <returns>An IEnumerable of Type <see cref="System.Diagnostics.ProcessModule"/></returns>
    /// <remarks>This process is <b>Deferred</b>. If you want to use the normal, <b>non deferred</b> method, use <b><see cref="GetLoadedUnmanagedLibraries(Process)"/></b>.</remarks>
    public static IEnumerable<ProcessModule> GetLoadedUnmanagedLibrariesDef(this Process proc)
    {
        var enumerator = proc.Modules.GetEnumerator();
        while (enumerator.MoveNext())
        {
            // Execution is deferred, yield return.
            yield return (enumerator.Current as ProcessModule)!;
        }
    }

    /// <summary>
    /// Obtains the processes Currently Loaded Unmanaged Libraries.
    /// </summary>
    /// <param name="proc">Process Instance</param>
    /// <returns>An IEnumerable of Type <see cref="System.Diagnostics.ProcessModule"/></returns>
    /// <remarks>This process is <b>Not Deferred</b>. If you want to use the <b>Deferred</b> method, use <b><see cref="GetLoadedUnmanagedLibrariesDef(Process)"/></b>.</remarks>
    public static IEnumerable<ProcessModule> GetLoadedUnmanagedLibraries(this Process proc)
    {
        ProcessModule[] modules = new ProcessModule[proc.Modules.Count];
        proc.Modules.CopyTo(modules, 0); // Copy to new container
        return modules;
    }
}