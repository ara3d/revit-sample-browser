# CmdCableTray

| Field | Value |
|-------|-------|
| **Sample** | CableTraySample |
| **Class** | `CmdCableTray` |
| **Source** | `src/CableTraySample/CmdCableTray.cs` |
| **Origin** | [jeremytammik/CableTraySample](https://github.com/jeremytammik/CableTraySample) (MIT) |
| **MCP rating** | 2/5 |

Creates a five-segment cable tray path using fixed coordinates, connects the first horizontal segment to the vertical riser, and calls `NewElbowFitting`. Demonstrates the forum repro case for "Failed to insert elbow" on cable trays.

## What it demonstrates

- `CableTray.Create` for multiple connected segments
- Locating connectors by endpoint `XYZ` with `IsAlmostEqualTo`
- `Connector.ConnectTo` followed by `Document.Create.NewElbowFitting`
- Lookup of `CableTrayType` and `Level` by name via `FilteredElementCollector`

## Prerequisites

- MEP-enabled project with a cable tray type named `Default`
- Level named `Level 1`

## User interaction

- Non-interactive; creates geometry at fixed model coordinates

## MCP notes

- Proposed tool: `revit_cable_tray_elbow_fitting`
- Hard-coded coordinates limit reuse; parameterize endpoints and type/level names for MCP
- MCP descriptor: `src/CableTraySample/cable-tray-elbow-fitting.json`

## See also

- MCP descriptor: `src/CableTraySample/cable-tray-elbow-fitting.json`
- `CmdCableTray4` — rotates the vertical segment to align connector coordinate systems before inserting the elbow
- [Revit API forum: Failed to insert elbow for cable tray](http://forums.autodesk.com/t5/revit-api/get-quot-failed-to-insert-elbow-quot-when-calling/m-p/5398815)
