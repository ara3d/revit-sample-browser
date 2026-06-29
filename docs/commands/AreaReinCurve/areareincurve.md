# Command

| Field | Value |
|-------|-------|
| **Sample** | AreaReinCurve |
| **Class** | `Command` |
| **Source** | `src/AreaReinCurve/CS/AreaReinCurve.cs` |
| **SDK ReadMe** | `src/AreaReinCurve/CS/ReadMe_AreaReinCurve.rtf` |
| **MCP rating** | 2/5 |

Modifies layer visibility and parameters on curves belonging to a single rectangular area reinforcement.

## What it demonstrates

- Validating selection contains exactly one rectangular `AreaReinforcement`
- Accessing `AreaReinforcementCurve` children and `BuiltInParameter` values
- Toggling reinforcement layer visibility via view/graphic overrides

## Prerequisites

- Pre-select one rectangular area reinforcement; view layers must exist as expected

## User interaction

- Uses current selection only; no dialog
- Fails with explicit messages for wrong selection or layer state

## MCP notes

- Niche structural demo; parameter names and layer rules are hard-coded to the sample scenario
