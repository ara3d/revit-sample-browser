# DuplicateAcrossDocumentsCommand

| Field | Value |
|-------|-------|
| **Sample** | DuplicateViews |
| **Class** | `DuplicateAcrossDocumentsCommand` |
| **Source** | `src/DuplicateViews/DuplicateAcrossDocumentsCommand.cs` |
| **SDK ReadMe** | `src/DuplicateViews/Readme_DuplicateViews.rtf` |
| **MCP rating** | 4/5 |

Copies all schedules and drafting views from the active document into the only other open project, including view-specific detailing on drafting views.

## What it demonstrates

- Collecting `ViewSchedule` and `ViewDrafting` views with `FilteredElementCollector` and `ElementMulticlassFilter`
- Cross-document copy via `ElementTransformUtils.CopyElements` with `CopyPasteOptions` and duplicate-type handlers
- Batch schedule copy vs. `TransactionGroup` for drafting views plus per-view detail copy
- Skipping view-specific schedules (e.g. revision schedules) with `WhereElementIsViewIndependent`

## Prerequisites

- Exactly two documents open: source (active) and target
- Schedules and drafting views present in the source document

## User interaction

- No picks; runs immediately and shows a statistics `TaskDialog`
- Target document is inferred automatically (the non-active open document)

## MCP notes

- Proposed tool: `revit_copy_views_across_documents`
- Parameters: `source_doc` (optional, default active), `target_doc_id`, optional `view_ids[]` to limit schedules/drafting views
- Returns: counts of copied schedules, drafting views, and detailing elements created
- Refactor to accept explicit document ids instead of requiring exactly two open documents
- MCP descriptor: `src/DuplicateViews/duplicateacrossdocumentscommand.json`

## See also

- MCP descriptor: `src/DuplicateViews/duplicateacrossdocumentscommand.json`
- Helper: `DuplicateViewUtils` in `src/DuplicateViews/DuplicateViewUtils.cs`
