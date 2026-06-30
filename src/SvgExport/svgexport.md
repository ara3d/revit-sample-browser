# Command

| Field | Value |
|-------|-------|
| **Sample** | SvgExport |
| **Class** | `Command` |
| **Source** | `src/SvgExport/Command.cs` |
| **MCP rating** | 3/5 |
| **Upstream** | [jeremytammik/SvgExport](https://github.com/jeremytammik/SvgExport) (MIT) |

Exports a room's plan boundary as a scaled SVG file. Adapted from Jeremy Tammik's SvgExport sample; the original opened a browser to a Node web server — this version writes a local `.svg` file instead.

## What it demonstrates

- Resolving a `Room` from the only spatial element, pre-selection, or interactive pick (`RoomTag` supported)
- `SpatialElementBoundaryOptions` with `SpatialElementBoundaryLocation.Center` for closed loops
- `Room.GetBoundarySegments` and `BoundarySegment.GetCurve` endpoint chaining
- Mapping Revit XY coordinates to a 100×100 SVG canvas with Y-axis flip

## Prerequisites

- A model containing at least one placed room with boundary segments

## User interaction

- Auto-picks the sole room when only one `SpatialElement` exists
- Otherwise uses pre-selected room/room tag or prompts for a room pick
- `SaveFileDialog` to choose output path; opens the saved file when not in journal playback
- Non-linear boundary curves are not handled (upstream limitation)

## MCP notes

- Proposed tool: `revit_export_room_svg`
- Parameters: `room_id`, `output_path`
- Returns: path to written SVG file
- Current command requires UI for room selection and save dialog; headless use would need `room_id` and `output_path` parameters
- MCP descriptor: `src/SvgExport/svgexport.json`

## See also

- MCP descriptor: `src/SvgExport/svgexport.json`
