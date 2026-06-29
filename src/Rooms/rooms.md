# Command

| Field | Value |
|-------|-------|
| **Sample** | Rooms |
| **Class** | `Command` |
| **Source** | `src/Rooms/Command.cs` |
| **MCP rating** | 4/5 |

Displays room and room-tag information and supports tagging, renaming, and department assignment operations from a WinForms UI.

## What it demonstrates

- `RoomsData` collecting rooms, tags, and department metadata from the document
- `RoomsInformationForm` for browsing and editing room properties
- Transaction wrapping UI-driven room modifications

## Prerequisites

- Architectural rooms in the project for meaningful data

## User interaction

- Modal `RoomsInformationForm`; commit on close after edits
- `RoomsData` methods can be called directly for automation

## MCP notes

- Proposed tools: `revit_list_rooms` and `revit_update_room`
- Parameters: room id, name, number, department, tag type for create-tag operations
- Returns: room inventory and operation results
- MCP descriptor: `src/Rooms/rooms.json`

## See also

- MCP descriptor: `src/Rooms/rooms.json`
- Data class: `src/Rooms/RoomsData.cs`
