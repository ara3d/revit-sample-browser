# PartInfo

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `PartInfo` |
| **Source** | `src/FabricationPartLayout/CS/PartInfo.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 5/5 |

Displays detailed metadata for a picked fabrication part: alias, CID, materials, specs, service, status, and centerline length.

## What it demonstrates

- Read-only properties on `FabricationPart` and lookup tables on `FabricationConfiguration`
- Formatted length via `UnitFormatUtils.Format`

## Prerequisites

- Loaded fabrication configuration

## User interaction

- Single element pick; results in `TaskDialog`

## MCP notes

- Proposed tool: `revit_get_fabrication_part_info`
- Parameters: `element_id`
- Returns: structured part metadata dictionary
- MCP descriptor: `docs/mcp/FabricationPartLayout/partinfo.json`

## See also

- MCP descriptor: `docs/mcp/FabricationPartLayout/partinfo.json`
