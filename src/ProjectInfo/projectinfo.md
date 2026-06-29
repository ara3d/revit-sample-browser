# Command

| Field | Value |
|-------|-------|
| **Sample** | ProjectInfo |
| **Class** | `Command` |
| **Source** | `src/ProjectInfo/Command.cs` |
| **SDK ReadMe** | `src/ProjectInfo/ReadMe_ProjectInfo.rtf` |
| **MCP rating** | 5/5 |

Opens a property-grid editor for project information, site location, construction, and energy analysis settings on the active document.

## What it demonstrates

- Wrapping `ProjectInformation` with `ProjectInfoWrapper` and nested wrappers (`EnergyDataSettingsWrapper`, `SiteLocationWrapper`, etc.)
- Custom type converters for Revit enums, angles, element ids, and time zones
- Transaction commit on OK, rollback on cancel via `ProjectInfoForm`
- `RevitStartInfo` static maps for gbXML building types, service types, and HVAC enums

## Prerequisites

- Active project document with `ProjectInformation` element

## User interaction

- Modal `ProjectInfoForm` with property grid; changes persist only on OK
- Read and write paths are separable for MCP if dialogs are bypassed

## MCP notes

- Proposed tools: `revit_get_project_info` and `revit_set_project_info`
- Parameters: field names matching wrapper properties (issue date, status, energy settings, site location, etc.)
- Returns: structured project metadata object
- MCP descriptor: `src/ProjectInfo/projectinfo.json`

## See also

- MCP descriptor: `src/ProjectInfo/projectinfo.json`
- Wrappers: `src/ProjectInfo/Wrappers/`
