# Command

| Field | Value |
|-------|-------|
| **Sample** | RoofsRooms |
| **Class** | `Command` |
| **Source** | `src/RoofsRooms/CS/Command.cs` |
| **MCP rating** | 4/5 |

Analyzes whether each room or space in the project has a bounding roof element using spatial element geometry.

## What it demonstrates

- `SpatialElementGeometryCalculator` for room solid faces and boundary face info
- `LogicalOrFilter` on `OST_Roofs` and `OST_RoofSoffit` categories
- `RoomFilter` and `SpaceFilter` collection of all spatial elements
- Journal data and trace logging for regression testing

## Prerequisites

- Project with placed rooms or spaces; roofs improve meaningful results

## User interaction

- Shows `TaskDialog` with results when not in journal replay mode
- Core analysis in `FindRoomBoundingRoofs` is headless-friendly

## MCP notes

- Proposed tool: `revit_find_rooms_without_roofs`
- Parameters: optional `level_id` or room id filter
- Returns: lists of rooms with bounding roof ids and rooms lacking roofs
- MCP descriptor: `docs/mcp/RoofsRooms/roofsrooms.json`

## See also

- MCP descriptor: `docs/mcp/RoofsRooms/roofsrooms.json`
