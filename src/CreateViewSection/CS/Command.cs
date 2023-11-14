// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CreateViewSection.CS
{
    /// <summary>
    ///     The main class which given a linear element, such as a wall, floor or beam,
    ///     generates a section view across the mid point of the element.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private const double PRECISION = 0.0000000001; // Define a precision of double data

        // 0 - wall, 1 - beam, 2 - floor, -1 - invalid
        private const double LENGTH = 10; // Define half length and width of BoudingBoxXYZ
        private const double HEIGHT = 5; // Define height of the BoudingBoxXYZ

        private BoundingBoxXYZ m_box; // Store the BoundingBoxXYZ reference used in creation
        private Element m_currentComponent; // Store the selected element

        private string m_errorInformation; // Store the error information

        // Private Members
        private UIDocument m_project; // Store the current document in revit   
        private SelectType m_type; // Indicate the type of the selected element.

        // Methods
        /// <summary>
        ///     Default constructor of Command
        /// </summary>
        public Command()
        {
            m_type = SelectType.INVALID;
        }

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                m_project = commandData.Application.ActiveUIDocument;

                // Get the selected element and store it to data member.
                if (!GetSelectedElement())
                {
                    message = m_errorInformation;
                    return Result.Failed;
                }

                // Create a BoundingBoxXYZ instance which used in NewViewSection() method
                if (!GenerateBoundingBoxXYZ())
                {
                    message = m_errorInformation;
                    return Result.Failed;
                }

                // Create a section view. 
                var transaction = new Transaction(m_project.Document, "CreateSectionView");
                transaction.Start();
                //ViewSection section = m_project.Document.Create.NewViewSection(m_box);
                var DetailViewId = ElementId.InvalidElementId;
                var elems = new FilteredElementCollector(m_project.Document).OfClass(typeof(ViewFamilyType))
                    .ToElements();
                foreach (var e in elems)
                {
                    var v = e as ViewFamilyType;

                    if (v != null && v.ViewFamily == ViewFamily.Detail)
                    {
                        DetailViewId = e.Id;
                        break;
                    }
                }

                var section = ViewSection.CreateDetail(m_project.Document, DetailViewId, m_box);
                if (null == section)
                {
                    message = "Can't create the ViewSection.";
                    return Result.Failed;
                }

                // Modify some parameters to make it look better.
                section.get_Parameter(BuiltInParameter.VIEW_DETAIL_LEVEL).Set(2);
                transaction.Commit();

                // If everything goes right, give successful information and return succeeded.
                TaskDialog.Show("Revit", "Create view section succeeded.");
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }


        /// <summary>
        ///     Get the selected element, and check whether it is a wall, a floor or a beam.
        /// </summary>
        /// <returns>true if the process succeed; otherwise, false.</returns>
        private bool GetSelectedElement()
        {
            // First get the selection, and make sure only one element in it.
            var collection = new ElementSet();
            foreach (var elementId in m_project.Selection.GetElementIds())
                collection.Insert(m_project.Document.GetElement(elementId));
            if (1 != collection.Size)
            {
                m_errorInformation =
                    "Please select only one element, such as a wall, a beam or a floor.";
                return false;
            }

            // Get the selected element.
            foreach (Element e in collection) m_currentComponent = e;

            // Make sure the element to be a wall, beam or a floor.
            if (m_currentComponent is Wall)
            {
                // Check whether the wall is a linear wall
                var location = m_currentComponent.Location as LocationCurve;
                if (null == location)
                {
                    m_errorInformation = "The selected wall should be linear.";
                    return false;
                }

                if (location.Curve is Line)
                {
                    m_type = SelectType.WALL; // when the element is a linear wall
                    return true;
                }

                m_errorInformation = "The selected wall should be linear.";
                return false;
            }

            var beam = m_currentComponent as FamilyInstance;
            if (null != beam && StructuralType.Beam == beam.StructuralType)
            {
                m_type = SelectType.BEAM; // when the element is a beam
                return true;
            }

            if (m_currentComponent is Floor)
            {
                m_type = SelectType.FLOOR; // when the element is a floor.
                return true;
            }

            // If it is not a wall, a beam or a floor, give error information.
            m_errorInformation = "Please select an element, such as a wall, a beam or a floor.";
            return false;
        }


        /// <summary>
        ///     Generate a BoundingBoxXYZ instance which used in NewViewSection() method
        /// </summary>
        /// <returns>true if the instance can be created; otherwise, false.</returns>
        private bool GenerateBoundingBoxXYZ()
        {
            var transaction = new Transaction(m_project.Document, "GenerateBoundingBox");
            transaction.Start();
            // First new a BoundingBoxXYZ, and set the MAX and Min property.
            m_box = new BoundingBoxXYZ();
            m_box.Enabled = true;
            var maxPoint = new XYZ(LENGTH, LENGTH, 0);
            var minPoint = new XYZ(-LENGTH, -LENGTH, -HEIGHT);
            m_box.Max = maxPoint;
            m_box.Min = minPoint;

            // Set Transform property is the most important thing.
            // It define the Orgin and the directions(include RightDirection, 
            // UpDirection and ViewDirection) of the created view.
            var transform = GenerateTransform();
            if (null == transform) return false;
            m_box.Transform = transform;
            transaction.Commit();

            // If all went well, return true.
            return true;
        }


        /// <summary>
        ///     Generate a Transform instance which as Transform property of BoundingBoxXYZ
        /// </summary>
        /// <returns>the reference of Transform, return null if it can't be generated</returns>
        private Transform GenerateTransform()
        {
            // Because different element have different ways to create Transform.
            // So, this method just call corresponding method.
            if (SelectType.WALL == m_type) return GenerateWallTransform();

            if (SelectType.BEAM == m_type) return GenerateBeamTransform();

            if (SelectType.FLOOR == m_type)
            {
                return GenerateFloorTransform();
            }

            m_errorInformation = "The program should never go here.";
            return null;
        }


        /// <summary>
        ///     Generate a Transform instance which as Transform property of BoundingBoxXYZ,
        ///     when the user select a wall, this method will be called
        /// </summary>
        /// <returns>the reference of Transform, return null if it can't be generated</returns>
        private Transform GenerateWallTransform()
        {
            var wall = m_currentComponent as Wall;

            // Because the architecture wall and curtain wall don't have analytical Model lines.
            // So Use Location property of wall object is better choice.
            // First get the location line of the wall
            var location = wall.Location as LocationCurve;
            var locationLine = location.Curve as Line;
            var transform = Transform.Identity;

            // Second find the middle point of the wall and set it as Origin property.
            var mPoint = XYZMath.FindMidPoint(locationLine.GetEndPoint(0), locationLine.GetEndPoint(1));
            // midPoint is mid point of the wall location, but not the wall's.
            // The different is the elevation of the point. Then change it.

            var midPoint = new XYZ(mPoint.X, mPoint.Y, mPoint.Z + GetWallMidOffsetFromLocation(wall));

            transform.Origin = midPoint;

            // At last find out the directions of the created view, and set it as Basis property.
            var basisZ = XYZMath.FindDirection(locationLine.GetEndPoint(0), locationLine.GetEndPoint(1));
            var basisX = XYZMath.FindRightDirection(basisZ);
            var basisY = XYZMath.FindUpDirection(basisZ);

            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);
            return transform;
        }


        /// <summary>
        ///     Generate a Transform instance which as Transform property of BoundingBoxXYZ,
        ///     when the user select a beam, this method will be called
        /// </summary>
        /// <returns>the reference of Transform, return null if it can't be generated</returns>
        private Transform GenerateBeamTransform()
        {
            Transform transform = null;
            var instance = m_currentComponent as FamilyInstance;

            // First check whether the beam is horizontal.
            // In order to predigest the calculation, only allow it to be horizontal
            var startOffset = instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();
            var endOffset = instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble();
            if (-PRECISION > startOffset - endOffset || PRECISION < startOffset - endOffset)
            {
                m_errorInformation = "Please select a horizontal beam.";
                return transform;
            }

            if (!(instance.Location is LocationCurve))
            {
                m_errorInformation = "The program should never go here.";
                return transform;
            }

            var curve = (instance.Location as LocationCurve).Curve;
            if (null == curve)
            {
                m_errorInformation = "The program should never go here.";
                return transform;
            }

            // Now I am sure I can create a transform instance.
            transform = Transform.Identity;

            // Third find the middle point of the line and set it as Origin property.
            var startPoint = curve.GetEndPoint(0);
            var endPoint = curve.GetEndPoint(1);
            var midPoint = XYZMath.FindMidPoint(startPoint, endPoint);
            transform.Origin = midPoint;

            // At last find out the directions of the created view, and set it as Basis property.   
            var basisZ = XYZMath.FindDirection(startPoint, endPoint);
            var basisX = XYZMath.FindRightDirection(basisZ);
            var basisY = XYZMath.FindUpDirection(basisZ);

            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);
            return transform;
        }


        /// <summary>
        ///     Generate a Transform instance which as Transform property of BoundingBoxXYZ,
        ///     when the user select a floor, this method will be called
        /// </summary>
        /// <returns>the reference of Transform, return null if it can't be generated</returns>
        private Transform GenerateFloorTransform()
        {
            Transform transform = null;
            var floor = m_currentComponent as Floor;

            // First get the Analytical Model lines
            AnalyticalPanel model = null;
            var document = floor.Document;
            var assocManager =
                AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            if (assocManager != null)
            {
                var associatedElementId = assocManager.GetAssociatedElementId(floor.Id);
                if (associatedElementId != ElementId.InvalidElementId)
                {
                    var associatedElement = document.GetElement(associatedElementId);
                    if (associatedElement != null && associatedElement is AnalyticalPanel)
                        model = associatedElement as AnalyticalPanel;
                }
            }

            if (null == model)
            {
                m_errorInformation = "Please select a structural floor.";
                return transform;
            }

            var curves = m_project.Document.Application.Create.NewCurveArray();
            IList<Curve> curveList = model.GetOuterContour().ToList();
            foreach (var curve in curveList) curves.Append(curve);

            if (null == curves || curves.IsEmpty)
            {
                m_errorInformation = "The program should never go here.";
                return transform;
            }

            // Now I am sure I can create a transform instance.
            transform = Transform.Identity;

            // Third find the middle point of the floor and set it as Origin property.
            var midPoint = XYZMath.FindMiddlePoint(curves);
            transform.Origin = midPoint;

            // At last find out the directions of the created view, and set it as Basis property.
            var basisZ = XYZMath.FindFloorViewDirection(curves);
            var basisX = XYZMath.FindRightDirection(basisZ);
            var basisY = XYZMath.FindUpDirection(basisZ);

            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);
            return transform;
        }

        private double GetWallMidOffsetFromLocation(Wall wall)
        {
            // First get the "Base Offset" property.
            var baseOffset = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();

            // Second get the "Unconnected Height" property. 
            var height = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

            // Get the middle of of wall elevation from the wall location.
            // The elevation of wall location equals the elevation of "Base Constraint" level
            var midOffset = baseOffset + height / 2;
            return midOffset;
        }

        // Define a enum to indicate the selected element type
        private enum SelectType
        {
            WALL = 0,
            BEAM = 1,
            FLOOR = 2,
            INVALID = -1
        }
    }

    /// <summary>
    ///     Create a drafting view.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateDraftingView : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;
                var transaction = new Transaction(doc, "CreateDraftingView");
                transaction.Start();

                ViewFamilyType viewFamilyType = null;
                var collector = new FilteredElementCollector(doc);
                var viewFamilyTypes = collector.OfClass(typeof(ViewFamilyType)).ToElements();
                foreach (var e in viewFamilyTypes)
                {
                    var v = e as ViewFamilyType;
                    if (v.ViewFamily == ViewFamily.Drafting)
                    {
                        viewFamilyType = v;
                        break;
                    }
                }

                var drafting = ViewDrafting.Create(doc, viewFamilyType.Id);
                if (null == drafting)
                {
                    message = "Can't create the ViewDrafting.";
                    return Result.Failed;
                }

                transaction.Commit();
                TaskDialog.Show("Revit", "Create view drafting succeeded.");
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}
