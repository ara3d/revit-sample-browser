// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Mep;
using Ara3D.RevitSampleBrowser.Common.Views;
using Ara3D.RevitSampleBrowser.InCanvasControlAPI.CS;
using Ara3D.RevitSampleBrowser.PerformanceAdviserControl.CS;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Document = Autodesk.Revit.DB.Document;
using GeoElement = Autodesk.Revit.DB.GeometryElement;
using RevitElement = Autodesk.Revit.DB.Element;

namespace Ara3D.RevitSampleBrowser.Common.Documents
{
    public static class ElementQuery
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

        public static ReferenceWithContext FindReferenceInList(List<ReferenceWithContext> arr, ReferenceWithContext entry)
        {
            foreach (var tmp in arr)
            {
                if (tmp.GetReference().ElementId == entry.GetReference().ElementId)
                    return tmp;
            }

            return null;
        }

        public static Connector FindConnectedTo(Pipe pipe, XYZ conXyz)
        {
            var connItself = ConnectorHelper.FindConnector(pipe, conXyz);
            foreach (Connector conn in connItself.AllRefs)
            {
                if (conn.Owner.Id != pipe.Id && conn.ConnectorType == ConnectorType.End)
                    return conn;
            }

            return null;
        }

        public static ElementId GetSelectedObject(UIDocument uiDoc, string msg)
        {
            return uiDoc.Selection.PickObject(ObjectType.Element, msg)?.ElementId ?? ElementId.InvalidElementId;
        }

        public static ISet<ElementId> GetSelectedObjects(UIDocument uiDoc, string msg)
        {
            return uiDoc.Selection.PickObjects(ObjectType.Element, msg)?
                        .Select(x => x.ElementId)
                        .ToHashSet() ?? [];
        }

        public static int FindIntersection(float u0, float u1, float v0, float v1, ref float[] w)
        {
            if (u1 < v0 || u0 > v1) return 0;
            if (u1 == v0) { w[0] = u1; return 1; }
            if (u0 == v1) { w[0] = u0; return 1; }
            if (u1 > v0)
            {
                if (u0 < v1)
                {
                    w[0] = u0 < v0 ? v0 : u0;
                    w[1] = u1 > v1 ? v1 : u1;
                    return 2;
                }

                w[0] = u0;
                return 1;
            }

            w[0] = u1;
            return 1;
        }

        public static XYZ FindMidPoint(XYZ first, XYZ second)
        {
            return new XYZ((first.X + second.X) / 2, (first.Y + second.Y) / 2, (first.Z + second.Z) / 2);
        }

        public static double FindDistance(XYZ first, XYZ second)
        {
            var x = first.X - second.X;
            var y = first.Y - second.Y;
            var z = first.Z - second.Z;
            return Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        public static XYZ FindDirection(XYZ first, XYZ second)
        {
            var distance = FindDistance(first, second);
            return new XYZ((second.X - first.X) / distance, (second.Y - first.Y) / distance,
                (second.Z - first.Z) / distance);
        }

        public static XYZ FindRightDirection(XYZ viewDirection)
        {
            return new XYZ(-viewDirection.Y, viewDirection.X, viewDirection.Z);
        }

        public static XYZ FindUpDirection(XYZ viewDirection)
        {
            return new XYZ(0, 0, 1);
        }

        public static XYZ FindMiddlePoint(CurveArray curveArray)
        {
            List<XYZ> array = new();
            foreach (Curve curve in curveArray)
            {
                array.Add(curve.GetEndPoint(0));
                array.Add(curve.GetEndPoint(1));
            }

            var maxX = array[0].X;
            var minX = array[0].X;
            var maxY = array[0].Y;
            var minY = array[0].Y;
            var maxZ = array[0].Z;
            var minZ = array[0].Z;

            foreach (var point in array)
            {
                if (maxX < point.X) maxX = point.X;
                if (minX > point.X) minX = point.X;
                if (maxY < point.Y) maxY = point.Y;
                if (minY > point.Y) minY = point.Y;
                if (maxZ < point.Z) maxZ = point.Z;
                if (minZ > point.Z) minZ = point.Z;
            }

            return new XYZ((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);
        }

        public static IEnumerable<Wall> CollectExteriorWalls(Document document)
        {
            FilteredElementCollector collector = new(document);
            return from wall in collector.OfClass(typeof(Wall)).ToElements().Cast<Wall>()
                   where ViewHelper.IsExterior(document.GetElement(wall.GetTypeId()) as ElementType)
                   select wall;
        }

        public static IEnumerable<FamilyInstance> CollectWindows(Document document)
        {
            ElementClassFilter familyInstanceFilter = new(typeof(FamilyInstance));
            ElementCategoryFilter windowCategoryFilter = new(BuiltInCategory.OST_Windows);
            LogicalAndFilter andFilter = new(familyInstanceFilter, windowCategoryFilter);
            return new FilteredElementCollector(document)
                .WherePasses(andFilter)
                .ToElements()
                .Cast<FamilyInstance>();
        }

        public static Dictionary<ElementId, ElementId> DuplicateElementsAcrossDocuments(Document fromDocument,
                    ICollection<ElementId> elementIds,
                    Document toDocument,
                    bool findMatchingElements)
        {
            Dictionary<ElementId, ElementId> elementMap = new();
            ICollection<ElementId> copiedIds;
            using (Transaction t1 = new(toDocument, "Duplicate elements"))
            {
                t1.Start();
                CopyPasteOptions options = new();
                options.SetDuplicateTypeNamesHandler(RevitToolkitCopyPaste.UseDestinationTypes);
                copiedIds = ElementTransformUtils.CopyElements(fromDocument, elementIds, toDocument,
                    Transform.Identity, options);
                var failureOptions = t1.GetFailureHandlingOptions();
                failureOptions.SetFailuresPreprocessor(new HidePasteDuplicateTypesPreprocessor());
                t1.Commit(failureOptions);
            }

            if (!findMatchingElements)
                return elementMap;

            var nameToFromElementsMap = elementIds
                .Select(id => fromDocument.GetElement(id))
                .Where(e => !string.IsNullOrEmpty(e.Name))
                .ToDictionary(e => e.Name, e => e.Id);

            var nameToToElementsMap = copiedIds
                .Select(id => toDocument.GetElement(id))
                .Where(e => !string.IsNullOrEmpty(e.Name))
                .ToDictionary(e => e.Name, e => e.Id);

            foreach (var name in nameToFromElementsMap.Keys)
            {
                if (nameToToElementsMap.TryGetValue(name, out var copiedId))
                    elementMap.Add(nameToFromElementsMap[name], copiedId);
            }

            return elementMap;
        }

        public static string GetElementsWithSchema(Document doc, Schema schema)
        {
            StringBuilder sBuilder = new();
            sBuilder.AppendLine($"Schema: {schema.GUID}, {schema.SchemaName}");
            var elementsofSchema = ElementsWithStorage(doc, schema);
            if (elementsofSchema.Count == 0)
                sBuilder.AppendLine("No elements.");
            else
                foreach (var id in elementsofSchema)
                    sBuilder.AppendLine(ViewHelper.PrintElementInfo(id, doc));
            return sBuilder.ToString();
        }

        public static List<ElementId> ElementsWithStorage(Document doc, Schema schema)
        {
            FilteredElementCollector collector = new(doc);
            collector.WherePasses(new ExtensibleStorageFilter(schema.GUID));
            return collector.ToElementIds().ToList();
        }

        public static Face GetElementFace(Element element, View view, bool exterior)
        {
            var faces = FaceAndSolidGeometry.GetSolidFaces(element, view);
            return faces == null ? null : exterior ? FaceAndSolidGeometry.GetExteriorFace(faces) : FaceAndSolidGeometry.GetInteriorFace(faces);
        }

        public static View3D Get3DView(Document document, string viewName = "{3D}")
        {
            foreach (View3D v in new FilteredElementCollector(document).OfClass(typeof(View3D)).ToElements())
            {
                if (v != null && !v.IsTemplate && v.Name == viewName)
                    return v;
            }

            return null;
        }

        public static double GetElevationForRay(Document document, Wall wall)
        {
            var level = document.GetElement(wall.LevelId) as Level;
            return level.Elevation + 1.0;
        }

        public static void FindColumnsWithin(IList<ReferenceWithContext> references, double proximity, Wall wall,
                    List<ElementId> allColumnsOnWalls, Dictionary<ElementId, List<ElementId>> columnsOnWall)
        {
            foreach (var reference in references)
            {
                if (reference.Proximity >= proximity)
                    continue;

                if (wall.Document.GetElement(reference.GetReference()) is not FamilyInstance familyInstance)
                    continue;

                var familyInstanceId = familyInstance.Id;
                var wallId = wall.Id;
                var categoryValue = familyInstance.Category.BuiltInCategory;
                if (categoryValue is not BuiltInCategory.OST_Columns and
                    not BuiltInCategory.OST_StructuralColumns)
                    continue;

                if (columnsOnWall.TryGetValue(wallId, out var columnsOnWallList))
                {
                    if (!columnsOnWallList.Contains(familyInstanceId))
                        columnsOnWallList.Add(familyInstanceId);
                }
                else
                {
                    columnsOnWall.Add(wallId, [familyInstanceId]);
                }

                if (!allColumnsOnWalls.Contains(familyInstanceId))
                    allColumnsOnWalls.Add(familyInstanceId);
            }
        }

        public static IList<Curve> GetContiguousCurvesFromSelectedCurveElements(Document doc,
                    IList<Reference> boundaries)
        {
            var curves = boundaries
                .Select(reference => (doc.GetElement(reference) as CurveElement).GeometryCurve.Clone())
                .ToList();

            for (var i = 0; i < curves.Count; i++)
            {
                var endPoint = curves[i].GetEndPoint(1);
                for (var j = i + 1; j < curves.Count; j++)
                {
                    if (curves[j].GetEndPoint(0).IsAlmostEqualTo(endPoint, 1e-05))
                    {
                        (curves[i + 1], curves[j]) = (curves[j], curves[i + 1]);
                    }
                    else if (curves[j].GetEndPoint(1).IsAlmostEqualTo(endPoint, 1e-05))
                    {
                        (curves[i + 1], curves[j]) = (XyzMath.CreateReversedCurve(curves[j]), curves[i + 1]);
                    }
                }
            }

            return curves;
        }

        public static void FindMinMax(XYZ point, ref double xMin, ref double xMax, ref double yMin, ref double yMax)
        {
            if (point.X < xMin) xMin = point.X;
            if (point.X > xMax) xMax = point.X;
            if (point.Y < yMin) yMin = point.Y;
            if (point.Y > yMax) yMax = point.Y;
        }

        public static ElementSet WallFilter(ElementSet miscellanea)
        {
            ElementSet walls = new();
            foreach (Element e in miscellanea)
            {
                if (e is Wall) walls.Insert(e);
            }

            return walls.Size == 0 ? throw new System.InvalidOperationException("Please select wall first.") : walls;
        }

        public static ElementType GetDefaultElementType(Document document, ElementTypeGroup group)
        {
            var defaultElementTypeId = document.GetDefaultElementTypeId(group);
            return document.GetElement(defaultElementTypeId) as ElementType;
        }

        public static void SelectMarker(Document document, int controlIndex)
        {
            var tempGraphicsManager = TemporaryGraphicsManager.GetTemporaryGraphicsManager(document);
            var issueMarkerTracking = IssueMarkerTrackingManager.GetInstance().GetTracking(document);
            var provider = ResourceProvider.GetInstance();

            var newSelectedMarker = issueMarkerTracking.GetMarkerByIndex(controlIndex);
            if (newSelectedMarker == null) return;

            var selectedMarker = issueMarkerTracking.GetMarkerByIndex(issueMarkerTracking.GetSelected());
            if (selectedMarker != null)
            {
                selectedMarker.InCanvasControlData.ImagePath = provider.IssueImage;
                tempGraphicsManager.UpdateControl(selectedMarker.ControlIndex, selectedMarker.InCanvasControlData);
                issueMarkerTracking.SetSelected(-1);
            }

            if (newSelectedMarker == selectedMarker) return;

            newSelectedMarker.InCanvasControlData.ImagePath = provider.SelectedIssueImage;
            tempGraphicsManager.UpdateControl(newSelectedMarker.ControlIndex, newSelectedMarker.InCanvasControlData);
            issueMarkerTracking.SetSelected(controlIndex);
        }

        public static Element GetElementById(Document document, ElementId id)
        {
            return document.GetElement(id);
        }

        public static Solid GetElementSolid(Element element)
        {
            var gelem = element.get_Geometry(new Options { ComputeReferences = true });
            Solid resultSolid = null;
            foreach (var gobj in gelem)
            {
                if (gobj is GeometryInstance gIns)
                {
                    foreach (var gobj2 in gIns.GetInstanceGeometry())
                    {
                        if (gobj2 is Solid tSolid && tSolid.Faces.Size > 0 && tSolid.Volume > 0)
                        {
                            resultSolid = tSolid;
                            break;
                        }
                    }
                }

                if (resultSolid == null && gobj is Solid tSolid2 && tSolid2.Faces.Size > 0 && tSolid2.Volume > 0)
                    resultSolid = tSolid2;
            }

            return resultSolid;
        }

        public static string FormatElementIdParameter(string name, Parameter param, Document document)
        {
            var elem = document.GetElement(param.AsElementId());
            return $"{name}\tElement\t{(elem != null ? elem.Name : "Not set")}";
        }

        public static List<RuleInfo> CollectRuleInfo(PerformanceAdviser performanceAdviser)
        {
            List<RuleInfo> ruleInfoList = new();
            foreach (var ruleId in performanceAdviser.GetAllRuleIds())
            {
                RuleInfo oneRule = new(
                    ruleId,
                    ruleId == FlippedDoorCheck.Id,
                    performanceAdviser.GetRuleName(ruleId),
                    performanceAdviser.GetRuleDescription(ruleId),
                    performanceAdviser.IsRuleEnabled(ruleId));
                ruleInfoList.Add(oneRule);
            }

            return ruleInfoList;
        }

        public static bool CheckSelectedElement(RevitElement elem, UIDocument revitDoc, List<Face> faceList,
                    List<string> faceNameList)
        {
            if (elem == null)
                return false;

            Options opts = new() { View = revitDoc.Document.ActiveView, ComputeReferences = true };
            SampleBrowserUtils.InquireGeometry(elem.get_Geometry(opts), elem, faceList, faceNameList, revitDoc.Document.ActiveView);
            return true;
        }

        public static List<FamilySymbol> FindProperFamilySymbol(Document document, BuiltInCategory category)
        {
            var symbollList = new FilteredElementCollector(document)
                .WherePasses(new ElementCategoryFilter(category))
                .ToElements()
                .ToList();
            return !symbollList.Any() ? null : symbollList.OfType<FamilySymbol>().ToList();
        }

        public static List<Element> GetRoomAndSpaceElements(Document document)
        {
            LogicalOrFilter roomSpaceFilter = new(new RoomFilter(), new SpaceFilter());
            return new FilteredElementCollector(document).WherePasses(roomSpaceFilter).ToElements().ToList();
        }

    }
}