using System.Diagnostics;
using System.Runtime.Versioning;
using Masked.Sys.Interop.Windows;

namespace Masked.Sys.Windows;

/// <summary>
/// An easy to use class that allows you to interact with the Win32 API in an easy manner.
/// </summary>
[SupportedOSPlatform("Windows")]
public sealed partial class WinAPI {
    /// <summary>
    /// Provides functionality to change the current state a window.
    /// </summary>
    /// <param name="windowHandle">The handle to the window you want to change the state of, you can get this by using the method Masked.Sys.Interop.Windows.GetWindowHandle()</param>
    /// <param name="windowState">The enumeration of WindowState declaring the state of the Window</param>
    /// <returns>True if the operation was a success, false if it was not, and an exception if an error occured during the P/Invoke operation.</returns>
    [SupportedOSPlatform("Windows")]
    public static bool SetWindowState(nint windowHandle, WindowState windowState) {
        var result = WindowManipulation.ChangeWindowState(windowHandle, windowState);

        var hResultEx =
            System.Runtime.InteropServices.Marshal.GetExceptionForHR(System.Runtime.InteropServices.Marshal
                .GetLastPInvokeError());

        return hResultEx is not null ? throw hResultEx : result;
    }

    /// <summary>
    /// Get the window handle of the current-running process
    /// </summary>
    /// <returns></returns>
    public static nint GetWindowHandle() {
        return Process.GetCurrentProcess().MainWindowHandle;
    }
}