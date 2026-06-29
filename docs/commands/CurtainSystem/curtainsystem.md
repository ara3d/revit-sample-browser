# Command

| Field | Value |
|-------|-------|
| **Sample** | CurtainSystem |
| **Class** | `Command` |
| **Source** | `src/CurtainSystem/CS/Command.cs` |
| **SDK ReadMe** | `src/CurtainSystem/CS/ReadMe_CurtainSystem.rtf` |
| **MCP rating** | 4/5 |

Validates a selected mass form and opens a dialog to create, list, and delete curtain systems on mass faces.

## What it demonstrates

- `MassChecker` verifying a parallelepiped-shaped mass selection
- `Document.Create.NewCurtainSystem` / `NewCurtainSystem2` on `FaceArray` or `ReferenceArray`
- `TransactionGroup` wrapping UI-driven curtain system operations
- `SystemData` and `CurtainForm` for face picking and type assignment

## Prerequisites

- Project with a mass in a parallelepiped shape (sample `CurtainSystem.rvt` provided)

## User interaction

- `CurtainForm` modal workflow; invalid mass selection cancels with a message

## MCP notes

- Proposed tool: `revit_create_curtain_system`
- Parameters: `face_references[]` or `mass_id` + face indices, `curtain_system_type_id`
- Returns: curtain system element id(s)
- MCP descriptor: `docs/mcp/CurtainSystem/curtainsystem.json`

## See also

- MCP descriptor: `docs/mcp/CurtainSystem/curtainsystem.json`
