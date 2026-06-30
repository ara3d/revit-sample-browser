# Bowerbird Revit MCP Product Roadmap

## Product Direction

The MCP server should not be treated as a thin wrapper around Revit sample commands. The stronger product is an AEC automation workbench: a safe, inspectable layer where agents can query models, apply firm standards, generate reusable tools, and run approved changes through host-specific adapters.

The current repo already points in this direction. `src/BB_McpServer` exposes a localhost MCP server with tools, prompts, resources, descriptor-backed sample tools, Revit `ExternalEvent` dispatch, basic change sets, and audit logging. That is a solid foundation for a roadmap centered on useful workflows rather than a large flat command palette.

The long-term shape is:

```text
AEC Agent Workbench
  - MCP protocol surface
  - Revit/Bowerbird host adapter
  - Ara 3D Studio host adapter
  - Query, validation, geometry, and data services
  - Firm standards and preferences
  - Tool generation and packaging
  - Workflow recipes
  - Change preview, audit, and rollback
```

The product wedge is:

> Teach your design tool new capabilities safely, using firm standards, reusable skills, and inspectable automation recipes.

## Current Implementation Fit

The roadmap matches the codebase well in the following areas:

- `CommandMcpServer` starts a Streamable HTTP MCP endpoint for Cursor at `http://127.0.0.1:8765/mcp`.
- `McpToolRegistry` registers implemented `aec.*`, `dev.*`, `standards.*`, and `revit_*` tools, then loads descriptor-backed sample tools from `mcp-manifest.json`.
- `McpPromptProvider` exposes workflow prompts such as `inspect_current_context`, `explain_selection`, `plan_safe_model_edit`, `audit_model_health`, and `create_custom_tool`.
- `McpResourceProvider` exposes model, standards, sample catalog, export, and audit resources.
- `RevitBridge` routes Revit API calls through `ExternalEvent`, which is the right threading model for MCP calls into Revit.
- Change-set tools exist and are used by visualization workflows before applying view overrides.
- The command catalog and descriptor manifest create a useful bridge from the sample browser to future tool generation.

The current gaps are also clear:

- There is no real host-neutral core yet; the implementation is mostly Revit/Bowerbird-specific.
- The Ara 3D adapter is a product direction, not an implemented adapter.
- Change-set validation is shallow and supports only a small set of operations.
- Descriptor-backed tools are discoverable but return `not_implemented` until a headless adapter is wired.
- Generated tools are persisted for review, but not compiled, loaded, tested, registered dynamically, or executed.
- Firm standards are searchable resources, but rule extraction, compliance checking, and fix generation are not implemented as a full loop.
- Workflow recipes, reusable skills, snapshots, approvals, and rollback logs are still roadmap items.

## Roadmap Themes

### 1. Model Understanding

Agents need dependable read tools before they can safely modify models. The existing foundation includes host context, selection, element query, parameter reading, model statistics, view images, warnings, schedules, rooms, sheets, materials, and export resources.

Next investment should improve query breadth and result quality:

- Add `aec.get_active_view` as a dedicated tool instead of folding this into `aec.get_host_context`.
- Add stable reference resolution for `ElementId`, `UniqueId`, descriptor references, and selected elements.
- Extend `aec.query_elements` with phase, workset, view visibility, parameter filters, type filters, and room/level/system constraints.
- Add graph-style tools for hosted elements, rooms, MEP systems, and nearby/intersecting elements.

### 2. Safe Change Sets

The product differentiator is not that an agent can modify Revit. It is that changes are previewable, explainable, approved, auditable, and reversible where possible.

The current change-set layer is the right start, but it needs to become the write path for all medium/high-risk operations.

Target capabilities:

- Declarative operations with typed schemas.
- Model validation before execution.
- Human-readable previews with affected elements and expected output.
- Risk classification based on deletes, geometry changes, parameters, views, sheets, exports, and generated code.
- Approval tokens or host-side confirmation for medium/high-risk work.
- Transaction result records with warnings, failures, changed elements, and output artifacts.
- Snapshot summaries before risky operations.

### 3. BIM QA and Standards

QA workflows are the highest-value near-term product path because they are mostly read-only, useful in real projects, and easier to make safe than generative modeling.

Already represented:

- model warnings
- unplaced rooms
- missing parameter values
- naming convention checks
- level-less elements
- room-door relationships
- overlapping rooms
- MEP system completeness
- model health audit

Next investment should connect these checks to standards resources:

- Search standards markdown by discipline/category/workflow.
- Cite the standard that motivated each check.
- Produce violation sets with element references, severity, and suggested fixes.
- Generate change sets for simple fixes, but do not apply them automatically.
- Export a BIM manager report as Markdown/HTML/JSON.

### 4. Visualization and Inspection

Visual verification makes the MCP workflow tangible. The existing `aec.capture_view_image` and `aec.color_by_parameter` tools are good anchors.

Next investment:

- Make view overrides consistently change-set based.
- Add issue highlighting for arbitrary element sets.
- Add section-box/focus tools around selected or queried elements.
- Add simple legends for color-by-parameter and issue views.
- Capture before/after images as part of change-set previews and reports.

### 5. Data and Delivery

Export workflows are practical because they convert Revit models into artifacts that agents and downstream tools can inspect.

Already represented:

- JSON element exports
- room exports
- schedule CSV/HTML/Excel export
- IFC, DWG, PDF, gbXML export
- model resources for levels, rooms, warnings, sheets, materials, and exports

Next investment:

- Normalize export result metadata through a common artifact model.
- Add parameter matrix export.
- Add saved query result resources.
- Add comparison between current model summaries and previous snapshots.
- Prepare data for DuckDB/BOS/lakehouse workflows after the core export path is reliable.

### 6. Custom Tool Generation

This is a major differentiator, but it should mature after the safe read/query/change foundation is credible.

Current state:

- `dev.search_examples` searches the sample catalog.
- `dev.generate_tool_spec` creates a basic tool schema proposal.
- `dev.generate_bowerbird_script` creates a basic `NamedCommand` scaffold.
- `dev.compile_script` and `dev.review_tool_for_safety` perform lightweight textual checks.
- `dev.create_mcp_tool_from_script` persists generated source for review.

Target path:

- Generate useful headless tool handlers, not only interactive `NamedCommand` scaffolds.
- Compile generated code with real Roslyn diagnostics.
- Run generated tools against sample fixtures or extracted model data.
- Package generated tools with descriptors, docs, tests, examples, and risk metadata.
- Register tools dynamically only after review and approval.

### 7. Ara 3D Adapter

Ara 3D should become the high-performance reasoning and visualization sandbox, while Revit remains the authoritative production authoring backend.

Likely Ara roles:

- fast model loading
- geometry queries
- spatial indexing
- mesh and instance analysis
- visual diff
- portfolio analytics
- BOS/lakehouse data workflows
- safe preflight validation

This should wait until the Revit MCP workflows have a clean core abstraction for host context, query specs, element references, change sets, artifacts, and audit records.

## Highest-Value Implementation Plan

### Phase 1: Harden the Existing Demo Loop

Goal: make the five workflows in `docs/MCP-DEMOS.md` reliable enough to use repeatedly in front of users.

Scope:

- Inspect model: `inspect_current_context`.
- Explain selection: `explain_selection`.
- Highlight problems: query or QA check -> change-set preview -> apply view override -> capture image.
- Export for analysis: JSON, rooms, schedules, and model resources.
- Teach a new checker: search samples -> generate spec -> generate scaffold -> review -> persist.

Implementation steps:

1. Add missing `aec.get_active_view` as a dedicated read tool.
2. Add `aec.resolve_element_refs` for `ElementId`, `UniqueId`, and current selection references.
3. Improve `aec.query_elements` with parameter filters, type filters, and view visibility.
4. Make `aec.color_by_parameter` require the same approval path as `aec.apply_changes`, or route it entirely through `aec.apply_changes`.
5. Add a compact demo readiness checklist to `docs/MCP-DEMOS.md`.

Success criteria:

- Each demo prompt can be run without guessing tool names or manually stitching undocumented arguments.
- Read-only demos do not mutate the model.
- Visualization demos produce a preview, apply only after approval, and capture an image.
- Tool-generation demos honestly report that generated code is persisted for review, not live-loaded.

### Phase 2: Build Real Change-Set Validation

Goal: turn change sets from a useful concept into the required safety layer for writes.

Implementation steps:

1. Define typed change operation schemas for parameter edits, view overrides, element creation, and export artifacts.
2. Validate element references, writable parameters, active view compatibility, required family/type/level inputs, and transaction preconditions.
3. Expand `aec.preview_changes` to include affected elements, created elements, parameter names, risk level, warnings, and suggested approval text.
4. Record applied transactions with changed element ids, operation counts, warnings, result status, and output artifact paths.
5. Add `audit://changesets/{changeSetId}` and `audit://transactions/{transactionId}` resources.

Success criteria:

- Medium/high-risk writes cannot bypass preview and approval.
- Failed validations explain what input or model state blocked the change.
- Audit resources can reconstruct what an agent attempted and what actually changed.

### Phase 3: Ship BIM QA Reports

Goal: deliver a useful read-mostly workflow that BIM managers would recognize immediately.

Implementation steps:

1. Create a shared issue/violation result shape with severity, category, element refs, message, suggested fix, and source standard URI.
2. Normalize existing QA handlers to return that shape.
3. Add `standards.check_model` for applying selected standards/search results to current model checks.
4. Add `standards.generate_standards_report` or `qa.generate_model_health_report`.
5. Add report export as Markdown and JSON first; HTML can follow.

Success criteria:

- A user can ask for a model health report and get warnings, rooms, parameters, naming, and MEP completeness in one coherent artifact.
- Standards citations are included when the issue came from markdown standards.
- Suggested fixes are change sets, not immediate writes.

### Phase 4: Wire Descriptor Tools to Headless Adapters

Goal: convert the sample catalog from a searchable reference into real callable tools where the underlying sample can run without UI.

Implementation steps:

1. Pick 5-10 high-value descriptor tools with `requiresUi: false` and MCP rating 5.
2. For each, extract command logic into a headless service method with explicit arguments.
3. Wrap each service method in an `IToolHandler`.
4. Keep descriptor metadata as documentation, but replace `DescriptorToolAdapter` with real handlers for wired tools.
5. Add tests or at least build-time checks around argument schema and handler registration.

Success criteria:

- Calling a selected descriptor-backed tool performs useful work instead of returning `not_implemented`.
- The implementation pattern is repeatable for future sample migration.
- Interactive samples remain documented but are not exposed as misleading headless tools.

### Phase 5: Make Tool Generation Real

Goal: move from scaffolding to reviewed, testable, reusable tool packages.

Implementation steps:

1. Generate `IToolHandler` scaffolds in addition to `NamedCommand` scaffolds.
2. Replace textual compile checks with Roslyn compilation diagnostics.
3. Add generated descriptor, README, examples, and safety metadata.
4. Add a review step that checks transactions, Revit threading, destructive operations, file writes, and UI dependencies.
5. Register generated tools only after explicit approval and a clean compile.

Success criteria:

- Generated tools are package-shaped, reviewable, and reproducible.
- Unsafe generated code is blocked with actionable findings.
- Dynamic registration is tied to approval and audit history.

## Recommended Next Sprint

The next sprint should focus on Phase 1 plus the first slice of Phase 2:

1. Add `aec.get_active_view`.
2. Add `aec.resolve_element_refs`.
3. Extend `aec.query_elements` with parameter and type filters.
4. Tighten `aec.color_by_parameter` so preview/apply behavior is consistent with change sets.
5. Expand `aec.preview_changes` with affected element details.
6. Add audit resources for change sets.
7. Update `docs/MCP-DEMOS.md` so the demos match the actual behavior.

That gives the project a credible loop:

```text
inspect -> query -> preview -> approve -> apply -> capture -> audit
```

Once that loop is solid, QA reports and generated checker workflows will have a safer foundation to build on.
