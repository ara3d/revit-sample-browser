# Agent runbook: command docs & MCP descriptors

Instructions for AI agents continuing the work tracked in [`todo.md`](../todo.md).

## Quick start

1. Open `todo.md` and find rows where **Doc** or **MCP** is `[ ]`.
2. Pick a **work session** scope (or one sample folder) and finish all rows in that group before moving on.
3. For each command, complete **Task A** (markdown doc) and, when MCP ≥ 3, **Task B** (MCP JSON).
4. Mark `[x]` in `todo.md` only after both required tasks are done.
5. Update the **Progress Summary** counts at the top of `todo.md`.

## Per-command tasks

Every command has at least one task. Commands rated **MCP 3 or above** have two.

| Task | Output | Required when |
|------|--------|---------------|
| **A — Command doc** | `src/.../<slug>.md` | Always |
| **B — MCP descriptor** | `src/.../<slug>.json` | MCP rating ≥ 3 |

### Task A — Write the command markdown

**Sources (read in this order):**

1. **SDK ReadMe RTF** — each sample folder has a `ReadMe_*.rtf` or `Readme_*.rtf` beside its source code. The sample browser loads the first `*.rtf` found in the command's folder (see `SampleData.ReadmePath` in `src/SampleData.cs`). Strip RTF markup or read plain-text content; do not paste the raw RTF into the doc.
2. **Command source** — the `.cs` file listed in `todo.md`.
3. **Related files** — forms, helpers, or `Application.cs` in the same sample only when needed to explain behavior.

**RTF lookup:** for source `src/<Sample>/Command.cs`, check `src/<Sample>/*.rtf`. For nested samples (e.g. `src/Events/PrintLog/Command.cs`), check `src/Events/PrintLog/*.rtf`.

**Writing rules:**

- **Be clear and concise.** Target **150–300 words** in the body (excluding the metadata table). Prefer short paragraphs and bullet lists over long prose.
- **Lead with purpose.** First sentence: what the command does in plain language.
- **Skip boilerplate.** Do not restate obvious Revit concepts or duplicate the entire SDK ReadMe.
- **Be specific.** Name key API types, transactions, filters, and elements touched.
- **Note UI honestly.** If the command opens a dialog or requires picks, say so and whether headless/MCP use would need refactoring.
- **Omit empty sections.** Delete any section that would only say "none" or "N/A".
- Use the template at [`src/_template.md`](../src/_template.md). Replace placeholders; remove HTML comments before saving.

### Task B — Write the MCP tool descriptor JSON

Required when the command's MCP rating is **3, 4, or 5**.

**Output path:** mirror the command doc path under `src/` (same folders and slug, `.json` extension).

Example: doc `src/AllViews/allviews.md` → MCP `src/AllViews/allviews.json`.

**Format:** follow [`src/_template.json`](../src/_template.json) and match existing Bowerbird tools (`revit_document_info`, `echo`):

- `name` — snake_case, unique, prefixed with `revit_` when exposing Revit document operations (e.g. `revit_list_views`).
- `description` — one sentence an LLM can use to choose this tool.
- `arguments` — JSON Schema object; list only parameters an agent can supply. Omit interactive-only inputs unless you document them as optional/future.

Add optional metadata fields from the template (`commandClass`, `sample`, `mcpRating`, `requiresUi`) for traceability; they are not sent to MCP clients.

**MCP JSON is a specification, not an implementation.** Describe the ideal tool surface. Note in the command doc's **MCP notes** section if the current command would need refactoring (remove dialogs, accept element IDs, etc.).

### Commands with MCP rating 1–2

- Complete **Task A** only.
- In `todo.md`, set the **MCP** column to `—` (not applicable).

## File naming

| Item | Rule |
|------|------|
| Command doc | `src/<SampleFolder>/<slug>.md` |
| MCP descriptor | `src/<SampleFolder>/<slug>.json` |
| Slug | kebab-case class name; for class `Command`, use the leaf subfolder (e.g. `Events/PrintLog` → `printlog`) |

Nested samples use the full subfolder path (e.g. `src/Events/PrintLog/printlog.md`).

## Updating `todo.md`

After finishing a command:

1. Set **Doc** to `[x]` when the markdown file exists and meets the writing rules.
2. Set **MCP** to `[x]` when the JSON exists (rating ≥ 3), or leave as `—` (rating ≤ 2).
3. Recompute **Progress Summary** totals (commands documented, MCP descriptors created, remaining).
4. If you revise an MCP rating while writing the doc, update the **MCP** column number and add/remove Task B as needed.

## Suggested session order

Work highest-value commands first so MCP tooling covers real automation scenarios early:

1. MCP **5** commands (~33) — query, export, read-only
2. MCP **4** commands (~85) — parameterized create/modify
3. MCP **3** commands (~7) — niche but worth specifying
4. MCP **2** and **1** — docs only, no MCP JSON

## Quality checklist

Before marking a row complete:

- [ ] Read the sample's RTF ReadMe (if present)
- [ ] Read the command `.cs` source
- [ ] Doc is 150–300 words, no filler sections
- [ ] Doc metadata table has correct class, source path, and final MCP rating
- [ ] MCP JSON created when rating ≥ 3, valid JSON, snake_case `name`
- [ ] `todo.md` row updated and progress counts refreshed

## Reference

- MCP rating scale — `todo.md` § MCP usefulness scale
- Command template — `src/_template.md`
- MCP template — `src/_template.json`
- Existing MCP examples — `.cursor/projects/.../mcps/user-bowerbird-revit/tools/` (local Cursor config; repo copies live under `src/`)
