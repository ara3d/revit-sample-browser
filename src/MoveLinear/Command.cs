// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;

namespace Ara3D.RevitSampleBrowser.MoveLinear.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData cmdData, ref string msg, ElementSet eleSet)
        {
            var res = Result.Succeeded;
            Transaction trans = new(cmdData.Application.ActiveUIDocument.Document, "Ara3D.RevitSampleBrowser.MoveLinear");
            trans.Start();
            try
            {
                var sel = cmdData.Application.ActiveUIDocument.Selection;

                ElementSet elemSet = new();
                foreach (var elementId in sel.GetElementIds())
                {
                    elemSet.Insert(cmdData.Application.ActiveUIDocument.Document.GetElement(elementId));
                }

                //Check whether user has selected only one element
                if (0 == elemSet.Size)
                {
                    TaskDialog.Show("MoveLinear", "Please select an element");
                    trans.Commit();
                    return res;
                }

                if (1 < elemSet.Size)
                {
                    TaskDialog.Show("MoveLinear", "Please select only one element");
                    trans.Commit();
                    return res;
                }

                IEnumerator iter = elemSet.ForwardIterator();

                iter.MoveNext();

                var element = (Element)iter.Current;

                if (element != null)
                {
                    if (element.Location is not LocationCurve lineLoc)
                    {
                        TaskDialog.Show("MoveLinear", "Please select an element which based on a Line");
                        trans.Commit();
                        return res;
                    }

                    //get start point via "get_EndPoint(0)"
                    XYZ newStart = new(
                        lineLoc.Curve.GetEndPoint(0).X + 100,
                        lineLoc.Curve.GetEndPoint(0).Y,
                        lineLoc.Curve.GetEndPoint(0).Z);
                    //get end point via "get_EndPoint(1)"
                    XYZ newEnd = new(
                        lineLoc.Curve.GetEndPoint(1).X,
                        lineLoc.Curve.GetEndPoint(1).Y + 100,
                        lineLoc.Curve.GetEndPoint(1).Z);

                    //get a new line and use it to move current element 
                    //with property "Autodesk.Revit.DB.LocationCurve.Curve"
                    var line = Line.CreateBound(newStart, newEnd);
                    lineLoc.Curve = line;
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("MoveLinear", ex.Message);
                res = Result.Failed;
            }

            trans.Commit();
            return res;
        }
    }
}
