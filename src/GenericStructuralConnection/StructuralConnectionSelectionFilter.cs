// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.GenericStructuralConnection.CS
{
    public class StructuralConnectionSelectionFilter : ISelectionFilter
    {
        private readonly LogicalOrFilter m_filter;

        public StructuralConnectionSelectionFilter(LogicalOrFilter elemTypesAllowed)
        {
            m_filter = elemTypesAllowed;
        }

        public bool AllowElement(Element element)
        {
            return m_filter.PassesFilter(element);
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }
}
