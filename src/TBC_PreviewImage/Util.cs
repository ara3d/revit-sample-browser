#region Namespaces

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PreviewImage sample.</summary>
    internal static partial class Util
    {
        public static BitmapSource ConvertBitmapToBitmapSource(
            Bitmap bmp)
        {
            return Imaging
                .CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
