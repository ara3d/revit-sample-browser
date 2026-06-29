// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.Selections.CS
{
    /// <summary>
    ///     A default filter.
    ///     All objects are allowed to be picked.
    /// </summary>
    public class DefaultElementsFilter : ISelectionFilter
    {
        /// <summary>
        ///     Allow all the element to be selected
        /// </summary>
        /// <param name="element">A candidate element in selection operation.</param>
        /// <returns>Return true to allow the user to select this candidate element.</returns>
        public bool AllowElement(Element element)
        {
            return true;
        }

        /// <summary>
        ///     Allow all the reference to be selected
        /// </summary>
        /// <param name="refer">A candidate reference in selection operation.</param>
        /// <param name="point">The 3D position of the mouse on the candidate reference.</param>
        /// <returns>Return true to allow the user to select this candidate reference.</returns>
        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }

    /// <summary>
    ///     A Filter for Wall Face.
    ///     Only wall faces are allowed to be picked.
    /// </summary>
    public class WallFaceFilter : ISelectionFilter
    {
        // Revit document.
        private readonly Document m_doc;

        /// <summary>
        ///     Constructor the filter and initialize the document.
        /// </summary>
        /// <param name="doc">The document.</param>
        public WallFaceFilter(Document doc)
        {
            m_doc = doc;
        }

        /// <summary>
        ///     Allow wall to be selected
        /// </summary>
        /// <param name="element">A candidate element in selection operation.</param>
        /// <returns>Return true for wall. Return false for non wall element.</returns>
        public bool AllowElement(Element element)
        {
            return element is Wall;
        }

        /// <summary>
        ///     Allow face reference to be selected
        /// </summary>
        /// <param name="refer">A candidate reference in selection operation.</param>
        /// <param name="point">The 3D position of the mouse on the candidate reference.</param>
        /// <returns>Return true for face reference. Return false for non face reference.</returns>
        public bool AllowReference(Reference refer, XYZ point)
        {
            var geoObject = m_doc.GetElement(refer).GetGeometryObjectFromReference(refer);
            return geoObject != null && geoObject is Face;
        }
    }

    /// <summary>
    ///     A Filter for planar face.
    ///     Only planar faces are allowed to be picked.
    /// </summary>
    public class PlanarFaceFilter : ISelectionFilter
    {
        // Revit document.
        private readonly Document m_doc;

        /// <summary>
        ///     Constructor the filter and initialize the document.
        /// </summary>
        /// <param name="doc">The document.</param>
        public PlanarFaceFilter(Document doc)
        {
            m_doc = doc;
        }

        /// <summary>
        ///     Allow all the element to be selected
        /// </summary>
        /// <param name="element">A candidate element in selection operation.</param>
        /// <returns>Return true to allow the user to select this candidate element.</returns>
        public bool AllowElement(Element element)
        {
            return true;
        }

        /// <summary>
        ///     Allow planar face reference to be selected
        /// </summary>
        /// <param name="refer">A candidate reference in selection operation.</param>
        /// <param name="point">The 3D position of the mouse on the candidate reference.</param>
        /// <returns>Return true for planar face reference. Return false for non planar face reference.</returns>
        public bool AllowReference(Reference refer, XYZ point)
        {
            var geoObject = m_doc.GetElement(refer).GetGeometryObjectFromReference(refer);
            return geoObject != null && geoObject is PlanarFace;
        }
    }
}
