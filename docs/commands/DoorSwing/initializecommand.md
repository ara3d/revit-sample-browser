# InitializeCommand

| Field | Value |
|-------|-------|
| **Sample** | DoorSwing |
| **Class** | `InitializeCommand` |
| **Source** | `src/DoorSwing/CS/Command.cs` |
| **SDK ReadMe** | `src/DoorSwing/CS/ReadMe_DoorSwing.rtf` |
| **MCP rating** | 2/5 |

Adds shared parameters and initializes door swing, room, and public-door flags from family geometry and regional standards.

## What it demonstrates

- `DoorSwingData` setup and `InitializeForm` for country/standard choices
- `UpdateDoorFamiliesOpeningFeature` on door types from family geometry
- `DoorSwingData.UpdateDoorsInfo` syncing instance parameters after temp instances are removed

## Prerequisites

- Project with door families; shared parameter file configured per sample

## User interaction

- `InitializeForm` modal workflow; cancel rolls back transaction

## MCP notes

- Domain-specific door swing rules and shared-parameter setup — niche automation, heavy UI coupling.

## See also

- Related: [updateparamscommand.md](updateparamscommand.md), [updategeometrycommand.md](updategeometrycommand.md)
