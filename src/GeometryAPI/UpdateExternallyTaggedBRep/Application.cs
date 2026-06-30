// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Ara3D.RevitSampleBrowser.UpdateExternallyTaggedBRep.CS.Properties;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.UpdateExternallyTaggedBRep.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        private static readonly string AddinAssemblyPath = typeof(Application).Assembly.Location;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            CreateRibbonButtons(application);
            return Result.Succeeded;
        }

        private void CreateRibbonButtons(UIControlledApplication application)
        {
            application.CreateRibbonTab("Create ExternallyTaggedBRep");
            var rp = application.CreateRibbonPanel("Create ExternallyTaggedBRep",
                "Create Geometry with persistent tags");

            var pbd1 = new PushButtonData("CreateTaggedBRep", "Create tagged BRep",
                AddinAssemblyPath,
                "Ara3D.RevitSampleBrowser.UpdateExternallyTaggedBRep.CS.CreateBRep")
            {
                LargeImage = ConvertFromBitmap(Resources.large_shape),
                Image = ConvertFromBitmap(Resources.small_shape)
            };
            _ = rp.AddItem(pbd1) as PushButton;

            var pbd2 = new PushButtonData("UpdateTaggedBRep", "Update tagged BRep",
                AddinAssemblyPath,
                "Ara3D.RevitSampleBrowser.UpdateExternallyTaggedBRep.CS.UpdateBRep")
            {
                LargeImage = ConvertFromBitmap(Resources.large_shape),
                Image = ConvertFromBitmap(Resources.small_shape)
            };
            _ = rp.AddItem(pbd2) as PushButton;
        }

        private BitmapSource ConvertFromBitmap(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
