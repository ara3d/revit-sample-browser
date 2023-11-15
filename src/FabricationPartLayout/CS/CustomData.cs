// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
{
    /// <summary>
    ///     Helper class to report custom data from fabrication part selection.
    /// </summary>
    public class CustomDataHelper
    {
        /// <summary>
        ///     Report the custom data.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="uiDoc"></param>
        /// <param name="setNewValues"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result ReportCustomData(Document doc, UIDocument uiDoc, bool setNewValues, ref string message)
        {
            FabricationPart fabPart = null;
            FabricationConfiguration config = null;

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
                var refObj = uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part to start.");
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

            // get custom data from loaded fabrication config
            var customDataIds = config.GetAllPartCustomData();
            var customDataCount = customDataIds.Count;
            var results = string.Empty;

            // report custom data info
            if (customDataCount > 0)
            {
                var resultsBuilder = new StringBuilder();
                resultsBuilder.AppendLine(
                    $"Fabrication config contains {customDataCount} custom data entries {Environment.NewLine}");

                foreach (var customDataId in customDataIds)
                {
                    var customDataType = config.GetPartCustomDataType(customDataId);
                    var customDataName = config.GetPartCustomDataName(customDataId);

                    resultsBuilder.AppendLine($"Type: {customDataType} Name: {customDataName}");
                    // check custom data exists on selected part
                    if (fabPart.HasCustomData(customDataId))
                    {
                        var fabPartCurrentValue = string.Empty;
                        var fabPartNewValue = string.Empty;

                        switch (customDataType)
                        {
                            case FabricationCustomDataType.Text:

                                fabPartCurrentValue = $"\"{fabPart.GetPartCustomDataText(customDataId)}\"";

                                if (setNewValues)
                                {
                                    var installDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    fabPart.SetPartCustomDataText(customDataId, installDateTime);
                                    fabPartNewValue = installDateTime;
                                }

                                break;
                            case FabricationCustomDataType.Integer:

                                fabPartCurrentValue = fabPart.GetPartCustomDataInteger(customDataId).ToString();

                                if (setNewValues)
                                {
                                    var installHours = new Random().Next(1, 10);
                                    fabPart.SetPartCustomDataInteger(customDataId, installHours);
                                    fabPartNewValue = installHours.ToString();
                                }

                                break;
                            case FabricationCustomDataType.Real:

                                fabPartCurrentValue = $"{fabPart.GetPartCustomDataReal(customDataId):0.##}";

                                if (setNewValues)
                                {
                                    var installCost = new Random().NextDouble() * new Random().Next(100, 1000);
                                    fabPart.SetPartCustomDataReal(customDataId, installCost);
                                    fabPartNewValue = $"{installCost:0.##}";
                                }

                                break;
                        }

                        resultsBuilder.AppendLine("Current custom data entry value = "
                                                  + $"{fabPartCurrentValue} {Environment.NewLine}");

                        if (setNewValues)
                            resultsBuilder.AppendLine("New custom data entry value = "
                                                      + $"{fabPartNewValue} {Environment.NewLine}");
                    }
                    else
                    {
                        resultsBuilder.AppendLine($"Custom data entry is not set on the part {Environment.NewLine}");
                    }
                }

                results = resultsBuilder.ToString();
            }

            var td = new TaskDialog("Custom Data")
            {
                MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                TitleAutoPrefix = false,
                MainInstruction = $"{customDataCount} custom data entries found in the loaded fabrication config",
                MainContent = results
            };

            td.Show();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class GetCustomData : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var uidoc = commandData.Application.ActiveUIDocument;

            return CustomDataHelper.ReportCustomData(doc, uidoc, false, ref message);
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SetCustomData : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var uidoc = commandData.Application.ActiveUIDocument;
            Result result;

            using (var tr = new Transaction(doc, "Setting Custom Data"))
            {
                tr.Start();
                result = CustomDataHelper.ReportCustomData(doc, uidoc, true, ref message);

                if (result == Result.Succeeded)
                    tr.Commit();
                else
                    tr.RollBack();
            }

            return result;
        }
    }
}
