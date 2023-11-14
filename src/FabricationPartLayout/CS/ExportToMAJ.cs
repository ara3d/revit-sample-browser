// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExportToMaj : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uidoc = commandData.Application.ActiveUIDocument;
                var doc = uidoc.Document;
                var elementIds = new HashSet<ElementId>();
                uidoc.Selection.GetElementIds().ToList().ForEach(x => elementIds.Add(x));

                var hasFabricationParts = false;
                foreach (var elementId in elementIds)
                {
                    if (doc.GetElement(elementId) is FabricationPart part)
                    {
                        hasFabricationParts = true;
                        break;
                    }
                }

                if (hasFabricationParts == false)
                {
                    message = "Select at least one fabrication part";
                    return Result.Failed;
                }

                var callingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var saveAsDlg = new FileSaveDialog("MAJ Files (*.maj)|*.maj");
                saveAsDlg.InitialFileName = callingFolder + "\\majExport";
                saveAsDlg.Title = "Export To MAJ";
                var result = saveAsDlg.Show();

                if (result == ItemSelectionDialogResult.Canceled)
                    return Result.Cancelled;

                var filename = ModelPathUtils.ConvertModelPathToUserVisiblePath(saveAsDlg.GetSelectedModelPath());

                var exported =
                    FabricationPart.SaveAsFabricationJob(doc, elementIds, filename,
                        new FabricationSaveJobOptions(true));
                if (exported.Count > 0)
                {
                    var td = new TaskDialog("Export to MAJ")
                    {
                        MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                        TitleAutoPrefix = false,
                        MainInstruction = string.Concat("Export to MAJ was successful - ", exported.Count.ToString(),
                            " Parts written"),
                        MainContent = filename,
                        AllowCancellation = false,
                        CommonButtons = TaskDialogCommonButtons.Ok
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
