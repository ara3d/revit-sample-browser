using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using RApplication = Autodesk.Revit.ApplicationServices.Application;
using System.Windows.Interop;


namespace Revit.SDK.Samples.UIAPI.CS
{
   /// <summary>
   /// Interaction logic for UserControl3.xaml
   /// </summary>
   public partial class UserControl3 : UserControl
   {
      public UserControl3(string name)
      {
         InitializeComponent();

         _name = name;

         image1.Source = getBitmapAsImageSource(Properties.Resources.autodesk);
      }

      public static ImageSource getBitmapAsImageSource(System.Drawing.Bitmap bitmap)
      {
         var hBitmap = bitmap.GetHbitmap();
         return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
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

      string _name;
   }
}
