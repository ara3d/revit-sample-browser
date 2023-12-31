// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.Selections.CS
{
    /// <summary>
    ///     This command allows to pick some elements and then delete from document.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class PickforDeletion : IExternalCommand
    {
        /// <summary>
        ///     store the application
        /// </summary>
        private UIApplication m_application;

        /// <summary>
        ///     store the document
        /// </summary>
        private UIDocument m_document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_application = commandData.Application;
            m_document = m_application.ActiveUIDocument;
            var trans = new Transaction(m_document.Document, "PickforDeletion");
            trans.Start();
            try
            {
                // Select elements. Click "Finish" or "Cancel" buttons on the dialog bar to complete the selection operation.
                var elemDeleteList = new List<ElementId>();
                var eRefList = m_document.Selection.PickObjects(ObjectType.Element,
                    "Please pick some element to delete. ESC for Cancel.");
                foreach (var eRef in eRefList)
                {
                    if (eRef != null && eRef.ElementId != ElementId.InvalidElementId)
                        elemDeleteList.Add(eRef.ElementId);
                }

                // Delete elements
                m_document.Document.Delete(elemDeleteList);
                trans.Commit();
                return Result.Succeeded;
            }
            catch (OperationCanceledException)
            {
                // Selection Cancelled.
                trans.RollBack();
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                // If any error, give error information and return failed
                message = ex.Message;
                trans.RollBack();
                return Result.Failed;
            }
        }
    }

    /// <summary>
    ///     This command allows to pick a point on wall face, and then place a window with Fixed 36" x 48" type to the point.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PlaceAtPointOnWallFace : IExternalCommand
    {
        /// <summary>
        ///     store the application
        /// </summary>
        private UIApplication m_application;

        /// <summary>
        ///     store the document
        /// </summary>
        private UIDocument m_document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                m_application = commandData.Application;
                m_document = m_application.ActiveUIDocument;

                // Pick a point on wall face.
                var pickedRefer = PickPointOnWallFace();
                if (pickedRefer != null)
                {
                    var trans = new Transaction(m_document.Document, "PlaceAtPointOnWallFace");
                    trans.Start();
                    // Place the 36" x 48" window at the reference.
                    PlaceWindowAtReference(pickedRefer);
                    trans.Commit();
                }
                else
                {
                    return Result.Cancelled;
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                // If any error, give error information and return failed
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Pick a point on wall face.
        /// </summary>
        /// <returns>The point reference picked on wall face. Null for selection cancel.</returns>
        protected Reference PickPointOnWallFace()
        {
            try
            {
                return m_document.Selection.PickObject(ObjectType.PointOnElement,
                    new WallFaceFilter(m_document.Document), "Please pick a point on Wall face.");
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        /// <summary>
        ///     // Place the 36" x 48" window at the reference.
        /// </summary>
        /// <param name="eRef">The point reference picked from wall face.</param>
        protected void PlaceWindowAtReference(Reference eRef)
        {
            // Find the window type 36" x 48".
            var windowType = FindFamilySymbol("36\" x 48\"");
            if (windowType != null)
                // Create the window.
                m_document.Document.Create.NewFamilyInstance(eRef.GlobalPoint, windowType,
                    m_document.Document.GetElement(eRef), StructuralType.NonStructural);
        }

        /// <summary>
        ///     Finding a Family Symbol with symbol name.
        /// </summary>
        /// <param name="symbolName">The name of FamilySymbol to be found.</param>
        /// <returns>The specific FamilySymbol.</returns>
        public FamilySymbol FindFamilySymbol(string symbolName)
        {
            var elemCollector = new FilteredElementCollector(m_document.Document);
            elemCollector.WhereElementIsElementType();
            var query = from element in elemCollector where element.Name == symbolName select element;
            var elemType = query.Single();
            return elemType as FamilySymbol;
        }
    }

    /// <summary>
    ///     This command allows to pick a face and set the work plane on it, then pick a point on the work plane as center to
    ///     create a model circle.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PlaceAtPickedFaceWorkplane : IExternalCommand
    {
        /// <summary>
        ///     store the application.
        /// </summary>
        private UIApplication m_application;

        /// <summary>
        ///     For basic creation.
        /// </summary>
        private ItemFactoryBase m_creationBase;

        /// <summary>
        ///     store the document
        /// </summary>
        private UIDocument m_document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                m_application = commandData.Application;
                m_document = m_application.ActiveUIDocument;
                if (m_document.Document.IsFamilyDocument)
                    m_creationBase = m_document.Document.FamilyCreate;
                else
                    m_creationBase = m_document.Document.Create;

                //Pick a face from UI, create a new sketch plane via the face and set it to the current view.
                var faceRef = m_document.Selection.PickObject(ObjectType.Face,
                    new PlanarFaceFilter(m_document.Document),
                    "Please pick a planar face to set the work plane. ESC for cancel.");
                var geoObject = m_document.Document.GetElement(faceRef).GetGeometryObjectFromReference(faceRef);
                var planarFace = geoObject as PlanarFace;
                var faceSketchPlane = CreateSketchPlane(planarFace.FaceNormal, planarFace.Origin);
                if (faceSketchPlane != null)
                {
                    var changeSketchPlane = new Transaction(m_document.Document, "Change Sketch Plane.");
                    changeSketchPlane.Start();
                    m_document.Document.ActiveView.SketchPlane = faceSketchPlane;
                    m_document.Document.ActiveView.ShowActiveWorkPlane();
                    changeSketchPlane.Commit();
                }

                // Pick point from current work plane with snaps.
                var snapType = ObjectSnapTypes.Centers | ObjectSnapTypes.Endpoints | ObjectSnapTypes.Intersections
                               | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Nearest | ObjectSnapTypes.WorkPlaneGrid;
                var point = m_document.Selection.PickPoint(snapType, "Please pick a point to place component.");

                // Create a model curve by a circle with picked point as center.
                var createModelCurve = new Transaction(m_document.Document, "Create a circle.");
                createModelCurve.Start();
                Curve circle = Arc.Create(point, 5, 0, Math.PI * 2, faceSketchPlane.GetPlane().XVec,
                    faceSketchPlane.GetPlane().YVec);
                m_creationBase.NewModelCurve(circle, faceSketchPlane);
                createModelCurve.Commit();

                return Result.Succeeded;
            }
            catch (OperationCanceledException)
            {
                // Selection Cancelled. For picking face and picking point.
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                // If any error, give error information and return failed
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Create a sketch plane via given normal and origin points.
        /// </summary>
        /// <param name="normal">The vector for normal of sketch plane.</param>
        /// <param name="origin">The vector for origin of sketch plane.</param>
        /// <returns>The new sketch plane created by specific normal and origin.</returns>
        public SketchPlane CreateSketchPlane(XYZ normal, XYZ origin)
        {
            // First create a Geometry.Plane which need in NewSketchPlane() method
            var geometryPlane = Plane.CreateByNormalAndOrigin(normal, origin);

            // Then create a sketch plane using the Geometry Plane
            var createSketchPlane = new Transaction(m_document.Document, "Create a sketch plane.");
            createSketchPlane.Start();
            var plane = SketchPlane.Create(m_document.Document, geometryPlane);
            createSketchPlane.Commit();

            return plane;
        }
    }

    /// <summary>
    ///     This command allows to pick an element and a point from dialog. After picking the point, the element will be moved
    ///     to the picked point.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SelectionDialog : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var manager = new SelectionManager(commandData);
                // Create a form to select objects.
                var result = DialogResult.None;
                while (result == DialogResult.None || result == DialogResult.Retry)
                {
                    // Picking Objects.
                    if (result == DialogResult.Retry) manager.SelectObjects();
                    // Show the dialog.
                    using (var selectionForm = new SelectionForm(manager))
                    {
                        result = selectionForm.ShowDialog();
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                // If any error, give error information and return failed
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
