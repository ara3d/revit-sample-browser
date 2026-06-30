using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BuildingCoder
{
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
    internal static partial class Util
    {
        internal static WindowHandle GetRevitWindowHandle()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            return new WindowHandle(process.MainWindowHandle);
        }
    }
}
