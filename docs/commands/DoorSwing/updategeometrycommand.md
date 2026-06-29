# UpdateGeometryCommand

| Field | Value |
|-------|-------|
| **Sample** | DoorSwing |
| **Class** | `UpdateGeometryCommand` |
| **Source** | `src/DoorSwing/CS/Command.cs` |
| **SDK ReadMe** | `src/DoorSwing/CS/ReadMe_DoorSwing.rtf` |
| **MCP rating** | 2/5 |

Updates door instance geometry to match current To/From room parameter values for the selection or entire project.

## What it demonstrates

- `DoorSwingData.UpdateDoorsGeometry(document, selectionOnly)` 
- Selection-aware path when `GetElementIds` is non-empty
- Transaction-wrapped geometry sync from room assignment parameters

## Prerequisites

- Doors initialized with DoorSwing shared parameters

## User interaction

- Optional pre-selection limits scope; empty selection updates all doors

## MCP notes

- Geometry flip driven by room parameters — possible to automate with door ids but tied to sample-specific parameter schema.

## See also

- Related: [initializecommand.md](initializecommand.md), [updateparamscommand.md](updateparamscommand.md)
