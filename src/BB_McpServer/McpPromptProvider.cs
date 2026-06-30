using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class McpPromptProvider
{
    readonly McpToolRegistry _registry;

    public McpPromptProvider(McpToolRegistry registry) => _registry = registry;

    public JObject ListPrompts()
    {
        var prompts = new List<JObject>();
        foreach (var name in PromptNames)
        {
            prompts.Add(new JObject
            {
                ["name"] = name,
                ["description"] = Describe(name),
            });
        }

        return new JObject { ["prompts"] = new JArray(prompts) };
    }

    public JObject GetPrompt(string name, JObject arguments)
    {
        if (!PromptBodies.TryGetValue(name, out var body))
            throw new McpProtocolException(-32602, $"Unknown prompt: {name}");

        return new JObject
        {
            ["description"] = Describe(name),
            ["messages"] = new JArray
            {
                new JObject
                {
                    ["role"] = "user",
                    ["content"] = new JObject
                    {
                        ["type"] = "text",
                        ["text"] = body,
                    },
                },
            },
        };
    }

    static string Describe(string name) => name switch
    {
        "inspect_current_context" => "Inspect active host, document, view, and capabilities.",
        "explain_selection" => "Explain selected elements, parameters, and relationships.",
        "plan_safe_model_edit" => "Produce a change plan before touching the model.",
        "find_elements_natural_language" => "Convert a natural-language request into structured queries.",
        "create_custom_tool" => "Generate a reusable checker/tool from the sample library.",
        "apply_firm_standards" => "Find standards, check the model, and propose fixes.",
        _ => name,
    };

    static readonly string[] PromptNames =
    [
        "inspect_current_context",
        "explain_selection",
        "plan_safe_model_edit",
        "find_elements_natural_language",
        "create_custom_tool",
        "apply_firm_standards",
    ];

    static readonly Dictionary<string, string> PromptBodies = new()
    {
        ["inspect_current_context"] =
            "Call aec.get_host_context, aec.list_capabilities, and aec.get_model_statistics. Summarize what model and view I am in, what tools/resources are available, and what I can safely do next.",
        ["explain_selection"] =
            "Call aec.get_selection, then aec.read_elements and aec.read_parameters for the selected ids. Explain what is selected, key parameters, and any obvious issues.",
        ["plan_safe_model_edit"] =
            "Before making any model edits, inspect context, propose aec.create_change_set operations, run aec.validate_change_set and aec.preview_changes, and ask for approval if risk is medium or high.",
        ["find_elements_natural_language"] =
            "Translate my request into aec.query_elements filters. If ambiguous, ask one clarifying question before querying.",
        ["create_custom_tool"] =
            "Use dev.search_examples to find similar samples, dev.generate_tool_spec, dev.generate_bowerbird_script, dev.compile_script, dev.review_tool_for_safety, and dev.create_mcp_tool_from_script.",
        ["apply_firm_standards"] =
            "Search standards.search and standards resources, inspect the model with aec.query_elements/read_parameters, report violations with citations, and propose fix change sets without applying them automatically.",
    };
}
