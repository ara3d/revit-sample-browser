using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RevitMesh = Autodesk.Revit.DB.Mesh;

namespace Ara3D.Bowerbird.RevitSamples;

public static class FaceDataExtensions
{
    public static FaceType GetFaceType(this Face face)
    {
        return face switch
        {
            ConicalFace conicalFace => FaceType.Conical,
            CylindricalFace cylindricalFace => FaceType.Cylindrical,
            HermiteFace hermiteFace => FaceType.Hermite,
            PlanarFace planarFace => FaceType.Planar,
            RevolvedFace revolvedFace => FaceType.Revolved,
            RuledFace ruledFace => FaceType.Ruled,
            _ => throw new ArgumentOutOfRangeException(nameof(face)),
        };
    }

    public static object ToSpecificFaceData(this Face face)
    {
        return face switch
        {
            ConicalFace conicalFace => new ConicalFaceData(
                                conicalFace.Axis.ToData(),
                                conicalFace.HalfAngle,
                                conicalFace.Origin.ToData(),
                                conicalFace.get_Radius(0).ToData(),
                                conicalFace.get_Radius(1).ToData()),
            CylindricalFace cylindricalFace => new CylindricalFaceData(
                                cylindricalFace.Axis.ToData(),
                                cylindricalFace.Origin.ToData(),
                                cylindricalFace.get_Radius(0).ToData(),
                                cylindricalFace.get_Radius(1).ToData()),
            HermiteFace hermiteFace => new HermiteFaceData(
                                hermiteFace.MixedDerivs.ToData(),
                                hermiteFace.Points.ToData(),
                                hermiteFace.get_Params(0).ToData(),
                                hermiteFace.get_Params(1).ToData(),
                                hermiteFace.get_Tangents(0).ToData(),
                                hermiteFace.get_Tangents(1).ToData()),
            PlanarFace planarFace => new PlanarFaceData(
                                planarFace.Origin.ToData(),
                                planarFace.FaceNormal.ToData(),
                                planarFace.XVector.ToData(),
                                planarFace.YVector.ToData()),
            RevolvedFace revolvedFace => new RevolvedFaceData(
                                revolvedFace.Axis.ToData(),
                                revolvedFace.Curve.ToData(),
                                revolvedFace.Origin.ToData(),
                                revolvedFace.get_Radius(0).ToData(),
                                revolvedFace.get_Radius(1).ToData()),
            RuledFace ruledFace => new RuledFaceData(
                                ruledFace.IsExtruded,
                                ruledFace.RulingsAreParallel,
                                ruledFace.get_Curve(0).ToData(),
                                ruledFace.get_Curve(1).ToData(),
                                ruledFace.get_Point(0).ToData(),
                                ruledFace.get_Point(1).ToData()),
            _ => throw new ArgumentOutOfRangeException(nameof(face)),
        };
    }

    public static List<double> ToData(this DoubleArray self)
    {
        List<double> r = new(self.Size);
        for (var i = 0; i < self.Size; i++)
            r.Add(self.get_Item(i));
        return r;
    }

    public static UVData ToData(this UV self)
    {
        return new(self.U, self.V);
    }

    public static List<UVData> ToData(this IEnumerable<UV> self)
    {
        return self.Select(ToData).ToList();
    }

    public static XYZData ToData(this XYZ self)
    {
        return new(self.X, self.Y, self.Z);
    }

    public static List<XYZData> ToData(this IEnumerable<XYZ> self)
    {
        return self.Select(ToData).ToList();
    }

    public static List<UVData> GetEdgePoints(this Face face, Edge edge)
    {
        return edge.TessellateOnFace(face).Select(ToData).ToList();
    }

    public static List<UVData> GetEdgePoints(this Face face, EdgeArray edgeArray)
    {
        List<UVData> r = [];
        foreach (Edge edge in edgeArray)
            r.AddRange(face.GetEdgePoints(edge));
        return r;
    }

    public static UVLoop GetUVLoop(this Face face, EdgeArray edgeArray)
    {
        return new(face.GetEdgePoints(edgeArray));
    }

    public static List<UVLoop> GetUVLoops(this Face face)
    {
        List<UVLoop> r = [];
        var edgeLoops = face.EdgeLoops;
        foreach (EdgeArray edgeArray in edgeLoops)
            r.Add(GetUVLoop(face, edgeArray));
        return r;
    }

    public static FaceData ToData(this Face face)
    {
        var bounds = face.GetBoundingBox();
        var regions = face.HasRegions ? face.GetRegions().Select(ToData).ToList() : [];
        return new(
            face.Id,
            bounds.Min.ToData(),
            bounds.Max.ToData(),
            GetUVLoops(face),
            face.get_IsCyclic(0),
            face.get_IsCyclic(1),
            face.get_IsCyclic(0) ? face.get_Period(0) : 0.0,
            face.get_IsCyclic(1) ? face.get_Period(1) : 0.0,
            regions,
            face.GetFaceType(),
            face.OrientationMatchesSurfaceOrientation,
            face.IsTwoSided,
            face.ToSpecificFaceData());
    }

    public static CurveData ToData(this Curve self)
    {
        var points = self.IsBound ? self.Tessellate().ToData() : null;
        return new(self.ApproximateLength, self.IsBound, self.IsClosed, self.IsCyclic, self.Period, points);
    }

    public static TransformData ToData(this Transform self)
    {
        return self.IsIdentity
            ? null
            : new TransformData(
            self.Origin.ToData(),
            self.BasisX.ToData(),
            self.BasisY.ToData(),
            self.BasisZ.ToData(),
            self.HasReflection,
            self.IsConformal,
            self.IsTranslation);
    }

    public static string GetUniqueIdString(this GeometryInstance self)
    {
        return self.GetSymbolGeometryId().AsUniqueIdentifier();
    }

    public static TransformData GetTransform(this GeometryInstance self)
    {
        return self.Transform.ToData();
    }

    public static int ProcessGeometryElement(this GeometryElement self, GeometryData data)
    {
        var childIds = self.Select(g => ProcessGeometryObject(g, data)).Where(index => index >= 0).ToList();
        if (childIds.Count == 0) return -1;
        GeometryElementData tmp = new(self.Id, childIds);
        return data.Add(tmp);
    }

    public static int NotProcessedMessage<T>(T self)
    {
        Debug.WriteLine($"Object {self} of type {typeof(T)} is not processed");
        return -1;
    }

    public static int ProcessFace(this Face self, GeometryData data)
    {
        return data.Add(self.ToData());
    }

    public static int ProcessGeometryInstance(this GeometryInstance self, GeometryData data)
    {
        GeometryInstanceData r = new(self.Id, self.GetTransform(), self.GetUniqueIdString());
        if (!data.SymbolIdsToGeometry.ContainsKey(r.SymbolUniqueId))
            ProcessSymbolGeometry(self.SymbolGeometry, r.SymbolUniqueId, data);
        return data.Add(r);
    }

    public static int ProcessSolid(this Solid self, GeometryData data)
    {
        List<FaceData> faces = [];
        foreach (Face face in self.Faces)
            faces.Add(face.ToData());
        var faceIds = data.AddRange(faces);
        SolidGeometryData tmp = new(self.Id, faceIds);
        return data.Add(tmp);
    }

    public static MeshData ToData(this RevitMesh mesh)
    {
        var numTris = mesh.NumTriangles;
        var points = mesh.Vertices.ToData();
        List<int> indices = [];
        for (var i = 0; i < numTris; i++)
        {
            var tri = mesh.get_Triangle(i);
            indices.Add((int)tri.get_Index(0));
            indices.Add((int)tri.get_Index(1));
            indices.Add((int)tri.get_Index(2));
        }

        return new MeshData(mesh.Id, points, indices);
    }

    public static int ProcessMesh(this RevitMesh mesh, GeometryData data)
    {
        return data.Add(mesh.ToData());
    }

    public static void ProcessSymbolGeometry(this GeometryElement element, string id, GeometryData data)
    {
        if (data.SymbolIdsToGeometry.ContainsKey(id))
            throw new Exception("Internal error: ID already exists.");

        var index = ProcessGeometryElement(element, data);
        data.SymbolIdsToGeometry.Add(id, index);
    }

    public static int ProcessGeometryObject(this GeometryObject self, GeometryData data)
    {
        return self switch
        {
            Arc arc => NotProcessedMessage(arc),
            CylindricalHelix cylindricalHelix => NotProcessedMessage(cylindricalHelix),
            Ellipse ellipse => NotProcessedMessage(ellipse),
            HermiteSpline hermiteSpline => NotProcessedMessage(hermiteSpline),
            Line line => NotProcessedMessage(line),
            NurbSpline nurbSpline => NotProcessedMessage(nurbSpline),
            Curve curve => NotProcessedMessage(curve),
            Edge edge => NotProcessedMessage(edge),
            Face face => ProcessFace(face, data),
            GeometryElement geometryElement => ProcessGeometryElement(geometryElement, data),
            GeometryInstance geometryInstance => ProcessGeometryInstance(geometryInstance, data),
            Autodesk.Revit.DB.Mesh mesh => ProcessMesh(mesh, data),
            Point point => NotProcessedMessage(point),
            PolyLine polyLine => NotProcessedMessage(polyLine),
            Profile profile => NotProcessedMessage(profile),
            Solid solid => ProcessSolid(solid, data),
            _ => throw new ArgumentOutOfRangeException(nameof(self)),
        };
        return -1;
    }

    public static void ProcessElement(this Element e, GeometryData data, Options options)
    {
        try
        {
            if (e == null || e.get_BoundingBox(null) == null)
                return;
            var geo = e.get_Geometry(options);
            if (geo == null) return;
            var tmp = ProcessGeometryElement(geo, data);
            var transform = e is Instance inst
                ? inst.GetTransform().ToData()
                : null;
            ElementData ed = new(e.Id.Value, tmp, transform);
            data.Elements.Add(ed);
        }
        catch (Exception)
        {
            Debug.WriteLine($"Error occured while processing element {e.Id.Value}");
            Debugger.Break();
        }
    }

    public static void ProcessGeometryElements(this Document doc, GeometryData data, Options options)
    {
        var collector = new FilteredElementCollector(doc)
            .WhereElementIsNotElementType();

        foreach (var e in collector)
            ProcessElement(e, data, options);
    }

    public static void ProcessDocument(this Document doc, DocumentData data, Options options)
    {
        data.DocumentName = doc.PathName;
        ProcessGeometryElements(doc, data.Geometry, options);

        foreach (var link in new FilteredElementCollector(doc)
                     .OfClass(typeof(RevitLinkInstance))
                     .OfType<RevitLinkInstance>())
        {
            var linkDoc = link.GetLinkDocument();
            if (linkDoc == null) continue;
            DocumentData childDoc = new();
            data.Documents.Add(childDoc);
            childDoc.Transform = link.GetTransform().ToData();
            ProcessDocument(linkDoc, childDoc, options);
        }
    }

    public static DocumentData ProcessDocumentBrep(this Document doc)
    {
        Options options = new()
        {
            DetailLevel = ViewDetailLevel.Fine,
            IncludeNonVisibleObjects = false,
            ComputeReferences = false // huge: speed-up unless you need reference objects
        };

        DocumentData dd = new();
        ProcessDocument(doc, dd, options);
        return dd;
    }
}