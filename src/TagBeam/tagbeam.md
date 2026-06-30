# Command

| Field | Value |
|-------|-------|
| **Sample** | TagBeam |
| **Class** | `Command` |
| **Source** | `src/TagBeam/Command.cs` |
| **MCP rating** | 4/5 |

Tags both ends of each selected structural beam using user-chosen tag mode, family symbol, leader, and orientation.

## What it demonstrates

- `TagBeamData` collecting beams and tag `FamilySymbol` types by category
- `IndependentTag.Create` at beam curve endpoints with `TagMode` and `TagOrientation`
- `ChangeTypeId` to apply the selected tag family symbol
- `TagBeamForm` for tag options

## Prerequisites

- Active view and one or more structural beams selected
- Loaded tag families for structural framing, material, or multi-category tags

## User interaction

- `TagBeamForm` required for tag type and mode; core `CreateTag` logic is separable

## MCP notes

- Proposed tool: `revit_tag_beams`
- Parameters: `beam_ids[]`, `tag_type_id`, `tag_mode`, `has_leader`, `orientation`
- Returns: created tag element ids
- MCP descriptor: `src/TagBeam/tagbeam.json`

## See also

- MCP descriptor: `src/TagBeam/tagbeam.json`
