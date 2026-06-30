using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class DescriptorToolAdapter : ToolHandlerBase
{
    readonly McpToolDescriptor _descriptor;
    readonly JObject _manifestEntry;
    readonly IToolHandler _inner;

    public DescriptorToolAdapter(McpToolDescriptor descriptor, JObject manifestEntry, IToolHandler inner = null)
    {
        _descriptor = descriptor;
        _manifestEntry = manifestEntry;
        _inner = inner;
    }

    public override McpToolDescriptor Descriptor => _descriptor;

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        if (_inner != null)
            return _inner.InvokeAsync(arguments, context);

        return Task.FromResult(McpToolResult.Failure(
            $"Descriptor tool '{_descriptor.Name}' is cataloged but not yet wired to a headless adapter.",
            "not_implemented",
            new JObject
            {
                ["sample"] = _manifestEntry?["sample"]?.Value<string>(),
                ["source"] = _manifestEntry?["source"]?.Value<string>(),
                ["requires_ui"] = _manifestEntry?["requiresUi"]?.Value<bool?>(),
            }));
    }

    public static McpToolDescriptor CreateDescriptor(JObject item)
    {
        var args = item["arguments"] as JObject ?? new JObject { ["type"] = "object" };
        return new McpToolDescriptor
        {
            Name = item["name"]?.Value<string>(),
            Description = item["description"]?.Value<string>(),
            InputSchema = args,
            Tier = item["tier"]?.Value<string>() ?? "descriptor",
            Source = item["source"]?.Value<string>(),
            RequiresRevit = item["requiresRevit"]?.Value<bool?>() ?? true,
            RiskClass = Enum.TryParse<ToolRiskClass>(item["riskClass"]?.Value<string>(), true, out var risk)
                ? risk
                : ToolRiskClass.ReadOnly,
        };
    }
}
