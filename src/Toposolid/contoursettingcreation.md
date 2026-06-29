# ContourSettingCreation

| Field | Value |
|-------|-------|
| **Sample** | Toposolid |
| **Class** | `ContourSettingCreation` |
| **Source** | `src/Toposolid/Command.cs` |
| **SDK ReadMe** | `src/Toposolid/ReadMe_Toposolid.rtf` |
| **MCP rating** | 2/5 |

Adds contour range and single-contour items to the first `ToposolidType` in the active document.

## What it demonstrates

- `ToposolidType.GetContourSetting()` and `ContourSetting.AddContourRange` / `AddSingleContour`
- Assigning contour categories (`OST_ToposolidContours`, `OST_ToposolidSecondaryContours`, `OST_ToposolidSplitLines`)
- Type-level contour configuration inside a manual `Transaction`

## Prerequisites

- At least one `ToposolidType` loaded in the project

## User interaction

- Runs headlessly with hard-coded elevation values (1–13 ft ranges)
- No selection or dialog required

## MCP notes

- Niche type-setting workflow; better exposed as parameters on a dedicated toposolid-type tool than as a standalone command.

## See also

- [ContourSettingModification](contoursettingmodification.md)
- [ToposolidCreation](toposolidcreation.md)
