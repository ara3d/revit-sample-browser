# CmdCableTray4

| Field | Value |
|-------|-------|
| **Sample** | CableTraySample |
| **Class** | `CmdCableTray4` |
| **Source** | `src/CableTraySample/CmdCableTray4.cs` |
| **Origin** | [jeremytammik/CableTraySample](https://github.com/jeremytammik/CableTraySample) (MIT) |
| **MCP rating** | 3/5 |

Creates horizontal and vertical cable tray segments, rotates the vertical segment so connector coordinate systems align, then connects and inserts an elbow fitting. This is the working solution from the upstream forum investigation.

## What it demonstrates

- Reading `Connector.CoordinateSystem` on cable tray connectors
- Computing rotation with `BasisY.AngleOnPlaneTo` to align tray orientation
- `LocationCurve.Rotate` about a vertical axis at the junction
- DEBUG assertions verifying connector Z aligns with tray direction

## Prerequisites

- MEP-enabled project with cable tray type `Default` and level `Level 1`

## User interaction

- Non-interactive; uses fixed coordinates from the forum repro case

## MCP notes

- Proposed tool: `revit_cable_tray_elbow_align`
- Most reusable variant for automated cable tray routing once endpoints are parameterized
- MCP descriptor: `src/CableTraySample/cable-tray-elbow-align.json`

## See also

- MCP descriptor: `src/CableTraySample/cable-tray-elbow-align.json`
- `CmdCableTray` — same geometry without pre-rotation alignment
- [Revit API forum: Failed to insert elbow for cable tray](http://forums.autodesk.com/t5/revit-api/get-quot-failed-to-insert-elbow-quot-when-calling/m-p/5398815)
