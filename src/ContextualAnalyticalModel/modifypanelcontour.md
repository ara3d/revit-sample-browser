# ModifyPanelContour

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `ModifyPanelContour` |
| **Source** | `src/ContextualAnalyticalModel/ModifyPanelContour.cs` |
| **MCP rating** | 2/5 |

Replaces one straight boundary edge of an analytical panel with an arc using `SketchEditScope` and sketch editing.

## What it demonstrates

- `SketchEditScope.StartWithNewSketch(panelId)` to edit panel boundary sketch
- Locating a `Line` in `Sketch.Profile`, deleting it, and adding `Arc` via `NewModelCurve`
- `FailurePreproccessor` (`IFailuresPreprocessor`) passed to `sketchEditScope.Commit`
- Depends on `CreateAnalyticalPanel.CreateAmPanel` for initial geometry

## Prerequisites

- Panel with a rectangular sketch profile containing at least one line segment

## User interaction

- None — creates panel then modifies first found line automatically
- Arc bulge offset (20 ft) is hard-coded from line midpoint and normal

## MCP notes

- Sketch-edit workflows are hard to expose generically; would need parameterized edge index, arc geometry, and failure handling — not a strong MCP candidate as written.

## See also

- Related: [createanalyticalpanel.md](createanalyticalpanel.md), [moveanalyticalpanelusingsketcheditscope.md](moveanalyticalpanelusingsketcheditscope.md)
