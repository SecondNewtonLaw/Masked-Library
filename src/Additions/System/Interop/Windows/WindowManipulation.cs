using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Masked.Sys.Interop.Windows;

[SupportedOSPlatform("Windows")]
internal sealed partial class WindowManipulation {
#if NET7_0 // Use NEW P/Invoke
    [LibraryImport("user32.dll", SetLastError = true)]
    [SupportedOSPlatform("Windows")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(nint windowHandle, int windowState);
#else // Use OLD P/Invoke
    [
        // Dynamic Link Library Import (Ext)
        DllImport(dllName: "user32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall),
        // Declare the supported platform (Windows Dll, so Windows only).
        SupportedOSPlatform("Windows")
    ]
    private static extern bool ShowWindow(IntPtr windowHandle, int windowState);
#endif
    [SupportedOSPlatform("Windows")]
    internal static bool ChangeWindowState(nint windowHandle, WindowState windowState) {
        return ShowWindow(windowHandle, (int)windowState);
    }
}

/// <summary>
/// Enumeration used to set the WindowState of a window with ease
/// </summary>
public enum WindowState {
    /// <summary>
    /// Indicates that the window will be hidden.
    /// </summary>
    Hide = 0,

    /// <summary>
    /// Indicates that the window will be shown as normal.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Indicates that the window will show minimized.
    /// </summary>
    ShowMinimized = 2,

    /// <summary>
    /// Indicates that the window will show maximized.
    /// </summary>
    ShowMaximized = 3,

    /// <summary>
    /// Check Microsoft Documentation.
    /// </summary>
    ShowNoActivate = 4,

    /// <summary>
    /// Indicates that the window will be shown.
    /// </summary>
    Show = 5,

    /// <summary>
    /// Indicates that the window will be minimized.
    /// </summary>
    Minimize = 6,

    /// <summary>
    /// Check Microsoft Documentation.
    /// </summary>
    ShowMinNoActive = 7,

    /// <summary>
    /// Check Microsoft Documentation.
    /// </summary>
    ShowNA = 8,

    /// <summary>
    /// Indicates that the window will be restored to the previous state.
    /// </summary>
    Restore = 9,

    /// <summary>
    /// Indicates that the window will be shown as it is set by default.
    /// </summary>
    ShowDefault = 10,

    /// <summary>
    /// Indicates that the window will be forcefully minimized.
    /// </summary>
    ForceMinimize = 11
}