//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ButtonPaletteExclusions : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;

                using (var tr = new Transaction(doc, "Set button and palette exclusions"))
                {
                    tr.Start();

                    var config = FabricationConfiguration.GetFabricationConfiguration(doc);

                    if (config == null)
                    {
                        message = "No fabrication configuration loaded.";
                        return Result.Failed;
                    }

                    // get all loaded fabrication services
                    var allLoadedServices = config.GetAllLoadedServices();
                    // get the "ADSK - HVAC:Supply Air" service
                    var serviceName = "ADSK - HVAC: Supply Air";
                    var selectedService = allLoadedServices.FirstOrDefault(x => x.Name == serviceName);

                    if (selectedService == null)
                    {
                        message = $"Could not find fabrication service {serviceName}";
                        return Result.Failed;
                    }

                    var rectangularPaletteName = "Rectangular";
                    var roundPaletteName = "Round Bought Out";
                    var excludeButtonName = "Square Bend";

                    var rectangularPaletteIndex = -1;
                    var roundPaletteIndex = -1;

                    // find Rectangular and Round palettes in service
                    for (var i = 0; i < selectedService.PaletteCount; i++)
                    {
                        if (selectedService.GetPaletteName(i) == rectangularPaletteName) rectangularPaletteIndex = i;

                        if (selectedService.GetPaletteName(i) == roundPaletteName) roundPaletteIndex = i;

                        if (rectangularPaletteIndex > -1 && roundPaletteIndex > -1) break;
                    }

                    if (rectangularPaletteIndex > -1)
                    {
                        // exclude square bend in Rectangular palette
                        for (var i = 0; i < selectedService.GetButtonCount(rectangularPaletteIndex); i++)
                            if (selectedService.GetButton(rectangularPaletteIndex, i).Name == excludeButtonName)
                            {
                                selectedService.OverrideServiceButtonExclusion(rectangularPaletteIndex, i, true);
                                break;
                            }
                    }
                    else
                    {
                        message = $"Unable to locate {excludeButtonName} button to exclude.";
                        return Result.Failed;
                    }

                    // exclude entire Round Bought Out service palette
                    if (roundPaletteIndex > -1)
                    {
                        selectedService.SetServicePaletteExclusions(new List<int> { roundPaletteIndex });
                    }
                    else
                    {
                        message = $"Unable to locate {roundPaletteName} service palette to exclude.";
                        return Result.Failed;
                    }

                    tr.Commit();

                    var td = new TaskDialog("Button and Palette Exclsuions")
                    {
                        MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                        TitleAutoPrefix = false,
                        MainInstruction = "Operation Successful",
                        MainContent =
                            $"Excluded {excludeButtonName} button from {serviceName} {rectangularPaletteName} Palette {Environment.NewLine}"
                            + $"Excluded {roundPaletteName} Palette from {serviceName}"
                    };

                    td.Show();
                }


                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}