# AdnRme MEP Sample

| Field | Value |
|-------|-------|
| **Sample** | AdnRme |
| **Source** | `src/AdnRme/` |
| **Upstream** | [jeremytammik/AdnRme](https://github.com/jeremytammik/AdnRme) (MIT) |
| **MCP rating** | 2/5 |

ADN Revit MEP sample demonstrating HVAC space/terminal workflows and electrical system hierarchy inspection using the generic and MEP-specific Revit APIs.

## Commands

### HVAC

| Class | Description |
|-------|-------------|
| `CmdAssignFlowToTerminals` | Group supply-air terminals by space; assign flow from calculated supply airflow |
| `CmdChangeSize` | Change diffuser type/size based on terminal flow |
| `CmdPopulateCfmPerSf` | Populate CFM-per-SF on all spaces |
| `CmdUnhostedElements` | List unhosted MEP family instances |
| `CmdResetDemo` | Reset demo model to original state |

### Electrical

| Class | Description |
|-------|-------------|
| `CmdElectricalConnectors` | Traverse electrical systems via connectors; show hierarchy in a tree view |
| `CmdElectricalSystemBrowser` | Reproduce system browser info using parameter data only |
| `CmdElectricalHierarchy2` | Display full electrical connection hierarchy |
| `CmdInspectElectricalForm` | WinForms UI for electrical system inspection |
| `CmdInspectElectricalForm2` | Alternate WinForms electrical inspection UI |
| `CmdElectricalHierarchy` | Legacy hierarchy command (parameter-based) |
| `CmdAbout` | About dialog for the sample |

## What it demonstrates

- HVAC: space-to-terminal mapping, calculated supply airflow, terminal flow assignment, CFM-per-SF
- Electrical: `ElectricalSystem` traversal, panel/circuit parameters, connector-based hierarchy
- WinForms progress and tree-view UI patterns for long-running MEP operations
- Shared helpers in `Util.cs`, `Bip.cs`, `ParameterName.cs`

## Prerequisites

- **HVAC commands:** MEP project with spaces and supply-air duct terminals (hosted in spaces)
- **Electrical commands:** Project with electrical equipment, panels, and power circuits

## User interaction

- Most HVAC commands run on the whole model with a progress dialog
- `CmdChangeSize` opens a family-type selector dialog
- Electrical commands open tree-view WinForms or write debug output

## MCP notes

- Individual commands could be exposed as tools (e.g. populate CFM-per-SF, list unhosted elements) but most are model-wide batch operations with WinForms UI
- Headless use would require replacing progress dialogs and parameter-name lookups with shared-parameter GUIDs
- MCP descriptors: `cmdpopulatecfmpersf.json`, `cmdunhostedelements.json`

## See also

- [TraverseSystem](../TraverseSystem/traversesystem.md) — mechanical/piping system tree export
- [PowerCircuit](../PowerCircuit/powercircuit.md) — power circuit UI operations
- [CustomExporter/AdnMeshJsonExporter](../CustomExporter/AdnMeshJsonExporter/adnmeshjsonexporter.md) — another Jeremy Tammik ADN sample in this repo
