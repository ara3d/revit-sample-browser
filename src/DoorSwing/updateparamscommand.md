# UpdateParamsCommand

| Field | Value |
|-------|-------|
| **Sample** | DoorSwing |
| **Class** | `UpdateParamsCommand` |
| **Source** | `src/DoorSwing/CS/Command.cs` |
| **SDK ReadMe** | `src/DoorSwing/CS/ReadMe_DoorSwing.rtf` |
| **MCP rating** | 2/5 |

Refreshes door opening, To/From room, and public-door parameters from current door geometry for selected doors or the whole model.

## What it demonstrates

- `DoorSwingData.UpdateDoorsInfo(document, selectionOnly, updateRooms, ref message)`
- Selection vs whole-document modes based on `GetElementIds`
- Transaction commit/rollback tied to `Result` from update helper

## Prerequisites

- DoorSwing shared parameters bound to door instances

## User interaction

- No dialog; optional selection scopes the update

## MCP notes

- Parameter sync utility — automatable in principle but depends on sample-specific `DoorSwingData` rules.

## See also

- Related: [initializecommand.md](initializecommand.md), [updategeometrycommand.md](updategeometrycommand.md)
