#region Namespaces

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewTextNote sample.</summary>
    internal static partial class Util
    {
        internal static void SetTextAlignment(TextNote textNote)
        {
            var doc = textNote.Document;

            using var t = new Transaction(doc);
            t.Start("AlignTextNote");

            var p = textNote.get_Parameter(
                BuiltInParameter.TEXT_ALIGN_VERT);

            p.Set((int)
                TextAlignFlags.TEF_ALIGN_MIDDLE);

            t.Commit();
        }

        /// <summary>
        ///     Return the first text note type matching the given name.
        /// </summary>
        internal static TextNoteType GetTextNoteTypeByName(
            Document doc,
            string name)
        {
            return new FilteredElementCollector(doc)
                    .OfClass(typeof(TextNoteType))
                    .First(q => q.Name.Equals(name))
                as TextNoteType;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd);

        /// <summary>
        ///     Determine the current display
        ///     horizontal dots per inch.
        /// </summary>
        internal static float DpiX
        {
            get
            {
                float xDpi, yDpi;

                var dc = GetDC(IntPtr.Zero);

                using (var g = Graphics.FromHdc(dc))
                {
                    xDpi = g.DpiX;
                    yDpi = g.DpiY;
                }

                if (ReleaseDC(IntPtr.Zero) != 0)
                {
                    // GetLastError and handle...
                }

                return xDpi;
            }
        }

        internal static float GetDpiX()
        {
            float xDpi, yDpi;

            using var g = Graphics.FromHwnd(IntPtr.Zero);
            xDpi = g.DpiX;
            yDpi = g.DpiY;

            return xDpi;
        }

        internal static double GetStringWidth(string text, Font font)
        {
            var textWidth = 0.0;

            using var g = Graphics.FromHwnd(IntPtr.Zero);
            textWidth = g.MeasureString(text, font).Width;

            return textWidth;
        }
    }
}
