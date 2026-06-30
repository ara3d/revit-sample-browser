// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;

using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.Common.Documents
{
    public class IsPaintedFaceSelectionFilter : ISelectionFilter
    {
        private Document m_selectedDocument;

        public bool AllowElement(Element element)
        {
            m_selectedDocument = element.Document;
            return true;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            if (m_selectedDocument == null)
                throw new Exception("AllowElement was never called for this reference...");

            var element = m_selectedDocument.GetElement(refer);
            var face = element.GetGeometryObjectFromReference(refer) as Face;
            return m_selectedDocument.IsPainted(element.Id, face);
        }
    }

    public class WallTypeComparer : IComparer<WallType>
    {
        private static readonly IComparer Comp = new CaseInsensitiveComparer();

        public int Compare(WallType x, WallType y)
        {
            return Comp.Compare(x.Name, y.Name);
        }
    }

    public class ViewComparer : IComparer<View>
    {
        private static readonly IComparer Comp = new CaseInsensitiveComparer();

        public int Compare(View x, View y)
        {
            return Comp.Compare($"{x.ViewType} : {x.Name}", $"{y.ViewType} : {y.Name}");
        }
    }

    public class SubRegionSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            return element is TopographySurface ts && ts.IsSiteSubRegion;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }

    public class TopographySurfaceSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            return element is TopographySurface ts && !ts.IsSiteSubRegion;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }

    public class TopographyEditFailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            return FailureProcessingResult.Continue;
        }
    }
}
