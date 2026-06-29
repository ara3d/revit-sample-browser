# Command

| Field | Value |
|-------|-------|
| **Sample** | RebarContainerAnyShapeType |
| **Class** | `Command` |
| **Source** | `src/RebarContainerAnyShapeType/Command.cs` |
| **MCP rating** | 2/5 |

Creates reinforcement rebars on a selected concrete beam or column that has no existing reinforcement, using any-shape rebar container logic.

## What it demonstrates

- `FrameReinMakerFactory` validating selection (`AssertData`) and creating rebars (`Work`)
- Transaction-wrapped reinforcement placement for structural concrete members
- Factory pattern shared with the Reinforcement sample

## Prerequisites

- Selected concrete beam or column without existing reinforcement

## User interaction

- Relies on current selection; no separate property dialog in the command entry point

## MCP notes

Structural rebar creation is specialized and selection-dependent; poor fit for generic MCP without host element id and bar layout parameters.

## See also

- Related sample: [Reinforcement](../Reinforcement/reinforcement.md)
