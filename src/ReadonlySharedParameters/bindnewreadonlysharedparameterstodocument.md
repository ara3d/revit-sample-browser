# BindNewReadonlySharedParametersToDocument

| Field | Value |
|-------|-------|
| **Sample** | ReadonlySharedParameters |
| **Class** | `BindNewReadonlySharedParametersToDocument` |
| **Source** | `src/ReadonlySharedParameters/ReadonlySharedParametersCommands.cs` |
| **MCP rating** | 2/5 |

Creates a temporary shared parameter file and binds read-only **ReadonlyId** and **ReadonlyCost** parameters to selected categories.

## What it demonstrates

- `SharedParameterBindingManager` for definition metadata, categories, and `UserModifiable = false`
- Creating definitions in a new shared parameter file under `c:\tmp\Meridian\`
- Instance binding for **ReadonlyId** on walls, floors, ceilings, roofs; type binding for **ReadonlyCost** on furniture and planting

## Prerequisites

- Writable shared parameter file path; sample uses a random file name under `c:\tmp\Meridian\`

## User interaction

- No dialog; runs immediately and binds parameters in one transaction

## MCP notes

Useful as a setup demo for read-only shared params, but hard-coded paths and demo-specific parameter names limit general MCP reuse.

## See also

- Setter commands: `SetReadonlyCost1`, `SetReadonlyId1`, and variants in the same source file
