# Command

| Field | Value |
|-------|-------|
| **Sample** | StairsAutomation |
| **Class** | `Command` |
| **Source** | `src/StairsAutomation/Command.cs` |
| **MCP rating** | 4/5 |

Generates one of five predefined stair configurations on each run, cycling through straight, curved, and multi-run layouts.

## What it demonstrates

- `StairsAutomationUtility.Create` and `GenerateStairs` orchestrating stair creation
- Component classes: `StairsSingleStraightRun`, `StairsSingleCurvedRun`, `StairsStandardConfiguration`, sketched run variants
- Level pairing from hardcoded "Level 1", "Level 2", "Level 3" names
- Landing components and `IStairsConfiguration` pattern for runs and landings

## Prerequisites

- Project with Level 1, Level 2, and Level 3 (for multi-level configs)

## User interaction

- No dialog; each invocation advances an internal config index (0–4)

## MCP notes

- Proposed tool: `revit_create_stairs_automation`
- Parameters: `configuration_index` (0–4), optional level names and transform offsets
- Returns: new stairs element id
- MCP descriptor: `src/StairsAutomation/stairsautomation.json`

## See also

- MCP descriptor: `src/StairsAutomation/stairsautomation.json`
