// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using RApplication = Autodesk.Revit.ApplicationServices.Application;


namespace Revit.SDK.Samples.UIAPI.CS
{
    /// <summary>
    ///     Interaction logic for UserControl3.xaml
    /// </summary>
    public partial class UserControl3 : UserControl
    {
        private readonly string _name;

        public UserControl3(string name)
        {
            InitializeComponent();

            _name = name;

            image1.Source = getBitmapAsImageSource(Properties.Resources.autodesk);
        }

        public static ImageSource getBitmapAsImageSource(Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public void OnOK()
        {
            TaskDialog.Show("OK", _name);
        }

        public void OnCancel()
        {
            TaskDialog.Show("OnCancel", _name);
        }

        public void OnRestoreDefaults()
        {
            TaskDialog.Show("OnRestoreDefaults", _name);
        }
    }
}
