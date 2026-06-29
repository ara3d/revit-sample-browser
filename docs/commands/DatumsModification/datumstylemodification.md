# DatumStyleModification

| Field | Value |
|-------|-------|
| **Sample** | DatumsModification |
| **Class** | `DatumStyleModification` |
| **Source** | `src/DatumsModification/CS/DatumsModificationCmd.cs` |
| **SDK ReadMe** | `src/DatumsModification/CS/ReadMe_DatumsModification.rtf` |
| **MCP rating** | 2/5 |

Applies bubble visibility, 2D vs model extent, and leader elbow settings to selected datums in the active view.

## What it demonstrates

- `DatumPlane.ShowBubbleInView` / `HideBubbleInView` for `DatumEnds.End0` and `End1`
- `SetDatumExtentType` toggling `DatumExtentType.ViewSpecific` vs `Model`
- `AddLeader`, `GetLeader`, `SetLeader`, and `CalculateLeader` for elbow placement
- Static flags set from `DatumStyleSetting` dialog before the transaction loop

## Prerequisites

- Pre-selected datum planes in the active view

## User interaction

- `DatumStyleSetting` modal form configures all style options

## MCP notes

- Style changes are per-view and configured through a dialog — poor MCP fit without parameterized API wrapper.

## See also

- Related: [datumalignment.md](datumalignment.md), [datumpropagation.md](datumpropagation.md)
