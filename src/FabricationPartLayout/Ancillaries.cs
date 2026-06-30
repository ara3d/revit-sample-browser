// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Ancillaries : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var uidoc = commandData.Application.ActiveUIDocument;

            FabricationPart fabPart;
            FabricationConfiguration config;

            try
            {
                config = FabricationConfiguration.GetFabricationConfiguration(doc);

                if (config == null)
                {
                    message = "No fabrication configuration loaded.";
                    return Result.Failed;
                }

                var refObj = uidoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part to start.");
                fabPart = doc.GetElement(refObj) as FabricationPart;
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }

            if (fabPart == null)
            {
                message = "The selected element is not a fabrication part.";
                return Result.Failed;
            }

            var ancillaries = fabPart.GetPartAncillaryUsage();
            List<string> ancillaryDescriptions = new();

            foreach (var ancillaryUsage in ancillaries)
            {
                var ancilType = ancillaryUsage.Type;
                var usageType = ancillaryUsage.UsageType;
                ancillaryDescriptions.Add(
                    $"{ancilType}: {usageType} - {config.GetAncillaryName(ancillaryUsage.AncillaryId)}");
            }

            var results = string.Empty;

            if (ancillaryDescriptions.Count > 0)
            {
                ancillaryDescriptions.Sort();
                StringBuilder resultsBuilder = new();
                var currentAncillary = string.Empty;

                foreach (var ancillaryName in ancillaryDescriptions)
                {
                    if (ancillaryName != currentAncillary)
                    {
                        resultsBuilder.AppendLine(
                            $"{ancillaryName} x {ancillaryDescriptions.Count(x => x == ancillaryName)}");
                        currentAncillary = ancillaryName;
                    }
                }

                results = resultsBuilder.ToString();
            }

            TaskDialog td = new("Ancillaries")
            {
                MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                TitleAutoPrefix = false,
                MainInstruction = ancillaryDescriptions.Count > 0
                    ? $"{ancillaryDescriptions.Count} ancillaries found on selected part"
                    : "No ancillaries found on selected part",
                MainContent = results
            };

            td.Show();

            return Result.Succeeded;
        }
    }
}
