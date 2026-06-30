// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Parameters;
using Ara3D.RevitSampleBrowser.FreeFormElement.CS;
using Ara3D.RevitSampleBrowser.TagBeam.CS;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Document = Autodesk.Revit.DB.Document;
using RevitTransform = Autodesk.Revit.DB.Transform;
using RevitView = Autodesk.Revit.DB.View;
using SystemInvalidOperationException = System.InvalidOperationException;

namespace Ara3D.RevitSampleBrowser.Common.Views
{
    public static class ViewHelper
    {
        private const double ClickTolerance = 0.0001;

        public static XYZ FindWallViewDirection(CurveArray curveArray)
        {
            foreach (Curve curve in curveArray)
            {
                var startPoint = curve.GetEndPoint(0);
                var endPoint = curve.GetEndPoint(1);
                var distanceX = startPoint.X - endPoint.X;
                var distanceY = startPoint.Y - endPoint.Y;
                if (-XyzMath.Precision > distanceX || XyzMath.Precision < distanceX
                                           || -XyzMath.Precision > distanceY || XyzMath.Precision < distanceY)
                {
                    return ElementQuery.FindDirection(new XYZ(startPoint.X, startPoint.Y, 0),
                        new XYZ(endPoint.X, endPoint.Y, 0));
                }
            }

            return new XYZ();
        }

        public static XYZ FindFloorViewDirection(CurveArray curveArray)
        {
            var curve = curveArray.get_Item(0);
            return ElementQuery.FindDirection(curve.GetEndPoint(0), curve.GetEndPoint(1));
        }

        public static void HideAllInView(RevitView view)
        {
            var hideElemIds = new FilteredElementCollector(view.Document, view.Id).ToElementIds()
                .Where(id => view.Document.GetElement(id).CanBeHidden(view))
                .ToList();
            view.HideElements(hideElemIds);
        }

        public static bool IsSouthFacing(XYZ direction)
        {
            return Math.Abs(direction.AngleTo(-XYZ.BasisY)) < Math.PI / 4;
        }

        public static XYZ TransformByProjectLocation(Document document, XYZ direction)
        {
            var position = document.ActiveProjectLocation.GetProjectPosition(XYZ.Zero);
            var transform = RevitTransform.CreateRotation(XYZ.BasisZ, position.Angle);
            return transform.OfVector(direction);
        }

        public static bool IsExterior(ElementType wallType)
        {
            var wallFunction = wallType.get_Parameter(BuiltInParameter.FUNCTION_PARAM);
            return (WallFunction)wallFunction.AsInteger() == WallFunction.Exterior;
        }

        public static XYZ GetExteriorWallDirection(Wall wall)
        {
            var exteriorDirection = XYZ.BasisZ;
            if (wall.Location is not LocationCurve locationCurve)
                return exteriorDirection;

            var curve = locationCurve.Curve;
            var direction = curve is Line
                        ? curve.ComputeDerivatives(0, true).BasisX.Normalize()
                        : (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();

            exteriorDirection = XYZ.BasisZ.CrossProduct(direction);
            if (wall.Flipped)
                exteriorDirection = -exteriorDirection;
            return exteriorDirection;
        }

        public static XYZ GetWindowDirection(FamilyInstance window)
        {
            var geomElem = window.get_Geometry(new Options());
            var objects = geomElem.GetEnumerator();
            while (objects.MoveNext())
            {
                if (objects.Current is not GeometryInstance instance)
                    continue;

                var facingDirection = instance.Transform.BasisY;
                if ((window.FacingFlipped && !window.HandFlipped) || (!window.FacingFlipped && window.HandFlipped))
                    facingDirection = -facingDirection;
                return facingDirection;
            }

            return XYZ.BasisZ;
        }

        public static void DuplicateSchedules(Document fromDocument,
                    IEnumerable<ViewSchedule> views,
                    Document toDocument)
        {
            var ids = views.Cast<View>().Select(v => v.Id).ToList();
            ElementQuery.DuplicateElementsAcrossDocuments(fromDocument, ids, toDocument, false);
        }

        public static int DuplicateDraftingViews(Document fromDocument,
                    IEnumerable<ViewDrafting> views,
                    Document toDocument)
        {
            var numberOfDetailElements = 0;
            using (var tg = new TransactionGroup(toDocument,
                       "API - Duplication across documents with detailing"))
            {
                tg.Start();
                var ids = views.Cast<View>().Select(v => v.Id).ToList();
                var viewMap = ElementQuery.DuplicateElementsAcrossDocuments(fromDocument, ids, toDocument, true);
                foreach (var viewId in viewMap.Keys)
                {
                    var fromView = fromDocument.GetElement(viewId) as View;
                    var toView = toDocument.GetElement(viewMap[viewId]) as View;
                    numberOfDetailElements += DuplicateDetailingAcrossViews(fromView, toView);
                }

                tg.Assimilate();
            }

            return numberOfDetailElements;
        }

        private static int DuplicateDetailingAcrossViews(RevitView fromView, RevitView toView)
        {
            var collector = new FilteredElementCollector(fromView.Document, fromView.Id);
            collector.WherePasses(new ElementCategoryFilter(ElementId.InvalidElementId, true));
            var toCopy = collector.ToElementIds();
            if (toCopy.Count == 0)
                return 0;

            using var t2 = new Transaction(toView.Document, "Duplicate view detailing");
            t2.Start();
            var options = new CopyPasteOptions();
            options.SetDuplicateTypeNamesHandler(RevitToolkitCopyPaste.UseDestinationTypes);
            var copiedElements = ElementTransformUtils.CopyElements(fromView, toCopy, toView,
                        RevitTransform.Identity, options);
            var failureOptions = t2.GetFailureHandlingOptions();
            failureOptions.SetFailuresPreprocessor(new HidePasteDuplicateTypesPreprocessor());
            t2.Commit(failureOptions);
            return copiedElements.Count;
        }

        public static string PrintElementInfo(ElementId id, Document document)
        {
            var element = document.GetElement(id);
            var retval = $"{element.Id}, {element.Name}, {element.GetType().FullName}";
            Debug.WriteLine(retval);
            return retval;
        }

        public static string FindGenericModelTemplate(string familyPath)
        {
            var hardCodedResult = Path.Combine(
                Path.GetDirectoryName(typeof(CreateNegativeBlockCommand).Assembly.Location) ?? "",
                "Generic Model.rft");
            try
            {
                var result = Directory.EnumerateFiles(familyPath, "Generic Model.rft", SearchOption.AllDirectories)
                    .FirstOrDefault();
                if (!string.IsNullOrEmpty(result))
                    return result;

                result = Directory.EnumerateFiles(familyPath, "Metric Generic Model.rft", SearchOption.AllDirectories)
                    .FirstOrDefault();
                return !string.IsNullOrEmpty(result) ? result : hardCodedResult;
            }
            catch (Exception)
            {
                return hardCodedResult;
            }
        }

        public static PointF Translate(XYZ pointXyz, BoundingBoxXYZ boundingbox)
        {
            var centerX = (boundingbox.Min.X + boundingbox.Max.X) / 2;
            var centerY = (boundingbox.Min.Y + boundingbox.Max.Y) / 2;
            return new PointF((float)(pointXyz.X - centerX), -(float)(pointXyz.Y - centerY));
        }

        public static string ParseBracketedId(string text)
        {
            var leftBracket = text.IndexOf('[');
            var rightBracket = text.IndexOf(']');
            return text.Substring(leftBracket + 1, rightBracket - leftBracket - 1);
        }

        public static void AddScheduleToNewSheet(Document document, ViewSchedule schedule)
        {
            var collector = new FilteredElementCollector(document);
            collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            collector.WhereElementIsElementType();

            var t = new Transaction(document, "Create and populate sheet");
            t.Start();

            var titleBlockId = collector.FirstElementId();
            var newSheet = ViewSheet.Create(document, titleBlockId);
            newSheet.Name = $"Sheet for {schedule.Name}";

            document.Regenerate();

            var upperLeft = new XYZ();
            if (titleBlockId != ElementId.InvalidElementId)
            {
                collector = new FilteredElementCollector(document);
                collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
                collector.OwnedByView(newSheet.Id);
                var titleBlock = collector.FirstElement();
                var bbox = titleBlock.get_BoundingBox(newSheet);
                upperLeft = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);
                upperLeft += new XYZ(2.0 / 12.0, -2.0 / 12.0, 0);
            }

            ScheduleSheetInstance.Create(document, newSheet.Id, schedule.Id, upperLeft);
            t.Commit();
        }

        public static ICollection<ViewSchedule> CreateSchedules(UIDocument uiDocument)
        {
            var document = uiDocument.Document;
            var t = new Transaction(document, "Create Schedules");
            t.Start();

            var schedules = new List<ViewSchedule>();
            var schedule = ViewSchedule.CreateSchedule(document, new ElementId(BuiltInCategory.OST_Walls),
                ElementId.InvalidElementId);
            schedule.Name = "Wall Schedule 1";
            schedules.Add(schedule);

            foreach (var schedulableField in schedule.Definition.GetSchedulableFields())
            {
                if (schedulableField.FieldType != ScheduleFieldType.Instance)
                    continue;

                var parameterId = schedulableField.ParameterId;
                if (ParameterAccess.ShouldSkip(parameterId))
                    continue;

                var field = schedule.Definition.AddField(schedulableField);

                if (Autodesk.Revit.DB.ParameterUtils.IsBuiltInParameter(parameterId))
                {
                    var bip = (BuiltInParameter)parameterId.Value;
                    var st = document.get_TypeOfStorage(bip);
                    if (st is StorageType.String or StorageType.ElementId)
                    {
                        field.GridColumnWidth = 3 * field.GridColumnWidth;
                        field.HorizontalAlignment = ScheduleHorizontalAlignment.Left;
                    }
                    else
                    {
                        field.HorizontalAlignment = ScheduleHorizontalAlignment.Center;
                    }
                }

                if (field.ParameterId == new ElementId(BuiltInParameter.HOST_VOLUME_COMPUTED))
                {
                    var volumeFilterInCubicFt = 0.8 * Math.Pow(3.2808399, 3.0);
                    var filter = new ScheduleFilter(field.FieldId, ScheduleFilterType.GreaterThan,
                        volumeFilterInCubicFt);
                    schedule.Definition.AddFilter(filter);
                }

                if (field.ParameterId == new ElementId(BuiltInParameter.ELEM_TYPE_PARAM))
                {
                    var sortGroupField = new ScheduleSortGroupField(field.FieldId) { ShowHeader = true };
                    schedule.Definition.AddSortGroupField(sortGroupField);
                }
            }

            t.Commit();
            uiDocument.ActiveView = schedule;
            return schedules;
        }

        public static Result MakeFromViewportClick(UIDocument uidoc)
        {
            if (uidoc == null)
                throw new ArgumentNullException(nameof(uidoc));

            var doc = uidoc.Document ?? throw new SystemInvalidOperationException("The document can't be found.");

            var click = uidoc.Selection.PickPoint(
                        "Click on a plan view viewport on a sheet to create a perspective View3D with its camera at that point.");
            if (click == null)
                throw new SystemInvalidOperationException("Please click on a plan view viewport on a sheet.");

            if (uidoc.ActiveGraphicalView is not ViewSheet viewSheet)
                throw new SystemInvalidOperationException("The click was not on a sheet.");

            var clickedViewport = GetViewportAtClick(viewSheet, click);
            if (clickedViewport == null)
                throw new SystemInvalidOperationException("The click was not on a viewport.");

            if (doc.GetElement(clickedViewport.ViewId) is not RevitView clickedView ||
                !clickedView.HasViewTransforms() ||
                !clickedViewport.HasViewportTransforms())
                throw new SystemInvalidOperationException(
                    "The clicked viewport doesn't report 3D model space to sheet space transforms.");

            if (clickedView.ViewType is not ViewType.AreaPlan and
                not ViewType.CeilingPlan and
                not ViewType.EngineeringPlan and
                not ViewType.FloorPlan)
                throw new SystemInvalidOperationException("Only plan views are supported by this demo application.");

            if (clickedView is not ViewPlan plan)
                throw new SystemInvalidOperationException("Only plan views are supported by this demo application.");

            var clickAsModelRay = CalculateClickAsModelRay(clickedViewport, click);
            if (clickAsModelRay == null)
                throw new SystemInvalidOperationException("The click was outside the view crop regions.");

            var cutPlane = GetViewPlanCutPlane(plan);
            if (cutPlane == null)
                throw new SystemInvalidOperationException("An error occured when getting the view's cut plane.");

            var view3dCameraLocation = XyzMath.ProjectPointOnPlane(cutPlane, clickAsModelRay);
            if (view3dCameraLocation == null)
                throw new SystemInvalidOperationException("An error occured when calculating the View3D camera position.");

            using var tran = new Transaction(doc, "New 3D View");
            tran.Start();
            var view3d = Create3DView(doc, view3dCameraLocation, XYZ.BasisZ, XYZ.BasisY);
            if (view3d == null)
            {
                tran.RollBack();
                throw new SystemInvalidOperationException("Failed to generate the 3D view.");
            }

            tran.Commit();
            uidoc.ActiveView = view3d;

            return Result.Succeeded;
        }

        public static Viewport GetViewportAtClick(ViewSheet viewSheet, XYZ click)
        {
            if (viewSheet == null || click == null)
                return null;

            var doc = viewSheet.Document;
            if (doc == null)
                return null;

            foreach (var vpId in viewSheet.GetAllViewports())
            {
                if (doc.GetElement(vpId) is Viewport viewport &&
                    viewport.GetBoxOutline().Contains(click, ClickTolerance))
                    return viewport;
            }

            return null;
        }

        public static RevitTransform MakeSheetToModelTransform(RevitTransform trfModelToProjection,
                    RevitTransform trfProjectionToSheet)
        {
            return trfModelToProjection == null || trfProjectionToSheet == null
                        ? null
                        : trfProjectionToSheet.Multiply(trfModelToProjection).Inverse;
        }

        public static Plane GetViewPlanCutPlane(ViewPlan plan)
        {
            if (plan == null)
                return null;

            var levelElevation = plan.GenLevel?.Elevation ?? 0.0;
            var cutPlaneOffset = plan.GetViewRange().GetOffset(PlanViewPlane.CutPlane);
            return Plane.CreateByNormalAndOrigin(plan.ViewDirection,
                new XYZ(0.0, 0.0, levelElevation + cutPlaneOffset));
        }

        public static View3D Create3DView(Document doc, XYZ eyePosition, XYZ upDir, XYZ forwardDir)
        {
            if (doc == null || eyePosition == null || upDir == null || forwardDir == null)
                return null;

            var vft = new FilteredElementCollector(doc)
                        .OfClass(typeof(ViewFamilyType))
                        .Cast<ViewFamilyType>()
                        .FirstOrDefault(t => t.ViewFamily == ViewFamily.ThreeDimensional);
            if (vft == null)
                return null;

            var view3d = View3D.CreatePerspective(doc, vft.Id);
            if (view3d == null)
                return null;

            view3d.SetOrientation(new ViewOrientation3D(eyePosition, upDir, forwardDir));
            return view3d;
        }

        public static void SetIconsForPushButtonData(PushButtonData button, Icon icon)
        {
            button.LargeImage = BitmapHelper.GetStdIcon(icon);
            button.Image = BitmapHelper.GetSmallIcon(icon);
        }

        public static void AddTagSymbolByCategory(FamilySymbol tagSymbol, List<FamilySymbolWrapper> categoryTagTypes,
                    List<FamilySymbolWrapper> materialTagTypes, List<FamilySymbolWrapper> multiCategoryTagTypes)
        {
            if (tagSymbol == null)
                return;

            switch (tagSymbol.Category.Name)
            {
                case "Structural Framing Tags":
                    categoryTagTypes.Add(new FamilySymbolWrapper(tagSymbol));
                    break;
                case "Material Tags":
                    materialTagTypes.Add(new FamilySymbolWrapper(tagSymbol));
                    break;
                case "Multi-Category Tags":
                    multiCategoryTagTypes.Add(new FamilySymbolWrapper(tagSymbol));
                    break;
            }
        }

        public static string GetDocumentDisplayName(Document dbDoc)
        {
            if (dbDoc.IsFamilyDocument)
                return dbDoc.PathName;

            var projName = dbDoc.ProjectInformation.Name;
            return string.IsNullOrEmpty(projName) ||
                        string.Equals(projName, "project name", StringComparison.OrdinalIgnoreCase)
                        ? string.IsNullOrEmpty(dbDoc.PathName) ? projName : new FileInfo(dbDoc.PathName).Name
                        : projName;
        }

        public static XYZ CalculateClickAsModelRay(Viewport viewport, XYZ click)
        {
            if (viewport == null || click == null)
                return null;

            var doc = viewport.Document;
            if (doc?.GetElement(viewport.ViewId) is not RevitView view)
                return null;

            var trfProjectionToSheet = new RevitTransform(viewport.GetProjectionToSheetTransform());

            foreach (var trfWithBoundary in view.GetModelToProjectionTransforms())
            {
                var trfSheetToModel = MakeSheetToModelTransform(trfWithBoundary.GetModelToProjectionTransform(),
                            trfProjectionToSheet);
                if (trfSheetToModel == null)
                    throw new SystemInvalidOperationException(
                        "An error occured when calculating the sheet-to-model transforms.");

                var clickAsModelRay = trfSheetToModel.OfPoint(click);
                var modelCurveLoop = trfWithBoundary.GetBoundary();

                if (modelCurveLoop == null)
                    return clickAsModelRay;
                if (XyzMath.IsPointInsideCurveLoop(clickAsModelRay, modelCurveLoop))
                    return clickAsModelRay;
            }

            return null;
        }

    }
}