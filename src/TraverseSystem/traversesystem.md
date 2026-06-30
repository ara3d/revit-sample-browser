# Command

| Field | Value |
|-------|-------|
| **Sample** | TraverseSystem |
| **Class** | `Command` |
| **Source** | `src/TraverseSystem/Command.cs` |
| **MCP rating** | 2/5 |

Traverses a well-connected mechanical or piping system from a single selected element and writes the tree to `traversal.xml`.

## What it demonstrates

- Resolving `MechanicalSystem` or `PipingSystem` from a selected `MEPSystem`, `FamilyInstance`, or `MEPCurve`
- `IsWellConnected` filtering on connector `MEPSystem` references
- `TraversalTree` flow-direction traversal and XML export via `DumpIntoXml`

## Prerequisites

- Exactly one pre-selected element belonging to a well-connected duct or pipe system

## User interaction

- Requires a single element selection before running; no dialog
- Output is a file beside the add-in assembly, not shown in Revit UI

## MCP notes

- Useful pattern for system topology export, but the XML file output and well-connected constraint limit direct MCP value without returning structured JSON instead.

## See also

- [NetworkPressureLossReport](../NetworkPressureLossReport/networkpressurelossreport.md)
