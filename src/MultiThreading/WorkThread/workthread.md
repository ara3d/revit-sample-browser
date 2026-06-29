# Command

| Field | Value |
|-------|-------|
| **Sample** | MultiThreading/WorkThread |
| **Class** | `Command` |
| **Source** | `src/MultiThreading/WorkThread/CS/Command.cs` |
| **SDK ReadMe** | `src/MultiThreading/WorkThread/CS/Readme_WorkThread.rtf` |
| **MCP rating** | 2/5 |

Prompts for a wall or face-wall face, then runs a background worker thread to analyze face geometry while respecting Revit API threading rules.

## What it demonstrates

- `Selection.PickObject(ObjectType.Face)` with stable reference serialization via `ConvertToStableRepresentation`
- `Application.RunAnalyzer` dispatching work to a worker thread with shared results
- `FaceAnalyzer` pattern for CPU-heavy geometry work outside the main API thread

## Prerequisites

- Project with walls or face-walls; user must pick a face interactively

## User interaction

- Face pick required; analysis progress is driven by the external application, not this command alone

## MCP notes

- Threading demonstration rather than a document operation; poor MCP fit unless face reference and analysis type are parameterized.
