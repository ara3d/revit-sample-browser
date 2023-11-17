// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.Selections.CS
{
    /// <summary>
    ///     A enum class for specific selection type.
    /// </summary>
    public enum SelectionType
    {
        /// <summary>
        ///     type for select element.
        /// </summary>
        Element,

        /// <summary>
        ///     type for select face.
        /// </summary>
        Face,

        /// <summary>
        ///     type for select edge.
        /// </summary>
        Edge,

        /// <summary>
        ///     type for select point.
        /// </summary>
        Point
    }

    /// <summary>
    ///     A class for object selection and storage.
    /// </summary>
    public class SelectionManager
    {
        /// <summary>
        ///     store the application
        /// </summary>
        private readonly UIApplication m_application;

        /// <summary>
        ///     To store a reference to the commandData.
        /// </summary>
        private readonly ExternalCommandData m_commandData;

        /// <summary>
        ///     For basic creation.
        /// </summary>
        private ItemFactoryBase m_creationBase;

        /// <summary>
        ///     store the document
        /// </summary>
        private readonly UIDocument m_document;

        /// <summary>
        ///     The picked point of element.
        /// </summary>
        private XYZ m_elemPickedPoint;

        private XYZ m_selectedPoint;

        /// <summary>
        ///     constructor of SelectionManager
        /// </summary>
        /// <param name="commandData"></param>
        public SelectionManager(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_application = m_commandData.Application;
            m_document = m_application.ActiveUIDocument;

            if (m_document.Document.IsFamilyDocument)
                m_creationBase = m_document.Document.FamilyCreate;
            else
                m_creationBase = m_document.Document.Create;
        }

        /// <summary>
        ///     For specific selection type.
        /// </summary>
        public SelectionType SelectionType { get; set; } = SelectionType.Element;

        /// <summary>
        ///     Store the selected element.
        /// </summary>
        public Element SelectedElement { get; set; }

        /// <summary>
        ///     Store the selected point.
        ///     When the point is picked, move the element to the point.
        /// </summary>
        public XYZ SelectedPoint
        {
            get => m_selectedPoint;
            set
            {
                m_selectedPoint = value;
                if (SelectedElement != null && m_selectedPoint != null) MoveElement(SelectedElement, m_selectedPoint);
            }
        }

        /// <summary>
        ///     Select objects according to the selection type.
        /// </summary>
        public void SelectObjects()
        {
            switch (SelectionType)
            {
                case SelectionType.Element:
                    PickElement(); // pick element
                    break;
                case SelectionType.Face:
                    break;
                case SelectionType.Edge:
                    break;
                case SelectionType.Point:
                    PickPoint(); // pick point
                    break;
            }
        }

        /// <summary>
        ///     Pick the element from UI.
        /// </summary>
        public void PickElement()
        {
            try
            {
                // Pick an element.
                var eRef = m_document.Selection.PickObject(ObjectType.Element, "Please pick an element.");
                if (eRef != null && eRef.ElementId != ElementId.InvalidElementId)
                {
                    SelectedElement = m_document.Document.GetElement(eRef);
                    m_elemPickedPoint = eRef.GlobalPoint;
                }
            }
            catch (OperationCanceledException)
            {
                // Element selection cancelled.
                SelectedElement = null;
            }
        }

        /// <summary>
        ///     Pick the point from UI.
        /// </summary>
        public void PickPoint()
        {
            try
            {
                // Pick a point.
                var targetPoint = m_document.Selection.PickPoint("Please pick a point.");
                SelectedPoint = targetPoint;
            }
            catch (OperationCanceledException)
            {
                // Point selection cancelled.
                SelectedPoint = null;
            }
        }

        /// <summary>
        ///     Move an element to the point.
        /// </summary>
        /// <param name="elem">The element to be moved.</param>
        /// <param name="targetPoint">The location element to be moved.</param>
        public void MoveElement(Element elem, XYZ targetPoint)
        {
            var vecToMove = targetPoint - m_elemPickedPoint;
            m_elemPickedPoint = targetPoint;
            ElementTransformUtils.MoveElement(m_document.Document, elem.Id, vecToMove);
        }
    }
}
