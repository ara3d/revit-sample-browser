# Command

| Field | Value |
|-------|-------|
| **Sample** | StringSearch |
| **Class** | `Command` |
| **Source** | `src/StringSearch/Command.cs` |
| **SDK ReadMe** | `src/StringSearch/Readme_StringSearch.rtf` |
| **MCP rating** | 4/5 |
| **Upstream** | [jeremytammik/StringSearch](https://github.com/jeremytammik/StringSearch) (MIT) |

Searches element parameter values in the current view, current selection, or entire project for a user-specified string. Adapted from Jeremy Tammik's ADN Plugin of the Month sample; results appear in a modeless navigator and a temp log file.

## What it demonstrates

- `FilteredElementCollector` scoped to view, selection, or document
- Category filter via `BuiltInCategory`, instance vs type elements, standard vs user vs built-in parameters
- String matching with optional case sensitivity, whole-value match, and .NET regular expressions
- `StringSearcher` iterating parameters and recording `SearchHit` rows
- Modeless `SearchHitNavigator` with idling-based zoom via `UIApplication.Idling` and `UIDocument.ShowElements`

## Prerequisites

- An active project document with elements matching the chosen search scope

## User interaction

- `SearchForm` dialog collects the search string, scope (selection / view / project), category, parameter name, and advanced options
- Re-prompts when no hits are found; cancel returns `Result.Cancelled`
- Double-click a navigator row to select and zoom to the element
- Right-click the search form for Help or Display Log File (`%TEMP%\SearchString.log`)

## MCP notes

- Proposed tool: `revit_search_parameters`
- Parameters: `search_string`, `scope` (`view` | `selection` | `project`), optional `category`, `parameter_name`, `match_case`, `whole_word`, `regex`, `built_in_params`
- Returns: array of hits with element id, parameter name, value, and match index
- Current command requires `SearchForm`; headless MCP use would need programmatic options instead of the dialog
- MCP descriptor: `src/StringSearch/string-search.json`

## See also

- MCP descriptor: `src/StringSearch/string-search.json`
- Related: `ElementFilterSample` for view filter creation, not parameter text search
