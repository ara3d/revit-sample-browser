# Bowerbird Revit MCP — Tools, Prompts, and Resources

Server: `bowerbird-revit-mcp` v2.0.0  
Endpoint: `http://127.0.0.1:8765/mcp`  
Protocol: MCP `2025-03-26` (`initialize`, `tools/list`, `tools/call`, `resources/list`, `resources/read`, `prompts/list`, `prompts/get`, `ping`)

At startup the server registers **27 built-in tools** plus up to **25 descriptor-backed tools** from [`src/BB_McpServer/mcp-manifest.json`](../src/BB_McpServer/mcp-manifest.json) (currently **12** additional tools after skipping names already registered). Descriptor-backed tools return `not_implemented` until a headless adapter is wired.

---

## MCP Tools

### Legacy / smoke test

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `echo` | Echoes the message back to the client. | semantic | No |
| `revit_document_info` | Returns the active Revit document title and path. | semantic | Yes |

### `aec.*` — Host context and inspection

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.get_host_context` | Returns host app, document, units, active view, selection mode, and capabilities. | semantic | Yes |
| `aec.list_capabilities` | Lists registered tools, resources, prompts, and risk classes exposed by this server. | semantic | No |
| `aec.get_selection` | Returns the current Revit selection with stable element references. | semantic | Yes |
| `aec.query_elements` | Structured query over category, class, level, name, and element ids. | semantic | Yes |
| `aec.read_elements` | Returns hydrated element summaries for the given element ids. | semantic | Yes |
| `aec.read_parameters` | Returns parameter metadata and values for an element. | semantic | Yes |
| `aec.get_model_statistics` | Returns counts by category, class, level, workset, and phase. | semantic | Yes |
| `aec.report_status` | Summarizes warnings, failures, blocked operations, and recent agent tool activity. | semantic | Yes |

### `aec.*` — Visualization

| Tool | Description | Tier | Revit | Notes |
|------|-------------|------|-------|-------|
| `aec.capture_view_image` | Exports the active view to a PNG and returns the output path. | semantic | Yes | |
| `aec.color_by_parameter` | Applies view overrides to elements grouped by a parameter value. | semantic | Yes | Requires change-set preview/apply for writes |

### `aec.*` — Change-set safety

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.create_change_set` | Builds a declarative edit plan without executing it. | semantic | No |
| `aec.validate_change_set` | Validates a change set against the current model context. | semantic | No |
| `aec.preview_changes` | Returns a human-readable preview of a change set before execution. | semantic | No |
| `aec.apply_changes` | Applies a validated change set through the Revit transaction system. | semantic | Yes |
| `aec.undo_last_agent_change` | Reports the last applied agent change set and whether host undo is available. | semantic | No |
| `aec.classify_operation_risk` | Classifies risk for a proposed change set or tool name. | semantic | No |

### `dev.*` — Sample catalog and tool generation

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `dev.search_examples` | Search `COMMANDS.md`, command docs, descriptors, and sample metadata. | semantic | No |
| `dev.generate_tool_spec` | Converts a user request into a proposed MCP tool schema using retrieved examples. | semantic | No |
| `dev.generate_bowerbird_script` | Generates a single-file Bowerbird `NamedCommand` scaffold from a tool spec. | semantic | No |
| `dev.compile_script` | Performs lightweight validation on generated C# source. | semantic | No |
| `dev.review_tool_for_safety` | Reviews generated tool source for destructive operations and threading issues. | semantic | No |
| `dev.create_mcp_tool_from_script` | Persists a generated tool descriptor and source under `.agent/generated-tools/`. | semantic | No |

### `standards.*`

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `standards.search` | Search firm and project standards markdown as citeable context. | semantic | No |

### `revit_*` — Headless Bowerbird adapters

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `revit_export_all_json` | Exports all document elements to a JSON file. | bowerbird | Yes |
| `revit_list_rooms` | Returns structured room data for all rooms in the active document. | bowerbird | Yes |

### Descriptor-backed tools (manifest, currently exposed)

Loaded from `mcp-manifest.json` when tiers match `semantic`, `bowerbird`, or `mcp5` and `maxTools` (25) is not exceeded. These are registered via `DescriptorToolAdapter` and return **`not_implemented`** until wired to a headless sample service.

| Tool | Tier | Description |
|------|------|-------------|
| `revit_export_view_3d_mesh_json` | mcp5 | Exports tessellated 3D mesh geometry from a 3D view to ADN mesh JSON format. |
| `revit_n3p_application` | mcp5 | Logs optional Revit module availability flags and `AsControlledApplication`. |
| `revit_n3p_category` | mcp5 | Resolves `BuiltInCategory` to `Category` objects and compares category `ElementId`s. |
| `revit_n3p_collector` | mcp5 | Fluent `FilteredElementCollector` extension demos. |
| `revit_n3p_display` | mcp5 | Formats `ForgeTypeId` labels and converts Revit `Color` values to hex/RGB strings. |
| `revit_n3p_document` | mcp5 | Reads document version, `CheckAllFamilies`, and global parameter counts. |
| `revit_n3p_element` | mcp5 | Nice3point `Element` extensions: `ToElement`, `FindParameter`, `CanBeDeleted`. |
| `revit_n3p_external_command` | mcp5 | Read-only demo of toolkit `ExternalCommand` base class and `RevitContext`. |
| `revit_n3p_geometry` | mcp5 | Bounding-box volume/centroid and solid tessellation flags via geometry extensions. |
| `revit_n3p_parameters` | mcp5 | Parameter helpers: `ToElementId`, `AsBool`, `WhereParameter` filter chaining. |
| `revit_n3p_unit` | mcp5 | Converts wall offset values using unit conversion helpers. |
| `revit_n3p_view` | mcp5 | Counts elements visible/selectable in the active view. |

The manifest catalogs **185** total descriptors; regenerate with:

```bash
python scripts/build_mcp_manifest.py
```

---

## MCP Prompts

| Prompt | Description |
|--------|-------------|
| `inspect_current_context` | Inspect active host, document, view, and capabilities. |
| `explain_selection` | Explain selected elements, parameters, and relationships. |
| `plan_safe_model_edit` | Produce a change plan before touching the model. |
| `find_elements_natural_language` | Convert a natural-language request into structured queries. |
| `create_custom_tool` | Generate a reusable checker/tool from the sample library. |
| `apply_firm_standards` | Find standards, check the model, and propose fixes. |

### Prompt workflows (tool chains)

| Prompt | Suggested tools |
|--------|-----------------|
| `inspect_current_context` | `aec.get_host_context` → `aec.list_capabilities` → `aec.get_model_statistics` |
| `explain_selection` | `aec.get_selection` → `aec.read_elements` → `aec.read_parameters` |
| `plan_safe_model_edit` | `aec.create_change_set` → `aec.validate_change_set` → `aec.preview_changes` → `aec.apply_changes` |
| `find_elements_natural_language` | `aec.query_elements` |
| `create_custom_tool` | `dev.search_examples` → `dev.generate_tool_spec` → `dev.generate_bowerbird_script` → `dev.compile_script` → `dev.review_tool_for_safety` → `dev.create_mcp_tool_from_script` |
| `apply_firm_standards` | `standards.search` + standards resources → `aec.query_elements` / `aec.read_parameters` |

---

## MCP Resources

### Fixed resources (`resources/list`)

| URI | MIME type | Description |
|-----|-----------|-------------|
| `dev://catalog/commands` | `text/markdown` | Full `COMMANDS.md` index (462 commands). |
| `dev://catalog/mcp-tools` | `application/json` | Summary of MCP tool descriptors from the sample catalog index. |
| `aec://document/current/summary` | `application/json` | Live model summary (title, path, element/room counts). |
| `aec://view/active/image` | `text/plain` | Path to the latest `aec.capture_view_image` output. |
| `standards://firm/index` | `application/json` | Index of firm standards markdown files. |
| `audit://agent/history` | `application/json` | Recent agent tool audit log entries. |

### Dynamic resources

| URI pattern | MIME type | Description |
|-------------|-----------|-------------|
| `standards://firm/{path}` | `text/markdown` | Individual firm standard file under `.agent/standards/firm/`. |
| `dev://samples/{path}` | `text/plain` | Per-command markdown doc from `src/<Sample>/<slug>.md`. |

### Starter standards files (dynamic `standards://firm/...`)

| URI | Source file |
|-----|-------------|
| `standards://firm/firm/naming.md` | [`.agent/standards/firm/naming.md`](../.agent/standards/firm/naming.md) |
| `standards://firm/firm/rooms.md` | [`.agent/standards/firm/rooms.md`](../.agent/standards/firm/rooms.md) |

---

## Source references

| Area | Location |
|------|----------|
| Server entry | [`src/BB_McpServer/CommandMcpServer.cs`](../src/BB_McpServer/CommandMcpServer.cs) |
| Tool handlers | [`src/BB_McpServer/Handlers/`](../src/BB_McpServer/Handlers/) |
| Tool registry | [`src/BB_McpServer/McpToolRegistry.cs`](../src/BB_McpServer/McpToolRegistry.cs) |
| Resources | [`src/BB_McpServer/McpResourceProvider.cs`](../src/BB_McpServer/McpResourceProvider.cs) |
| Prompts | [`src/BB_McpServer/McpPromptProvider.cs`](../src/BB_McpServer/McpPromptProvider.cs) |
| Demo runbook | [`docs/MCP-DEMOS.md`](MCP-DEMOS.md) |
| Product plan | [`docs/MCP-PLAN.md`](MCP-PLAN.md) |
