#region Namespaces

using System;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PickPoint3d sample.</summary>
    internal static partial class Util
    {
        public static void PickPointsForArea(UIDocument uidoc)
        {
            var doc = uidoc.Document;
            var view = doc.ActiveView;

            XYZ p1, p2;

            try
            {
                p1 = uidoc.Selection.PickPoint(
                    "Please pick first point for area");

                p2 = uidoc.Selection.PickPoint(
                    "Please pick second point for area");
            }
            catch (OperationCanceledException)
            {
                return;
            }

            var plane = view.SketchPlane.GetPlane();

            var q1 = plane.ProjectInto(p1);
            var q2 = plane.ProjectInto(p2);
            var d = q2 - q1;

            var area = d.U * d.V;

            area = Math.Round(area, 2);

            if (area < 0) area = area * -1;

            TaskDialog.Show("Area", area.ToString());
        }
        public static bool PickFaceSetWorkPlaneAndPickPoint(
            UIDocument uidoc,
            out XYZ point_in_3d)
        {
            point_in_3d = null;

            var doc = uidoc.Document;

            try
            {
                var r = uidoc.Selection.PickObject(
                    ObjectType.Face,
                    "Please select a planar face to define work plane");

                var e = doc.GetElement(r.ElementId);

                if (null != e)
                {
                    var face
                        = e.GetGeometryObjectFromReference(r)
                            as PlanarFace;

                    if (face != null)
                    {
                        var plane = Plane.CreateByNormalAndOrigin(
                            face.FaceNormal, face.Origin);

                        using var t = new Transaction(doc);
                        t.Start("Temporarily set work plane"
                                + " to pick point in 3D");

                        var sp = SketchPlane.Create(doc, plane);

                        uidoc.ActiveView.SketchPlane = sp;
                        uidoc.ActiveView.ShowActiveWorkPlane();

                        point_in_3d = uidoc.Selection.PickPoint(
                            "Please pick a point on the plane"
                            + " defined by the selected face");

                        t.RollBack();
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }

            return null != point_in_3d;
        }
        public static bool TransformPickPointToScreen(UIDocument uidoc)
        {
            var view = uidoc.ActiveGraphicalView;

            var t = Transform.Identity;
            t.Origin = view.Origin;
            t.BasisX = view.RightDirection;
            t.BasisY = view.UpDirection;
            t.BasisZ = view.ViewDirection;

            var x_model_to_screen = t.Inverse;

            Reference r;

            try
            {
                r = uidoc.Selection.PickObject(ObjectType.PointOnElement);
            }
            catch (OperationCanceledException)
            {
                return false;
            }

            var p = r.GlobalPoint;
            var q = x_model_to_screen.OfPoint(p);

            var sb = new StringBuilder();
            sb.AppendFormat("Model x={0:F3},y={1:F3},z={2:F3}", p.X, p.Y, p.Z);
            sb.AppendFormat("Screen x={0:F3},y={1:F3},z={2:F3}", q.X, q.Y, q.Z);

            TaskDialog.Show("Coords", sb.ToString());

            return true;
        }
    }
}
