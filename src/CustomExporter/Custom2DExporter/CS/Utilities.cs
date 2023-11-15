// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.Custom2DExporter.CS
{
    /// <summary>
    ///     Collection of helpful utility classes.
    /// </summary>
    public class Utilities
    {
        /// <summary>
        ///     Utility to properly append points from 1 list to another.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void AddTo(IList<XYZ> to, IList<XYZ> from)
        {
            var cnt = from.Count;
            for (var ii = 0; ii < cnt; ii++)
                if (ii < cnt - 1)
                {
                    var p0 = from[ii];
                    var p1 = from[ii + 1];
                    to.Add(p0);
                    to.Add(p1);
                }
        }

        private static Plane GetAppropriatePlane(View view)
        {
            var plane = Plane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);
            return plane;
        }

        /// <summary>
        ///     Draw all lines defined by the list of (start, end) points, which are bigger than the given tolerance.
        ///     The lines are drawn as detail lines (DetailLine).
        /// </summary>
        /// <param name="view">The view to draw the lines in.</param>
        /// <param name="points">The list of (start, end) points of the lines.</param>
        /// <param name="tolerance">The smallest curve length allowed to be drawn. Lines smaller than this are not drawn.</param>
        private static void DrawLines(View view, IList<XYZ> points, double tolerance)
        {
            var plane = GetAppropriatePlane(view);
            if (plane != null)
                for (var ii = 0; ii < points.Count; ii++)
                {
                    UV uvStart, uvEnd;
                    plane.Project(points[ii], out uvStart, out _);
                    plane.Project(points[ii + 1], out uvEnd, out _);

                    var projectionStart = uvStart.U * plane.XVec + uvStart.V * plane.YVec + plane.Origin;
                    var projectionEnd = uvEnd.U * plane.XVec + uvEnd.V * plane.YVec + plane.Origin;

                    if (projectionStart.DistanceTo(projectionEnd) < tolerance)
                    {
                        ii++;
                        continue;
                    }

                    var geomLine = Line.CreateBound(projectionStart, projectionEnd);
                    _ = view.Document.Create.NewDetailCurve(view, geomLine) as DetailLine;

                    ii++;
                }
        }

        /// <summary>
        ///     Hide all elements visibe in the view.
        /// </summary>
        /// <param name="view">The view</param>
        private static void HideAllInView(View view)
        {
            var viewElems = new FilteredElementCollector(view.Document, view.Id);
            var elementIds = viewElems.ToElementIds();
            ICollection<ElementId> hideElemIds = new List<ElementId>();
            foreach (var id in elementIds)
                if (view.Document.GetElement(id).CanBeHidden(view))
                    hideElemIds.Add(id);

            view.HideElements(hideElemIds);
        }

        /// <summary>
        ///     Displays the exported list of points by drawing detail lines with (start, end) defined by the points.
        ///     All elements besides the drawn lines will be hidden.
        /// </summary>
        /// <param name="view">The view in which to draw lines and hide elements.</param>
        /// <param name="points">The points which define the lines (start, end) points</param>
        public static void DisplayExport(View view, IList<XYZ> points)
        {
            var doc = view.Document;
            using (var tran = new Transaction(view.Document, "ExportViewGeometry"))
            {
                tran.Start("Draw Exported Lines and turn off everything but Lines");
                HideAllInView(view);
                DrawLines(view, points, doc.Application.ShortCurveTolerance);
                tran.Commit();
            }
        }
    }
}
