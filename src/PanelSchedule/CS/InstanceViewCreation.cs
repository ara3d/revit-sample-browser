// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Revit.SDK.Samples.PanelSchedule.CS
{
    /// <summary>
    ///     Create view instance for an electrical panel.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class InstanceViewCreation : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var selected = commandData.Application.ActiveUIDocument.Selection.PickObject(ObjectType.Element);

            var newInstanceView = new Transaction(doc, "Create instance view for an electrical panel.");
            newInstanceView.Start();
            var instanceView = PanelScheduleView.CreateInstanceView(doc, doc.GetElement(selected).Id);
            if (null == instanceView)
            {
                newInstanceView.RollBack();
                message = "Please select one electrical panel.";
                return Result.Failed;
            }

            newInstanceView.Commit();
            return Result.Succeeded;
        }
    }
}
