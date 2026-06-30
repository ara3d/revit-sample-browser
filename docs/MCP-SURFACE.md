# Bowerbird Revit MCP — Tools, Prompts, and Resources

Server: `bowerbird-revit-mcp` v2.0.0  
Endpoint: `http://127.0.0.1:8765/mcp`  
Protocol: MCP `2025-03-26` (`initialize`, `tools/list`, `tools/call`, `resources/list`, `resources/read`, `prompts/list`, `prompts/get`, `ping`)

At startup the server registers **92 built-in tools** (65 new domain tools + 27 core tools) plus up to **25 descriptor-backed tools** from [`src/BB_McpServer/mcp-manifest.json`](../src/BB_McpServer/mcp-manifest.json). Descriptor-backed tools return `not_implemented` until a headless adapter is wired.

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

### `aec.*` — Geometry and shapes

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.create_direct_shape_box` | Create a box solid as DirectShape at a point. | semantic | Yes |
| `aec.create_direct_shape_cylinder` | Create a cylinder solid as DirectShape. | semantic | Yes |
| `aec.create_direct_shape_extrusion` | Extrude a 2D polyline profile to a solid DirectShape. | semantic | Yes |
| `aec.get_element_geometry_json` | Extract solid/mesh geometry of elements as JSON. | semantic | Yes |
| `aec.compute_element_bounding_box` | Return world-space bounding boxes for elements. | semantic | Yes |
| `aec.compute_material_quantities` | Compute material volumes for selected or queried elements. | semantic | Yes |
| `aec.find_intersecting_elements` | Find elements whose bounding boxes intersect a given box. | semantic | Yes |
| `aec.compute_wall_face_areas` | Compute net/gross wall face areas for rooms with opening deductions. | semantic | Yes |

### `aec.*` — Building creation

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.create_wall` | Create a wall from start/end points, level, type, and height. | semantic | Yes |
| `aec.create_floor` | Create a floor from a boundary profile. | semantic | Yes |
| `aec.create_ceiling` | Create a ceiling from a boundary profile. | semantic | Yes |
| `aec.create_room` | Create a room at a point on a level. | semantic | Yes |
| `aec.create_column` | Place a column family instance at a point on a level. | semantic | Yes |
| `aec.create_beam` | Create a structural beam between two points on a level. | semantic | Yes |
| `aec.create_level` | Create a new level at an elevation. | semantic | Yes |
| `aec.create_grid_line` | Create a grid line between two points. | semantic | Yes |
| `aec.place_family_instance` | Place any family instance by family/type name at a point. | semantic | Yes |
| `aec.create_rectangular_floor_plan` | Create walls, floor, and rooms for a rectangular layout in one transaction. | semantic | Yes |

### `aec.*` — Massing

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.create_mass_box` | Create a conceptual mass box (DirectShape in Mass category). | semantic | Yes |
| `aec.create_mass_from_profile` | Extrude a 2D profile polygon to a conceptual mass solid. | semantic | Yes |
| `aec.apply_mass_floors` | Apply mass floor level data to a mass family instance. | semantic | Yes |
| `aec.compute_mass_properties` | Compute volume and surface area for mass or DirectShape elements. | semantic | Yes |
| `aec.create_curtain_system_on_faces` | Apply a curtain system to planar faces of a host element. | semantic | Yes |
| `aec.create_divided_surface` | Configure grid divisions on an existing divided surface. | semantic | Yes |

### `aec.*` — Schedules

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.create_schedule` | Create a view schedule for a category with specified fields. | semantic | Yes |
| `aec.read_schedule_data` | Read schedule rows as structured JSON. | semantic | Yes |
| `aec.add_schedule_filter` | Add a filter to an existing schedule. | semantic | Yes |
| `aec.add_schedule_sort` | Add a sort/group field to a schedule. | semantic | Yes |
| `aec.export_schedule_csv` | Export a named schedule to CSV on disk. | semantic | Yes |
| `aec.export_schedule_html` | Export a named schedule to HTML on disk. | semantic | Yes |
| `aec.list_schedules` | List all schedules with field names. | semantic | Yes |
| `aec.duplicate_schedule` | Duplicate an existing schedule. | semantic | Yes |

### `aec.*` — Import / export

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.export_dwg` | Export active or named view to DWG file. | semantic | Yes |
| `aec.export_ifc` | Export document to IFC. | semantic | Yes |
| `aec.export_pdf` | Export views or sheets to PDF. | semantic | Yes |
| `aec.export_gbxml` | Export document to gbXML for energy analysis. | semantic | Yes |
| `aec.export_3d_mesh_json` | Export 3D mesh geometry from a 3D view as JSON. | bowerbird | Yes |
| `aec.export_collada` | Export visible 3D geometry as mesh JSON (Collada-compatible payload). | bowerbird | Yes |
| `aec.export_all_elements_json` | Export full document element data as JSON. | bowerbird | Yes |
| `aec.export_rooms_json` | Export room data as structured JSON. | bowerbird | Yes |
| `aec.export_schedule_excel` | Export schedules to Excel-compatible CSV files. | semantic | Yes |

### `aec.*` — BIM validation (high ROI)

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.get_model_warnings_detailed` | Get all model warnings with element IDs, severity, and description. | semantic | Yes |
| `aec.find_unplaced_rooms` | Find rooms that are not bounded or placed. | semantic | Yes |
| `aec.find_elements_without_parameter` | Find elements missing a required parameter value. | semantic | Yes |
| `aec.validate_naming_convention` | Check element names against a regex pattern by category. | semantic | Yes |
| `aec.find_level_less_elements` | Find elements not assigned to a level. | semantic | Yes |
| `aec.check_room_door_relationships` | Verify doors have valid ToRoom/FromRoom assignments. | semantic | Yes |
| `aec.find_overlapping_rooms` | Find rooms whose bounding boxes overlap on the same level. | semantic | Yes |
| `aec.audit_model_health` | Compound BIM health check with health score and recommendations. | semantic | Yes |
| `aec.check_mep_system_completeness` | Check duct/pipe systems for open connectors. | semantic | Yes |

### `aec.*` — Views and sheets

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.create_3d_view` | Create a new 3D isometric view with optional section box. | semantic | Yes |
| `aec.create_plan_view` | Create a floor plan view at a level. | semantic | Yes |
| `aec.create_section_view` | Create a section view through a bounding box. | semantic | Yes |
| `aec.create_sheet` | Create a new sheet with a title block. | semantic | Yes |
| `aec.place_view_on_sheet` | Place a view or schedule on a sheet at a point. | semantic | Yes |
| `aec.apply_view_template` | Apply a named view template to one or more views. | semantic | Yes |
| `aec.list_views_and_sheets` | List views and sheets with types, levels, and template names. | semantic | Yes |
| `aec.isolate_category_in_view` | Temporarily isolate categories in the active or named view. | semantic | Yes |

### `aec.*` — Parameters

| Tool | Description | Tier | Revit |
|------|-------------|------|-------|
| `aec.set_parameters_bulk` | Set a parameter value on all elements matching a query. | semantic | Yes |
| `aec.copy_parameters_between_elements` | Copy parameter values from a source element to targets. | semantic | Yes |
| `aec.export_parameters_csv` | Export parameter name/value pairs for a category to CSV. | semantic | Yes |
| `aec.import_parameters_csv` | Import parameter values from CSV by UniqueId or ElementId. | semantic | Yes |
| `aec.list_project_parameters` | List project parameters with bound categories. | semantic | Yes |
| `aec.find_elements_by_parameter_value` | Find elements where a parameter matches a value or regex. | semantic | Yes |
| `aec.compute_parameter_statistics` | Compute min/max/avg/count for a numeric parameter. | semantic | Yes |

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

The manifest catalogs **185** total descriptors; regenerate with:

```bash
python scripts/build_mcp_manifest.py
```

---

## High-ROI Workflow Demos

| Workflow | Prompt | Key tools |
|----------|--------|-----------|
| Design a floor | `design_floor_plan` | `aec.create_level` → `aec.create_rectangular_floor_plan` → `aec.create_schedule` → `aec.create_plan_view` |
| BIM QA report | `audit_model_health` | `aec.audit_model_health` → `aec.get_model_warnings_detailed` → `aec.find_unplaced_rooms` |
| Massing to concept | `create_building_massing` | `aec.create_mass_from_profile` → `aec.apply_mass_floors` → `aec.create_curtain_system_on_faces` |
| Delivery package | `export_delivery_package` | `aec.export_ifc` → `aec.export_dwg` → `aec.export_pdf` → `aec.export_schedule_excel` |
| Parameter mass update | `validate_and_fix_names` | `aec.validate_naming_convention` → `aec.set_parameters_bulk` |

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
| `design_floor_plan` | Create levels, rectangular floor plans, schedules, and plan views. |
| `audit_model_health` | Run BIM health checks and produce a remediation report. |
| `create_building_massing` | Build conceptual massing with floors and curtain systems. |
| `export_delivery_package` | Export IFC, DWG, PDF, and schedule deliverables. |
| `analyze_rooms` | Query rooms, compute quantities, schedule, and color by parameter. |
| `build_structural_frame` | Create levels, grids, columns, and beams. |
| `validate_and_fix_names` | Validate naming conventions and bulk-fix parameters. |
| `create_geometry_visualization` | Create DirectShape geometry and capture view images. |

### Prompt workflows (tool chains)

| Prompt | Suggested tools |
|--------|-----------------|
| `inspect_current_context` | `aec.get_host_context` → `aec.list_capabilities` → `aec.get_model_statistics` |
| `explain_selection` | `aec.get_selection` → `aec.read_elements` → `aec.read_parameters` |
| `plan_safe_model_edit` | `aec.create_change_set` → `aec.validate_change_set` → `aec.preview_changes` → `aec.apply_changes` |
| `find_elements_natural_language` | `aec.query_elements` |
| `create_custom_tool` | `dev.search_examples` → `dev.generate_tool_spec` → `dev.generate_bowerbird_script` → `dev.compile_script` → `dev.review_tool_for_safety` → `dev.create_mcp_tool_from_script` |
| `apply_firm_standards` | `standards.search` + standards resources → `aec.query_elements` / `aec.read_parameters` |
| `design_floor_plan` | `aec.create_level` → `aec.create_rectangular_floor_plan` → `aec.create_schedule` → `aec.create_plan_view` |
| `audit_model_health` | `aec.audit_model_health` → `aec.get_model_warnings_detailed` → `aec.find_unplaced_rooms` → `aec.find_elements_without_parameter` |
| `create_building_massing` | `aec.create_mass_from_profile` → `aec.apply_mass_floors` → `aec.compute_mass_properties` → `aec.create_curtain_system_on_faces` |
| `export_delivery_package` | `aec.export_ifc` → `aec.export_dwg` → `aec.export_pdf` → `aec.export_schedule_excel` |
| `analyze_rooms` | `aec.query_elements` → `aec.compute_material_quantities` → `aec.create_schedule` → `aec.color_by_parameter` |
| `build_structural_frame` | `aec.create_level` → `aec.create_grid_line` → `aec.create_column` → `aec.create_beam` |
| `validate_and_fix_names` | `aec.validate_naming_convention` → `aec.read_parameters` → `aec.set_parameters_bulk` |
| `create_geometry_visualization` | `aec.create_direct_shape_extrusion` → `aec.get_element_geometry_json` → `aec.capture_view_image` |

---

## MCP Resources

### Fixed resources (`resources/list`)

| URI | MIME type | Description |
|-----|-----------|-------------|
| `dev://catalog/commands` | `text/markdown` | Full `COMMANDS.md` index (462 commands). |
| `dev://catalog/mcp-tools` | `application/json` | Summary of MCP tool descriptors from the sample catalog index. |
| `dev://tools/catalog/implemented` | `application/json` | Summary of all fully-implemented MCP tools. |
| `aec://document/current/summary` | `application/json` | Live model summary (title, path, element/room counts). |
| `aec://view/active/image` | `text/plain` | Path to the latest `aec.capture_view_image` output. |
| `aec://model/levels` | `application/json` | All levels with elevations. |
| `aec://model/rooms/summary` | `application/json` | Room count, total area, unplaced count. |
| `aec://model/warnings/summary` | `application/json` | Warning counts by severity. |
| `aec://model/sheets` | `application/json` | All sheets with placed views. |
| `aec://model/materials` | `application/json` | All materials with render properties. |
| `aec://model/grids-levels` | `application/json` | Grid lines and levels for spatial orientation. |
| `aec://exports/last` | `application/json` | Most recent export path and metadata. |
| `standards://firm/index` | `application/json` | Index of firm standards markdown files. |
| `audit://agent/history` | `application/json` | Recent agent tool audit log entries. |

### Dynamic resources

| URI pattern | MIME type | Description |
|-------------|-----------|-------------|
| `standards://firm/{path}` | `text/markdown` | Individual firm standard file under `.agent/standards/firm/`. |
| `dev://samples/{path}` | `text/plain` | Per-command markdown doc from `src/<Sample>/<slug>.md`. |
| `aec://model/types/{category}` | `application/json` | Family types for a category (e.g. Walls, Rooms). |
| `aec://model/schedule/{name}` | `application/json` | Live schedule data as JSON. |

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
| Shared helpers | [`src/BB_McpServer/HandlerHelpers.cs`](../src/BB_McpServer/HandlerHelpers.cs) |
| Tool handlers | [`src/BB_McpServer/Handlers/`](../src/BB_McpServer/Handlers/) |
| Tool registry | [`src/BB_McpServer/McpToolRegistry.cs`](../src/BB_McpServer/McpToolRegistry.cs) |
| Resources | [`src/BB_McpServer/McpResourceProvider.cs`](../src/BB_McpServer/McpResourceProvider.cs) |
| Prompts | [`src/BB_McpServer/McpPromptProvider.cs`](../src/BB_McpServer/McpPromptProvider.cs) |
| Demo runbook | [`docs/MCP-DEMOS.md`](MCP-DEMOS.md) |
| Product plan | [`docs/MCP-PLAN.md`](MCP-PLAN.md) |
