using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace BuildingCoder
{
    internal static partial class Util
    {
        [DllImport("user32.dll", SetLastError = true,
            CharSet = CharSet.Auto)]
        private static extern int GetWindowText(
            IntPtr hWnd,
            StringBuilder lpString,
            int nMaxCount);

        [DllImport("user32.dll", SetLastError = true,
            CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(
            IntPtr hWnd);

        internal static StringBuilder GetStatusTextMadmed(
            IntPtr mainWindow)
        {
            StringBuilder s = new();
            if (mainWindow != IntPtr.Zero)
            {
                var length = GetWindowTextLength(mainWindow);
                StringBuilder sb = new(length + 1);
                GetWindowText(mainWindow, sb, sb.Capacity);
                sb.Replace("Autodesk Revit Architecture 2013 - ", "");
                return sb;
            }

            return s;
        }

        internal static string GetWindowTextUsingWinApi(IntPtr mainWindow)
        {
            if (IntPtr.Zero == mainWindow)
                throw new ArgumentException(
                    "Expected valid window handle.");
            var len = GetWindowTextLength(mainWindow);
            StringBuilder sb = new(len + 1);
            GetWindowText(mainWindow, sb, sb.Capacity);
            return sb.ToString();
        }

        internal static string GetWindowTextUsingNet()
        {
            return Process.GetCurrentProcess().MainWindowTitle;
        }
    }
}
