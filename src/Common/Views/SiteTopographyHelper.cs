// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Size = System.Drawing.Size;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using Document = Autodesk.Revit.DB.Document;
using RevitView = Autodesk.Revit.DB.View;

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;

namespace Ara3D.RevitSampleBrowser.Common.Views
{
    public static class SiteTopographyHelper
    {
        public static void ChangeSubregionAndPointsElevation(UIDocument uiDoc, double elevationDelta)
                {
                    var doc = uiDoc.Document;
                    var subregion = PickSubregion(uiDoc);
                    var toposurface = GetTopographySurfaceHost(subregion);
                    var points = GetNonBoundaryPoints(subregion);
                    if (points.Count == 0)
                        return;

                    var delta = elevationDelta * XYZ.BasisZ;
                    using (var editScope = new TopographyEditScope(doc, "Raise/lower terrain"))
                    {
                        editScope.Start(toposurface.Id);
                        using (var t = new Transaction(doc, "Raise/lower terrain"))
                        {
                            t.Start();
                            toposurface.MovePoints(points, delta);
                            t.Commit();
                        }

                        editScope.Commit(new TopographyEditFailuresPreprocessor());
                    }
                }

        public static TopographySurface PickSubregion(UIDocument uiDoc) =>
                    uiDoc.Document.GetElement(uiDoc.Selection.PickObject(ObjectType.Element,
                        new SubRegionSelectionFilter(), "Select subregion")) as TopographySurface;

        public static TopographySurface PickTopographySurface(UIDocument uiDoc) =>
                    uiDoc.Document.GetElement(uiDoc.Selection.PickObject(ObjectType.Element,
                        new TopographySurfaceSelectionFilter(), "Select topography surface")) as TopographySurface;

        public static XYZ PickPointNearToposurface(UIDocument uiDoc, TopographySurface toposurface, string message)
                {
                    var point = uiDoc.Selection.PickPoint(message);
                    var elevation = GetAverageElevation(toposurface.GetPoints());
                    var viewDirection = uiDoc.ActiveView.ViewDirection.Normalize();
                    var elevationDelta = (elevation - point.Z) / viewDirection.Z;
                    return point + viewDirection * elevationDelta;
                }

        public static double GetAverageElevation(IList<XYZ> existingPoints) =>
                    existingPoints.Count == 0 ? 0 : existingPoints.Average(xyz => xyz.Z);

        public static IList<XYZ> GeneratePondPointsSurrounding(XYZ center, double maxRadius)
                {
                    if (maxRadius <= 8)
                        throw new Exception("Pond radius must be greater than 8");

                    var points = new List<XYZ> { center };
                    GenerateCircleSurrounding(points, center, 1, maxRadius - 8);
                    GenerateCircleSurrounding(points, center, 2, maxRadius - 5);
                    GenerateCircleSurrounding(points, center, 3, maxRadius - 1);
                    return points;
                }

        public static XYZ GetCenterOf(Element element)
                {
                    var bbox = element.get_BoundingBox(null);
                    return (bbox.Min + bbox.Max) / 2.0;
                }

        public static IList<XYZ> GetPointsFromSubregionRough(TopographySurface subregion)
                {
                    var bbox = subregion.get_BoundingBox(null);
                    var toposurface = GetTopographySurfaceHost(subregion);
                    return toposurface.FindPoints(new Outline(bbox.Min, bbox.Max));
                }

        public static IList<XYZ> GetPointsFromSubregionExact(TopographySurface subregion) =>
                    subregion.GetPoints();

        public static TopographySurface GetTopographySurfaceHost(TopographySurface subregion) =>
                    subregion.Document.GetElement(subregion.AsSiteSubRegion().HostId) as TopographySurface;

        public static IList<XYZ> GetNonBoundaryPoints(TopographySurface toposurface) =>
                    toposurface.GetInteriorPoints();

        public static bool TryGetFirstToposolidTypeAndLevel(Document doc, out ElementId typeId, out ElementId levelId)
                {
                    typeId = new FilteredElementCollector(doc).OfClass(typeof(ToposolidType)).OfType<ToposolidType>()
                        .FirstOrDefault()?.Id;
                    levelId = new FilteredElementCollector(doc).OfClass(typeof(Level)).OfType<Level>().FirstOrDefault()?.Id;
                    return typeId != null && levelId != null;
                }

        public static List<XYZ> CollectPointsFromImportInstance(ImportInstance import)
                {
                    var ptList = new List<XYZ>();
                    foreach (var gObject in import.get_Geometry(new Options()))
                    {
                        if (!(gObject is GeometryInstance gInstance))
                            continue;

                        foreach (var obj in gInstance.GetSymbolGeometry())
                        {
                            switch (obj)
                            {
                                case PolyLine polyLine:
                                    ptList.AddRange(polyLine.GetCoordinates());
                                    break;
                                case Line line:
                                    ptList.Add(line.GetEndPoint(0));
                                    ptList.Add(line.GetEndPoint(1));
                                    break;
                            }
                        }
                    }

                    return ptList;
                }

        private static void GenerateCircleSurrounding(IList<XYZ> points, XYZ center, double deltaElevation,
                    double radius)
                {
                    for (var theta = 0.0; theta < 2 * Math.PI; theta += Math.PI / 6.0)
                        points.Add(center + new XYZ(radius * Math.Cos(theta), radius * Math.Sin(theta), deltaElevation));
                }

    }
}