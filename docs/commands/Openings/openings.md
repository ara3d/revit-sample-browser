# Command

| Field | Value |
|-------|-------|
| **Sample** | Openings |
| **Class** | `Command` |
| **Source** | `src/Openings/CS/Command.cs` |
| **SDK ReadMe** | `src/Openings/CS/ReadMe_Openings.rtf` |
| **MCP rating** | 4/5 |

Enumerates all `Opening` elements in the project and displays their host, geometry, and bounding information in a review dialog.

## What it demonstrates

- `FilteredElementCollector.OfClass(typeof(Opening))` to find openings project-wide
- `OpeningInfo` packaging host references, geometry summaries, and bounding boxes per opening
- `OpeningForm` for read-only inspection (with optional model line creation options in related forms)

## Prerequisites

- Project containing at least one `Opening` element

## User interaction

- Modal `OpeningForm` listing opening data; cancels if no openings exist
- Collector and `OpeningInfo` logic is headless-friendly

## MCP notes

- Proposed tool: `revit_list_openings`
- Parameters: optional `host_element_id` filter
- Returns: opening id, name, host id, and bounding box per opening
- MCP descriptor: `docs/mcp/Openings/openings.json`

## See also

- MCP descriptor: `docs/mcp/Openings/openings.json`
- Related: `NewOpenings/newopenings.md`
