# RotateFramingObjects

| Field | Value |
|-------|-------|
| **Sample** | RotateFramingObjects |
| **Class** | `RotateFramingObjects` |
| **Source** | `src/RotateFramingObjects/RotateFramingObjects.cs` |
| **MCP rating** | 4/5 |

Rotates selected structural beams, braces, or columns by updating cross-section rotation or column location rotation.

## What it demonstrates

- **Cross-Section Rotation** parameter updates for beams and braces (radians)
- `LocationPoint.Rotate` about the column insertion point Z axis
- `RotateFramingObjectsForm` for absolute versus relative angle entry
- `RotateElement()` transaction applying changes after dialog confirmation

## Prerequisites

- Pre-selected structural framing `FamilyInstance` elements (beam, brace, or column)

## User interaction

- Dialog for rotation angle and absolute/relative mode when one or many elements selected
- `RotateElement` callable with properties set for headless use

## MCP notes

- Proposed tool: `revit_rotate_framing`
- Parameters: `element_ids[]`, `angle_degrees`, `absolute` boolean
- Returns: updated element ids
- MCP descriptor: `src/RotateFramingObjects/rotateframingobjects.json`

## See also

- MCP descriptor: `src/RotateFramingObjects/rotateframingobjects.json`
