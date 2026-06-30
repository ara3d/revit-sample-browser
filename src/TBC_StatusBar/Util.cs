#region Namespaces

using System;
using System.Runtime.InteropServices;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        [DllImport("user32.dll",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        private static extern int SetWindowText(
            IntPtr hWnd,
            string lpString);

        [DllImport("user32.dll",
            SetLastError = true)]
        private static extern IntPtr FindWindowEx(
            IntPtr hwndParent,
            IntPtr hwndChildAfter,
            string lpszClass,
            string lpszWindow);

        public static void SetStatusText(
            IntPtr mainWindow,
            string text)
        {
            var statusBar = FindWindowEx(
                mainWindow, IntPtr.Zero,
                "msctls_statusbar32", "");

            if (statusBar != IntPtr.Zero) SetWindowText(statusBar, text);
        }
    }
}
