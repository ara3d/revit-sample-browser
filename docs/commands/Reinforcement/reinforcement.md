# Command

| Field | Value |
|-------|-------|
| **Sample** | Reinforcement |
| **Class** | `Command` |
| **Source** | `src/Reinforcement/CS/Command.cs` |
| **MCP rating** | 2/5 |

Adds standard reinforcement rebars to a selected concrete beam or column that lacks existing reinforcement.

## What it demonstrates

- `FrameReinMakerFactory.AssertData` validating concrete structural selection
- `FrameReinMakerFactory.Work` generating rebars for the chosen member type
- Single transaction wrapping the reinforcement workflow

## Prerequisites

- Pre-selected concrete beam or column without reinforcement

## User interaction

- Uses current selection only; no form in the command class

## MCP notes

Same factory pattern as RebarContainerAnyShapeType but for conventional rebar layouts; selection-bound and not a general MCP tool.

## See also

- Related: [RebarContainerAnyShapeType](../RebarContainerAnyShapeType/rebarcontaineranyshapetype.md)
