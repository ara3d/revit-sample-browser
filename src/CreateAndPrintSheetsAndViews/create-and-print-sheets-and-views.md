# CreateAndPrintSheetsAndViews

| Field | Value |
|-------|-------|
| **Sample** | CreateAndPrintSheetsAndViews |
| **Classes** | `Command`, `CmdCreateAndPrintSheetAndViews` |
| **Source** | `src/CreateAndPrintSheetsAndViews/` |
| **MCP rating** | 3/5 |

Dynamically generates from scratch and prints on the fly to PDF and JPG an individual sheet and top, side, front, and isometric views for HVAC ductwork fabrication parts, optionally without committing a transaction.

## What it demonstrates

- Creating section views, 3D isometric views, and a sheet with placed viewports for a single element
- Orienting views using fabrication-part duct local coordinate system (LCS) from the primary connector
- Temporary element isolation in views for per-part export
- `DialogBoxShowing` handler to auto-accept the "Export with Temporary Hide/Isolate" task dialog
- Batch processing with per-element transaction rollback so exports run without persisting model changes
- PDF export via `PDFExportOptions` and image export via `ImageExportOptions`

## Commands

| Class | Purpose |
|-------|---------|
| `Command` | Batch-process pre-selected or picked fabrication parts matching known product codes; roll back each transaction after export |
| `CmdCreateAndPrintSheetAndViews` | Pick one element, create sheet and four views, export, and optionally save via user prompt |

## Prerequisites

- Project containing fabrication ductwork (`FabricationPart` elements)
- Title block family symbol named **AIR CRO - Plano DIN A4** loaded in the project
- Writable output directory (default `C:/tmp`)

## User interaction

- `Command`: uses pre-selection or prompts to pick fabrication parts; shows summary task dialog
- `CmdCreateAndPrintSheetAndViews`: prompts to pick one element; asks whether to save the sheet
- Both commands subscribe to `DialogBoxShowing` to suppress the temporary hide/isolate export warning

## MCP notes

- Proposed tools: `revit_create_and_print_sheets_batch`, `revit_create_and_print_sheet_for_element`
- Headless use would require parameterizing element selection, output path, title block name, and save/commit behavior
- MCP descriptors:
  - `src/CreateAndPrintSheetsAndViews/create-and-print-sheets-batch.json`
  - `src/CreateAndPrintSheetsAndViews/create-and-print-sheet-for-element.json`

## See also

- Upstream: [jeremytammik/CreateAndPrintSheetsAndViews](https://github.com/jeremytammik/CreateAndPrintSheetsAndViews)
- Related: [CreateViewSection/createviewsection.md](../CreateViewSection/createviewsection.md) (section view creation)
