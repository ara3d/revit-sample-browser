# Command

| Field | Value |
|-------|-------|
| **Sample** | SinePlotter |
| **Class** | `Command` |
| **Source** | `src/SinePlotter/Command.cs` |
| **MCP rating** | 1/5 |

Arrays family instances along a sine curve using ribbon-entered period, amplitude, cycle count, partition count, and prism type.

## What it demonstrates

- `IExternalApplication` ribbon UI (`Application`) feeding static parameters to `Command`
- `FamilyInstancePlotter.PlaceInstancesOnCurve` placing symbols at computed sine positions
- `FilteredElementCollector` lookup of prism `FamilySymbol` by name

## Prerequisites

- Project with loaded prism family symbols (cylinder, rectangle, regularpolygon, isotriangle)
- Ribbon panel configured by the SinePlotter add-in application

## User interaction

- All inputs come from custom ribbon text boxes and combo box; command has no standalone dialog

## MCP notes

Demo-oriented ribbon toy; low value as an MCP tool compared to generic family placement along curves.
