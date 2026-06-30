using Ara3D.Bowerbird.RevitSamples.AecAgent;
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
            Resource("aec://document/current/summary", "Live model summary", "application/json"),
            Resource("aec://view/active/image", "Latest captured view image path", "text/plain"),
            Resource("standards://firm/index", "Firm standards markdown index", "application/json"),
            Resource("audit://agent/history", "Recent agent tool audit entries", "application/json"),
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
            "aec://document/current/summary" => JsonResource(uri, ReadDocumentSummary()),
            "aec://view/active/image" => TextResource(uri, _latestViewImagePath ?? "(no image captured yet)"),
            "standards://firm/index" => JsonResource(uri, ReadStandardsIndex(context)),
            "audit://agent/history" => JsonResource(uri, context?.Audit.GetEntries().ToString()),
            _ when uri.StartsWith("standards://firm/", StringComparison.OrdinalIgnoreCase)
                => TextResource(uri, ReadStandardsFile(context, uri["standards://firm/".Length..])),
            _ when uri.StartsWith("dev://samples/", StringComparison.OrdinalIgnoreCase)
                => TextResource(uri, ReadSampleDoc(context, uri["dev://samples/".Length..])),
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
}
