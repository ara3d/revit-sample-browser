# Command

| Field | Value |
|-------|-------|
| **Sample** | CreateBeamSystem |
| **Class** | `Command` |
| **Source** | `src/CreateBeamSystem/Command.cs` |
| **MCP rating** | 4/5 |

Collects a closed polygon sketch from the user and creates a `BeamSystem` with chosen layout rule and beam type in the active view.

## What it demonstrates

- `BeamSystemData` and interactive sketching via `BeamSystemForm`
- `BeamSystem.Create(document, curves, sketchPlane, elevation)` with `LayoutRule` and `BeamType` assignment
- `BeamSystemBuilder` translating form data into API calls

## Prerequisites

- Active plan or sketch view with a valid sketch plane
- At least one structural framing type for beam system members

## User interaction

- `BeamSystemForm` drives profile sketching and parameter entry; cancel rolls back the transaction
- `BeamSystemBuilder.CreateBeamSystem` is the automation entry point

## MCP notes

- Proposed tool: `revit_create_beam_system`
- Parameters: `profile_curve_points[]`, `layout_rule`, `beam_type_id`, optional `elevation`
- Returns: new `BeamSystem` element id
- MCP descriptor: `src/CreateBeamSystem/createbeamsystem.json`

## See also

- MCP descriptor: `src/CreateBeamSystem/createbeamsystem.json`
