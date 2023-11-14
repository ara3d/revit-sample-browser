// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitMultiSample.FabricationPartLayout.CS
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
                // check for a load fabrication config
                config = FabricationConfiguration.GetFabricationConfiguration(doc);

                if (config == null)
                {
                    message = "No fabrication configuration loaded.";
                    return Result.Failed;
                }

                // pick a fabrication part
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

            // get ancillary data from selected part and report to user
            var ancillaries = fabPart.GetPartAncillaryUsage();
            var ancillaryDescriptions = new List<string>();

            // create list of ancillary descriptions using the Ancillary UseageType and Name
            foreach (var ancillaryUsage in ancillaries)
            {
                var ancilType = ancillaryUsage.Type;
                var usageType = ancillaryUsage.UsageType;
                ancillaryDescriptions.Add($"{ancilType}: {usageType} - "
                                          + $"{config.GetAncillaryName(ancillaryUsage.AncillaryId)}");
            }

            var results = string.Empty;

            // group and quantify 
            if (ancillaryDescriptions.Count > 0)
            {
                ancillaryDescriptions.Sort();
                var resultsBuilder = new StringBuilder();
                var currentAncillary = string.Empty;

                foreach (var ancillaryName in ancillaryDescriptions)
                    if (ancillaryName != currentAncillary)
                    {
                        resultsBuilder.AppendLine(
                            $"{ancillaryName} x {ancillaryDescriptions.Count(x => x == ancillaryName)}");
                        currentAncillary = ancillaryName;
                    }

                results = resultsBuilder.ToString();
            }

            var td = new TaskDialog("Ancillaries")
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
