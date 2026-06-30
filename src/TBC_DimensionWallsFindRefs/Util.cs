using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_DimensionWallsFindRefs sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return a 3D view from the given document.
        /// </summary>
        internal static View3D Get3DView(Document doc)
        {
            var collector = new FilteredElementCollector(doc);

            collector.OfClass(typeof(View3D));

            foreach (View3D v in collector)
                if (v is {IsTemplate: false, Name: "{3D}"})
                    return v;
            return null;
        }

        /// <summary>
        ///     Return reference to ceiling face to place
        ///     lighting fixture above a given point.
        /// </summary>
        internal static Reference GetCeilingReferenceAbove(
            View3D view,
            XYZ p)
        {
            var filter = new ElementClassFilter(
                typeof(Ceiling));

            var refIntersector
                = new ReferenceIntersector(filter,
                    FindReferenceTarget.Face, view);

            refIntersector.FindReferencesInRevitLinks = true;

            var rwc = refIntersector.FindNearest(
                p, XYZ.BasisZ);

            var r = null == rwc
                ? null
                : rwc.GetReference();

            if (null == r) System.Windows.MessageBox.Show("no intersecting geometry");
            return r;
        }

        internal static void TestGetCeilingReferenceAbove(Document doc)
        {
            var view = doc.GetElement(new ElementId((Int64)147335)) as View3D;
            var space = doc.GetElement(new ElementId((Int64) 151759)) as Space;
            var center = ((LocationPoint) space.Location).Point;

            var r = GetCeilingReferenceAbove(view, center);

            XYZ startPoint = null;
            FamilySymbol sym = null;

            doc.Create.NewFamilyInstance(r, startPoint, XYZ.BasisY, sym);
        }
    }
}
