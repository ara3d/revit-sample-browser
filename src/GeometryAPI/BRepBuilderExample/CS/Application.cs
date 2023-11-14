// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

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
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Revit.SDK.Samples.BRepBuilderExample.CS.Properties;

namespace Revit.SDK.Samples.BRepBuilderExample.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        private static readonly string AddinAssmeblyPath = typeof(Application).Assembly.Location;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            CreateRibbonButtons(application);
            return Result.Succeeded;
        }

        private BitmapSource ConvertFromBitmap(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void CreateRibbonButtons(UIControlledApplication application)
        {
            application.CreateRibbonTab("Create Geometry");
            var rp = application.CreateRibbonPanel("Create Geometry", "Create Geometry using BRepBuilder");

            var pbd1 = new PushButtonData("CreateCube", "Create Cube",
                AddinAssmeblyPath,
                "Revit.SDK.Samples.BRepBuilderExample.CS.CreateCube")
            {
                LargeImage = ConvertFromBitmap(Resources.large_shape),
                Image = ConvertFromBitmap(Resources.small_shape)
            };
            _ = rp.AddItem(pbd1) as PushButton;

            var pbd2 = new PushButtonData("CreateNURBS", "Create NURBS Surface",
                AddinAssmeblyPath,
                "Revit.SDK.Samples.BRepBuilderExample.CS.CreateNURBS")
            {
                LargeImage = ConvertFromBitmap(Resources.large_shape),
                Image = ConvertFromBitmap(Resources.small_shape)
            };
            _ = rp.AddItem(pbd2) as PushButton;

            var pbd3 = new PushButtonData("CreatePeriodic", "Create Periodic Surface",
                AddinAssmeblyPath,
                "Revit.SDK.Samples.BRepBuilderExample.CS.CreatePeriodic")
            {
                LargeImage = ConvertFromBitmap(Resources.large_shape),
                Image = ConvertFromBitmap(Resources.small_shape)
            };
            _ = rp.AddItem(pbd3) as PushButton;
        }
    }
}
