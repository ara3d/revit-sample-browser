# Command

| Field | Value |
|-------|-------|
| **Sample** | ViewPrinter |
| **Class** | `Command` |
| **Source** | `src/ViewPrinter/CS/Command.cs` |
| **MCP rating** | 4/5 |

Wraps Revit's `PrintManager` in a WinForms print dialog mirroring File → Print options.

## What it demonstrates

- `PrintMgr` facade over `Document.PrintManager` (printer, range, collate, copies, print-to-file)
- `PrintMgrForm` UI for view/sheet set selection, print setup, and save-as naming
- Transaction wrapping print settings changes with commit on OK or rollback on cancel
- Installed printer enumeration via `System.Drawing.Printing.PrinterSettings`

## Prerequisites

- At least one installed printer (physical or virtual PDF/DWF/XPS)
- Views or sheet sets to print

## User interaction

- `PrintMgrForm` modal dialog replicating native print UI
- Automation would accept printer name, print range, view/sheet set id, and file output path

## MCP notes

- Proposed tool: `revit_print_views`
- Parameters: `printer_name`, `print_range`, `view_sheet_set_name`, `print_to_file`, `file_path`, `copies`, `collate`
- Returns: print job status or output file path
- MCP descriptor: `src/ViewPrinter/viewprinter.json`

## See also

- MCP descriptor: `src/ViewPrinter/viewprinter.json`
