using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ara3D.Bowerbird.RevitSamples;

internal static class McpExportTracker
{
    public static string LastExportPath { get; private set; }
    public static JObject LastExportMeta { get; private set; } = new();

    public static void Record(string path, JObject meta = null)
    {
        LastExportPath = path;
        LastExportMeta = meta ?? new JObject { ["path"] = path, ["timestamp"] = DateTime.UtcNow.ToString("o") };
    }
}

internal static class McpHandlerHelpers
{
    public static Document GetDoc(UIApplication app) => app.ActiveUIDocument?.Document;

    public static McpToolResult NoDocument() => McpToolResult.Failure("No document open", "no_document");

    public static McpToolResult RunTransaction(Document doc, string name, Func<McpToolResult> work)
    {
        using var tx = new Transaction(doc, name);
        tx.Start();
        try
        {
            var result = work();
            if (!result.Ok)
            {
                tx.RollBack();
                return result;
            }

            tx.Commit();
            return result;
        }
        catch (Exception ex)
        {
            if (tx.HasStarted() && tx.GetStatus() == TransactionStatus.Started)
                tx.RollBack();
            return McpToolResult.Failure(ex.Message, "transaction_failed");
        }
    }

    public static Level FindLevel(Document doc, string levelName)
    {
        if (string.IsNullOrWhiteSpace(levelName))
            return new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>()
                .OrderBy(l => l.Elevation).FirstOrDefault();

        return new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>()
            .FirstOrDefault(l => string.Equals(l.Name, levelName, StringComparison.OrdinalIgnoreCase)
                || l.Name.IndexOf(levelName, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    public static WallType FindWallType(Document doc, string typeName)
    {
        var types = new FilteredElementCollector(doc).OfClass(typeof(WallType)).Cast<WallType>();
        if (!string.IsNullOrWhiteSpace(typeName))
            return types.FirstOrDefault(t => string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase));
        return types.FirstOrDefault();
    }

    public static FloorType FindFloorType(Document doc, string typeName)
    {
        var types = new FilteredElementCollector(doc).OfClass(typeof(FloorType)).Cast<FloorType>();
        if (!string.IsNullOrWhiteSpace(typeName))
            return types.FirstOrDefault(t => string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase));
        return types.FirstOrDefault();
    }

    public static CeilingType FindCeilingType(Document doc, string typeName)
    {
        var types = new FilteredElementCollector(doc).OfClass(typeof(CeilingType)).Cast<CeilingType>();
        if (!string.IsNullOrWhiteSpace(typeName))
            return types.FirstOrDefault(t => string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase));
        return types.FirstOrDefault();
    }

    public static FamilySymbol FindFamilySymbol(Document doc, string familyName, string typeName, BuiltInCategory? category = null)
    {
        var symbols = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>();
        if (category.HasValue)
            symbols = symbols.Where(s => s.Category != null && s.Category.Id.Value == (long)category.Value);

        if (!string.IsNullOrWhiteSpace(familyName) && !string.IsNullOrWhiteSpace(typeName))
        {
            return symbols.FirstOrDefault(s =>
                string.Equals(s.FamilyName, familyName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(s.Name, typeName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(typeName))
            return symbols.FirstOrDefault(s => string.Equals(s.Name, typeName, StringComparison.OrdinalIgnoreCase));

        return symbols.FirstOrDefault();
    }

    public static Category FindCategory(Document doc, string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName)) return null;
        if (Enum.TryParse<BuiltInCategory>(categoryName, true, out var bic))
            return Category.GetCategory(doc, bic);

        return doc.Settings.Categories.Cast<Category>()
            .FirstOrDefault(c => string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase));
    }

    public static View FindViewByName(Document doc, string viewName)
        => new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>()
            .FirstOrDefault(v => !v.IsTemplate && string.Equals(v.Name, viewName, StringComparison.OrdinalIgnoreCase));

    public static ViewSchedule FindScheduleByName(Document doc, string scheduleName)
        => new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>()
            .FirstOrDefault(s => string.Equals(s.Name, scheduleName, StringComparison.OrdinalIgnoreCase));

    public static ViewSheet FindSheetByName(Document doc, string sheetName)
        => new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).Cast<ViewSheet>()
            .FirstOrDefault(s => string.Equals(s.Name, sheetName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(s.SheetNumber, sheetName, StringComparison.OrdinalIgnoreCase));

    public static View FindViewByIdOrName(Document doc, string viewRef, UIApplication app)
    {
        if (!string.IsNullOrWhiteSpace(viewRef))
        {
            if (long.TryParse(viewRef, out var id))
            {
                var view = doc.GetElement(new ElementId(id)) as View;
                if (view != null) return view;
            }

            var byName = FindViewByName(doc, viewRef);
            if (byName != null) return byName;
        }

        return doc.ActiveView;
    }

    public static XYZ ParsePoint(JToken token, double defaultZ = 0)
    {
        if (token is JArray arr && arr.Count >= 2)
        {
            var x = arr[0].Value<double>();
            var y = arr[1].Value<double>();
            var z = arr.Count > 2 ? arr[2].Value<double>() : defaultZ;
            return new XYZ(x, y, z);
        }

        if (token is JObject obj)
            return new XYZ(obj["x"]?.Value<double>() ?? 0, obj["y"]?.Value<double>() ?? 0,
                obj["z"]?.Value<double>() ?? defaultZ);

        return XYZ.Zero;
    }

    public static List<XYZ> ParsePoints(JArray points, double defaultZ = 0)
        => points?.Select(p => ParsePoint(p, defaultZ)).Where(p => p != null).ToList() ?? [];

    public static CurveLoop CreateRectLoop(double width, double depth, XYZ origin)
    {
        var p0 = origin;
        var p1 = origin + new XYZ(width, 0, 0);
        var p2 = origin + new XYZ(width, depth, 0);
        var p3 = origin + new XYZ(0, depth, 0);
        var loop = new CurveLoop();
        loop.Append(Line.CreateBound(p0, p1));
        loop.Append(Line.CreateBound(p1, p2));
        loop.Append(Line.CreateBound(p2, p3));
        loop.Append(Line.CreateBound(p3, p0));
        return loop;
    }

    public static IList<CurveLoop> ParseProfile(JArray profilePoints, double defaultZ = 0)
    {
        var pts = ParsePoints(profilePoints, defaultZ);
        if (pts.Count < 3)
            throw new InvalidOperationException("Profile requires at least 3 points.");

        var loop = new CurveLoop();
        for (var i = 0; i < pts.Count; i++)
        {
            var a = pts[i];
            var b = pts[(i + 1) % pts.Count];
            loop.Append(Line.CreateBound(a, b));
        }

        return [loop];
    }

    public static void AccumulateSolidMetrics(GeometryElement geom, ref double volume, ref double surface)
    {
        if (geom == null) return;
        foreach (var obj in geom)
        {
            if (obj is Solid solid && solid.Volume > 0)
            {
                volume += solid.Volume;
                surface += solid.SurfaceArea;
            }
            else if (obj is GeometryInstance inst)
            {
                AccumulateSolidMetrics(inst.GetInstanceGeometry(), ref volume, ref surface);
            }
        }
    }

    public static FaceArray CollectPlanarFaces(Element host)
    {
        var faces = new FaceArray();
        var geom = host.get_Geometry(new Options { ComputeReferences = true });
        if (geom == null) return faces;
        foreach (var obj in geom)
        {
            if (obj is not Solid solid) continue;
            foreach (Face face in solid.Faces)
            {
                if (face is PlanarFace && face.Reference != null)
                    faces.Append(face);
            }
        }
        return faces;
    }

    public static ElementId CreateDirectShape(Document doc, Solid solid, BuiltInCategory category = BuiltInCategory.OST_GenericModel)
    {
        var ds = DirectShape.CreateElement(doc, new ElementId(category));
        ds.ApplicationId = "BowerbirdMcp";
        ds.ApplicationDataId = Guid.NewGuid().ToString("N");
        ds.SetShape([solid]);
        return ds.Id;
    }

    public static Solid CreateBoxSolid(XYZ origin, double width, double depth, double height)
    {
        var profile = CreateRectLoop(width, depth, origin);
        return GeometryCreationUtilities.CreateExtrusionGeometry([profile], XYZ.BasisZ, height);
    }

    public static Solid CreateCylinderSolid(XYZ baseCenter, double radius, double height)
    {
        var loop = new CurveLoop();
        var sides = 16;
        var pts = Enumerable.Range(0, sides).Select(i =>
        {
            var angle = 2 * Math.PI * i / sides;
            return baseCenter + new XYZ(radius * Math.Cos(angle), radius * Math.Sin(angle), 0);
        }).ToList();
        for (var i = 0; i < sides; i++)
            loop.Append(Line.CreateBound(pts[i], pts[(i + 1) % sides]));
        return GeometryCreationUtilities.CreateExtrusionGeometry([loop], XYZ.BasisZ, height);
    }

    public static JObject BboxToJson(BoundingBoxXYZ bbox)
    {
        if (bbox == null) return null;
        return new JObject
        {
            ["min"] = new JArray(bbox.Min.X, bbox.Min.Y, bbox.Min.Z),
            ["max"] = new JArray(bbox.Max.X, bbox.Max.Y, bbox.Max.Z),
        };
    }

    public static string EnsureOutputPath(string outputPath, string defaultName, string extension)
    {
        if (!string.IsNullOrWhiteSpace(outputPath))
            return outputPath;

        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BowerbirdExports");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, $"{SanitizeFileName(defaultName)}{extension}");
    }

    public static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "export";
        foreach (var ch in Path.GetInvalidFileNameChars())
            name = name.Replace(ch, '_');
        return name;
    }

    public static bool TrySetParameter(Parameter param, JToken value, out string error)
    {
        error = null;
        if (param == null || param.IsReadOnly)
        {
            error = "Parameter missing or read-only.";
            return false;
        }

        try
        {
            switch (param.StorageType)
            {
                case StorageType.String:
                    return param.Set(value?.ToString() ?? "");
                case StorageType.Integer:
                    return param.Set(value.Type == JTokenType.Boolean ? (value.Value<bool>() ? 1 : 0) : value.Value<int>());
                case StorageType.Double:
                    return param.Set(value.Value<double>());
                case StorageType.ElementId:
                    if (long.TryParse(value?.ToString(), out var id))
                        return param.Set(new ElementId(id));
                    error = "ElementId parameter requires numeric id.";
                    return false;
                default:
                    error = $"Unsupported storage type: {param.StorageType}";
                    return false;
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    public static string GetParameterText(Parameter param)
    {
        if (param == null || !param.HasValue) return null;
        return param.StorageType switch
        {
            StorageType.String => param.AsString(),
            StorageType.Integer => param.AsInteger().ToString(CultureInfo.InvariantCulture),
            StorageType.Double => param.AsValueString() ?? param.AsDouble().ToString(CultureInfo.InvariantCulture),
            StorageType.ElementId => param.AsElementId()?.Value.ToString(CultureInfo.InvariantCulture),
            _ => param.AsValueString(),
        };
    }

    public static IEnumerable<Element> ElementsByCategory(Document doc, string categoryName)
    {
        var cat = FindCategory(doc, categoryName);
        if (cat == null) return [];
        return new FilteredElementCollector(doc).OfCategoryId(cat.Id).WhereElementIsNotElementType().ToElements();
    }

    public static bool ParameterMatches(Parameter param, string value, bool useRegex)
    {
        var text = GetParameterText(param);
        if (string.IsNullOrEmpty(text)) return string.IsNullOrEmpty(value);
        if (useRegex) return Regex.IsMatch(text, value, RegexOptions.IgnoreCase);
        return string.Equals(text, value, StringComparison.OrdinalIgnoreCase)
            || text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static ViewFamilyType FindViewFamilyType(Document doc, ViewFamily family)
        => new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>()
            .FirstOrDefault(vft => vft.ViewFamily == family);

    public static FamilySymbol FindTitleBlock(Document doc, string name = null)
    {
        var symbols = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks)
            .OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>();
        if (!string.IsNullOrWhiteSpace(name))
            return symbols.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)
                || string.Equals(s.FamilyName, name, StringComparison.OrdinalIgnoreCase));
        return symbols.FirstOrDefault();
    }

    public static JObject ElementRef(Element e)
        => new()
        {
            ["element_id"] = e.Id.Value,
            ["unique_id"] = e.UniqueId,
            ["name"] = e.Name,
            ["category"] = e.Category?.Name,
        };
}
