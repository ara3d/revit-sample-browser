# DatumPropagation

| Field | Value |
|-------|-------|
| **Sample** | DatumsModification |
| **Class** | `DatumPropagation` |
| **Source** | `src/DatumsModification/DatumsModificationCmd.cs` |
| **SDK ReadMe** | `src/DatumsModification/ReadMe_DatumsModification.rtf` |
| **MCP rating** | 2/5 |

Propagates view-specific appearance of a single selected datum to other views chosen from its propagation view list.

## What it demonstrates

- `DatumPlane.GetPropagationViews(activeView)` to enumerate candidate views
- `DatumPlane.PropagateToViews(sourceView, targetViewIds)` inside a transaction
- `PropogateSetting` dialog with checked list of target views

## Prerequisites

- Pre-select exactly one datum plane
- Active view must be a valid propagation source for that datum

## User interaction

- `PropogateSetting` checked list selects destination views

## MCP notes

- Propagation is view-appearance specific and UI-driven — not a strong headless MCP operation as implemented.

## See also

- Related: [datumalignment.md](datumalignment.md), [datumstylemodification.md](datumstylemodification.md)
