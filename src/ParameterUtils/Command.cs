// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Units;
namespace Ara3D.RevitSampleBrowser.ParameterUtils.CS
{
    /// <summary>
    ///     display a Revit element property-like form related to the selected element.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            // set out default result to failure.
            var retRes = Result.Failed;

            var app = commandData.Application;

            // get the elements selected
            // The current selection can be retrieved from the active 
            // document via the selection object
            var seletion = new ElementSet();
            foreach (var elementId in app.ActiveUIDocument.Selection.GetElementIds())
            {
                seletion.Insert(app.ActiveUIDocument.Document.GetElement(elementId));
            }

            // we need to make sure that only one element is selected.
            if (seletion.Size == 1)
            {
                // we need to get the first and only element in the selection. Do this by getting 
                // an iterator. MoveNext and then get the current element.
                var it = seletion.ForwardIterator();
                it.MoveNext();
                var element = it.Current as Element;

                // Next we need to iterate through the parameters of the element,
                // as we iterating, we will store the strings that are to be displayed
                // for the parameters in a string list "parameterItems"
                var parameterItems = new List<string>();
                var parameters = element.Parameters;
                foreach (Parameter param in parameters)
                {
                    if (param == null) continue;
                    parameterItems.Add(ValueFormatting.FormatParameterLine(param, app.ActiveUIDocument.Document));
                }

                // Create our dialog, passing it the parameters array for display.
                var propertiesForm = new PropertiesForm(parameterItems.ToArray())
                {
                    StartPosition = FormStartPosition.CenterParent
                };
                propertiesForm.ShowDialog();
                retRes = Result.Succeeded;
            }
            else
            {
                message = "Please select only one element";
            }

            return retRes;
        }
    }
}
