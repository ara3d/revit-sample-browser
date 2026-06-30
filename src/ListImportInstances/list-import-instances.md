# ListImportInstancesCommand

| Field | Value |
|-------|-------|
| **Sample** | ListImportInstances |
| **Class** | `ListImportInstancesCommand` |
| **Source** | `src/ListImportInstances/ListImportInstancesCommand.cs` |
| **MCP rating** | 4/5 |

Lists `ImportInstance` elements in the active project, groups view-specific and model imports, flags duplicate view-only CAD imports, and writes a text report.

## What it demonstrates

- Collecting `ImportInstance` elements with `FilteredElementCollector.OfClass`
- Separating view-specific imports (by `OwnerViewId`) from model imports
- Mapping import category names back to source CAD file names by stripping `(n)` suffixes
- Sanity check for the same CAD data imported in Current View Only mode into multiple views
- Text report output via `SimpleTextFileBasedReporter` implementing `IReportImportData`

## Prerequisites

- Active project document

## User interaction

- Writes `<project>-ListOfImportedData.txt` next to the RVT (or `Default-ListOfImportedData.txt` if unsaved)
- `TaskDialog` offers to open the report in Notepad when complete
- Warning dialog when duplicate view-specific imports are detected

## MCP notes

- Proposed tool: `revit_list_import_instances`
- Parameters: `output_path` (optional)
- Returns: report file path and duplicate-import warnings
- Headless use would require replacing `TaskDialog` prompts with returned structured data
- MCP descriptor: `src/ListImportInstances/list-import-instances.json`

## See also

- MCP descriptor: `src/ListImportInstances/list-import-instances.json`
- Related: [TBC_ImportsInFamilies/imports-in-families.md](../TBC_ImportsInFamilies/imports-in-families.md) (imports inside families)
- Related: [TBC_RemoveImportedJpgs/remove-imported-jpgs.md](../TBC_RemoveImportedJpgs/remove-imported-jpgs.md) (remove imported JPG references)
- Upstream: [jeremytammik/ListImportInstances](https://github.com/jeremytammik/ListImportInstances)
