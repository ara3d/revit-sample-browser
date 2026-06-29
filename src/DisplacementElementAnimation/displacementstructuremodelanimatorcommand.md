# DisplacementStructureModelAnimatorCommand

| Field | Value |
|-------|-------|
| **Sample** | DisplacementElementAnimation |
| **Class** | `DisplacementStructureModelAnimatorCommand` |
| **Source** | `src/DisplacementElementAnimation/DisplacementStructureModelAnimatorCommand.cs` |
| **SDK ReadMe** | `src/DisplacementElementAnimation/ReadMe_DisplacementElementAnimation.rtf` |
| **MCP rating** | 1/5 |

Starts a continuous structural displacement animation that moves selected elements through staged offsets using displacement paths.

## What it demonstrates

- `DisplacementStructureModelAnimator` constructed with `autoPlay: true`
- `StartAnimation` driving timed updates of element displacement
- Visual demonstration of `DisplacementElement` animation API (see animator class)

## Prerequisites

- Sample model with elements configured for displacement animation

## User interaction

- Fully visual/timed animation — no parameters or return data

## MCP notes

- Animation playback is UI-only with no stable automation surface — not suitable as an MCP tool.

## See also

- Related: [displacementstructuremodelanimatorcommandstepbystep.md](displacementstructuremodelanimatorcommandstepbystep.md)
