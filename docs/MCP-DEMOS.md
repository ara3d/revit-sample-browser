# MCP Demo Runbook

Run **Start MCP Server** from the sample browser, connect Cursor to `http://127.0.0.1:8765/mcp`, and open a Revit model.

## Demo A — Explain my model

1. Prompt: `inspect_current_context`
2. Tools: `aec.get_host_context` → `aec.get_model_statistics` → `aec.query_elements` with `{ "category": "Rooms" }`
3. Optional: `aec.capture_view_image`

## Demo B — Explain my selection

1. Select elements in Revit
2. Prompt: `explain_selection`
3. Tools: `aec.get_selection` → `aec.read_elements` → `aec.read_parameters`

## Demo C — Highlight problems

1. `aec.query_elements` for rooms or elements with missing data
2. `aec.create_change_set` with view override operations
3. `aec.preview_changes` → `aec.apply_changes` with approval token
4. `aec.capture_view_image`

## Demo D — Export for analysis

1. `revit_export_all_json` with optional `output_path`
2. `revit_list_rooms`
3. Resource: `dev://catalog/commands`

## Demo E — Teach the firm a new checker

1. Prompt: `create_custom_tool`
2. `dev.search_examples` with a query like `flow mismatch` or `room data`
3. `dev.generate_tool_spec` → `dev.generate_bowerbird_script`
4. `dev.compile_script` → `dev.review_tool_for_safety` → `dev.create_mcp_tool_from_script`

## Notes

- Set `REVIT_SAMPLE_BROWSER_ROOT` if the server cannot locate `COMMANDS.md` from the add-in output path.
- Descriptor-backed tools beyond the built-in set are loaded from `src/BB_McpServer/mcp-manifest.json`.
- Regenerate the manifest with `python scripts/build_mcp_manifest.py`.
