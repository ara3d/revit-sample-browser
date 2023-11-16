// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.UIAPI.CS.OptionsDialog
{
    /// <summary>
    ///     Interaction logic for UserControl3.xaml
    /// </summary>
    public partial class UserControl3 : UserControl
    {
        private readonly string m_name;

        public UserControl3(string name)
        {
            InitializeComponent();

            m_name = name;

            image1.Source = GetBitmapAsImageSource(Properties.Resources.autodesk);
        }

        public static ImageSource GetBitmapAsImageSource(Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public void OnOK()
        {
            TaskDialog.Show("OK", m_name);
        }

        public void OnCancel()
        {
            TaskDialog.Show("OnCancel", m_name);
        }

        public void OnRestoreDefaults()
        {
            TaskDialog.Show("OnRestoreDefaults", m_name);
        }
    }
}
