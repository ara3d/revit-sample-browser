# Command

| Field | Value |
|-------|-------|
| **Sample** | ImportExport |
| **Class** | `Command` |
| **Source** | `src/ImportExport/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 5/5 |

Provides a unified dialog for importing and exporting Revit data formats (DWG, DWF, images, GBXML, etc.).

## What it demonstrates

- `MainData` encapsulating export/import options for the active document
- `MainForm` driving format-specific sub-dialogs and API calls
- Validation that an active `Document` exists before showing the UI

## Prerequisites

- Active project document; target paths and views vary by chosen format in the form

## User interaction

- Large modal `MainForm`; each operation configured through nested UI

## MCP notes

- Proposed tool family: `revit_export` / `revit_import` with `format`, `path`, `view_id`, and format-specific options. Map each `MainData` operation to a headless function.

## See also

- MCP descriptor: `src/ImportExport/importexport.json`
