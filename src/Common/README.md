# src/Common — Shared Library Inventory

Reusable helpers consolidated from Revit SDK samples, Building Coder (TBC), Bowerbird (BB), and Nice3point (N3P) code. Prefer these over local `Util.cs` copies.

## Domains

| Folder | Namespace(s) | Key types |
|--------|----------------|-----------|
| [Geometry/](Geometry/) | `Ara3D.RevitSampleBrowser.Common.Geometry`, `BuildingCoder` | `XyzMath`, `Vector4`, `Matrix4`, `Util.GeometryComparison`, `Util.Solids`, `Creator` |
| [Document/](Document/) | `Ara3D.RevitSampleBrowser.Common.Documents`, `BuildingCoder` | `FilterBuilder`, `SelectionHelper`, `ElementQuery`, `Util.Filtering`, `Util.Selection` |
| [Mep/](Mep/) | `.Common.Mep` | `ConnectorHelper`, `MepSystemSearch`, `FabricationPartHelper` |
| [Parameters/](Parameters/) | `.Common.Parameters` | `ParameterAccess`, `ParameterExtensions`, `JtRevision` |
| [Units/](Units/) | `.Common.Units`, `BuildingCoder` | `ValueFormatting`, `UnitConversion`, **`Util.Formatting`** (PluralSuffix, PointString, RealString) |
| [Views/](Views/) | `.Common.Views` | `ViewHelper`, `ViewTemplateHelper`, `StairsHelper` |
| [Structural/](Structural/) | `.Common.Structural` | `RebarGeometry`, `AnalyticalModelHelper` |
| [Infrastructure/](Infrastructure/) | `.Common.Infrastructure`, `BuildingCoder` | `SampleBrowserUtils`, `Util.Messages`, `Util.Export`, `JtTimer` |

## Formatting API (most common dedup target)

```csharp
BuildingCoder.Util.PluralSuffix(n);
BuildingCoder.Util.DotOrColon(n);
BuildingCoder.Util.RealString(value);                    // default "0.##"
BuildingCoder.Util.RealString(value, "0.####");          // higher precision
BuildingCoder.Util.PointString(xyz);
BuildingCoder.Util.IsEqual(a, b);                         // geometry comparison
```

## Coordination

- Canonical homes and migration rules: [refactor-map.md](../../refactor-map.md) (repo root)
- Agent rules: [.cursor/rules/refactor.mdc](../../.cursor/rules/refactor.mdc)
- Coding style: [docs/STYLES.md](../../docs/STYLES.md)

## Analysis scripts

```bash
python scripts/find_all_util_dupes.py   # all Util.cs files
python scripts/find_tbc_util_dupes.py   # TBC partial Util only
```
