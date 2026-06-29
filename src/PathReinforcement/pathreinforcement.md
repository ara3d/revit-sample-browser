# Command

| Field | Value |
|-------|-------|
| **Sample** | PathReinforcement |
| **Class** | `Command` |
| **Source** | `src/PathReinforcement/Command.cs` |
| **SDK ReadMe** | `src/PathReinforcement/ReadMe_PathReinforcement.rtf` |
| **MCP rating** | 2/5 |

Opens an editor dialog for a selected `PathReinforcement` element, listing rebar bar types and exposing path reinforcement properties.

## What it demonstrates

- Validating selection of a single `Autodesk.Revit.DB.Structure.PathReinforcement`
- Collecting `RebarBarType` elements into a static `BarTypes` hashtable keyed by name
- `PathReinforcementForm` and `PathReinProperties` for inspecting and editing reinforcement settings

## Prerequisites

- Exactly one path reinforcement element selected in a structural project

## User interaction

- Modal `PathReinforcementForm`; edits occur through the dialog within a transaction

## MCP notes

- Inspection/editing UI for one reinforcement type; automation would need path reinforcement id and property payloads instead of the form.
