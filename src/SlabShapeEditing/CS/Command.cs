// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.SlabShapeEditing.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var selectFloor = GetSelectFloor(commandData);
            if (null == selectFloor)
            {
                message = "Make sure selected only one floor (Slab) in Revit.";
                return Result.Failed;
            }

            var slabShapeEditingForm =
                new SlabShapeEditingForm(selectFloor, commandData);
            slabShapeEditingForm.ShowDialog();

            return Result.Succeeded;
        }

        /// <summary>
        ///     get selected floor (slab)
        /// </summary>
        /// <param name="commandData">object which contains reference of Revit Application.</param>
        /// <returns>selected floor (slab)</returns>
        private Floor GetSelectFloor(ExternalCommandData commandData)
        {
            var eleSet = new ElementSet();
            foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                eleSet.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
            if (eleSet.Size != 1) return null;

            var iter = eleSet.GetEnumerator();
            iter.Reset();
            while (iter.MoveNext()) return iter.Current as Floor;
            return null;
        }
    }
}
