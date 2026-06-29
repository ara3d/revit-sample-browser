# AssociativeSectionUpdater

| Field | Value |
|-------|-------|
| **Sample** | DynamicModelUpdate |
| **Class** | `AssociativeSectionUpdater` |
| **Source** | `src/DynamicModelUpdate/Application.cs` |
| **SDK ReadMe** | `src/DynamicModelUpdate/ReadMe_DynamicModelUpdate.rtf` |
| **MCP rating** | 2/5 |

Registers a dynamic model updater so a section view moves and rotates when an associated window family instance changes geometry.

## What it demonstrates

- Implementing `IUpdater` with `UpdaterRegistry.RegisterUpdater` and geometry-change triggers
- `SectionUpdater.Execute` reacting to window moves via `ElementTransformUtils.RotateElement` and `MoveElement`
- Associating a `ViewSection` with a `FamilyInstance` through user picks and trigger setup
- Unregistering the updater on `DocumentClosing`

## Prerequisites

- Project with a section view and a window `FamilyInstance`
- Updater registered once per session via static `SectionUpdater` instance

## User interaction

- Prompts to pick a section element, then a window; shows confirmation dialogs
- Requires interactive selection; not suitable for headless runs without refactoring

## MCP notes

- Illustrates updater patterns but ties behavior to interactive picks and a demo association workflow, not a general automation tool
