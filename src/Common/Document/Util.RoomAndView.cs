// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Consolidated room and view helpers

        public static Room GetRoomNeighbourAt(
            BoundarySegment bs,
            Room r)
        {
            var doc = r.Document;

            var w = doc.GetElement(bs.ElementId) as Wall;

            var wallThickness = w.Width;

            var derivatives = bs.GetCurve()
                .ComputeDerivatives(0.5, true);

            var midPoint = derivatives.Origin;

            Debug.Assert(
                midPoint.IsAlmostEqualTo(
                    bs.GetCurve().Evaluate(0.5, true)),
                "expected same result from Evaluate and derivatives");

            var tangent = derivatives.BasisX.Normalize();

            var normal = new XYZ(tangent.Y,
                tangent.X * -1, tangent.Z);

            var p = midPoint + wallThickness * normal;

            var otherRoom = doc.GetRoomAtPoint(p);

            if (null != otherRoom)
                if (otherRoom.Id == r.Id)
                {
                    normal = new XYZ(tangent.Y * -1,
                        tangent.X, tangent.Z);

                    p = midPoint + wallThickness * normal;

                    otherRoom = doc.GetRoomAtPoint(p);

                    Debug.Assert(null == otherRoom
                                 || otherRoom.Id != r.Id,
                        "expected different room on other side");
                }

            return otherRoom;
        }

        internal static View FindElevationView(
            Document doc,
            ICollection<ElementId> ids)
        {
            View view = null;

            foreach (var id in ids)
            {
                view = doc.GetElement(id) as View;

                if (view.IsTemplate
                    && ViewType.Internal == view.ViewType)
                {
                    view = null;
                    continue;
                }

                if (view is {ViewType: ViewType.Elevation})
                    break;

                view = null;
            }

            return view;
        }

        internal static Reference FindTopMostReference(Element e)
        {
            Reference ret = null;
            var doc = e.Document;

            var opt = doc.Application.Create
                .NewGeometryOptions();

            opt.ComputeReferences = true;

            var geo = e.get_Geometry(opt);

            foreach (var obj in geo)
            {
                var inst = obj as GeometryInstance;

                if (null != inst)
                {
                    geo = inst.GetSymbolGeometry();
                    break;
                }
            }

            var solid = geo.OfType<Solid>()
                .First(sol => null != sol);

            var z = double.MinValue;

            foreach (Face f in solid.Faces)
            {
                var b = f.GetBoundingBox();
                var p = b.Min;
                var q = b.Max;
                var midparam = p + 0.5 * (q - p);
                var midpoint = f.Evaluate(midparam);
                var normal = f.ComputeNormal(midparam);

                if (PointsUpwards(normal))
                    if (midpoint.Z > z)
                    {
                        z = midpoint.Z;
                        ret = f.Reference;
                    }
            }

            return ret;
        }

        #endregion
    }
}
