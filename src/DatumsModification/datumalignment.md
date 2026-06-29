# DatumAlignment

| Field | Value |
|-------|-------|
| **Sample** | DatumsModification |
| **Class** | `DatumAlignment` |
| **Source** | `src/DatumsModification/CS/DatumsModificationCmd.cs` |
| **SDK ReadMe** | `src/DatumsModification/CS/ReadMe_DatumsModification.rtf` |
| **MCP rating** | 2/5 |

Aligns multiple selected grid or level datums in the active view to match a user-chosen reference datum's position along the shared axis.

## What it demonstrates

- Building a dictionary of selected `DatumPlane` elements by name
- `DatumPlane.GetCurvesInView` and `SetCurveInView` with view-specific or model extent
- `AlignmentSetting` dialog to pick the reference datum
- `CalculateCurve` shifting parallel datum lines to the base curve coordinates

## Prerequisites

- Pre-selected datum planes (grids or levels) in the active view

## User interaction

- `AlignmentSetting` list box chooses the reference datum; requires selection before launch

## MCP notes

- View-specific alignment with interactive datum picker — limited automation value without explicit reference id.

## See also

- Related: [datumpropagation.md](datumpropagation.md), [datumstylemodification.md](datumstylemodification.md)
