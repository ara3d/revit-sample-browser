using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public interface IToolHandler
{
    McpToolDescriptor Descriptor { get; }
    Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context);
}

public abstract class ToolHandlerBase : IToolHandler
{
    public abstract McpToolDescriptor Descriptor { get; }
    public abstract Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context);

    protected static McpToolDescriptor Desc(
        string name,
        string description,
        JObject inputSchema,
        ToolRiskClass risk = ToolRiskClass.ReadOnly,
        int timeoutMs = 30_000,
        string tier = "semantic",
        bool requiresRevit = false)
        => new()
        {
            Name = name,
            Description = description,
            InputSchema = inputSchema ?? Schema.Object(),
            RiskClass = risk,
            TimeoutMs = timeoutMs,
            Tier = tier,
            RequiresRevit = requiresRevit,
        };

    protected static Task<McpToolResult> TaskResult(McpToolResult result)
        => Task.FromResult(result);
}

public static class Schema
{
    public static JObject Object(JObject properties, params string[] required)
        => new()
        {
            ["type"] = "object",
            ["properties"] = properties ?? new JObject(),
            ["required"] = new JArray(required ?? []),
        };

    public static JObject Object(params (string name, JObject prop)[] properties)
    {
        var props = new JObject();
        foreach (var (name, prop) in properties)
            props[name] = prop;
        return Object(props);
    }

    public static JObject ObjectReq((string name, JObject prop) property, string required)
    {
        var props = new JObject { [property.name] = property.prop };
        return Object(props, required);
    }

    public static JObject ObjectWithRequired((string name, JObject prop)[] properties, params string[] required)
    {
        var props = new JObject();
        foreach (var (name, prop) in properties)
            props[name] = prop;
        return Object(props, required);
    }

    public static JObject String(string description = null)
        => new()
        {
            ["type"] = "string",
            ["description"] = description,
        };

    public static JObject Integer(string description = null)
        => new()
        {
            ["type"] = "integer",
            ["description"] = description,
        };

    public static JObject StringArray(string description = null)
        => new()
        {
            ["type"] = "array",
            ["items"] = new JObject { ["type"] = "string" },
            ["description"] = description,
        };
}
