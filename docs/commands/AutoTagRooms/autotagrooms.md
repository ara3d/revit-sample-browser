# Command

| Field | Value |
|-------|-------|
| **Sample** | AutoTagRooms |
| **Class** | `Command` |
| **Source** | `src/AutoTagRooms/CS/Command.cs` |
| **SDK ReadMe** | `src/AutoTagRooms/CS/ReadMe_AutoTagRooms.rtf` |
| **MCP rating** | 4/5 |

Lists rooms and existing tags by level, then creates room tags for untagged rooms using a chosen tag type.

## What it demonstrates

- `RoomsData` collecting `Room`, `RoomTag`, `RoomTagType`, and `Level` instances
- Grouping rooms per level and counting existing tags
- Creating `RoomTag` elements for rooms missing tags on a selected level

## Prerequisites

- Architectural model with rooms on at least one level; loaded room tag types

## User interaction

- `AutoTagRoomsForm` modal dialog; transaction commits on OK only

## MCP notes

- Proposed tool: `revit_auto_tag_rooms`
- Parameters: `level_id`, `room_tag_type_id`, optional `view_id`
- Returns: count and ids of created room tags
- MCP descriptor: `docs/mcp/AutoTagRooms/autotagrooms.json`

## See also

- MCP descriptor: `docs/mcp/AutoTagRooms/autotagrooms.json`
