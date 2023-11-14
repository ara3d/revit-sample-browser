// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
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
    public class ExportToPcf : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uidoc = commandData.Application.ActiveUIDocument;
                var doc = uidoc.Document;
                var collection = uidoc.Selection.GetElementIds();
                var hasFabricationPart = false;

                using (var trans = new Transaction(doc, "Change Spool Name"))
                {
                    trans.Start();

                    foreach (var elementId in collection)
                    {
                        if (doc.GetElement(elementId) is FabricationPart part)
                        {
                            hasFabricationPart = true;
                            part.SpoolName = "My Spool";
                        }
                    }

                    trans.Commit();
                }

                if (hasFabricationPart == false)
                {
                    message = "Select at least one fabrication part";
                    return Result.Failed;
                }

                var callingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                var saveAsDlg = new FileSaveDialog("PCF Files (*.pcf)|*.pcf");

                saveAsDlg.InitialFileName = callingFolder + "\\pcfExport";
                saveAsDlg.Title = "Export To PCF";
                var result = saveAsDlg.Show();

                if (result == ItemSelectionDialogResult.Canceled)
                    return Result.Cancelled;

                var fabParts = collection.ToList();

                var filename = ModelPathUtils.ConvertModelPathToUserVisiblePath(saveAsDlg.GetSelectedModelPath());

                FabricationUtils.ExportToPCF(doc, fabParts, filename);

                var td = new TaskDialog("Export to PCF")
                {
                    MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                    TitleAutoPrefix = false,
                    MainInstruction = "Export to PCF was successful",
                    MainContent = filename,
                    AllowCancellation = false,
                    CommonButtons = TaskDialogCommonButtons.Ok
                };

                td.Show();

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
