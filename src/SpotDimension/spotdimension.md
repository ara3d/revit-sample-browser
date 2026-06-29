# Command

| Field | Value |
|-------|-------|
| **Sample** | SpotDimension |
| **Class** | `Command` |
| **Source** | `src/SpotDimension/Command.cs` |
| **SDK ReadMe** | `src/SpotDimension/ReadMe_SpotDimension.rtf` |
| **MCP rating** | 4/5 |

Lists all spot dimensions in the project by view and optionally highlights a user-selected dimension in the failure message set.

## What it demonstrates

- `SpotDimensionsData` collecting `SpotDimension` elements and their view names
- `SpotDimensionInfoDlg` for browsing dimensions and views
- Returning `Result.Failed` with `elements.Insert` to highlight a chosen spot dimension

## Prerequisites

- Project containing at least one spot dimension (dialog still opens if none)

## User interaction

- Dialog-driven browse and highlight; read-only except for selection feedback

## MCP notes

- Proposed tool: `revit_list_spot_dimensions`
- Parameters: optional `view_name` filter
- Returns: spot dimension ids, views, and key parameters from `SpotDimensionParams`
- MCP descriptor: `src/SpotDimension/spotdimension.json`

## See also

- MCP descriptor: `src/SpotDimension/spotdimension.json`
