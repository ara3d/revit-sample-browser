# DisplacementStructureModelAnimatorCommandStepByStep

| Field | Value |
|-------|-------|
| **Sample** | DisplacementElementAnimation |
| **Class** | `DisplacementStructureModelAnimatorCommandStepByStep` |
| **Source** | `src/DisplacementElementAnimation/CS/DisplacementStructureModelAnimatorCommand.cs` |
| **SDK ReadMe** | `src/DisplacementElementAnimation/CS/ReadMe_DisplacementElementAnimation.rtf` |
| **MCP rating** | 1/5 |

Advances a structural displacement animation one step per invocation, or starts the animator on first run.

## What it demonstrates

- Static `DisplacementstructuremodelAnimator` singleton retaining animator state
- First call: `new DisplacementStructureModelAnimator(app, false)` and `StartAnimation`
- Subsequent calls: `AnimateNextStep` for manual stepping

## Prerequisites

- Same displacement animation setup as the continuous command

## User interaction

- User must invoke the command repeatedly to advance frames

## MCP notes

- Step-by-step visual demo only — no queryable output for agents.

## See also

- Related: [displacementstructuremodelanimatorcommand.md](displacementstructuremodelanimatorcommand.md)
