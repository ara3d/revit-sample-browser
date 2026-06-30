// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Views;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Ara3D.RevitSampleBrowser.CreateViewSection.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private const double Precision = 0.0000000001;

        private const double Length = 10;
        private const double Height = 5;

        private BoundingBoxXYZ m_box;
        private Element m_currentComponent;

        private string m_errorInformation;

        private UIDocument m_project;
        private SelectType m_type;

        public Command()
        {
            m_type = SelectType.Invalid;
        }

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                m_project = commandData.Application.ActiveUIDocument;

                if (!GetSelectedElement())
                {
                    message = m_errorInformation;
                    return Result.Failed;
                }

                if (!GenerateBoundingBoxXyz())
                {
                    message = m_errorInformation;
                    return Result.Failed;
                }

                Transaction transaction = new(m_project.Document, "CreateSectionView");
                transaction.Start();
                //ViewSection section = m_project.Document.Create.NewViewSection(m_box);
                var detailViewId = ElementId.InvalidElementId;
                var elems = new FilteredElementCollector(m_project.Document).OfClass(typeof(ViewFamilyType))
                    .ToElements();
                foreach (var e in elems)
                {
                    if (e is ViewFamilyType v && v.ViewFamily == ViewFamily.Detail)
                    {
                        detailViewId = e.Id;
                        break;
                    }
                }

                var section = ViewSection.CreateDetail(m_project.Document, detailViewId, m_box);
                if (null == section)
                {
                    message = "Can't create the ViewSection.";
                    return Result.Failed;
                }

                section.get_Parameter(BuiltInParameter.VIEW_DETAIL_LEVEL).Set(2);
                transaction.Commit();

                TaskDialog.Show("Revit", "Create view section succeeded.");
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }

        private bool GetSelectedElement()
        {
            ElementSet collection = new();
            foreach (var elementId in m_project.Selection.GetElementIds())
            {
                collection.Insert(m_project.Document.GetElement(elementId));
            }

            if (1 != collection.Size)
            {
                m_errorInformation =
                    "Please select only one element, such as a wall, a beam or a floor.";
                return false;
            }

            foreach (Element e in collection)
            {
                m_currentComponent = e;
            }

            switch (m_currentComponent)
            {
                case Wall _:
                    {
                        if (m_currentComponent.Location is not LocationCurve location)
                        {
                            m_errorInformation = "The selected wall should be linear.";
                            return false;
                        }

                        if (location.Curve is Line)
                        {
                            m_type = SelectType.Wall;
                            return true;
                        }

                        m_errorInformation = "The selected wall should be linear.";
                        return false;
                    }
                case FamilyInstance beam when StructuralType.Beam == beam.StructuralType:
                    m_type = SelectType.Beam;
                    return true;
                case Floor _:
                    m_type = SelectType.Floor;
                    return true;
                default:
                    m_errorInformation = "Please select an element, such as a wall, a beam or a floor.";
                    return false;
            }
        }

        private bool GenerateBoundingBoxXyz()
        {
            Transaction transaction = new(m_project.Document, "GenerateBoundingBox");
            transaction.Start();
            m_box = new BoundingBoxXYZ
            {
                Enabled = true
            };
            XYZ maxPoint = new(Length, Length, 0);
            XYZ minPoint = new(-Length, -Length, -Height);
            m_box.Max = maxPoint;
            m_box.Min = minPoint;

            // BoundingBoxXYZ.Transform sets view origin and basis (RightDirection, UpDirection, ViewDirection).
            var transform = GenerateTransform();
            if (null == transform) return false;
            m_box.Transform = transform;
            transaction.Commit();

            return true;
        }

        private Transform GenerateTransform()
        {
            switch (m_type)
            {
                case SelectType.Wall:
                    return GenerateWallTransform();
                case SelectType.Beam:
                    return GenerateBeamTransform();
                case SelectType.Floor:
                    return GenerateFloorTransform();
                default:
                    m_errorInformation = "The program should never go here.";
                    return null;
            }
        }

        private Transform GenerateWallTransform()
        {
            var wall = m_currentComponent as Wall;

            // Architecture and curtain walls lack analytical model lines; use Location instead.
            var location = wall.Location as LocationCurve;
            var locationLine = location.Curve as Line;
            var transform = Transform.Identity;

            var mPoint = ElementQuery.FindMidPoint(locationLine.GetEndPoint(0), locationLine.GetEndPoint(1));
            // Offset Z from location curve to wall vertical midpoint.
            XYZ midPoint = new(mPoint.X, mPoint.Y, mPoint.Z + GetWallMidOffsetFromLocation(wall));

            transform.Origin = midPoint;

            var basisZ = ElementQuery.FindDirection(locationLine.GetEndPoint(0), locationLine.GetEndPoint(1));
            var basisX = ElementQuery.FindRightDirection(basisZ);
            var basisY = ElementQuery.FindUpDirection(basisZ);

            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);
            return transform;
        }

        private Transform GenerateBeamTransform()
        {
            Transform transform = null;
            var instance = m_currentComponent as FamilyInstance;

            var startOffset = instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();
            var endOffset = instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble();
            if (startOffset - endOffset is < -Precision or > Precision)
            {
                m_errorInformation = "Please select a horizontal beam.";
                return transform;
            }

            if (instance.Location is not LocationCurve locationCurve)
            {
                m_errorInformation = "The program should never go here.";
                return transform;
            }

            var curve = locationCurve.Curve;
            if (null == curve)
            {
                m_errorInformation = "The program should never go here.";
                return transform;
            }

            transform = Transform.Identity;

            var startPoint = curve.GetEndPoint(0);
            var endPoint = curve.GetEndPoint(1);
            var midPoint = ElementQuery.FindMidPoint(startPoint, endPoint);
            transform.Origin = midPoint;

            var basisZ = ElementQuery.FindDirection(startPoint, endPoint);
            var basisX = ElementQuery.FindRightDirection(basisZ);
            var basisY = ElementQuery.FindUpDirection(basisZ);

            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);
            return transform;
        }

        private Transform GenerateFloorTransform()
        {
            Transform transform = null;
            var floor = m_currentComponent as Floor;

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
                    if (associatedElement is not null and AnalyticalPanel panel)
                        model = panel;
                }
            }

            if (null == model)
            {
                m_errorInformation = "Please select a structural floor.";
                return transform;
            }

            var curves = m_project.Document.Application.Create.NewCurveArray();
            IList<Curve> curveList = model.GetOuterContour().ToList();
            foreach (var curve in curveList)
            {
                curves.Append(curve);
            }

            if (null == curves || curves.IsEmpty)
            {
                m_errorInformation = "The program should never go here.";
                return transform;
            }

            transform = Transform.Identity;

            var midPoint = ElementQuery.FindMiddlePoint(curves);
            transform.Origin = midPoint;

            var basisZ = ViewHelper.FindFloorViewDirection(curves);
            var basisX = ElementQuery.FindRightDirection(basisZ);
            var basisY = ElementQuery.FindUpDirection(basisZ);

            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);
            return transform;
        }

        private double GetWallMidOffsetFromLocation(Wall wall)
        {
            var baseOffset = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();

            var height = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

            // Wall location Z is at the base constraint level.
            var midOffset = baseOffset + (height / 2);
            return midOffset;
        }

        private enum SelectType
        {
            Wall = 0,
            Beam = 1,
            Floor = 2,
            Invalid = -1
        }
    }

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
                Transaction transaction = new(doc, "CreateDraftingView");
                transaction.Start();

                ViewFamilyType viewFamilyType = null;
                FilteredElementCollector collector = new(doc);
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
