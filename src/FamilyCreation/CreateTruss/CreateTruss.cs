// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.CreateTruss.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private static Application _application;
        private static Document _document;
        private Autodesk.Revit.Creation.Application m_appCreator;
        private FamilyItemFactory m_familyCreator;

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            try
            {
                _application = revit.Application.Application;
                _document = revit.Application.ActiveUIDocument.Document;

                // Truss creation requires an open truss family template.
                if (!_document.IsFamilyDocument
                    || _document.OwnerFamily.FamilyCategory.BuiltInCategory != BuiltInCategory.OST_Truss)
                {
                    message = "Cannot execute truss creation in non-truss family document";
                    return Result.Failed;
                }

                m_appCreator = _application.Create;
                m_familyCreator = _document.FamilyCreate;

                var newTran = new Transaction(_document);
                newTran.Start("NewTrussCurve");

                MakeNewTruss();

                newTran.Commit();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Builds a mono truss aligned to the template reference planes (Top, Bottom, Left, Right, Center) and Level 1 view.
        /// </summary>
        private void MakeNewTruss()
        {
            var webAngle = 35.0;
            var webAngleRadians = (180 - webAngle) * Math.PI / 180.0;
            var angleDirection = new XYZ(Math.Cos(webAngleRadians), Math.Sin(webAngleRadians), 0);

            Autodesk.Revit.DB.ReferencePlane top = null, bottom = null, left = null, right = null, center = null;
            View level1 = null;
            var elements = new List<Element>();
            var refPlaneFilter = new ElementClassFilter(typeof(Autodesk.Revit.DB.ReferencePlane));
            var viewFilter = new ElementClassFilter(typeof(View));
            var filter = new LogicalOrFilter(refPlaneFilter, viewFilter);
            var collector = new FilteredElementCollector(_document);
            elements.AddRange(collector.WherePasses(filter).ToElements());
            foreach (var e in elements)
            {
                // View templates are invalid for truss sketching.
                if (e is View view && view.IsTemplate)
                    continue;
                switch (e.Name)
                {
                    case "Top":
                        top = e as Autodesk.Revit.DB.ReferencePlane;
                        break;
                    case "Bottom":
                        bottom = e as Autodesk.Revit.DB.ReferencePlane;
                        break;
                    case "Right":
                        right = e as Autodesk.Revit.DB.ReferencePlane;
                        break;
                    case "Left":
                        left = e as Autodesk.Revit.DB.ReferencePlane;
                        break;
                    case "Center":
                        center = e as Autodesk.Revit.DB.ReferencePlane;
                        break;
                    case "Level 1":
                        level1 = e as View;
                        break;
                }
            }

            if (top == null || bottom == null || left == null
                || right == null || center == null || level1 == null)
                throw new InvalidOperationException("Could not find prerequisite named reference plane or named view.");

            var sPlane = level1.SketchPlane;

            var bottomLine = GetReferencePlaneLine(bottom);
            var leftLine = GetReferencePlaneLine(left);
            var rightLine = GetReferencePlaneLine(right);
            var topLine = GetReferencePlaneLine(top);
            var centerLine = GetReferencePlaneLine(center);

            var bottomLeft = GetIntersection(bottomLine, leftLine);
            var bottomRight = GetIntersection(bottomLine, rightLine);
            var bottomChord = MakeTrussCurve(bottomLeft, bottomRight, sPlane, TrussCurveType.BottomChord);
            if (null != bottomChord)
            {
                var geometryCurve = bottomChord.GeometryCurve;
                m_familyCreator.NewAlignment(level1, bottom.GetReference(), geometryCurve.Reference);
            }

            var topRight = GetIntersection(topLine, rightLine);
            var rightWeb = MakeTrussCurve(bottomRight, topRight, sPlane, TrussCurveType.Web);
            if (null != rightWeb)
            {
                var geometryCurve = rightWeb.GeometryCurve;
                m_familyCreator.NewAlignment(level1, right.GetReference(), geometryCurve.Reference);
            }

            var topChord = MakeTrussCurve(bottomLeft, topRight, sPlane, TrussCurveType.TopChord);
            if (null != topChord)
            {
                var geometryCurve = topChord.GeometryCurve;
                m_familyCreator.NewAlignment(level1, geometryCurve.GetEndPointReference(0), left.GetReference());
                m_familyCreator.NewAlignment(level1, geometryCurve.GetEndPointReference(0), bottom.GetReference());
                m_familyCreator.NewAlignment(level1, geometryCurve.GetEndPointReference(1), top.GetReference());
                m_familyCreator.NewAlignment(level1, geometryCurve.GetEndPointReference(1), right.GetReference());
            }

            var bottomMidPoint = GetIntersection(bottomLine, centerLine);
            var webDirection = Line.CreateUnbound(bottomMidPoint, angleDirection);
            var endOfWeb = GetIntersection(topChord.GeometryCurve as Line, webDirection);
            var angledWeb = MakeTrussCurve(bottomMidPoint, endOfWeb, sPlane, TrussCurveType.Web);

            // Locked angular dimensions keep web angles stable when truss length/height change.
            var dimensionArc = Arc.Create(
                bottomMidPoint, angledWeb.GeometryCurve.Length / 2, webAngleRadians, Math.PI, XYZ.BasisX, XYZ.BasisY);
            var createdDim = m_familyCreator.NewAngularDimension(
                level1, dimensionArc, angledWeb.GeometryCurve.Reference, bottomChord.GeometryCurve.Reference);
            if (null != createdDim)
                createdDim.IsLocked = true;

            var bottomRight2 = GetIntersection(bottomLine, rightLine);
            webDirection = Line.CreateUnbound(bottomRight2, angleDirection);
            endOfWeb = GetIntersection(topChord.GeometryCurve as Line, webDirection);
            var angledWeb2 = MakeTrussCurve(bottomRight, endOfWeb, sPlane, TrussCurveType.Web);

            dimensionArc = Arc.Create(
                bottomRight, angledWeb2.GeometryCurve.Length / 2, webAngleRadians, Math.PI, XYZ.BasisX, XYZ.BasisY);
            createdDim = m_familyCreator.NewAngularDimension(
                level1, dimensionArc, angledWeb2.GeometryCurve.Reference, bottomChord.GeometryCurve.Reference);
            if (null != createdDim)
                createdDim.IsLocked = true;

            MakeTrussCurve(bottomMidPoint, endOfWeb, sPlane, TrussCurveType.Web);
        }

        private ModelCurve MakeTrussCurve(XYZ start, XYZ end, SketchPlane sketchPlane, TrussCurveType type)
        {
            var line = Line.CreateBound(start, end);
            var trussCurve = m_familyCreator.NewModelCurve(line, sketchPlane);
            trussCurve.TrussCurveType = type;
            _document.Regenerate();

            return trussCurve;
        }

        private Line GetReferencePlaneLine(Autodesk.Revit.DB.ReferencePlane plane)
        {
            // Flatten reference-plane geometry to Z=0 so intersection math matches sketch placement.
            var origin = new XYZ(
                plane.BubbleEnd.X,
                plane.BubbleEnd.Y,
                0.0);

            var line = Line.CreateUnbound(origin, plane.Direction);

            return line;
        }

        private XYZ GetIntersection(Line line1, Line line2)
        {
            IntersectionResultArray results;
            var result = line1.Intersect(line2, out results);

            if (result != SetComparisonResult.Overlap)
                throw new InvalidOperationException("Input lines did not intersect.");

            if (results == null || results.Size != 1)
                throw new InvalidOperationException("Could not extract intersection point for lines.");

            var iResult = results.get_Item(0);
            var intersectionPoint = iResult.XYZPoint;

            return intersectionPoint;
        }
    }
}
