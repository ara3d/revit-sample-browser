// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.AvoidObstruction.CS
{
    /// <summary>
    ///     This class is used to detect the obstructions of a Line or a ray.
    /// </summary>
    internal class Detector
    {
        /// <summary>
        ///     Revit Document.
        /// </summary>
        private readonly Document m_rvtDoc;

        /// <summary>
        ///     Revit 3D view.
        /// </summary>
        private readonly View3D m_view3d;

        /// <summary>
        ///     Constructor, initialize all the fields.
        /// </summary>
        /// <param name="rvtDoc">Revit Document</param>
        public Detector(Document rvtDoc)
        {
            m_rvtDoc = rvtDoc;
            var collector = new FilteredElementCollector(m_rvtDoc);
            var iter = collector.OfClass(typeof(View3D)).GetElementIterator();
            iter.Reset();
            while (iter.MoveNext())
            {
                m_view3d = iter.Current as View3D;
                if (null != m_view3d && !m_view3d.IsTemplate)
                    break;
            }
        }

        /// <summary>
        ///     Return all the obstructions which intersect with a ray given by an origin and a direction.
        /// </summary>
        /// <param name="origin">Ray's origin</param>
        /// <param name="dir">Ray's direction</param>
        /// <returns>Obstructions intersected with the given ray</returns>
        public List<ReferenceWithContext> Obstructions(XYZ origin, XYZ dir)
        {
            var result = new List<ReferenceWithContext>();
            var referenceIntersector = new ReferenceIntersector(m_view3d);
            referenceIntersector.TargetType = FindReferenceTarget.Face;
            var obstructionsOnUnboundLine = referenceIntersector.Find(origin, dir);
            foreach (var gRef in obstructionsOnUnboundLine)
                if (!InArray(result, gRef))
                    result.Add(gRef);

            result.Sort(CompareReferencesWithContext);
            return result;
        }

        /// <summary>
        ///     Return all the obstructions which intersect with a bound line.
        /// </summary>
        /// <param name="boundLine">Bound line</param>
        /// <returns>Obstructions intersected with the bound line</returns>
        public List<ReferenceWithContext> Obstructions(Line boundLine)
        {
            var result = new List<ReferenceWithContext>();
            var startPt = boundLine.GetEndPoint(0);
            var endPt = boundLine.GetEndPoint(1);
            var dir = (endPt - startPt).Normalize();
            var referenceIntersector = new ReferenceIntersector(m_view3d);
            referenceIntersector.TargetType = FindReferenceTarget.Face;
            var obstructionsOnUnboundLine = referenceIntersector.Find(startPt, dir);
            foreach (var gRefWithContext in obstructionsOnUnboundLine)
            {
                var gRef = gRefWithContext.GetReference();
                // Judge whether the point is in the bound line or not, if the distance between the point and line
                // is Zero, then the point is in the bound line.
                if (boundLine.Distance(gRef.GlobalPoint) < 1e-9)
                    if (!InArray(result, gRefWithContext))
                        result.Add(gRefWithContext);
            }

            result.Sort(CompareReferencesWithContext);
            return result;
        }

        /// <summary>
        ///     Judge whether a given Reference is in a Reference list.
        ///     Give two References, if their Proximity and Element Id is equal,
        ///     we say the two reference is equal.
        /// </summary>
        /// <param name="arr">Reference Array</param>
        /// <param name="entry">Reference</param>
        /// <returns>True of false</returns>
        private bool InArray(List<ReferenceWithContext> arr, ReferenceWithContext entry)
        {
            foreach (var tmp in arr)
                if (Math.Abs(tmp.Proximity - entry.Proximity) < 1e-9 &&
                    tmp.GetReference().ElementId == entry.GetReference().ElementId)
                    return true;
            return false;
        }

        /// <summary>
        ///     Used to compare two references, just compare their ProximityParameter.
        /// </summary>
        /// <param name="a">First Reference to compare</param>
        /// <param name="b">Second Reference to compare</param>
        /// <returns>-1, 0, or 1</returns>
        private int CompareReferencesWithContext(ReferenceWithContext a, ReferenceWithContext b)
        {
            if (a.Proximity > b.Proximity) return 1;

            if (a.Proximity < b.Proximity) return -1;

            return 0;
        }
    }
}
