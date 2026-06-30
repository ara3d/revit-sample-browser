# Command

| Field | Value |
|-------|-------|
| **Sample** | ErrorHandling |
| **Class** | `Command` |
| **Source** | `src/ErrorHandling/Command.cs` |
| **MCP rating** | 1/5 |

Demonstrates four strategies for handling Revit transaction failures: custom failure definitions, preprocessors, events, and a registered failures processor.

## What it demonstrates

- Creating custom `FailureDefinition` objects with resolution types in `OnStartup`
- `IFailuresPreprocessor` deleting warnings or built-in overlap warnings before commit
- `Application.FailuresProcessing` event handler resolving posted errors by transaction name
- `IFailuresProcessor` registered via `Application.RegisterFailuresProcessor`
- Posting failures with `Document.PostFailure` and resolving via `DeleteElements`

## Prerequisites

- Document containing a level named **Level 1** (used to create demo walls)

## User interaction

- Runs four sequential test transactions automatically; no user input
- Modifies the document by creating overlapping walls and posting custom failures

## MCP notes

- Educational sample for failure-handling API patterns; not intended as an MCP automation tool
