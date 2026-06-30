// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.AvoidObstruction.CS
{
    public class Detector
    {
        private readonly Document m_rvtDoc;

        private readonly View3D m_view3d;

        public Detector(Document rvtDoc)
        {
            m_rvtDoc = rvtDoc;
            m_view3d = m_rvtDoc.GetElements<View3D>().FirstOrDefault(v => !v.IsTemplate);
        }

        public List<ReferenceWithContext> Obstructions(XYZ origin, XYZ dir)
        {
            var result = new List<ReferenceWithContext>();
            var referenceIntersector = new ReferenceIntersector(m_view3d)
            {
                TargetType = FindReferenceTarget.Face
            };
            var obstructionsOnUnboundLine = referenceIntersector.Find(origin, dir);
            foreach (var gRef in obstructionsOnUnboundLine)
            {
                if (!SampleBrowserUtils.InReferenceArray(result, gRef))
                    result.Add(gRef);
            }

            result.Sort(SampleBrowserUtils.CompareReferencesWithContext);
            return result;
        }

        public List<ReferenceWithContext> Obstructions(Line boundLine)
        {
            var result = new List<ReferenceWithContext>();
            var startPt = boundLine.GetEndPoint(0);
            var endPt = boundLine.GetEndPoint(1);
            var dir = (endPt - startPt).Normalize();
            var referenceIntersector = new ReferenceIntersector(m_view3d)
            {
                TargetType = FindReferenceTarget.Face
            };
            var obstructionsOnUnboundLine = referenceIntersector.Find(startPt, dir);
            foreach (var gRefWithContext in obstructionsOnUnboundLine)
            {
                var gRef = gRefWithContext.GetReference();
                // Judge whether the point is in the bound line or not, if the distance between the point and line
                // is Zero, then the point is in the bound line.
                if (boundLine.Distance(gRef.GlobalPoint) < 1e-9)
                    if (!SampleBrowserUtils.InReferenceArray(result, gRefWithContext))
                        result.Add(gRefWithContext);
            }

            result.Sort(SampleBrowserUtils.CompareReferencesWithContext);
            return result;
        }
    }
}
