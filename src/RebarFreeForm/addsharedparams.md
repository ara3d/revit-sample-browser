# AddSharedParams

| Field | Value |
|-------|-------|
| **Sample** | RebarFreeForm |
| **Class** | `AddSharedParams` |
| **Source** | `src/RebarFreeForm/AddSharedParams.cs` |
| **MCP rating** | 4/5 |

Creates and binds two shared instance parameters on the Rebar category used by the free-form rebar update server sample.

## What it demonstrates

- Creating `Updated` (Yes/No) and `CurveElementId` (string) shared parameters with `HideWhenNoValue`
- `ExternalDefinitionCreationOptions` with `UserModifiable` flags
- `InstanceBinding` to `OST_Rebar` and duplicate-check via `ShareParameterExists`
- Temporary shared parameter file beside the add-in assembly

## Prerequisites

- Run once before the main RebarFreeForm command so regeneration dependencies can be registered

## User interaction

- No dialog; idempotent if parameters already exist

## MCP notes

- Proposed tool: `revit_bind_rebar_shared_parameters`
- Parameters: optional override paths or parameter names
- Returns: confirmation that **Updated** and **CurveElementId** are bound
- MCP descriptor: `src/RebarFreeForm/addsharedparams.json`

## See also

- MCP descriptor: `src/RebarFreeForm/addsharedparams.json`
- Main command: [rebarfreeform.md](rebarfreeform.md)
