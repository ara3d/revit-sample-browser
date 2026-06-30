// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Ara3D.RevitSampleBrowser.DuplicateViews.CS.Properties;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Size = System.Drawing.Size;

namespace Ara3D.RevitSampleBrowser.DuplicateViews.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        private static readonly string AddAssemblyPath = typeof(Application).Assembly.Location;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            CreateCopyPastePanel(application);

            return Result.Succeeded;
        }

        private void CreateCopyPastePanel(UIControlledApplication application)
        {
            var rp = application.CreateRibbonPanel("CopyPaste");

            var pbd2 = new PushButtonData("DuplicateAll", "Duplicate across documents",
                AddAssemblyPath,
                typeof(DuplicateAcrossDocumentsCommand).FullName)
            {
                LongDescription = "Duplicate all duplicatable drafting views and schedules."
            };

            var duplicateAllPb = rp.AddItem(pbd2) as PushButton;
            SetIconsForPushButton(duplicateAllPb, Resources.ViewCopyAcrossFiles);
        }

        private static void SetIconsForPushButton(RibbonButton button, Icon icon)
        {
            button.LargeImage = GetStdIcon(icon);
            button.Image = GetSmallIcon(icon);
        }

        private static BitmapSource GetStdIcon(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private static BitmapSource GetSmallIcon(Icon icon)
        {
            var smallIcon = new Icon(icon, new Size(16, 16));
            return Imaging.CreateBitmapSourceFromHIcon(
                smallIcon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
