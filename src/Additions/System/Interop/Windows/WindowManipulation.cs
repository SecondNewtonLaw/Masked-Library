using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Masked.Sys.Interop.Windows;

[SupportedOSPlatform("Windows")]
internal sealed partial class WindowManipulation
{
#if NET7_0
    [
        LibraryImport(libraryName: "user32.dll", EntryPoint = "ShowWindowA", SetLastError = true),
        // Declare the supported platform (Windows Dll, so Windows only).
        SupportedOSPlatform("Windows")
    ]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr windowHandle, int windowState);
#elif NET6_0_OR_GREATER
    [
        // Dynamic Link Library Import (Ext)
        DllImport(dllName: "user32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall),
        // Declare the supported platform (Windows Dll, so Windows only).
        SupportedOSPlatform("Windows")
    ]
    private static extern bool ShowWindow(IntPtr windowHandle, int windowState);
#endif
    [SupportedOSPlatform("Windows")]
    internal static bool ChangeWindowState(IntPtr windowHandle, WindowState windowState)
        => ShowWindow(windowHandle, (int)windowState);
}

/// <summary>
/// Enumeration used to set the WindowState of a window with ease
/// </summary>
public enum WindowState
{
    Hide = 0,
    Normal = 1,
    ShowMinimized = 2,
    ShowMaximized = 3,
    ShowNoActivate = 4,
    Show = 5,
    Minimize = 6,
    ShowMinNoActive = 7,
    ShowNA = 8,
    Restore = 9,
    ShowDefault = 10,
    ForceMinimize = 11,
}