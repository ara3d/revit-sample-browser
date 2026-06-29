// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

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

                    // We will make a string that has the following format,
                    // name type value
                    // create a StringBuilder object to store the string of one parameter
                    // using the character '\t' to delimit parameter name, type and value 
                    var sb = new StringBuilder();

                    // the name of the parameter can be found from its definition.
                    sb.AppendFormat("{0}\t", param.Definition.Name);

                    // Revit parameters can be one of 5 different public storage types:
                    // double, int, string, Autodesk.Revit.DB.ElementId and None. 
                    // if it is double then use AsDouble to get the double value
                    // then int AsInteger, string AsString, None AsStringValue.
                    // Switch based on the storage type
                    switch (param.StorageType)
                    {
                        case StorageType.Double:
                            // append the type and value
                            sb.AppendFormat("double\t{0}", param.AsDouble());
                            break;
                        case StorageType.ElementId:
                            // for element ids, we will try and retrieve the element from the 
                            // document if it can be found we will display its name.
                            sb.Append("Element\t");

                            // using ActiveDocument.GetElement(the element id) to 
                            // retrieve the element from the active document
                            var elemId = param.AsElementId();
                            var elem = app.ActiveUIDocument.Document.GetElement(elemId);

                            // if there is an element then display its name, 
                            // otherwise display the fact that it is not set
                            sb.Append(elem != null ? elem.Name : "Not set");
                            break;
                        case StorageType.Integer:
                            // append the type and value
                            sb.AppendFormat("int\t{0}", param.AsInteger());
                            break;
                        case StorageType.String:
                            // append the type and value
                            sb.AppendFormat("string\t{0}", param.AsString());
                            break;
                        case StorageType.None:
                            // append the type and value
                            sb.AppendFormat("none\t{0}", param.AsValueString());
                            break;
                    }

                    // add the completed line to the string list
                    parameterItems.Add(sb.ToString());
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
