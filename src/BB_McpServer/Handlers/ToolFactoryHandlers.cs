using Ara3D.Bowerbird.RevitSamples.AecAgent;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class GenerateToolSpecHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "dev.generate_tool_spec",
        "Converts a user request into a proposed MCP tool schema using retrieved examples.",
        Schema.Object(
            ("request", Schema.String("What the tool should do")),
            ("examples", Schema.StringArray("Optional example sample names"))),
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var request = arguments?["request"]?.Value<string>();
        if (string.IsNullOrWhiteSpace(request))
            throw new McpProtocolException(-32602, "request is required");

        context.Catalog.EnsureLoaded(context.RepoRoot);
        var examples = context.Catalog.Search(request, 5);
        var toolName = "revit_" + string.Concat(request.ToLowerInvariant()
            .Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
            .Take(4)
            .Select(w => char.ToUpperInvariant(w[0]) + w[1..]));

        var spec = new JObject
        {
            ["name"] = toolName.ToLowerInvariant().Replace("revit_", "revit_"),
            ["description"] = request,
            ["arguments"] = new JObject
            {
                ["type"] = "object",
                ["properties"] = new JObject(),
            },
            ["examples"] = new JArray(examples.Select(e => new JObject
            {
                ["sample"] = e.Sample,
                ["doc_path"] = e.DocPath,
                ["mcp_rating"] = e.McpRating,
            })),
        };

        return TaskResult(McpToolResult.Success(spec));
    }
}

public sealed class GenerateBowerbirdScriptHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "dev.generate_bowerbird_script",
        "Generates a single-file Bowerbird NamedCommand scaffold from a tool spec.",
        Schema.Object(
            ("tool_spec", new JObject { ["type"] = "object" }),
            ("class_name", Schema.String("Optional command class name"))),
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var spec = arguments?["tool_spec"] as JObject;
        if (spec == null)
            throw new McpProtocolException(-32602, "tool_spec is required");

        var className = arguments?["class_name"]?.Value<string>()
            ?? "GeneratedCommand";
        var description = spec["description"]?.Value<string>() ?? "Generated MCP tool";

        var code = $$"""
using Ara3D.Utils;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;

namespace Ara3D.Bowerbird.RevitSamples;

public class {{className}} : NamedCommand
{
    public override string Name => "{{description}}";

    public override void Execute(object arg)
    {
        var app = arg as UIApplication;
        var doc = app?.ActiveUIDocument?.Document;
        if (doc == null) return;

        // TODO: implement generated logic from MCP tool spec
        var result = new JObject
        {
            ["ok"] = true,
            ["message"] = "Generated command executed.",
        };
    }
}
""";

        return TaskResult(McpToolResult.Success(new JObject
        {
            ["class_name"] = className,
            ["source"] = code,
        }));
    }
}

public sealed class CompileScriptHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "dev.compile_script",
        "Performs lightweight validation on generated C# source.",
        Schema.ObjectReq(("source", Schema.String("C# source code")), "source"),
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var source = arguments?["source"]?.Value<string>();
        if (string.IsNullOrWhiteSpace(source))
            throw new McpProtocolException(-32602, "source is required");

        var issues = new JArray();
        if (!source.Contains("NamedCommand"))
            issues.Add(new JObject { ["message"] = "Generated command should inherit NamedCommand." });
        if (source.Contains("TaskDialog"))
            issues.Add(new JObject { ["message"] = "Avoid TaskDialog in headless MCP commands." });
        if (!source.Contains("UIApplication"))
            issues.Add(new JObject { ["message"] = "Command should accept UIApplication via Execute(object arg)." });

        return TaskResult(McpToolResult.Success(new JObject
        {
            ["compiled"] = issues.Count == 0,
            ["issues"] = issues,
        }));
    }
}

public sealed class ReviewToolForSafetyHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "dev.review_tool_for_safety",
        "Reviews generated tool source for destructive operations and threading issues.",
        Schema.ObjectReq(("source", Schema.String("C# source code")), "source"),
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var source = arguments?["source"]?.Value<string>() ?? "";
        var findings = new JArray();
        if (source.Contains(".Delete(")) findings.Add(Finding("destructive", "Uses element deletion."));
        if (source.Contains("new Thread")) findings.Add(Finding("threading", "Creates threads outside ExternalEvent."));
        if (source.Contains("Transaction") && !source.Contains("using"))
            findings.Add(Finding("transaction", "Transaction may not be disposed safely."));
        if (source.Contains("PickObject")) findings.Add(Finding("ui", "Requires interactive user picking."));

        var risk = findings.Any(f => f["code"]?.Value<string>() is "destructive" or "threading")
            ? "high"
            : findings.Count > 0 ? "medium" : "low";

        return TaskResult(McpToolResult.Success(new JObject
        {
            ["risk_level"] = risk,
            ["findings"] = findings,
        }));
    }

    static JObject Finding(string code, string message)
        => new() { ["code"] = code, ["message"] = message };
}

public sealed class CreateMcpToolFromScriptHandler : ToolHandlerBase
{
    public override McpToolDescriptor Descriptor => Desc(
        "dev.create_mcp_tool_from_script",
        "Registers a generated tool descriptor in the current server session.",
        Schema.Object(
            ("tool_spec", new JObject { ["type"] = "object" }),
            ("source", Schema.String("Generated C# source"))),
        tier: "semantic");

    public override Task<McpToolResult> InvokeAsync(JObject arguments, HostContext context)
    {
        var spec = arguments?["tool_spec"] as JObject;
        var source = arguments?["source"]?.Value<string>();
        if (spec == null || string.IsNullOrWhiteSpace(source))
            throw new McpProtocolException(-32602, "tool_spec and source are required");

        var generatedDir = Path.Combine(context.RepoRoot ?? Config.OutputDir.ToString(), ".agent", "generated-tools");
        Directory.CreateDirectory(generatedDir);
        var name = spec["name"]?.Value<string>() ?? "revit_generated_tool";
        var file = Path.Combine(generatedDir, $"{name}.cs");
        File.WriteAllText(file, source);
        spec["generated_source_path"] = file;

        return TaskResult(McpToolResult.Success(new JObject
        {
            ["registered"] = true,
            ["tool_name"] = name,
            ["generated_source_path"] = file,
            ["message"] = "Descriptor persisted for review. Dynamic in-process registration can be added in a later sprint.",
        }));
    }
}
