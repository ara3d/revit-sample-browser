# Third-party notices

## Revit Database Explorer

Portions of `src/Common/` are adapted from [RevitDBExplorer](https://github.com/NeVeSpl/RevitDBExplorer) by NeVeSpl.

- **License:** [Apache License 2.0](https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md)
- **Imported version:** v2.6.1 (`6929da81491a7f9ef69ed4c346afa1c582b830b5`)
- **Import manifest:** [`scripts/rdbe-import-manifest.json`](scripts/rdbe-import-manifest.json)
- **Import script:** [`scripts/import_rdbe_common.py`](scripts/import_rdbe_common.py)

Included areas:

- Revit API extension methods (`ElementId`, geometry, document, parameters, bounding boxes, XYZ, curves)
- General infrastructure helpers (string, enumerable, double formatting)
- Revit database query (RDQ) engine under `src/Common/Document/Query/`

WPF-specific autocompletion UI from upstream was intentionally excluded.
