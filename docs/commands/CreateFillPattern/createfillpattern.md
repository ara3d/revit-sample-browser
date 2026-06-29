# Command

| Field | Value |
|-------|-------|
| **Sample** | CreateFillPattern |
| **Class** | `Command` |
| **Source** | `src/CreateFillPattern/CS/Command.cs` |
| **SDK ReadMe** | `src/CreateFillPattern/CS/Readme_CreateFillPattern.rtf` |
| **MCP rating** | 4/5 |

Opens a dialog to list, create, and apply model fill patterns and line patterns to selected wall surfaces and cut faces.

## What it demonstrates

- `FillPatternElement.GetFillPatternElementByName` and `FillPatternElement.Create`
- Simple and complex `FillPattern` construction with `FillPatternTarget.Model`
- `LinePatternElement` creation and assignment to wall `CompoundStructure` layers
- Applying patterns to wall surface and cut geometry via material/fill overrides

## Prerequisites

- Project with walls available for selection when applying patterns

## User interaction

- `PatternForm` tree view lists existing fill patterns; buttons create patterns and apply to pre-selected walls
- Requires wall selection before apply operations

## MCP notes

- Proposed tools: `revit_list_fill_patterns`, `revit_create_fill_pattern`, `revit_apply_fill_pattern`
- Parameters: `pattern_name`, `target` (model/drafting), `wall_id`, `apply_to` (surface/cut)
- MCP descriptor: `docs/mcp/CreateFillPattern/createfillpattern.json`

## See also

- MCP descriptor: `docs/mcp/CreateFillPattern/createfillpattern.json`
