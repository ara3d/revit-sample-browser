# Command

| Field | Value |
|-------|-------|
| **Sample** | ReferencePlane |
| **Class** | `Command` |
| **Source** | `src/ReferencePlane/CS/Command.cs` |
| **SDK ReadMe** | `src/ReferencePlane/CS/ReadMe_ReferencePlane.rtf` |
| **MCP rating** | 4/5 |

Manages reference planes in the active document: list existing planes and create new ones relative to walls or floors.

## What it demonstrates

- `ReferencePlaneMgr` collecting reference plane data into a `DataTable` for UI binding
- Host-specific creation delegates for walls and floors
- `ReferencePlaneForm` driving create and inspect operations inside a transaction

## Prerequisites

- Project document; wall or floor hosts needed for host-based creation

## User interaction

- Modal `ReferencePlaneForm`; commit on OK, rollback on cancel
- Core mgr methods are separable for parameterized creation

## MCP notes

- Proposed tools: `revit_list_reference_planes` and `revit_create_reference_plane`
- Parameters: host element id, offset or bubble/end coordinates as needed
- Returns: reference plane element ids and parameter snapshot
- MCP descriptor: `docs/mcp/ReferencePlane/referenceplane.json`

## See also

- MCP descriptor: `docs/mcp/ReferencePlane/referenceplane.json`
- Manager: `src/ReferencePlane/CS/ReferencePlaneMgr.cs`
