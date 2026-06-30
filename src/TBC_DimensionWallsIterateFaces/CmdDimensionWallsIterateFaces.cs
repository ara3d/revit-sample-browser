#region Header

//
// CmdDimensionWallsIterateFaces.cs - create dimensioning elements
// between opposing walls by iterating over their faces
//
// Copyright (C) 2011-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>
    ///     Dimension two opposing parallel walls.
    ///     For simplicity, the dimension is defined from
    ///     wall midpoint to midpoint, so the walls have
    ///     to be exactly opposite each other for it to work.
    ///     Iterate the wall solid faces to find the two
    ///     closest opposing faces and use references to
    ///     them to define the dimension element.
    ///     First sample solution for case
    ///     1263071 [Revit 2011 Dimension Wall].
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class CmdDimensionWallsIterateFaces : IExternalCommand
    {
        private const string _prompt
            = "Please select two parallel opposing straight walls.";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            // obtain the current selection and pick
            // out all walls from it:

            //Selection sel = uidoc.Selection; // 2014

            var ids = uidoc.Selection
                .GetElementIds(); // 2015

            List<Wall> walls = new(2);

            //foreach( Element e in sel.Elements ) // 2014

            foreach (var id in ids) // 2015
            {
                var e = doc.GetElement(id);

                if (e is Wall wall) walls.Add(wall);
            }

            if (2 != walls.Count)
            {
                message = _prompt;
                return Result.Failed;
            }

            // ensure the two selected walls are straight and
            // parallel; determine their mutual normal vector
            // and a point on each wall for distance
            // calculations:

            List<Line> lines = new(2);
            List<XYZ> midpoints = new(2);
            XYZ normal = null;

            foreach (var wall in walls)
            {
                var lc = wall.Location as LocationCurve;
                var curve = lc.Curve;

                if (curve is not Line line)
                {
                    message = _prompt;
                    return Result.Failed;
                }

                lines.Add(line);
                midpoints.Add(Util.Midpoint(line));

                if (null == normal)
                {
                    normal = Util.Normal(line);
                }
                else
                {
                    if (!Util.IsParallel(normal, Util.Normal(line)))
                    {
                        message = _prompt;
                        return Result.Failed;
                    }
                }
            }

            // find the two closest facing faces on the walls;
            // they are vertical faces that are parallel to the
            // wall curve and closest to the other wall.

            var opt = app.Create.NewGeometryOptions();

            opt.ComputeReferences = true;

            List<Face> faces = new(2)
            {
                Util.GetClosestFace(walls[0], midpoints[1], normal, opt),
                Util.GetClosestFace(walls[1], midpoints[0], normal, opt)
            };

            // create the dimensioning:

            Util.CreateDimensionElement(doc.ActiveView,
                midpoints[0], faces[0].Reference,
                midpoints[1], faces[1].Reference);

            return Result.Succeeded;
        }

        #region Dimension Filled Region Alexander

        [Transaction(TransactionMode.Manual)]
        public class CreateFillledRegionDimensionsCommand : IExternalCommand
        {
            public Result Execute(
                ExternalCommandData commandData,
                ref string message,
                ElementSet elements)
            {
                var uiapp = commandData.Application;
                var uidoc = uiapp.ActiveUIDocument;
                var doc = uidoc.Document;

                var view = uidoc.ActiveGraphicalView;

                var filledRegions = Util.FindFilledRegions(doc, view.Id);

                using Transaction transaction = new(doc,
                    "filled regions dimensions");
                transaction.Start();

                foreach (var filledRegion in filledRegions)
                {
                    Util.CreateFilledRegionDimensions(filledRegion,
                        -1 * view.RightDirection);

                    Util.CreateFilledRegionDimensions(filledRegion, view.UpDirection);
                }

                transaction.Commit();

                return Result.Succeeded;
            }
        }

        #endregion // Dimension Filled Region Alexander
    }
}