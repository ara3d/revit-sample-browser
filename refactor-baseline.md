# Refactor Baseline

Captured: 2026-06-30

## Build (Debug)

```
dotnet build src/Ara3D.RevitSampleBrowser.csproj -c Debug -v minimal
```

| Metric | Count |
|--------|------:|
| Errors | 0 |
| Warnings | 6177 |
| Result | Build succeeded |

Full log: `refactor-baseline-build.txt`

## Post-refactor (2026-06-30)

| Metric | Before | After |
|--------|-------:|------:|
| Exact duplicate Util body groups | 3 | 0 |
| Excess duplicate copies | 5 | 0 |
| Debug errors | 0 | 0 |
| Release errors | 1 (pre-existing) | 0 (fixed) |
| csproj explicit Compile entries | ~1041 | 0 (+ globs) |

## Git tag

`refactor-baseline` — marks the starting point before dedup/cleanup work.

## Scope notes

- Single project: `src/Ara3D.RevitSampleBrowser.csproj` (`net8.0-windows`)
- Shared library: `src/Common/` (~148 files, domain-organized)
- Per-sample helpers: ~92 `Util.cs` files (mostly `TBC_*` partial `BuildingCoder.Util`)
- Prior migration: `TBC_Shared` → `src/Common` (see `scripts/migrate_tbc_shared_to_common.py`)
