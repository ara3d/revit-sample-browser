# Command

| Field | Value |
|-------|-------|
| **Sample** | VersionChecking |
| **Class** | `Command` |
| **Source** | `src/VersionChecking/CS/VersionChecking.cs` |
| **SDK ReadMe** | `src/VersionChecking/CS/ReadMe_VersionChecking.rtf` |
| **MCP rating** | 5/5 |

Reads Revit product name, version number, and build from `Application` and displays them in a form.

## What it demonstrates

- `Application.VersionName`, `VersionNumber`, and `VersionBuild` on `revit.Application.Application`
- Read-only command (`TransactionMode.ReadOnly`) with no document changes
- Simple property binding to `VersionCheckingForm`

## Prerequisites

- None; uses the running Revit application instance

## User interaction

- `VersionCheckingForm` modal dialog showing the three version strings
- Trivially headless: return the properties without showing the form

## MCP notes

- Proposed tool: `revit_get_version_info`
- Parameters: none
- Returns: `product_name`, `version_number`, `build_number`
- MCP descriptor: `src/VersionChecking/versionchecking.json`

## See also

- MCP descriptor: `src/VersionChecking/versionchecking.json`
