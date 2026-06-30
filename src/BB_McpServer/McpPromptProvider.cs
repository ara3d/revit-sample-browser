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
        "design_floor_plan" => "Create levels, rectangular floor plans, schedules, and plan views.",
        "audit_model_health" => "Run BIM health checks and produce a remediation report.",
        "create_building_massing" => "Build conceptual massing with floors and curtain systems.",
        "export_delivery_package" => "Export IFC, DWG, PDF, and schedule deliverables.",
        "analyze_rooms" => "Query rooms, compute quantities, schedule, and color by parameter.",
        "build_structural_frame" => "Create levels, grids, columns, and beams.",
        "validate_and_fix_names" => "Validate naming conventions and bulk-fix parameters.",
        "create_geometry_visualization" => "Create DirectShape geometry and capture view images.",
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
        "design_floor_plan",
        "audit_model_health",
        "create_building_massing",
        "export_delivery_package",
        "analyze_rooms",
        "build_structural_frame",
        "validate_and_fix_names",
        "create_geometry_visualization",
    ];

    static readonly Dictionary<string, string> PromptBodies = new()
    {
        ["inspect_current_context"] =
            "Call aec.get_host_context, aec.get_active_view, aec.list_capabilities, and aec.get_model_statistics. Summarize what model and view I am in, what tools/resources are available, and what I can safely do next.",
        ["explain_selection"] =
            "Call aec.get_selection, then aec.resolve_element_refs with include_selection true, then aec.read_elements and aec.read_parameters for the selected ids. Explain what is selected, key parameters, and any obvious issues.",
        ["plan_safe_model_edit"] =
            "Before making any model edits, inspect context, propose aec.create_change_set operations, run aec.validate_change_set and aec.preview_changes, and ask for approval if risk is medium or high.",
        ["find_elements_natural_language"] =
            "Translate my request into aec.query_elements filters. If ambiguous, ask one clarifying question before querying.",
        ["create_custom_tool"] =
            "Use dev.search_examples to find similar samples, dev.generate_tool_spec, dev.generate_bowerbird_script, dev.compile_script, dev.review_tool_for_safety, and dev.create_mcp_tool_from_script.",
        ["apply_firm_standards"] =
            "Search standards.search and standards resources, inspect the model with aec.query_elements/read_parameters, report violations with citations, and propose fix change sets without applying them automatically.",
        ["design_floor_plan"] =
            "Use aec.create_level, aec.create_rectangular_floor_plan, aec.create_schedule, and aec.create_plan_view to design a floor from my room list. Capture the result with aec.capture_view_image.",
        ["audit_model_health"] =
            "Run aec.audit_model_health, then aec.get_model_warnings_detailed, aec.find_unplaced_rooms, and aec.find_elements_without_parameter. Summarize issues and propose fixes.",
        ["create_building_massing"] =
            "Use aec.create_mass_from_profile, aec.apply_mass_floors, aec.compute_mass_properties, and aec.create_curtain_system_on_faces to build conceptual massing from my footprint.",
        ["export_delivery_package"] =
            "Export a delivery package with aec.export_ifc, aec.export_dwg, aec.export_pdf, and aec.export_schedule_excel. Report output paths.",
        ["analyze_rooms"] =
            "Query rooms with aec.query_elements, compute material quantities with aec.compute_material_quantities, create a room schedule with aec.create_schedule, create a visualization change set with aec.color_by_parameter, preview with aec.preview_changes, and apply with aec.apply_changes.",
        ["build_structural_frame"] =
            "Create structural framing using aec.create_level, aec.create_grid_line, aec.create_column, and aec.create_beam based on my grid specification.",
        ["validate_and_fix_names"] =
            "Run aec.validate_naming_convention, inspect parameters with aec.read_parameters, and fix violations with aec.set_parameters_bulk after proposing changes.",
        ["create_geometry_visualization"] =
            "Create geometry with aec.create_direct_shape_extrusion, extract JSON with aec.get_element_geometry_json, and capture aec.capture_view_image.",
    };
}
