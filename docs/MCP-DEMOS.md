# MCP Demo Runbook

Run **Start MCP Server** from the sample browser, connect Cursor to `http://127.0.0.1:8765/mcp`, and open a Revit model.

## Demo readiness checklist

- [ ] Revit model is open and saved or unsaved-but-loaded
- [ ] MCP server is running from the sample browser
- [ ] Cursor is connected to `http://127.0.0.1:8765/mcp`
- [ ] Read-only demos use only query/read tools
- [ ] Write demos go through `aec.validate_change_set` → `aec.preview_changes` → `aec.apply_changes`
- [ ] Generated tools are persisted for review; they are not live-loaded into the MCP server

## Demo A — Explain my model

1. Prompt: `inspect_current_context`
2. Tools:
   - `aec.get_host_context`
   - `aec.get_active_view`
   - `aec.get_model_statistics`
   - `aec.query_elements` with `{ "category": "Rooms" }`
3. Optional: `aec.capture_view_image`
4. Resource: `aec://document/current/summary`

## Demo B — Explain my selection

1. Select elements in Revit
2. Prompt: `explain_selection`
3. Tools:
   - `aec.get_selection`
   - `aec.resolve_element_refs` with `{ "include_selection": true }`
   - `aec.read_elements` with returned element ids
   - `aec.read_parameters` for one selected element

## Demo C — Highlight problems

1. Find issues with a QA tool, for example:
   - `aec.find_unplaced_rooms`
   - `aec.find_elements_without_parameter` with `{ "parameter": "Department", "category": "Rooms" }`
2. Resolve element ids with `aec.resolve_element_refs` or `aec.query_elements`
3. Create a visualization change set:
   - `aec.color_by_parameter` with `element_ids` and `parameter_name`, or
   - `aec.create_change_set` with `SetViewOverride` operations
4. Review safely:
   - `aec.validate_change_set`
   - `aec.preview_changes`
   - Resource: `audit://changesets/{change_set_id}`
5. Apply:
   - `aec.apply_changes` with `change_set_id` and `approval_token` when required
   - Resource: `audit://transactions/{transaction_id}`
6. `aec.capture_view_image`

## Demo D — Export for analysis

1. `revit_export_all_json` with optional `output_path`
2. `revit_list_rooms`
3. Resources:
   - `dev://catalog/commands`
   - `aec://model/rooms/summary`
   - `aec://exports/last`

## Demo E — Teach the firm a new checker

1. Prompt: `create_custom_tool`
2. `dev.search_examples` with a query like `flow mismatch` or `room data`
3. `dev.generate_tool_spec` → `dev.generate_bowerbird_script`
4. `dev.compile_script` → `dev.review_tool_for_safety` → `dev.create_mcp_tool_from_script`
5. Review the generated source under `.agent/generated-tools/`

## Notes

- Set `REVIT_SAMPLE_BROWSER_ROOT` if the server cannot locate `COMMANDS.md` from the add-in output path.
- Descriptor-backed tools beyond the built-in set are loaded from `src/BB_McpServer/mcp-manifest.json`.
- Regenerate the manifest with `python scripts/build_mcp_manifest.py`.
- `aec.color_by_parameter` only creates a change set. All view overrides must be applied through `aec.apply_changes`.
