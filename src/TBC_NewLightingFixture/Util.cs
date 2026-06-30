#region Namespaces

using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewLightingFixture sample.</summary>
    internal static partial class Util
    {
        internal static FamilyInstance PlaceFamilyInstanceOnFace(
            UIDocument uidoc,
            FamilySymbol symbol)
        {
            var doc = uidoc.Document;

            var r = uidoc.Selection.PickObject(
                ObjectType.Face, "Please pick a point on "
                                 + " a face for family instance insertion");

            var e = doc.GetElement(r.ElementId);

            var obj
                = e.GetGeometryObjectFromReference(r);

            switch (obj)
            {
                case PlanarFace planarFace:
                    // Handle planar face case ...
                    break;
                case CylindricalFace cylindricalFace:
                    // Handle cylindrical face case ...
                    break;
            }

            Debug.Assert(
                ElementReferenceType.REFERENCE_TYPE_SURFACE
                == r.ElementReferenceType,
                "expected PickObject with ObjectType.Face to "
                + "return a surface reference");

            var face = obj as Face;
            var q = r.UVPoint;
            var p = r.GlobalPoint;

#if DEBUG
            var ir = face.Project(p);
            var q2 = ir.UVPoint;
            Debug.Assert(q.IsAlmostEqualTo(q2),
                "expected same UV point");
#endif // DEBUG

            var t = face.ComputeDerivatives(q);
            var v = t.BasisX;

            return doc.Create.NewFamilyInstance(r, p, v, symbol);
        }
    }
}
