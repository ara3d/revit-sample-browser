# Refactor Map — Canonical Shared API

Coordination contract for dedup work. Subagents **must not invent** new shared APIs; use these canonical homes only.

Analysis date: 2026-06-30 (`scripts/find_all_util_dupes.py`, `scripts/find_tbc_util_dupes.py`)

## Summary

| Metric | Before refactor |
|--------|----------------:|
| Util.cs files scanned | 92 |
| Exact duplicate body groups | 3 |
| Excess duplicate copies | 5 |
| TBC exact duplicate groups | 0 (prior `TBC_Shared` → `Common` migration complete) |

Most geometry/MEP/document helpers already live in `src/Common`. Remaining duplication is concentrated in **formatting helpers** copied across a few Autodesk samples.

---

## Group 1 — Formatting helpers (P0)

**Canonical home:** [`src/Common/Units/Util.Formatting.cs`](src/Common/Units/Util.Formatting.cs)  
**Namespace:** `BuildingCoder.Util` (partial static class)

| Method | Signature | Notes |
|--------|-----------|-------|
| `PluralSuffix` | `string PluralSuffix(int n)` | Also `PluralSuffix(long n)`, `PluralSuffixY(int n)` |
| `DotOrColon` | `string DotOrColon(int n)` | |
| `RealString` | `string RealString(double a, string format = "0.##")` | Default `"0.##"`; PipeSystemExporter used `"0.####"` — pass format explicitly |
| `PointString` | `string PointString(XYZ p, bool onlySpaceSeparator = false)` | Also UV overload |
| `AngleString` | `string AngleString(double angle, bool addUnits = true)` | |
| `MmString` | `string MmString(double length, bool addUnits = true)` | |

### Call sites to migrate (delete local copies)

| Sample folder | Local file | Action |
|---------------|------------|--------|
| `AdnRme` | `Util.cs` `#region Formatting` | Remove `PluralSuffix`, `DotOrColon`, `RealString`; call `BuildingCoder.Util.*` |
| `CreateAndPrintSheetsAndViews` | `Util.cs` `#region Formatting` | Remove formatting duplicates; call `BuildingCoder.Util.*` |
| `PipeSystemExporter` | `Util.cs` | Remove formatting methods; use `BuildingCoder.Util.RealString(a, "0.####")` |
| `CustomExporter/AdnMeshJsonExporter` | `Util.cs` | Remove `RealString`, `PointString`; keep geometry helpers |

### Migration pattern

```csharp
// Before (local Util in sample namespace)
Util.PluralSuffix(n)

// After
BuildingCoder.Util.PluralSuffix(n)
```

Or add at file top: `using FormatUtil = BuildingCoder.Util;`

---

## Group 2 — Geometry comparison (P1, partial)

**Canonical home:** [`src/Common/Geometry/Util.GeometryComparison.cs`](src/Common/Geometry/Util.GeometryComparison.cs)  
**Namespace:** `BuildingCoder.Util`

| Method | Signature |
|--------|-----------|
| `IsZero` | `bool IsZero(double a, double tolerance = Eps)` |
| `IsEqual` | `bool IsEqual(double a, double b, double tolerance = Eps)` |
| `Compare` | `int Compare(double a, double b, double tolerance = Eps)` |
| `Eps` | `const double` / `double Eps` property |

### Call sites

| Sample folder | Action |
|---------------|--------|
| `CreateAndPrintSheetsAndViews` | Replace local `#region Geometrical Comparison` with `BuildingCoder.Util` where signatures match |

---

## Group 3 — XYZ linear algebra (P2, deferred)

**Canonical home:** [`src/Common/Geometry/XyzMath.cs`](src/Common/Geometry/XyzMath.cs) (`Ara3D.RevitSampleBrowser.Common.Geometry.XyzMath`)

| Method | Signature |
|--------|-----------|
| `DotMatrix` | `double DotMatrix(XYZ p1, XYZ p2)` |
| `CrossMatrix` | `XYZ CrossMatrix(XYZ p1, XYZ p2)` |

Already consolidated from reinforcement samples.

---

## Group 4 — Vector4 / MathTools (P2, deferred)

**Canonical home:** [`src/Common/Geometry/GraphicsLinearAlgebra.cs`](src/Common/Geometry/GraphicsLinearAlgebra.cs) (`Vector4`, `Matrix4`)

Local copies remain in `SlabShapeEditing`, `NewPathReinforcement`, `PathReinforcement`, `CurtainSystem` — **float vs double** variants differ. Defer until a typed consolidation strategy is chosen.

---

## Group 5 — MEP connector helpers (P1)

**Canonical home:** [`src/Common/Mep/ConnectorHelper.cs`](src/Common/Mep/ConnectorHelper.cs), [`src/Common/Mep/MepSystemSearch.cs`](src/Common/Mep/MepSystemSearch.cs)

| Sample | Local helper | Canonical |
|--------|--------------|-----------|
| `PipeSystemExporter` | `Util.GetConnectors`, `GetConnectorPoints` | Evaluate move to `ConnectorHelper` in follow-up |
| `TBC_FlowMismatch` | Uses `MepSystemSearch.GetConnectors` | Already on Common |

---

## Group 6 — TBC partial Util (done)

~80 `TBC_*/Util.cs` files extend `BuildingCoder.Util` via `partial class`. Shared bodies already in `src/Common`. Per-folder files hold **command-specific** helpers only.

**No action** unless a new duplicate cluster appears in analysis.

---

## Domain folder reference

| Domain | Path | Purpose |
|--------|------|---------|
| Geometry | `src/Common/Geometry/` | XYZ math, solids, comparers, Vector4/Matrix4 |
| Document | `src/Common/Document/` | Filters, selection, query, extensions |
| Mep | `src/Common/Mep/` | Connectors, systems, fabrication |
| Parameters | `src/Common/Parameters/` | Parameter access, GUIDs, units |
| Units | `src/Common/Units/` | Formatting, unit conversion |
| Views | `src/Common/Views/` | View helpers, templates, stairs |
| Structural | `src/Common/Structural/` | Rebar, analytical model |
| Infrastructure | `src/Common/Infrastructure/` | Export, messages, scopes, misc |

---

## Phase 2 batch assignments (folder ownership)

Each subagent edits **only** its assigned folders:

| Batch | Folders |
|-------|---------|
| `batch-formatting-1` | `AdnRme`, `PipeSystemExporter` |
| `batch-formatting-2` | `CreateAndPrintSheetsAndViews`, `CustomExporter/AdnMeshJsonExporter` |
| `batch-tbc-*` | No migration needed (monitor only) |
