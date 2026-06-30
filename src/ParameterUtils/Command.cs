// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Units;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows.Forms;
namespace Ara3D.RevitSampleBrowser.ParameterUtils.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            var retRes = Result.Failed;

            var app = commandData.Application;

            ElementSet seletion = new();
            foreach (var elementId in app.ActiveUIDocument.Selection.GetElementIds())
            {
                seletion.Insert(app.ActiveUIDocument.Document.GetElement(elementId));
            }

            if (seletion.Size == 1)
            {
                var it = seletion.ForwardIterator();
                it.MoveNext();
                var element = it.Current as Element;

                List<string> parameterItems = new();
                var parameters = element.Parameters;
                foreach (Parameter param in parameters)
                {
                    if (param == null) continue;
                    parameterItems.Add(ValueFormatting.FormatParameterLine(param, app.ActiveUIDocument.Document));
                }

                PropertiesForm propertiesForm = new(parameterItems.ToArray())
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
