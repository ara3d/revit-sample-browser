using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Ara3D.Bowerbird.RevitSamples.AecAgent;

public sealed class McpIssue
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string Severity { get; set; } = "warning";
}

public sealed class McpToolResult
{
    public bool Ok { get; set; } = true;
    public JToken Data { get; set; }
    public List<McpIssue> Issues { get; set; } = [];
    public JObject Meta { get; set; } = [];

    public static McpToolResult Success(JToken data = null, JObject meta = null)
        => new() { Ok = true, Data = data ?? new JObject(), Meta = meta ?? [] };

    public static McpToolResult Failure(string message, string code = "error", JToken data = null)
        => new()
        {
            Ok = false,
            Data = data ?? new JObject { ["message"] = message },
            Issues = [new McpIssue { Code = code, Message = message, Severity = "error" }],
        };

    public string ToDisplayText()
    {
        if (Data is JValue v)
            return v.ToString();

        return Data?.ToString(Newtonsoft.Json.Formatting.Indented)
            ?? (Ok ? "{}" : Issues.Count > 0 ? Issues[0].Message : "Unknown error");
    }

    public JObject ToEnvelope()
    {
        var issues = new JArray();
        foreach (var issue in Issues)
        {
            issues.Add(new JObject
            {
                ["code"] = issue.Code,
                ["message"] = issue.Message,
                ["severity"] = issue.Severity,
            });
        }

        return new JObject
        {
            ["ok"] = Ok,
            ["data"] = Data ?? new JObject(),
            ["issues"] = issues,
            ["meta"] = Meta ?? [],
        };
    }
}

public enum ToolRiskClass
{
    ReadOnly,
    ViewOverride,
    Write,
    Destructive,
}

public sealed class McpToolDescriptor
{
    public string Name { get; set; }
    public string Description { get; set; }
    public JObject InputSchema { get; set; }
    public ToolRiskClass RiskClass { get; set; } = ToolRiskClass.ReadOnly;
    public int TimeoutMs { get; set; } = 30_000;
    public string Tier { get; set; } = "semantic";
    public string Source { get; set; }
    public bool RequiresRevit { get; set; }
}
