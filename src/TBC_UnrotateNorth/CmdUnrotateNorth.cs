#region Header

//
// CmdUnrotateNorth.cs - transform element location back to
// original coordinates to cancel effect of rotating project north
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdUnrotateNorth : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            #region Determine true north rotation

            var projectInfoElement
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ProjectBasePoint)
                    .FirstElement();

            var bipAtn
                = BuiltInParameter.BASEPOINT_ANGLETON_PARAM;

            var patn = projectInfoElement.get_Parameter(
                bipAtn);

            var atn = patn.AsDouble();

            Debug.Print(
                "Angle to north from project info: {0}",
                Util.AngleString(atn));

            #endregion // Determine true north rotation

            var ids = uidoc.Selection.GetElementIds();

            if (1 != ids.Count)
            {
                message = "Please select a single element.";
            }
            else
            {
                var e = doc.GetElement(ids.First());

                XYZ p;
                if (!Util.GetElementLocation(out p, e))
                {
                    message
                        = "Selected element has no location defined.";

                    Debug.Print(message);
                }
                else
                {
                    var msg
                        = $"Selected element location: {Util.PointString(p)}";

                    XYZ pnp;
                    double x, y, pna;

                    foreach (ProjectLocation location
                        in doc.ProjectLocations)
                    {
                        var projectPosition
                            = location.GetProjectPosition(XYZ.Zero);

                        x = projectPosition.EastWest;
                        y = projectPosition.NorthSouth;
                        pnp = new XYZ(x, y, 0.0);
                        pna = projectPosition.Angle;

                        msg +=
                            $"\nAngle between project north and true north: {Util.AngleString(pna)}";

                        var tr = Transform.CreateRotation(XYZ.BasisZ, pna);
                        var tt = Transform.CreateTranslation(pnp);

                        var t = tt.Multiply(tr);

                        msg +=
                            $"\nUnrotated element location: {Util.PointString(tr.OfPoint(p))} {Util.PointString(tt.OfPoint(p))} {Util.PointString(t.OfPoint(p))}";

                        Util.InfoMsg(msg);
                    }
                }
            }

            return Result.Failed;
        }
    }
}
