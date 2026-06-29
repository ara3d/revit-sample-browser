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

        /// <summary>
        ///     Default constructor of StructuralLayerFunction
        /// </summary>
        public Command()
        {
            //Construct the data members for the property
            Functions = new ArrayList();
        }

        /// <summary>
        ///     With the selected floor, export the function of each of its structural layers
        /// </summary>
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
            if (1 != collection.Size)
            {
                message = "Please select a floor.";
                return Result.Failed;
            }

            foreach (Element e in collection)
            {
                m_slab = e as Floor;
                if (null == m_slab)
                {
                    message = "Please select a floor.";
                    return Result.Failed;
                }
            }

            // Get the function of each of its structural layers
            foreach (var e in m_slab.FloorType.GetCompoundStructure().GetLayers())
                // With the selected floor, judge if the function of each of its structural layers
                // is exist, if it's not exist, there should be zero.
            {
                if (0 == e.Function)
                    Functions.Add("No function");
                else
                    Functions.Add(e.Function.ToString());
            }

            // Display them in a form
            var displayForm = new StructuralLayerFunctionForm(this);
            displayForm.ShowDialog();

            return Result.Succeeded;
        }
    }
}
