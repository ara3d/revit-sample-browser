// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using ArgumentException = Autodesk.Revit.Exceptions.ArgumentException;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using GeoElement = Autodesk.Revit.DB.GeometryElement;
using GeoInstance = Autodesk.Revit.DB.GeometryInstance;
using InvalidOperationException = Autodesk.Revit.Exceptions.InvalidOperationException;
using Point = System.Drawing.Point;
using RevitElement = Autodesk.Revit.DB.Element;
using RevitFreeFormElement = Autodesk.Revit.DB.FreeFormElement;
using Size = System.Drawing.Size;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WinFormsControl = System.Windows.Forms.Control;
using GridCreationResources = Ara3D.RevitSampleBrowser.GridCreation.CS.Properties.Resources;

using Document = Autodesk.Revit.DB.Document;
using RevitView = Autodesk.Revit.DB.View;
using Ara3D.RevitSampleBrowser.CreateTrianglesTopography.CS;
using Ara3D.RevitSampleBrowser.GenerateFloor.CS;
using Ara3D.RevitSampleBrowser.MultiplanarRebar.CS;
using Ara3D.RevitSampleBrowser.FrameBuilder.CS;
using Ara3D.RevitSampleBrowser.FoundationSlab.CS;
using Ara3D.RevitSampleBrowser.InPlaceMembers.CS;
using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.GeometryAPI.UpdateExternallyTaggedBRep.CS;
using Ara3D.RevitSampleBrowser.GetSetDefaultTypes.CS;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Parameters;
using Ara3D.RevitSampleBrowser.Common.Views;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public enum ValueType
    {
        General = 0,
        Angle
    }

    public static class SampleBrowserUtils
    {
        private const string NamespacePrefix = "Ara3D.RevitSampleBrowser.";
        private const string NamespaceSuffix = ".CS";

        public static string GetSourceFolder([CallerFilePath] string callerFilePath = null) =>
            new FileInfo(callerFilePath ?? "").DirectoryName;

        public static string NormalizeSampleNamespace(string typeNamespace)
        {
            var ns = typeNamespace ?? "";
            if (ns.StartsWith(NamespacePrefix))
                ns = ns.Substring(NamespacePrefix.Length);
            if (ns.EndsWith(NamespaceSuffix))
                ns = ns.Substring(0, ns.Length - NamespaceSuffix.Length);
            return ns;
        }

        public static void Rescale(RevitView view, double x, double y)
                {
                    var outline = new UV(view.Outline.Max.U - view.Outline.Min.U,
                        view.Outline.Max.V - view.Outline.Min.V);
                    var rescale = outline.U > outline.V ? outline.U / x * 2 : outline.V / y * 2;
                    if (view.Scale != 1 && rescale != 0)
                        view.Scale = (int)(view.Scale * rescale);
                }

        public static bool SupportsLoopUtilities(Curve curve) => curve is Line || curve is Arc;

        public static bool InReferenceArray(List<ReferenceWithContext> arr, ReferenceWithContext entry)
                {
                    foreach (var tmp in arr)
                    {
                        if (Math.Abs(tmp.Proximity - entry.Proximity) < 1e-9 &&
                            tmp.GetReference().ElementId == entry.GetReference().ElementId)
                            return true;
                    }

                    return false;
                }

        public static IList<Solid> GetTargetSolids(Element element)
                {
                    var solids = new List<Solid>();
                    var options = new Options { DetailLevel = ViewDetailLevel.Fine };
                    foreach (var geomObj in element.get_Geometry(options))
                    {
                        switch (geomObj)
                        {
                            case Solid obj when obj.Faces.Size > 0 && obj.Volume > 0.0:
                                solids.Add(obj);
                                break;
                            case GeometryInstance geomInst:
                                foreach (var instGeomObj in geomInst.GetInstanceGeometry())
                                {
                                    if (instGeomObj is Solid solid && solid.Faces.Size > 0 && solid.Volume > 0.0)
                                        solids.Add(solid);
                                }

                                break;
                        }
                    }

                    return solids;
                }

        public static int CompareReferencesWithContext(ReferenceWithContext a, ReferenceWithContext b)
                {
                    if (a.Proximity > b.Proximity) return 1;
                    if (a.Proximity < b.Proximity) return -1;
                    return 0;
                }

        public static List<XYZ> PerpendicularDirs(XYZ dir, int count)
                {
                    var dirs = new List<XYZ>();
                    var plane = Plane.CreateByNormalAndOrigin(dir, XYZ.Zero);
                    var arc = Arc.Create(plane, 1.0, 0, 6.28);
                    var delta = 1.0 / count;
                    for (var i = 1; i <= count; i++)
                        dirs.Add(arc.Evaluate(delta * i, true));
                    return dirs;
                }

        public static ReferenceWithContext[] GetClosestSectionsToOrigin(List<ReferenceWithContext> refs)
                {
                    var mins = new ReferenceWithContext[2];
                    if (refs.Count == 0)
                        return mins;

                    if (refs[0].Proximity > 0)
                    {
                        mins[1] = refs[0];
                        return mins;
                    }

                    for (var i = 0; i < refs.Count - 1; i++)
                    {
                        if (refs[i].Proximity < 0 && refs[i + 1].Proximity > 0)
                        {
                            mins[0] = refs[i];
                            mins[1] = refs[i + 1];
                            return mins;
                        }
                    }

                    mins[0] = refs[refs.Count - 1];
                    return mins;
                }

        public static CurveLoop CreateRectangleLoop(XYZ start, XYZ end)
                {
                    var profileloop = new CurveLoop();
                    profileloop.Append(Line.CreateBound(start, new XYZ(end.X, start.Y, 0)));
                    profileloop.Append(Line.CreateBound(new XYZ(end.X, start.Y, 0), end));
                    profileloop.Append(Line.CreateBound(end, new XYZ(start.X, end.Y, 0)));
                    profileloop.Append(Line.CreateBound(new XYZ(start.X, end.Y, 0), start));
                    return profileloop;
                }

        public const double DoubleEpsilon = 0.00001;

        public const float FloatEpsilon = 0.00001f;

        public static bool CompareDouble(double d1, double d2) =>
                    Math.Abs(d1 - d2) < DoubleEpsilon;

        public static float Dot(PointF pnt1, PointF pnt2) =>
                    pnt1.X * pnt2.X + pnt1.Y * pnt2.Y;

        public static PointF Multiply(float f, PointF pnt) =>
                    new PointF(f * pnt.X, f * pnt.Y);

        public static PointF Add(PointF f1, PointF f2) =>
                    new PointF(f1.X + f2.X, f1.Y + f2.Y);

        public static PointF Subtract(PointF f1, PointF f2) =>
                    new PointF(f1.X - f2.X, f1.Y - f2.Y);

        public static float GetMin(float f1, float f2) => f1 < f2 ? f1 : f2;

        public static float GetMax(float f1, float f2) => f1 > f2 ? f1 : f2;

        public static FaceArray GetFaces(Element elem)
                {
                    var geoOptions = elem.Document.Application.Create.NewGeometryOptions();
                    geoOptions.ComputeReferences = true;
                    foreach (var obj in elem.get_Geometry(geoOptions))
                        if (obj is Solid solid)
                            return solid.Faces;
                    return null;
                }

        public static bool IsHorizontalFace(Face face)
                {
                    var points = XyzMath.GetPoints(face);
                    if (points.Count < 4) return false;
                    return XyzMath.IsEqual(points[0].Z, points[1].Z) && XyzMath.IsEqual(points[1].Z, points[2].Z) &&
                           XyzMath.IsEqual(points[2].Z, points[3].Z) && XyzMath.IsEqual(points[3].Z, points[0].Z);
                }

        public static TrianglesData LoadTrianglesData()
                {
                    var assemblyFileFolder = Path.GetDirectoryName(typeof(TrianglesData).Assembly.Location);
                    var emmfilePath = Path.Combine(assemblyFileFolder ?? "", "TrianglesData.json");
                    return ParseTrianglesData(File.ReadAllText(emmfilePath));
                }

        public static TrianglesData ParseTrianglesData(string jsonString)
                {
                    var data = JsonSerializer.Deserialize<TrianglesDataDto>(jsonString);
                    return new TrianglesData
                    {
                        Points = data.Points.Select(point => new XYZ(point.X, point.Y, point.Z)).ToList(),
                        Facets = data.Facets
                    };
                }

        private class TrianglesDataDto
                {
                    public IList<PointDto> Points { get; set; }
                    public IList<IList<int>> Facets { get; set; }
                }

        private class PointDto
                {
                    public double X { get; set; }
                    public double Y { get; set; }
                    public double Z { get; set; }
                }

        private const double Tolerance = 0.0001d;

        public static bool AreFaceNormalsPaired(List<Vector4> normals)
                {
                    if (normals == null || normals.Count != 6)
                        return false;

                    var matchedList = new bool[6];
                    for (var i = 0; i < matchedList.Length; i++)
                    {
                        if (matchedList[i]) continue;
                        var vec4A = normals[i];
                        for (var j = 0; j < matchedList.Length; j++)
                        {
                            if (j == i || matchedList[j]) continue;
                            if (!ParameterAccess.IsLinesParallel(vec4A, normals[j])) continue;
                            matchedList[i] = true;
                            matchedList[j] = true;
                            break;
                        }
                    }

                    return matchedList.All(matchedItem => matchedItem);
                }

        public static Leader CalculateLeader(Leader leader, bool addElbow)
                {
                    var elbow = addElbow
                        ? new XYZ(
                            leader.Anchor.X + (leader.End.X - leader.Anchor.X) / 2,
                            leader.Anchor.Y + (leader.End.Y - leader.Anchor.Y) / 2,
                            leader.Anchor.Z + (leader.End.Z - leader.Anchor.Z) / 2)
                        : new XYZ(leader.Anchor.X, leader.Anchor.Y, leader.Anchor.Z);
                    leader.Elbow = elbow;
                    return leader;
                }

        public static XYZ ComputeLeaderPosition(XYZ direction, XYZ origin, double delta) =>
                    origin.Add(direction.Multiply(delta));

        public const string ApplicationName = "DockableDialogs";

        public const string DiagnosticsTabName = "DockableDialogs";

        public const string DiagnosticsPanelName = "DockableDialogs Panel";

        public const string RegisterPage = "Register Page";

        public const string HidePage = "Hide Page";

        public static DockablePaneId SmUserDockablePaneId =
                    new DockablePaneId(new Guid("{3BAFCF52-AC5C-4CF8-B1CB-D0B1D0E90237}"));

        public const string SampleSettingsName = "sample";

        private static int sCounter = DateTime.Now.Second;

        public static bool IsACoupling(FabricationPart fabPart)
                {
                    if (fabPart == null)
                        return false;
                    var cid = fabPart.ItemCustomId;
                    return cid == 522 || cid == 1522 || cid == 2522 || cid == 3522 || cid == 1112;
                }

        public static double MetricToImperial(double value) => value / 304.8;

        public static double ImperialToMetric(double value) => value * 304.8;

        public static Face GetWallFace(Wall wall, RevitView view, bool extOrInt) =>
                    ElementQuery.GetElementFace(wall, view, extOrInt);

        public static Face GetExtrusionFace(Extrusion extrusion, RevitView view, bool extOrInt) =>
                    extrusion.IsSolid ? ElementQuery.GetElementFace(extrusion, view, extOrInt) : null;

        public static void Distribute(Mesh mesh, ref XYZ startPoint, ref XYZ endPoint, ref XYZ thirdPnt)
                {
                    var count = mesh.Vertices.Count;
                    startPoint = mesh.Vertices[0];
                    endPoint = mesh.Vertices[count / 3];
                    thirdPnt = mesh.Vertices[count / 3 * 2];
                }

        public static bool IsVerticalEdge(Edge edge)
                {
                    var polyline = edge.Tessellate() as List<XYZ>;
                    var verticalVct = new XYZ(0, 0, 1);
                    var pointBuffer = polyline[0];
                    for (var i = 1; i < polyline.Count; i++)
                    {
                        if (Equal(XyzMath.GetVector(pointBuffer, polyline[i]), verticalVct))
                            return true;
                    }

                    return false;
                }

        public static bool Equal(XYZ vectorA, XYZ vectorB) =>
                    Math.Abs(vectorA.X - vectorB.X) < XyzMath.Precision && Math.Abs(vectorA.Y - vectorB.Y) < XyzMath.Precision;

        private static FaceArray GetSolidFaces(Element element, RevitView view)
                {
                    if (element == null) return null;
                    var options = new Options { ComputeReferences = true, View = view };
                    return element.get_Geometry(options)
                        .OfType<Solid>()
                        .Select(s => s.Faces)
                        .FirstOrDefault();
                }

        private static Face GetExteriorFace(FaceArray faces) =>
                    faces.Cast<Face>().OrderByDescending(AverageY).FirstOrDefault();

        private static Face GetInteriorFace(FaceArray faces) =>
                    faces.Cast<Face>().OrderBy(AverageY).FirstOrDefault();

        private static double AverageY(Face face)
                {
                    var mesh = face.Triangulate();
                    return mesh.Vertices.Sum(v => v.Y) / mesh.Vertices.Count;
                }

        public const double WallIncrement = 0.5;

        public const double WallEpsilon = 1.0 / 8.0 / 12.0;

        private const double PlanarPrecision = 0.00033;

        public static void DrawProfile(Graphics graphic, RectangleF rclip, Collection<RegularSlab> baseSlabList)
                {
                    var maxBBox = GetMaxBBox(baseSlabList);
                    var matrix = DialogHelper.GetTransformMatrix(rclip, maxBBox);
                    if (matrix == null)
                        return;

                    graphic.Clear(Color.Black);
                    graphic.Transform = matrix;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;

                    var yellowPen = new Pen(Color.Yellow, 0.05f);
                    var greenPen = new Pen(Color.Green, 0.2f);

                    foreach (var slab in baseSlabList)
                    {
                        if (slab.Profile != null)
                            XyzMath.DrawLine(yellowPen, graphic, slab.Profile);
                        if (slab.Selected)
                            XyzMath.DrawLine(greenPen, graphic, slab.OctagonalProfile);
                    }

                    yellowPen.Dispose();
                    greenPen.Dispose();
                    matrix.Dispose();
                }

        public static bool IsPlanarFloor(BoundingBoxXYZ bbXyz, Floor floor, UIApplication revit)
                {
                    var floorThickness = 0.0;
                    var floorType = revit.ActiveUIDocument.Document.GetElement(floor.GetTypeId()) as ElementType;
                    var attribute = floorType?.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM);
                    if (attribute != null)
                        floorThickness = attribute.AsDouble();

                    var boundThickness = Math.Abs(bbXyz.Max.Z - bbXyz.Min.Z);
                    return Math.Abs(boundThickness - floorThickness) < PlanarPrecision;
                }

        public static CurveArray GetFloorProfile(Floor floor, UIApplication revit)
                {
                    var floorProfile = new CurveArray();
                    var document = floor.Document;
                    AnalyticalPanel analyticalModel = null;
                    var relManager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
                    if (relManager != null)
                    {
                        var associatedElementId = relManager.GetAssociatedElementId(floor.Id);
                        if (associatedElementId != ElementId.InvalidElementId)
                        {
                            var associatedElement = document.GetElement(associatedElementId);
                            if (associatedElement is AnalyticalPanel panel)
                                analyticalModel = panel;
                        }
                    }

                    if (analyticalModel != null)
                    {
                        foreach (var curve in analyticalModel.GetOuterContour().ToList())
                            floorProfile.Append(curve);
                        return floorProfile;
                    }

                    var aOptions = revit.Application.Create.NewGeometryOptions();
                    var geometry = floor.get_Geometry(aOptions);
                    var objects = geometry.GetEnumerator();
                    while (objects.MoveNext())
                    {
                        if (!(objects.Current is Solid solid))
                            continue;

                        var edges = solid.Edges;
                        for (var i = 0; i < edges.Size / 3; i++)
                        {
                            var xyzArray = edges.get_Item(i).Tessellate() as List<XYZ>;
                            for (var j = 0; j < xyzArray.Count - 1; j++)
                                floorProfile.Append(Line.CreateBound(xyzArray[j], xyzArray[j + 1]));
                        }
                    }

                    return floorProfile;
                }

        private static RectangleF GetMaxBBox(Collection<RegularSlab> baseSlabList)
                {
                    var union = new RectangleF();
                    var count = 1;
                    foreach (var slab in baseSlabList)
                    {
                        var slabBox = new RectangleF(
                            (float)slab.BBox.Min.X,
                            (float)slab.BBox.Min.Y,
                            (float)(slab.BBox.Max.X - slab.BBox.Min.X),
                            (float)(slab.BBox.Max.Y - slab.BBox.Min.Y));
                        union = count == 1 ? slabBox : RectangleF.Union(union, slabBox);
                        count++;
                    }

                    return union;
                }

        private const int TotalMaxValue = 200;

        public static void CheckTotalNumber(int number)
                {
                    if (number > TotalMaxValue)
                        throw new ErrorMessageException($"The total number of columns should less than {TotalMaxValue}");
                }

        public static bool DuplicateSymbol(FrameTypesMgr typesMgr, object symbol)
                {
                    using (var typeFrm = new DuplicateTypeForm(symbol, typesMgr))
                        return typeFrm.ShowDialog() == DialogResult.OK;
                }

        public static void RefreshListControl(ListControl list, FrameTypesMgr typesMgr)
                {
                    list.DataSource = null;
                    list.DataSource = typesMgr.FramingSymbols;
                    list.DisplayMember = "Name";
                    list.SelectedIndex = 0;
                }

        public enum FailureCondition
                {
                    Success,
                    CurvesNotContigous,
                    CurveLoopAboveTarget,
                    NoIntersection
                }

        public static FailureCondition CreateNegativeBlock(Element targetElement, IList<Reference> boundaries,
                    IFamilyLoadOptions familyLoadOptions, string familyTemplate)
                {
                    var doc = targetElement.Document;
                    var app = doc.Application;
                    var curves = ElementQuery.GetContiguousCurvesFromSelectedCurveElements(doc, boundaries);
                    CurveLoop loop;
                    try
                    {
                        loop = CurveLoop.Create(curves);
                    }
                    catch (ArgumentException)
                    {
                        return FailureCondition.CurvesNotContigous;
                    }

                    var elevation = curves[0].GetEndPoint(0).Z;
                    var bbox = targetElement.get_BoundingBox(null);
                    var height = bbox.Max.Z - elevation;
                    if (height <= 1e-5)
                        return FailureCondition.CurveLoopAboveTarget;

                    height += 1;
                    var familyDoc = app.NewFamilyDocument(familyTemplate);
                    var block = GeometryCreationUtilities.CreateExtrusionGeometry(
                        new List<CurveLoop> { loop }, XYZ.BasisZ, height);
                    var fromElement = GetTargetSolids(targetElement);
                    var solidCount = fromElement.Count;
                    Solid toSubtract = null;
                    if (solidCount == 1)
                        toSubtract = fromElement[0];
                    else if (solidCount > 1)
                        toSubtract = BooleanOperationsUtils.ExecuteBooleanOperation(fromElement[0], fromElement[1],
                            BooleanOperationsType.Union);

                    if (solidCount > 2)
                        for (var i = 2; i < solidCount; i++)
                            toSubtract = BooleanOperationsUtils.ExecuteBooleanOperation(toSubtract, fromElement[i],
                                BooleanOperationsType.Union);

                    try
                    {
                        BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(block, toSubtract,
                            BooleanOperationsType.Difference);
                    }
                    catch (InvalidOperationException)
                    {
                        return FailureCondition.NoIntersection;
                    }

                    using (var t = new Transaction(familyDoc, "Add element"))
                    {
                        t.Start();
                        RevitFreeFormElement.Create(familyDoc, block);
                        t.Commit();
                    }

                    var family = familyDoc.LoadFamily(doc, familyLoadOptions);
                    familyDoc.Close(false);
                    var fs = new FilteredElementCollector(doc)
                        .WherePasses(new FamilySymbolFilter(family.Id))
                        .FirstElement() as FamilySymbol;

                    using (var t2 = new Transaction(doc, "Place instance"))
                    {
                        t2.Start();
                        if (!fs.IsActive)
                            fs.Activate();
                        doc.Create.NewFamilyInstance(XYZ.Zero, fs, StructuralType.NonStructural);
                        t2.Commit();
                    }

                    return FailureCondition.Success;
                }

        public static void CreateFloor(Data data, Document doc)
                {
                    var loop = new CurveLoop();
                    foreach (Curve curve in data.Profile)
                        loop.Append(curve);

                    Floor.Create(doc, new List<CurveLoop> { loop }, data.FloorType.Id, data.Level.Id, data.Structural, null, 0.0);
                }

        public static Result ExecuteCreateBRepCommand(Document document)
                {
                    var taggedBRep = CreateExternallyTaggedPodium(40.0, 12.0, 30.0);
                    if (taggedBRep == null)
                        return Result.Failed;

                    using (var transaction = new Transaction(document, "CreateExternallyTaggedBRep"))
                    {
                        transaction.Start();

                        CreateBRep.CreatedDirectShape = CreateDirectShapeWithExternallyTaggedBRep(document, taggedBRep);
                        if (CreateBRep.CreatedDirectShape == null)
                            return Result.Failed;

                        if (!(CreateBRep.CreatedDirectShape.GetExternallyTaggedGeometry(taggedBRep.ExternalId) is ExternallyTaggedBRep retrievedBRep))
                            return Result.Failed;

                        if (retrievedBRep.GetTaggedGeometry(new ExternalGeometryId("faceRiser1")) is not Face)
                            return Result.Failed;

                        if (retrievedBRep.GetTaggedGeometry(new ExternalGeometryId("edgeLeftRiser1")) is not Edge)
                            return Result.Failed;

                        transaction.Commit();
                        return Result.Succeeded;
                    }
                }

        public static ExternallyTaggedBRep CreateExternallyTaggedPodium(double width, double height, double depth) =>
                    new Podium(width, height, depth).CreateStairs();

        public static DirectShape CreateDirectShapeWithExternallyTaggedBRep(Document document,
                    ExternallyTaggedBRep taggedBRep)
                {
                    var directShape = DirectShape.CreateElement(document, new ElementId(BuiltInCategory.OST_Stairs));
                    if (directShape == null)
                        return null;
                    directShape.ApplicationId = "TestCreateExternallyTaggedBRep";
                    directShape.ApplicationDataId = "ExternallyTaggedBRep";
                    directShape.AddExternallyTaggedGeometry(taggedBRep);
                    return directShape;
                }

        public static bool DockablePanesExist() =>
                    DockablePane.PaneExists(DefaultFamilyTypes.PaneId) &&
                    DockablePane.PaneExists(DefaultElementTypes.PaneId);

        private static readonly ResourceManager ResManager = GridCreationResources.ResourceManager;

        public static bool ValidateNumbers(WinFormsControl number1Ctrl, WinFormsControl number2Ctrl)
                {
                    if (!ValidateNumber(number1Ctrl) || !ValidateNumber(number2Ctrl)) return false;

                    if (Convert.ToUInt32(number1Ctrl.Text) == 0 && Convert.ToUInt32(number2Ctrl.Text) == 0)
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString("NumbersCannotBeBothZero"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        number1Ctrl.Focus();
                        return false;
                    }

                    return true;
                }

        public static bool ValidateNumber(WinFormsControl numberCtrl)
                {
                    if (!ValidateNotNull(numberCtrl, "Number")) return false;

                    try
                    {
                        var number = Convert.ToUInt32(numberCtrl.Text);
                        if (number > 200)
                        {
                            DialogHelper.ShowWarningMessage(ResManager.GetString("NumberBetween0And200"),
                                GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                            numberCtrl.Focus();
                            return false;
                        }
                    }
                    catch (OverflowException)
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString("NumberBetween0And200"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        numberCtrl.Focus();
                        return false;
                    }
                    catch (Exception)
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString("NumberFormatWrong"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        numberCtrl.Focus();
                        return false;
                    }

                    return true;
                }

        public static bool ValidateLength(WinFormsControl lengthCtrl, string typeName, bool canBeZero)
                {
                    if (!ValidateNotNull(lengthCtrl, typeName)) return false;

                    try
                    {
                        var length = Convert.ToDouble(lengthCtrl.Text);
                        if (length <= 0 && !canBeZero)
                        {
                            DialogHelper.ShowWarningMessage(ResManager.GetString($"{typeName}CannotBeNegativeOrZero"),
                                GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                            lengthCtrl.Focus();
                            return false;
                        }

                        if (length < 0 && canBeZero)
                        {
                            DialogHelper.ShowWarningMessage(ResManager.GetString($"{typeName}CannotBeNegative"),
                                GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                            lengthCtrl.Focus();
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString($"{typeName}FormatWrong"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        lengthCtrl.Focus();
                        return false;
                    }

                    return true;
                }

        public static bool ValidateCoord(WinFormsControl coordCtrl)
                {
                    if (!ValidateNotNull(coordCtrl, "Coordinate")) return false;

                    try
                    {
                        Convert.ToDouble(coordCtrl.Text);
                    }
                    catch (Exception)
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString("CoordinateFormatWrong"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        coordCtrl.Focus();
                        return false;
                    }

                    return true;
                }

        public static bool ValidateDegrees(WinFormsControl startDegree, WinFormsControl endDegree)
                {
                    if (!ValidateDegree(startDegree) || !ValidateDegree(endDegree)) return false;

                    if (Math.Abs(Convert.ToDouble(startDegree.Text) - Convert.ToDouble(endDegree.Text)) <= double.Epsilon)
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString("DegreesAreTooClose"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        startDegree.Focus();
                        return false;
                    }

                    if (Convert.ToDouble(startDegree.Text) >= Convert.ToDouble(endDegree.Text))
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString("StartDegreeShouldBeLessThanEndDegree"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        startDegree.Focus();
                        return false;
                    }

                    return true;
                }

        public static bool ValidateDegree(WinFormsControl degreeCtrl)
                {
                    if (!ValidateNotNull(degreeCtrl, "Degree")) return false;

                    try
                    {
                        var startDegree = Convert.ToDouble(degreeCtrl.Text);
                        if (startDegree < 0 || startDegree > 360)
                        {
                            DialogHelper.ShowWarningMessage(ResManager.GetString("DegreeWithin0To360"),
                                GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                            degreeCtrl.Focus();
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString("DegreeFormatWrong"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        degreeCtrl.Focus();
                        return false;
                    }

                    return true;
                }

        public static bool ValidateLabel(WinFormsControl labelCtrl, ArrayList allLabels)
                {
                    if (!ValidateNotNull(labelCtrl, "Label")) return false;

                    var labelToBeValidated = labelCtrl.Text;
                    foreach (string label in allLabels)
                    {
                        if (label == labelToBeValidated)
                        {
                            DialogHelper.ShowWarningMessage(ResManager.GetString("LabelExisted"),
                                GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                            labelCtrl.Focus();
                            return false;
                        }
                    }

                    return true;
                }

        public static bool ValidateNotNull(WinFormsControl control, string typeName)
                {
                    if (string.IsNullOrEmpty(control.Text.TrimStart(' ').TrimEnd(' ')))
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString($"{typeName}CannotBeNull"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        control.Focus();
                        return false;
                    }

                    return true;
                }

        public static bool ValidateLabels(WinFormsControl label1Ctrl, WinFormsControl label2Ctrl)
                {
                    if (label1Ctrl.Text.TrimStart(' ').TrimEnd(' ') == label2Ctrl.Text.TrimStart(' ').TrimEnd(' '))
                    {
                        DialogHelper.ShowWarningMessage(ResManager.GetString("LabelsCannotBeSame"),
                            GridCreationResources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                        label1Ctrl.Focus();
                        return false;
                    }

                    return true;
                }

        public static GraphicsData CreateGraphicsData(AnalyticalElement model)
                {
                    IList<Curve> curveList = model switch
                    {
                        AnalyticalMember _ => new List<Curve> { model.GetCurve() },
                        AnalyticalPanel panel => panel.GetOuterContour().ToList(),
                        _ => new List<Curve>()
                    };

                    if (curveList.Count == 0)
                        throw new Exception("Can't get curves.");

                    var data = new GraphicsData();
                    foreach (var curve in curveList)
                    {
                        try
                        {
                            data.InsertCurve(curve.Tessellate() as List<XYZ>);
                        }
                        catch
                        {
                        }
                    }

                    data.UpdataData();
                    return data;
                }

        public static string GetSpecialData(IDictionary<string, string> dataMap, string key)
                {
                    var dataValue = dataMap[key];
                    if (string.IsNullOrEmpty(dataValue))
                        throw new Exception($"{key}information is not exist in journal.");
                    return dataValue;
                }

        public static ModelCurve MakeArc(UIApplication app, XYZ ptA, XYZ ptB, XYZ ptC)
                {
                    var doc = app.ActiveUIDocument.Document;
                    var arc = Arc.Create(ptA, ptB, ptC);
                    var ca = new CurveLoop();
                    ca.Append(Line.CreateBound(ptA, ptB));
                    ca.Append(Line.CreateBound(ptB, ptC));
                    ca.Append(Line.CreateBound(ptC, ptA));
                    var skplane = SketchPlane.Create(doc, ca.GetPlane());
                    return doc.FamilyCreate.NewModelCurve(arc, skplane);
                }

        public static Reference ParseReference(Document document, string stableRepresentation) =>
                    string.IsNullOrEmpty(stableRepresentation)
                        ? null
                        : Reference.ParseFromStableRepresentation(document, stableRepresentation);

        public static SpatialFieldManager GetSpatialFieldManager(RevitView view) =>
                    SpatialFieldManager.GetSpatialFieldManager(view);

        public static XYZ ProjectToTrackball(double width, double height, Point point)
                {
                    var x = point.X / (width / 2) - 1;
                    var y = 1 - point.Y / (height / 2);

                    var d = Math.Sqrt(x * x + y * y);
                    var z = d < 0.70710678118654752440
                        ? Math.Sqrt(1 - d * d)
                        : 0.5 / d;

                    return new XYZ(x, y, z);
                }

        public static XYZ Project(List<XYZ> xyzArray, XYZ point)
                {
                    var a = xyzArray[0] - xyzArray[1];
                    var b = xyzArray[0] - xyzArray[2];
                    var c = point - xyzArray[0];
                    var normal = a.CrossProduct(b);

                    try
                    {
                        normal = normal.Normalize();
                    }
                    catch (Exception)
                    {
                        normal = XYZ.Zero;
                    }

                    return point - normal.DotProduct(c) * normal;
                }

        public static bool InquireGeometry(GeoElement geoElement, RevitElement elem, List<Face> faceList,
                    List<string> faceNameList, RevitView view)
                {
                    if (geoElement == null || elem == null)
                        return false;

                    var objects = geoElement.GetEnumerator();
                    if (!objects.MoveNext())
                        return false;

                    objects.Reset();
                    while (objects.MoveNext())
                    {
                        var obj = objects.Current;
                        if (obj is GeoInstance instance)
                        {
                            InquireGeometry(instance.SymbolGeometry, elem, faceList, faceNameList, view);
                            continue;
                        }

                        if (!(obj is Solid solid) || solid.Faces.IsEmpty)
                            continue;

                        var category = elem.Category != null && elem.Name != null ? elem.Category.Name : string.Empty;
                        var ii = 0;
                        foreach (Face tempFace in solid.Faces)
                        {
                            if (tempFace is PlanarFace)
                            {
                                faceNameList.Add($"{category} : {elem.Name} ({ii})");
                                faceList.Add(tempFace);
                                ii++;
                            }
                        }
                    }

                    return true;
                }

        private const double DoubleTolerance = 1E-9;

        public const double DegreesToRadians = 0.0174532925199433;

        public static bool GetOffsetFromConstraintAtTarget(RebarUpdateCurvesData updateData, RebarConstraint constraint,
                    int targetIdx, out double offset)
                {
                    offset = 0.0;
                    if (updateData == null || constraint == null)
                        return false;

                    var barDiam = updateData.GetBarModelDiameter();
                    var rebarStyle = updateData.GetRebarStyle();
                    var attachment = updateData.GetAttachmentType();
                    var bIsInside = rebarStyle == RebarStyle.Standard || (rebarStyle != RebarStyle.Standard &&
                                                                          attachment == StirrupTieAttachmentType.InteriorFace);

                    if (constraint.IsToCover())
                    {
                        if (targetIdx < 0 || targetIdx >= constraint.NumberOfTargets)
                            return false;
                        var coverType = constraint.GetTargetCoverType(targetIdx);
                        var coverDist = coverType == null ? 0.0 : coverType.CoverDistance;
                        var diameterOffset = barDiam / 2;
                        if (bIsInside)
                            diameterOffset *= -1;
                        offset = constraint.GetDistanceToTargetCover() - coverDist + diameterOffset;
                        return true;
                    }

                    offset = constraint.GetDistanceToTargetHostFace();
                    return true;
                }

        public static Face GetBottomFace(FaceArray faces)
                {
                    Face face = null;
                    var elevation = 0.0;

                    foreach (Face f in faces)
                    {
                        if (IsVerticalFace(f))
                            continue;

                        var mesh = f.Triangulate();
                        var tempElevation = mesh.Vertices.Cast<XYZ>().Sum(xyz => xyz.Z) / mesh.Vertices.Count;

                        if (face == null || elevation > tempElevation)
                        {
                            face = f;
                            elevation = tempElevation;
                        }
                    }

                    return face;
                }

        private static bool IsVerticalFace(Face face) =>
                    face.EdgeLoops.Cast<EdgeArray>().SelectMany(ea => ea.Cast<Edge>())
                        .Any(IsVerticalEdge);

        public static string GetProperty(Document activeDoc, Room room, BuiltInParameter paraEnum, bool useValue)
                {
                    Parameter param;
                    try
                    {
                        param = room.get_Parameter(paraEnum);
                    }
                    catch (Exception)
                    {
                        if (room.Location == null)
                            return "Not Placed";
                        throw new Exception("Illegal built in parameter.");
                    }

                    if (param == null) return "";

                    switch (param.StorageType)
                    {
                        case StorageType.Integer:
                            return param.AsInteger().ToString();
                        case StorageType.String:
                            return param.AsString();
                        case StorageType.Double:
                            return useValue ? param.AsValueString() : param.AsDouble().ToString();
                        case StorageType.ElementId:
                            return activeDoc.GetElement(param.AsElementId()).Name;
                        default:
                            return param.AsString();
                    }
                }

        public const double RadiansToDegrees = 180 / Math.PI;

        private static bool ShouldSkip(ElementId parameterId) => ParameterAccess.ShouldSkip(parameterId);

        private const int DefaultPrecision = 3;

        private const double AngleRatio = 0.0174532925199433;

        public static double DealPrecision(double value, int precision)
                {
                    if (precision < 0 && precision > 15) return value;
                    if (value >= 1 || value <= -1 || value == 0)
                        return Math.Round(value, precision);

                    var temp = Math.Abs(value);
                    var firstNumberPos = 1;
                    for (; firstNumberPos < 16; firstNumberPos++)
                    {
                        temp *= 10;
                        if (temp >= 1) break;
                    }

                    if (firstNumberPos > 15) firstNumberPos = 15;
                    return Math.Round(value, firstNumberPos > precision ? firstNumberPos : precision);
                }

        public static string DoubleToString(double value, ValueType valueType)
                {
                    ValueConversion(value, ValueType.Angle, true, out var newValue);
                    newValue = DealPrecision(newValue, DefaultPrecision);
                    var displayText = DealDecimalNumber(newValue.ToString(), DefaultPrecision);
                    if (valueType == ValueType.Angle)
                        displayText += (char)0xb0;
                    return displayText;
                }

        public static string DealDecimalNumber(string value, int number)
                {
                    var newValue = value;
                    int dist;
                    if (newValue.Contains("."))
                        dist = newValue.Length - (newValue.IndexOf(".") + 1);
                    else
                    {
                        dist = 0;
                        newValue += ".";
                    }

                    if (dist < number)
                        for (var i = 0; i < number - dist; i++)
                            newValue += "0";
                    return newValue;
                }

        public static bool StringToDouble(string value, ValueType valueType, out double newValue)
                {
                    newValue = 0;
                    if (value == null) return false;
                    if (!ParseFromString(value, valueType, out var result)) return false;
                    ValueConversion(result, valueType, false, out newValue);
                    return true;
                }

        private static bool ParseFromString(string value, ValueType valueType, out double result)
                {
                    if (value.Length == 0)
                    {
                        result = 0;
                        return true;
                    }

                    string newValue;
                    var degree = ((char)0xb0).ToString();
                    if (valueType == ValueType.General)
                        newValue = value;
                    else if (value.Contains(degree))
                        newValue = value.Substring(0, value.IndexOf(degree));
                    else if (value.Contains(" "))
                        newValue = value.Substring(0, value.IndexOf(" "));
                    else
                        newValue = value;

                    return double.TryParse(newValue, out result);
                }

        private static void ValueConversion(double value, ValueType valueType, bool isDoubleToString, out double newValue)
                {
                    if (valueType == ValueType.General)
                    {
                        newValue = value;
                        return;
                    }

                    newValue = isDoubleToString ? value / AngleRatio : value * AngleRatio;
                }

        private const double ClickTolerance = 0.0001;

        private const double RandomXScale = 5631;

        private const double RandomYScale = 4369;

        public static XYZ CalculateClickAsModelRay(Viewport viewport, XYZ click)
                {
                    if (viewport == null || click == null)
                        return null;

                    var doc = viewport.Document;
                    if (!(doc?.GetElement(viewport.ViewId) is RevitView view))
                        return null;

                    var trfProjectionToSheet = new Transform(viewport.GetProjectionToSheetTransform());

                    foreach (var trfWithBoundary in view.GetModelToProjectionTransforms())
                    {
                        var trfSheetToModel = ViewHelper.MakeSheetToModelTransform(trfWithBoundary.GetModelToProjectionTransform(),
                            trfProjectionToSheet);
                        if (trfSheetToModel == null)
                            throw new System.InvalidOperationException(
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

        private static void GenerateCircleSurrounding(IList<XYZ> points, XYZ center, double deltaElevation,
                    double radius)
                {
                    for (var theta = 0.0; theta < 2 * Math.PI; theta += Math.PI / 6.0)
                        points.Add(center + new XYZ(radius * Math.Cos(theta), radius * Math.Sin(theta), deltaElevation));
                }

        public const double ToFractionalInches = 0.08333333;

        public static readonly List<string> SElevationOrigin = new List<string> { "Project", "Shared", "Relative" };

        public static readonly List<string> STextOrientation = new List<string> { "Horizontal Above", "Horizontal Below" };

        public static readonly List<string> SIndicator = new List<string> { "Prefix", "Suffix" };

        public static readonly List<string> STopBottomValue = new List<string> { "None", "North / South", "East / West" };

        public static readonly List<string> STextBackground = new List<string> { "Opaque", "Transparent" };

        public const string InSessionName = "<In-Session>";

        public const string SampleName = "RevitView Template Creation sample";

        private static bool CheckOrientation(IList<XYZ> controlPoints)
                {
                    XYZ previousDir = null;
                    for (var i = 1; i < controlPoints.Count - 1; i++)
                    {
                        var dir1 = controlPoints[i] - controlPoints[i - 1];
                        var dir2 = controlPoints[i + 1] - controlPoints[i];
                        if (previousDir == null)
                            previousDir = dir1.CrossProduct(dir2).Normalize();
                        else if (!previousDir.IsAlmostEqualTo(dir1.CrossProduct(dir2).Normalize())) return false;
                    }

                    return true;
                }

        private static IList<XYZ> CalculateOffset(IList<XYZ> controlPoints, double offset)
                {
                    IList<XYZ> innerPnts = new List<XYZ>();

                    for (var i = 1; i < controlPoints.Count - 1; i++)
                    {
                        var dir1 = (controlPoints[i] - controlPoints[i - 1]).Normalize();
                        var dir2 = (controlPoints[i + 1] - controlPoints[i]).Normalize();
                        var bisectDir = (dir2 - dir1).Normalize();

                        var stepInside1StDir = new XYZ(-dir1.Y, dir1.X, 0.0);
                        if (stepInside1StDir.DotProduct(bisectDir) < 0.0)
                            stepInside1StDir = stepInside1StDir.Negate();

                        if (i == 1) innerPnts.Add(controlPoints[i - 1] + stepInside1StDir * offset);

                        var stepInside2NdDir = new XYZ(-dir2.Y, dir2.X, 0.0);
                        if (stepInside2NdDir.DotProduct(bisectDir) < 0.0)
                            stepInside2NdDir = stepInside2NdDir.Negate();

                        var semiAngle = bisectDir.AngleTo(dir2);
                        var slopDist = offset / Math.Sin(semiAngle);
                        innerPnts.Add(controlPoints[i] + bisectDir * slopDist);

                        if (i == controlPoints.Count - 2)
                            innerPnts.Add(controlPoints[i + 1] + stepInside2NdDir * offset);
                    }

                    return innerPnts;
                }

    }
}