using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BuildingCoder
{
    /// <summary>
    ///     Wrapper class for converting IntPtr to IWin32Window.
    /// </summary>
    public class WindowHandle : IWin32Window
    {
        public WindowHandle(IntPtr h)
        {
            Debug.Assert(IntPtr.Zero != h,
                "expected non-null window handle");

            Handle = h;
        }

        public IntPtr Handle { get; }
    }

    /// <summary>Utilities extracted from TBC_WindowHandle sample.</summary>
    internal static partial class Util
    {
        internal static WindowHandle GetRevitWindowHandle()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            return new WindowHandle(process.MainWindowHandle);
        }
    }
}
