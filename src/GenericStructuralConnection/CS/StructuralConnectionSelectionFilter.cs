// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.GenericStructuralConnection.CS
{
    /// <summary>
    ///     Define filter for generic structural connection input elements.
    /// </summary>
    internal class StructuralConnectionSelectionFilter : ISelectionFilter
    {
        private readonly LogicalOrFilter m_filter;

        /// <summary>
        ///     Initialize the filter with the accepted element types.
        /// </summary>
        /// <param name="elemTypesAllowed">Logical filter containing accepted element types.</param>
        /// <returns></returns>
        public StructuralConnectionSelectionFilter(LogicalOrFilter elemTypesAllowed)
        {
            m_filter = elemTypesAllowed;
        }

        /// <summary>
        ///     Allows an element to be selected
        /// </summary>
        /// <param name="element">A candidate element in the selection operation.</param>
        /// <returns>Return true to allow the user to select this candidate element.</returns>
        public bool AllowElement(Element element)
        {
            return m_filter.PassesFilter(element);
        }

        /// <summary>
        ///     Allows a reference to be selected.
        /// </summary>
        /// <param name="refer"> A candidate reference in the selection operation.</param>
        /// <param name="point">The 3D position of the mouse on the candidate reference.</param>
        /// <returns>Return true to allow the user to select this candidate reference.</returns>
        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }
}
