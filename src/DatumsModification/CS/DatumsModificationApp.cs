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
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DatumsModification.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DatumsModificationApp : IExternalApplication
    {
        private static readonly string AddInPath = typeof(DatumsModificationApp).Assembly.Location;

        // Button icons directory
        private static readonly string ButtonIconsFolder = Path.GetDirectoryName(AddInPath);

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            var ribbonPanel = application.CreateRibbonPanel("DatumModification");
            var styleSettingButton = new PushButtonData("DatumStyle", "Datum Style", AddInPath,
                "Revit.SDK.Samples.DatumsModification.CS.DatumStyleModification")
            {
                LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "Style.png"), UriKind.Absolute))
            };
            var alignSettingButton = new PushButtonData("AlignDatum", "Align Datums", AddInPath,
                "Revit.SDK.Samples.DatumsModification.CS.DatumAlignment")
            {
                LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "Align.png"), UriKind.Absolute))
            };
            var propagateButton = new PushButtonData("PropagateDatum", "Propagate Extents", AddInPath,
                "Revit.SDK.Samples.DatumsModification.CS.DatumPropagation")
            {
                LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "Propagate.png"), UriKind.Absolute))
            };

            ribbonPanel.AddItem(styleSettingButton);
            ribbonPanel.AddItem(alignSettingButton);
            ribbonPanel.AddItem(propagateButton);
            return Result.Succeeded;
        }
    }
}
