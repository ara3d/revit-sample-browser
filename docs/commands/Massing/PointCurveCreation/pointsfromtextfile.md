# PointsFromTextFile

| Field | Value |
|-------|-------|
| **Sample** | Massing/PointCurveCreation |
| **Class** | `PointsFromTextFile` |
| **Source** | `src/Massing/PointCurveCreation/CS/Command.cs` |
| **SDK ReadMe** | `src/Massing/PointCurveCreation/CS/ReadMe_PointCurveCreation.rtf` |
| **MCP rating** | 2/5 |

Reads comma-separated XYZ coordinates from `sphere.csv` beside the add-in and creates a reference point at each location.

## What it demonstrates

- Loading CSV point data from the assembly directory via `Assembly.GetExecutingAssembly().Location`
- `FamilyCreate.NewReferencePoint(XYZ)` in a loop over file lines

## Prerequisites

- Conceptual mass family document
- `sphere.csv` deployed next to the add-in DLL (included in the sample folder)

## User interaction

- No UI; silently skips if the file is absent

## MCP notes

- Simple file-import demo; MCP would need an explicit file path parameter instead of a fixed filename.
