// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using RevitColor = Autodesk.Revit.DB.Color;
using Size = System.Drawing.Size;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class BitmapHelper
    {
        public static bool ColorsEqual(RevitColor color1, RevitColor color2) =>
            color1.Red == color2.Red && color1.Green == color2.Green && color1.Blue == color2.Blue;

        public static RevitColor GenerateRandomColor(int seed)
        {
            var r = new Random(seed);
            return new RevitColor(
                (byte)r.Next(0, 256),
                (byte)r.Next(0, 256),
                (byte)r.Next(0, 256));
        }

        public static int LimitValue(int value) => value < 0 ? 0 : value > 255 ? 255 : value;

        public static BitmapSource GetStdIcon(Icon icon) =>
                    Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        public static BitmapSource GetSmallIcon(Icon icon)
                {
                    var smallIcon = new Icon(icon, new Size(16, 16));
                    return Imaging.CreateBitmapSourceFromHIcon(smallIcon.Handle, Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }

        public static BitmapSource ConvertFromBitmap(Bitmap bitmap) =>
                    Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

        public static ImageSource GetBitmapAsImageSource(Bitmap bitmap) =>
                    Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

    }
}
