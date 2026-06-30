using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class McpResourceProvider
{
    readonly McpToolRegistry _registry;
    static string _latestViewImagePath;

    public McpResourceProvider(McpToolRegistry registry) => _registry = registry;

    public static void SetLatestViewImage(string path) => _latestViewImagePath = path;

    public JObject ListResources()
    {
        var resources = new List<JObject>
        {
            Resource("dev://catalog/commands", "Command catalog index from COMMANDS.md", "text/markdown"),
            Resource("dev://catalog/mcp-tools", "MCP tool descriptor manifest summary", "application/json"),
            Resource("dev://tools/catalog/implemented", "Summary of fully implemented MCP tools", "application/json"),
            Resource("aec://document/current/summary", "Live model summary", "application/json"),
            Resource("aec://view/active/image", "Latest captured view image path", "text/plain"),
            Resource("aec://model/levels", "All levels with elevations", "application/json"),
            Resource("aec://model/rooms/summary", "Room count, total area, unplaced count", "application/json"),
            Resource("aec://model/warnings/summary", "Warning counts by severity", "application/json"),
            Resource("aec://model/sheets", "All sheets with placed views", "application/json"),
            Resource("aec://model/materials", "All materials with properties", "application/json"),
            Resource("aec://model/grids-levels", "Grid lines and levels for spatial orientation", "application/json"),
            Resource("aec://exports/last", "Most recent export path and metadata", "application/json"),
            Resource("aec://model/types/{category}", "Family types for a category (dynamic URI)", "application/json"),
            Resource("aec://model/schedule/{name}", "Live schedule data as JSON (dynamic URI)", "application/json"),
            Resource("standards://firm/index", "Firm standards markdown index", "application/json"),
            Resource("audit://agent/history", "Recent agent tool audit entries", "application/json"),
            Resource("audit://changesets/{changeSetId}", "Change set audit record (dynamic URI)", "application/json"),
            Resource("audit://transactions/{transactionId}", "Applied transaction audit record (dynamic URI)", "application/json"),
        };

        var standardsRoot = RepoPaths.StandardsRoot(_registry.Context?.RepoRoot);
        if (!string.IsNullOrEmpty(standardsRoot) && Directory.Exists(standardsRoot))
        {
            foreach (var file in Directory.EnumerateFiles(standardsRoot, "*.md", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(standardsRoot, file).Replace('\\', '/');
                resources.Add(Resource($"standards://firm/{rel}", $"Standard: {Path.GetFileNameWithoutExtension(file)}", "text/markdown"));
            }
        }

        return new JObject { ["resources"] = new JArray(resources) };
    }

    public JObject ReadResource(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
            throw new McpProtocolException(-32602, "uri is required");

        var context = _registry.Context;
        context?.Catalog.EnsureLoaded(context.RepoRoot);

        return uri switch
        {
            "dev://catalog/commands" => TextResource(uri, ReadCommandsCatalog(context)),
            "dev://catalog/mcp-tools" => JsonResource(uri, ReadDescriptorCatalog(context)),
            "dev://tools/catalog/implemented" => JsonResource(uri, ReadImplementedToolsCatalog()),
            "aec://document/current/summary" => JsonResource(uri, ReadDocumentSummary()),
            "aec://view/active/image" => TextResource(uri, _latestViewImagePath ?? "(no image captured yet)"),
            "aec://model/levels" => JsonResource(uri, ReadModelLevels()),
            "aec://model/rooms/summary" => JsonResource(uri, ReadRoomsSummary()),
            "aec://model/warnings/summary" => JsonResource(uri, ReadWarningsSummary()),
            "aec://model/sheets" => JsonResource(uri, ReadSheetsSummary()),
            "aec://model/materials" => JsonResource(uri, ReadMaterialsSummary()),
            "aec://model/grids-levels" => JsonResource(uri, ReadGridsLevelsSummary()),
            "aec://exports/last" => JsonResource(uri, ReadLastExport()),
            "standards://firm/index" => JsonResource(uri, ReadStandardsIndex(context)),
            "audit://agent/history" => JsonResource(uri, context?.Audit.GetEntries().ToString()),
            _ when uri.StartsWith("audit://changesets/", StringComparison.OrdinalIgnoreCase)
                => JsonResource(uri, ReadChangeSetAudit(context, uri["audit://changesets/".Length..])),
            _ when uri.StartsWith("audit://transactions/", StringComparison.OrdinalIgnoreCase)
                => JsonResource(uri, ReadTransactionAudit(context, uri["audit://transactions/".Length..])),
            _ when uri.StartsWith("standards://firm/", StringComparison.OrdinalIgnoreCase)
                => TextResource(uri, ReadStandardsFile(context, uri["standards://firm/".Length..])),
            _ when uri.StartsWith("dev://samples/", StringComparison.OrdinalIgnoreCase)
                => TextResource(uri, ReadSampleDoc(context, uri["dev://samples/".Length..])),
            _ when uri.StartsWith("aec://model/types/", StringComparison.OrdinalIgnoreCase)
                => JsonResource(uri, ReadModelTypes(uri["aec://model/types/".Length..])),
            _ when uri.StartsWith("aec://model/schedule/", StringComparison.OrdinalIgnoreCase)
                => JsonResource(uri, ReadScheduleResource(uri["aec://model/schedule/".Length..])),
            _ => throw new McpProtocolException(-32602, $"Unknown resource: {uri}"),
        };
    }

    static JObject Resource(string uri, string description, string mimeType)
        => new()
        {
            ["uri"] = uri,
            ["name"] = uri,
            ["description"] = description,
            ["mimeType"] = mimeType,
        };

    static JObject TextResource(string uri, string text)
        => new()
        {
            ["contents"] = new JArray
            {
                new JObject
                {
                    ["uri"] = uri,
                    ["mimeType"] = "text/plain",
                    ["text"] = text ?? "",
                },
            },
        };

    static JObject JsonResource(string uri, string json)
        => TextResource(uri, json);

    string ReadCommandsCatalog(HostContext context)
    {
        var path = string.IsNullOrEmpty(context?.RepoRoot)
            ? null
            : Path.Combine(context.RepoRoot, "COMMANDS.md");
        return path != null && File.Exists(path) ? File.ReadAllText(path) : "COMMANDS.md not found.";
    }

    string ReadDescriptorCatalog(HostContext context)
    {
        context?.Catalog.EnsureLoaded(context.RepoRoot);
        var items = context?.Catalog.Entries
            .Where(e => !string.IsNullOrEmpty(e.DescriptorPath))
            .Select(e => new JObject
            {
                ["name"] = e.Name,
                ["sample"] = e.Sample,
                ["mcp_rating"] = e.McpRating,
                ["requires_ui"] = e.RequiresUi,
                ["descriptor_path"] = e.DescriptorPath,
            })
            .ToList() ?? [];

        return new JObject
        {
            ["count"] = items.Count,
            ["tools"] = new JArray(items),
        }.ToString();
    }

    string ReadDocumentSummary()
    {
        var context = _registry.Context;
        if (context?.Bridge == null)
            return new JObject { ["message"] = "Revit bridge not initialized" }.ToString();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return McpToolResult.Failure("No document open", "no_document");

            return McpToolResult.Success(new JObject
            {
                ["title"] = doc.Title,
                ["path"] = doc.PathName,
                ["is_workshared"] = doc.IsWorkshared,
                ["element_count"] = doc.GetElements().Count,
                ["room_count"] = doc.GetRooms().Count(),
            });
        });

        return result.ToEnvelope().ToString();
    }

    string ReadStandardsIndex(HostContext context)
    {
        var root = RepoPaths.StandardsRoot(context?.RepoRoot);
        if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            return new JArray().ToString();

        var items = Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories)
            .Select(f => new JObject
            {
                ["uri"] = $"standards://firm/{Path.GetRelativePath(root, f).Replace('\\', '/')}",
                ["path"] = f,
            })
            .ToList();
        return new JArray(items).ToString();
    }

    static string ReadStandardsFile(HostContext context, string relativePath)
    {
        var root = RepoPaths.StandardsRoot(context?.RepoRoot);
        var full = Path.GetFullPath(Path.Combine(root ?? "", relativePath));
        if (string.IsNullOrEmpty(root) || !full.StartsWith(Path.GetFullPath(root), StringComparison.OrdinalIgnoreCase))
            throw new McpProtocolException(-32602, "Invalid standards path.");
        return File.Exists(full) ? File.ReadAllText(full) : "Standard not found.";
    }

    static string ReadSampleDoc(HostContext context, string slugPath)
    {
        context?.Catalog.EnsureLoaded(context.RepoRoot);
        var entry = context?.Catalog.Entries.FirstOrDefault(e =>
            e.DocPath != null && e.DocPath.Replace('\\', '/').EndsWith(slugPath, StringComparison.OrdinalIgnoreCase));
        if (entry == null || !File.Exists(entry.DocPath))
            return "Sample doc not found.";
        return File.ReadAllText(entry.DocPath);
    }

    string ReadImplementedToolsCatalog()
    {
        var items = (_registry.Context?.ToolNames ?? [])
            .Select(name => new JObject { ["name"] = name })
            .ToList();
        return new JObject { ["count"] = items.Count, ["tools"] = new JArray(items) }.ToString();
    }

    string RunOnRevitJson(Func<Document, JObject> work, JObject fallback = null)
    {
        var context = _registry.Context;
        if (context?.Bridge == null)
            return (fallback ?? new JObject { ["message"] = "Revit bridge not initialized" }).ToString();

        var result = context.Bridge.RunOnRevitThread(app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null) return McpToolResult.Failure("No document open", "no_document");
            return McpToolResult.Success(work(doc));
        });
        return result.ToEnvelope().ToString();
    }

    string ReadModelLevels()
        => RunOnRevitJson(doc =>
        {
            var levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>()
                .OrderBy(l => l.Elevation)
                .Select(l => new JObject
                {
                    ["level_id"] = l.Id.Value,
                    ["name"] = l.Name,
                    ["elevation"] = l.Elevation,
                }).ToList();
            return new JObject { ["count"] = levels.Count, ["levels"] = new JArray(levels) };
        });

    string ReadRoomsSummary()
        => RunOnRevitJson(doc =>
        {
            var rooms = doc.GetRooms().ToList();
            var unplaced = rooms.Count(r => r.Area <= 0 || r.Location == null);
            return new JObject
            {
                ["room_count"] = rooms.Count,
                ["unplaced_count"] = unplaced,
                ["total_area"] = rooms.Where(r => r.Area > 0).Sum(r => r.Area),
            };
        });

    string ReadWarningsSummary()
        => RunOnRevitJson(doc =>
        {
            var warnings = doc.GetWarnings().ToList();
            var bySeverity = warnings.GroupBy(w => w.GetSeverity().ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            return new JObject
            {
                ["total"] = warnings.Count,
                ["by_severity"] = JObject.FromObject(bySeverity),
            };
        });

    string ReadSheetsSummary()
        => RunOnRevitJson(doc =>
        {
            var sheets = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).Cast<ViewSheet>()
                .Select(s => new JObject
                {
                    ["sheet_id"] = s.Id.Value,
                    ["sheet_number"] = s.SheetNumber,
                    ["name"] = s.Name,
                    ["placed_views"] = new JArray(s.GetAllPlacedViews().Select(id => id.Value)),
                }).ToList();
            return new JObject { ["count"] = sheets.Count, ["sheets"] = new JArray(sheets) };
        });

    string ReadMaterialsSummary()
        => RunOnRevitJson(doc =>
        {
            var materials = new FilteredElementCollector(doc).OfClass(typeof(Material)).Cast<Material>()
                .Select(m => new JObject
                {
                    ["material_id"] = m.Id.Value,
                    ["name"] = m.Name,
                    ["color"] = m.Color.IsValid ? $"{m.Color.Red},{m.Color.Green},{m.Color.Blue}" : null,
                }).ToList();
            return new JObject { ["count"] = materials.Count, ["materials"] = new JArray(materials) };
        });

    string ReadGridsLevelsSummary()
        => RunOnRevitJson(doc =>
        {
            var grids = new FilteredElementCollector(doc).OfClass(typeof(Grid)).Cast<Grid>()
                .Select(g => new JObject { ["grid_id"] = g.Id.Value, ["name"] = g.Name }).ToList();
            var levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>()
                .Select(l => new JObject { ["level_id"] = l.Id.Value, ["name"] = l.Name, ["elevation"] = l.Elevation }).ToList();
            return new JObject { ["grids"] = new JArray(grids), ["levels"] = new JArray(levels) };
        });

    static string ReadLastExport()
        => new JObject
        {
            ["path"] = McpExportTracker.LastExportPath,
            ["meta"] = McpExportTracker.LastExportMeta ?? new JObject(),
        }.ToString();

    string ReadModelTypes(string categoryName)
        => RunOnRevitJson(doc =>
        {
            var cat = McpHandlerHelpers.FindCategory(doc, categoryName);
            if (cat == null) return new JObject { ["error"] = "category_not_found" };

            var types = new FilteredElementCollector(doc).OfCategoryId(cat.Id).WhereElementIsElementType()
                .Select(t => new JObject
                {
                    ["type_id"] = t.Id.Value,
                    ["name"] = t.Name,
                    ["family_name"] = (t as FamilySymbol)?.FamilyName,
                }).ToList();
            return new JObject { ["category"] = cat.Name, ["count"] = types.Count, ["types"] = new JArray(types) };
        });

    string ReadScheduleResource(string scheduleName)
        => RunOnRevitJson(doc =>
        {
            var schedule = McpHandlerHelpers.FindScheduleByName(doc, scheduleName);
            if (schedule == null) return new JObject { ["error"] = "schedule_not_found" };
            var rows = ScheduleMcpHelpers.ReadScheduleRows(schedule);
            return new JObject
            {
                ["schedule"] = schedule.Name,
                ["row_count"] = rows.Count,
                ["rows"] = rows,
            };
        });

    static string ReadChangeSetAudit(HostContext context, string changeSetId)
    {
        var set = context?.ChangeSetSession?.Get(changeSetId);
        if (set == null)
            return new JObject { ["error"] = "change_set_not_found", ["change_set_id"] = changeSetId }.ToString();
        return context.ChangeSetSession.ToAuditJson(set).ToString();
    }

    static string ReadTransactionAudit(HostContext context, string transactionId)
    {
        var tx = context?.ChangeSetSession?.GetTransaction(transactionId);
        if (tx == null)
            return new JObject { ["error"] = "transaction_not_found", ["transaction_id"] = transactionId }.ToString();
        return context.ChangeSetSession.TransactionToAuditJson(tx).ToString();
    }
}
