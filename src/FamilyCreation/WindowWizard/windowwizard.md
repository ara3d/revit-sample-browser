# Command

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/WindowWizard |
| **Class** | `Command` |
| **Source** | `src/FamilyCreation/WindowWizard/Command.cs` |
| **MCP rating** | 2/5 |

Launches a multi-step wizard that builds a parametric window family from user inputs in a window family template.

## What it demonstrates

- Category check against `OST_Windows` before running `WindowWizard`
- Delegating creation to helper classes for reference planes, extrusions, dimensions, and alignments
- Wizard UI flow with success, cancel, and failure return codes

## Prerequisites

- Window family template open in the family editor

## User interaction

- Full WinForms wizard; requires interactive parameter entry throughout

## MCP notes

- Heavy UI tutorial; MCP use would need to bypass the wizard and accept structured window dimensions
