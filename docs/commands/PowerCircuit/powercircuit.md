# Command

| Field | Value |
|-------|-------|
| **Sample** | PowerCircuit |
| **Class** | `Command` |
| **Source** | `src/PowerCircuit/CS/Command.cs` |
| **SDK ReadMe** | `src/PowerCircuit/CS/ReadMe_PowerCircuit.rtf` |
| **MCP rating** | 2/5 |

Manages electrical circuits for the current selection through a sequence of WinForms dialogs and `CircuitOperationData.Operate()`.

## What it demonstrates

- Building `CircuitOperationData` from selected power elements
- `CircuitOperationForm`, `SelectCircuitForm`, and `EditCircuitForm` for create, select, and edit flows
- `ElectricalSystem` circuit operations executed after dialog confirmation

## Prerequisites

- At least one selected electrical element in the active document

## User interaction

- Multiple modal dialogs; operation type and target circuit chosen interactively
- Not headless without refactoring `CircuitOperationData`

## MCP notes

Electrical circuit editing is domain-specific and dialog-driven; limited value as a generic MCP tool without explicit operation and circuit parameters.

## See also

- Data layer: `src/PowerCircuit/CS/CircuitOperationData.cs`
