using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class SearchExamplesHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "dev.search_examples",
        "Search COMMANDS.md, command docs, descriptors, and sample metadata.",
        Schema.Object(
            ("query", Schema.String("Search terms")),
            ("limit", Schema.Integer("Maximum results"))),
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var query = arguments?["query"]?.Value<string>() ?? "";
        var limit = arguments?["limit"]?.Value<int?>() ?? 20;
        context.Catalog.EnsureLoaded(context.RepoRoot);
        var results = context.Catalog.Search(query, limit)
            .Select(e => new JObject
            {
                ["name"] = e.Name,
                ["sample"] = e.Sample,
                ["description"] = e.Description,
                ["category"] = e.Category,
                ["mcp_rating"] = e.McpRating,
                ["requires_ui"] = e.RequiresUi,
                ["doc_path"] = e.DocPath,
                ["descriptor_path"] = e.DescriptorPath,
            })
            .ToList();

        return TaskResult(McpToolResult.Success(new JObject
        {
            ["query"] = query,
            ["count"] = results.Count,
            ["results"] = new JArray(results),
        }));
    }
}

public sealed class StandardsSearchHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "standards.search",
        "Search firm and project standards markdown as citeable context.",
        Schema.Object(
            ("query", Schema.String("Search terms")),
            ("limit", Schema.Integer("Maximum results"))),
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var query = arguments?["query"]?.Value<string>() ?? "";
        var limit = arguments?["limit"]?.Value<int?>() ?? 10;
        var root = RepoPaths.StandardsRoot(context.RepoRoot);
        var results = new JArray();

        if (!string.IsNullOrEmpty(root) && Directory.Exists(root))
        {
            foreach (var file in Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories))
            {
                var text = File.ReadAllText(file);
                if (!string.IsNullOrWhiteSpace(query)
                    && text.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) < 0
                    && Path.GetFileName(file).IndexOf(query, System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                results.Add(new JObject
                {
                    ["path"] = file,
                    ["title"] = Path.GetFileNameWithoutExtension(file),
                    ["excerpt"] = text.Length > 500 ? text[..500] : text,
                    ["resource_uri"] = $"standards://{Path.GetRelativePath(root, file).Replace('\\', '/')}",
                });

                if (results.Count >= limit) break;
            }
        }

        return TaskResult(McpToolResult.Success(new JObject
        {
            ["query"] = query,
            ["count"] = results.Count,
            ["results"] = results,
        }));
    }
}
