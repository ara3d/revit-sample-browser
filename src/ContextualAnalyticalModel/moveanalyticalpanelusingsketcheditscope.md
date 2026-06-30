# MoveAnalyticalPanelUsingSketchEditScope

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `MoveAnalyticalPanelUsingSketchEditScope` |
| **Source** | `src/ContextualAnalyticalModel/MoveAnalyticalPanelUsingSketchEditScope.cs` |
| **MCP rating** | 2/5 |

Offsets every boundary line of an analytical panel sketch, which moves the panel and breaks connectivity to a connected member.

## What it demonstrates

- `SketchEditScope` on an existing `AnalyticalPanel` sketch
- Iterating `Sketch.Profile` curve arrays, deleting each `Line`, and recreating offset lines (+5 ft in X and Y)
- `sketchEditScope.Commit(new FailurePreproccessor())` after transaction
- Contrast with `ElementTransformUtils` panel move that preserves connections

## Prerequisites

- Panel with line-based sketch profile; command creates panel and member first

## User interaction

- None after automatic geometry setup
- Fixed 5 ft offset on all boundary segments

## MCP notes

- Sketch-level edits are complex to parameterize; poor MCP fit without refactoring to accept panel ID and offset vector.

## See also

- Related: [moveanalyticalpanelusingelementtransformutils.md](moveanalyticalpanelusingelementtransformutils.md), [modifypanelcontour.md](modifypanelcontour.md)
