// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.StructuralLayerFunction.CS
{
    /// <summary>
    ///     With the selected floor, display the function of each of its structural layers
    ///     in order from outside to inside in a dialog box
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private Floor m_slab; //Store the selected floor

        public Command()
        {
            //Construct the data members for the property
            Functions = new ArrayList();
        }

        public ArrayList Functions { get; }

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var revit = commandData.Application;

            // Get the selected floor
            var project = revit.ActiveUIDocument;
            var choices = project.Selection;
            var collection = new ElementSet();
            foreach (var elementId in choices.GetElementIds())
            {
                collection.Insert(project.Document.GetElement(elementId));
            }

            // Only allow to select one floor, or else report the failure
            if (collection.Size != 1)
            {
                message = "Please select a floor.";
                return Result.Failed;
            }

            foreach (Element e in collection)
            {
                m_slab = e as Floor;
                if (m_slab == null)
                {
                    message = "Please select a floor.";
                    return Result.Failed;
                }
            }

            // Get the function of each of its structural layers
            foreach (var e in m_slab.FloorType.GetCompoundStructure().GetLayers())
                if (0 == e.Function)
                    Functions.Add("No function");
                else
                    Functions.Add(e.Function.ToString());

            // Display them in a form
            using (var displayForm = new StructuralLayerFunctionForm(this))
            {
                displayForm.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}
