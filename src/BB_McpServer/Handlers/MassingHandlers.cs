using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class CreateMassBoxHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_mass_box",
        "Create a conceptual mass box (DirectShape in Mass category) at a point.",
        Schema.ObjectWithRequired(
            [
                ("origin", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "number" }, ["minItems"] = 2 }),
                ("width", Schema.Integer("Width in feet")),
                ("depth", Schema.Integer("Depth in feet")),
                ("height", Schema.Integer("Height in feet")),
            ],
            "origin", "width", "depth", "height"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var origin = McpHandlerHelpers.ParsePoint(arguments["origin"]);
        var width = arguments["width"]?.Value<double>() ?? 40;
        var depth = arguments["depth"]?.Value<double>() ?? 30;
        var height = arguments["height"]?.Value<double>() ?? 60;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Mass Box", () =>
            {
                var solid = McpHandlerHelpers.CreateBoxSolid(origin, width, depth, height);
                var id = McpHandlerHelpers.CreateDirectShape(doc, solid, BuiltInCategory.OST_Mass);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = id.Value,
                    ["origin"] = new JArray(origin.X, origin.Y, origin.Z),
                    ["width"] = width,
                    ["depth"] = depth,
                    ["height"] = height,
                    ["note"] = "Created as DirectShape in Mass category for project-document compatibility.",
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateMassFromProfileHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_mass_from_profile",
        "Extrude a 2D profile polygon to a conceptual mass solid.",
        Schema.ObjectWithRequired(
            [
                ("profile", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "array" } }),
                ("height", Schema.Integer("Extrusion height in feet")),
                ("base_z", Schema.Integer("Base elevation Z")),
            ],
            "profile", "height"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var profile = arguments["profile"] as JArray;
        var height = arguments["height"]?.Value<double>() ?? 40;
        var baseZ = arguments["base_z"]?.Value<double>() ?? 0;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Mass From Profile", () =>
            {
                IList<CurveLoop> loops;
                try { loops = McpHandlerHelpers.ParseProfile(profile, baseZ); }
                catch (Exception ex) { return McpToolResult.Failure(ex.Message, "invalid_profile"); }

                var solid = GeometryCreationUtilities.CreateExtrusionGeometry(loops, XYZ.BasisZ, height);
                var id = McpHandlerHelpers.CreateDirectShape(doc, solid, BuiltInCategory.OST_Mass);
                return McpToolResult.Success(new JObject
                {
                    ["element_id"] = id.Value,
                    ["height"] = height,
                    ["profile_point_count"] = profile?.Count ?? 0,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class ApplyMassFloorsHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.apply_mass_floors",
        "Apply mass floor level data to a mass family instance for the given levels.",
        Schema.ObjectWithRequired(
            [
                ("mass_element_id", Schema.Integer("Mass family instance id")),
                ("level_names", Schema.StringArray("Level names to add as mass floors; all levels when omitted")),
            ],
            "mass_element_id"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var massId = new ElementId(arguments["mass_element_id"]?.Value<long>() ?? 0);
        var levelNames = (arguments["level_names"] as JArray ?? []).Select(t => t.Value<string>()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            if (doc.GetElement(massId) == null)
                return McpToolResult.Failure("Mass element not found.", "not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Apply Mass Floors", () =>
            {
                var levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>()
                    .Where(l => levelNames.Count == 0 || levelNames.Any(n => string.Equals(n, l.Name, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                if (levels.Count == 0)
                    return McpToolResult.Failure("No matching levels found.", "level_not_found");

                var applied = new JArray();
                foreach (var level in levels)
                {
                    MassInstanceUtils.AddMassLevelDataToMassInstance(doc, massId, level.Id);
                    applied.Add(new JObject { ["level_id"] = level.Id.Value, ["level_name"] = level.Name });
                }

                return McpToolResult.Success(new JObject
                {
                    ["mass_element_id"] = massId.Value,
                    ["applied_levels"] = applied,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class ComputeMassPropertiesHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.compute_mass_properties",
        "Compute volume, floor area, and surface area for mass or DirectShape elements.",
        Schema.ObjectWithRequired(
            [("element_ids", new JObject { ["type"] = "array", ["items"] = new JObject { ["type"] = "integer" } })],
            "element_ids"),
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var ids = (arguments["element_ids"] as JArray ?? []).Select(t => new ElementId(t.Value<long>())).ToList();
        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var view = doc.ActiveView;
            var items = new JArray();

            foreach (var id in ids)
            {
                var element = doc.GetElement(id);
                if (element == null) continue;
                double volume = 0, surface = 0;
                McpHandlerHelpers.AccumulateSolidMetrics(element.get_Geometry(new Options { View = view }), ref volume, ref surface);

                items.Add(new JObject
                {
                    ["element_id"] = id.Value,
                    ["name"] = element.Name,
                    ["volume"] = volume,
                    ["surface_area"] = surface,
                });
            }

            return McpToolResult.Success(new JObject { ["elements"] = items, ["count"] = items.Count });
        });
        return TaskResult(result);
    }
}

public sealed class CreateCurtainSystemOnFacesHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_curtain_system_on_faces",
        "Apply a curtain system to planar faces of a mass or generic model element.",
        Schema.ObjectWithRequired(
            [
                ("host_element_id", Schema.Integer("Host element id")),
                ("curtain_system_type", Schema.String("Curtain system type name (optional)")),
            ],
            "host_element_id"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var hostId = new ElementId(arguments["host_element_id"]?.Value<long>() ?? 0);
        var typeName = arguments["curtain_system_type"]?.Value<string>();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            var host = doc.GetElement(hostId);
            if (host == null) return McpToolResult.Failure("Host element not found.", "not_found");

            var curtainType = new FilteredElementCollector(doc).OfClass(typeof(CurtainSystemType)).Cast<CurtainSystemType>()
                .FirstOrDefault(t => string.IsNullOrWhiteSpace(typeName) || string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase));
            if (curtainType == null)
                return McpToolResult.Failure("Curtain system type not found.", "type_not_found");

            return McpHandlerHelpers.RunTransaction(doc, "MCP Create Curtain System", () =>
            {
                var faceArray = McpHandlerHelpers.CollectPlanarFaces(host);
                if (faceArray.Size == 0)
                    return McpToolResult.Failure("No planar face references found on host.", "no_faces");

                var cs = doc.Create.NewCurtainSystem(faceArray, curtainType);
                return McpToolResult.Success(new JObject
                {
                    ["curtain_system_id"] = cs?.Id.Value,
                    ["host_element_id"] = hostId.Value,
                    ["face_count"] = faceArray.Size,
                });
            });
        });
        return TaskResult(result);
    }
}

public sealed class CreateDividedSurfaceHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "aec.create_divided_surface",
        "Configure grid divisions on an existing divided surface hosted by a mass element.",
        Schema.ObjectWithRequired(
            [
                ("host_element_id", Schema.Integer("Host element id")),
                ("u_divisions", Schema.Integer("U grid count")),
                ("v_divisions", Schema.Integer("V grid count")),
            ],
            "host_element_id", "u_divisions", "v_divisions"),
        ToolRiskClass.Write,
        requiresRevit: true);

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var hostId = new ElementId(arguments["host_element_id"]?.Value<long>() ?? 0);
        var u = arguments["u_divisions"]?.Value<int>() ?? 10;
        var v = arguments["v_divisions"]?.Value<int>() ?? 10;

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = McpHandlerHelpers.GetDoc(app);
            if (doc == null) return McpHandlerHelpers.NoDocument();
            if (doc.GetElement(hostId) == null)
                return McpToolResult.Failure("Host element not found.", "not_found");

            var dividedSurfaces = new FilteredElementCollector(doc).OfClass(typeof(DividedSurface)).Cast<DividedSurface>().ToList();
            if (dividedSurfaces.Count == 0)
            {
                return McpToolResult.Failure(
                    "No divided surfaces found. Create mass geometry in a mass family or conceptual mass workflow first.",
                    "divided_surface_missing");
            }

            return McpHandlerHelpers.RunTransaction(doc, "MCP Configure Divided Surface", () =>
            {
                var target = dividedSurfaces.First();
                var uParam = target.LookupParameter("Number of U Divisions");
                var vParam = target.LookupParameter("Number of V Divisions");
                if (uParam != null && !uParam.IsReadOnly && uParam.StorageType == StorageType.Integer) uParam.Set(u);
                if (vParam != null && !vParam.IsReadOnly && vParam.StorageType == StorageType.Integer) vParam.Set(v);

                return McpToolResult.Success(new JObject
                {
                    ["divided_surface_id"] = target.Id.Value,
                    ["host_element_id"] = hostId.Value,
                    ["u_divisions"] = u,
                    ["v_divisions"] = v,
                    ["note"] = "Configured first available divided surface in the document.",
                });
            });
        });
        return TaskResult(result);
    }
}
