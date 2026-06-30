#!/usr/bin/env python3
"""Import The Building Coder samples into revit-sample-browser."""

from __future__ import annotations

import json
import re
import shutil
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
SRC_TBC = Path(r"C:\Users\cdigg\git\the_building_coder_samples\BuildingCoder")
TARGET_SRC = REPO / "src"
SHARED_DIR = TARGET_SRC / "TBC_Shared"
MANIFEST_PATH = REPO / "scripts" / "tbc-import-manifest.json"

SHARED_FILES = [
    "Util.cs",
    "Creator.cs",
    "ParameterUnitConverter.cs",
    "MepSystemSearch.cs",
    "IntPoint2d.cs",
    "IntPoint3d.cs",
    "JtTimer.cs",
    "JtWindowHandle.cs",
    "JtRevision.cs",
    "JtSelectorMulti.cs",
    "JtNamedGuidStorage.cs",
    "JtParamValuesForCats.cs",
    "JtPairPicker.cs",
    "JtElementsOfClassSelectionFilter.cs",
]

# Source files where additional non-stem commands are imported (no shared helper duplication).
EXTRA_IMPORT_COMMANDS: dict[str, list[str]] = {
    "CmdFaceWall.cs": ["CreateWallsAutomaticallyCommand"],
}

# Files where stem-only import is insufficient — force explicit allow list.
PRIMARY_ONLY_FILES: dict[str, list[str]] = {
    "CmdCollectorPerformance.cs": ["CmdCollectorPerformance"],
    "CmdDeleteUnusedRefPlanes.cs": ["CmdDeleteUnusedRefPlanes"],
}

FORM_SAMPLES = {
    "CmdWindowHandle": ["CmdWindowHandleForm.cs", "CmdWindowHandleForm.Designer.cs", "CmdWindowHandleForm.resx"],
    "CmdLinkedFileElements": ["CmdLinkedFileElementsForm.cs", "CmdLinkedFileElementsForm.Designer.cs", "CmdLinkedFileElementsForm.resx"],
}

CMD_PATTERN = re.compile(
    r"(?P<attrs>(?:\s*\[[^\]]+\]\s*)*)"
    r"(?P<modifiers>(?:public|internal|private|protected)\s+)*"
    r"class\s+(?P<name>\w+)\s*(?::\s*IExternalCommand\b[^{]*)?\{",
    re.MULTILINE,
)

IEC_PATTERN = re.compile(
    r"class\s+(?P<name>\w+)\b\s*(?::\s*|\r?\n\s*:\s*)IExternalCommand\b",
    re.MULTILINE,
)


def to_kebab(name: str) -> str:
    s = re.sub(r"(.)([A-Z][a-z]+)", r"\1-\2", name)
    s = re.sub(r"([a-z0-9])([A-Z])", r"\1-\2", s)
    return s.replace("_", "-").lower()


def folder_name(class_name: str, source_file: str, index: int) -> str:
    if class_name.startswith("Cmd"):
        base = class_name[3:]
    else:
        base = class_name
    # Disambiguate duplicate simple names (e.g. two RevitCommand classes)
    if class_name == "RevitCommand" and index > 0:
        base = f"CollectorPerformance_{class_name}"
    if class_name == "Command":
        stem = Path(source_file).stem
        base = f"{stem}_Command"
    return f"TBC_{base}"


def parse_bc_samples(path: Path) -> dict[str, dict]:
    text = path.read_text(encoding="utf-8", errors="replace")
    lines = text.splitlines()
    entries: dict[str, dict] = {}
    i = 0
    while i < len(lines):
        line = lines[i].strip()
        if line.startswith("ADN Bc") and i + 6 < len(lines):
            title = lines[i + 1].strip()
            desc = lines[i + 2].strip()
            class_line = lines[i + 6].strip().lstrip("#")
            if class_line.startswith("BuildingCoder."):
                class_name = class_line.split(".", 1)[1].split()[0]
                entries[class_name] = {"title": title, "description": desc}
            i += 7
            continue
        i += 1
    return entries


def find_brace_end(content: str, open_index: int) -> int:
    depth = 0
    i = open_index
    in_str = False
    escape = False
    while i < len(content):
        ch = content[i]
        if in_str:
            if escape:
                escape = False
            elif ch == "\\":
                escape = True
            elif ch == '"':
                in_str = False
        else:
            if ch == '"':
                in_str = True
            elif ch == "{":
                depth += 1
            elif ch == "}":
                depth -= 1
                if depth == 0:
                    return i
        i += 1
    return len(content) - 1


def find_command_classes(content: str) -> list[tuple[str, int, int]]:
    results = []
    for m in IEC_PATTERN.finditer(content):
        name = m.group("name")
        brace = content.find("{", m.end() - 1)
        if brace == -1:
            continue
        end = find_brace_end(content, brace)
        # include attributes above class
        start = m.start()
        line_start = content.rfind("\n", 0, start) + 1
        # walk back through attribute lines
        pos = line_start
        while pos > 0:
            prev = content.rfind("\n", 0, pos - 1)
            segment = content[prev + 1 : pos].strip()
            if segment.startswith("[") or segment == "":
                pos = prev
                line_start = prev + 1
            else:
                break
        results.append((name, line_start, end + 1))
    return results


def is_nested_in_other_command(
    commands: list[tuple[str, int, int]], index: int
) -> bool:
    name, start, end = commands[index]
    for j, (_, ostart, oend) in enumerate(commands):
        if j == index:
            continue
        if ostart < start and end < oend:
            return True
    return False


def filter_importable_commands(
    commands: list[tuple[str, int, int]],
) -> list[tuple[str, int, int]]:
    """Skip IExternalCommand classes nested inside another IExternalCommand."""
    return [
        c
        for i, c in enumerate(commands)
        if not is_nested_in_other_command(commands, i)
    ]


def remove_other_commands(content: str, keep_class: str) -> str:
    commands = find_command_classes(content)
    if len(commands) <= 1:
        return content
    keep = next((c for c in commands if c[0] == keep_class), None)
    if not keep:
        return content
    ks, ke = keep[1], keep[2]
    removals: list[tuple[int, int]] = []
    for name, start, end in commands:
        if name == keep_class:
            continue
        if ks < start and end <= ke:
            continue
        if start < ks and ke <= end:
            continue
        removals.append((start, end))
    if not removals:
        return content
    out: list[str] = []
    pos = 0
    for start, end in sorted(removals, key=lambda x: x[0]):
        out.append(content[pos:start])
        pos = end
    out.append(content[pos:])
    return "".join(out)


def apply_integration(content: str) -> str:
    """Keep BuildingCoder namespace; ensure shared util namespace is available."""
    if "using BuildingCoder;" in content and content.strip().startswith("using BuildingCoder;"):
        return content
    return content


def transform_namespace(content: str, folder: str) -> str:
    """Legacy hook — namespace stays BuildingCoder to preserve cross-sample references."""
    return apply_integration(content)


def estimate_mcp_rating(class_name: str, title: str, desc: str, content: str) -> int:
    low = {
        "Idling", "PressKeys", "WindowHandle", "StatusBar", "DemoCheck", "ItemExecuted",
        "SelectionChanged", "SwitchDoc", "CloseDocument", "PreprocessFailure", "FailureGatherer",
        "CollectorPerformance", "FilterPerformance", "ParamFilterTest", "BrokenCommand",
        "SimpleUpdaterExample", "ElevationWatcherUpdater", "MiroTest2", "TestWall",
        "GetColumnGeometry1", "DeleteUnusedRefPlanes_2014", "Linq", "OmniClassParams",
    }
    name = class_name[3:] if class_name.startswith("Cmd") else class_name
    if name in low or class_name in low:
        return 2
    text = f"{title} {desc} {content}".lower()
    if any(k in text for k in ("benchmark", "performance", "demo", "test", "press key", "idling", "updater")):
        if "list" not in text and "export" not in text:
            return 2
    high = ("list", "export", "retrieve", "determine", "calculate", "query", "report")
    if any(k in text for k in high):
        return 4
    if any(k in text for k in ("create", "new ", "place", "set ", "copy", "mirror", "resize")):
        return 4
    if any(k in text for k in ("dimension", "geometry", "boundary", "adjacency", "material")):
        return 3
    return 3


def write_markdown(entry: dict, folder: Path, slug: str, rating: int) -> None:
    bc = entry.get("bcSamples") or {}
    title = bc.get("title") or entry["className"]
    desc = bc.get("description") or f"The Building Coder sample {entry['className']}."
    source_rel = f"src/{folder.name}/{entry['sourceFile']}"
    lines = [
        f"# {entry['className']}",
        "",
        "| Field | Value |",
        "|-------|-------|",
        f"| **Sample** | {folder.name} |",
        f"| **Class** | `{entry['className']}` |",
        f"| **Source** | `{source_rel}` |",
        f"| **BcSamples** | {title} — {desc} |" if bc else f"| **BcSamples** | — |",
        f"| **MCP rating** | {rating}/5 |",
        "",
        desc + ".",
        "",
        "## What it demonstrates",
        "",
        f"- Building Coder sample `{entry['className']}` from Jeremy Tammik's blog collection",
        f"- Source: `{entry['sourceFile']}`",
        "",
    ]
    ui_keywords = ("pick", "form", "dialog", "messagebox", "selection", "uidoc.Selection")
    content = (folder / entry["sourceFile"]).read_text(encoding="utf-8", errors="replace") if (folder / entry["sourceFile"]).exists() else ""
    if any(k in content for k in ui_keywords):
        lines.extend([
            "## User interaction",
            "",
            "- May require interactive selection or dialogs; headless MCP use would need refactoring.",
            "",
        ])
    if rating >= 3:
        lines.extend([
            "## MCP notes",
            "",
            f"- Proposed tool: `revit_{slug.replace('-', '_')}`",
            f"- MCP descriptor: `src/{folder.name}/{slug}.json`" if rating >= 3 else "",
            "",
        ])
    lines.extend([
        "## See also",
        "",
    ])
    if rating >= 3:
        lines.append(f"- MCP descriptor: `src/{folder.name}/{slug}.json`")
    (folder / f"{slug}.md").write_text("\n".join(lines), encoding="utf-8")


def write_json(entry: dict, folder: Path, slug: str, rating: int) -> None:
    tool_name = "revit_" + slug.replace("-", "_")
    source_rel = f"src/{folder.name}/{entry['sourceFile']}"
    data = {
        "name": tool_name,
        "description": (entry.get("bcSamples") or {}).get("description") or f"Runs the Building Coder {entry['className']} sample.",
        "arguments": {"type": "object", "properties": {}, "required": []},
        "commandClass": entry["className"],
        "sample": folder.name,
        "source": source_rel,
        "mcpRating": rating,
        "requiresUi": True,
        "notes": "Imported from The Building Coder; refactor for headless MCP execution.",
    }
    (folder / f"{slug}.json").write_text(json.dumps(data, indent=2) + "\n", encoding="utf-8")


def scan_commands() -> list[dict]:
    bc = parse_bc_samples(SRC_TBC.parent / "BcSamples.txt")
    commands: list[dict] = []
    revit_cmd_count = 0
    for cs_file in sorted(SRC_TBC.glob("Cmd*.cs")):
        content = cs_file.read_text(encoding="utf-8", errors="replace")
        all_cmds = find_command_classes(content)
        importable = filter_importable_commands(all_cmds)
        stem = cs_file.stem
        allowed = PRIMARY_ONLY_FILES.get(cs_file.name)
        if allowed is not None:
            importable = [c for c in importable if c[0] in allowed]
        else:
            stem_matches = [c for c in importable if c[0] == stem]
            extras = EXTRA_IMPORT_COMMANDS.get(cs_file.name, [])
            extra_matches = [c for c in importable if c[0] in extras]
            if stem_matches:
                importable = stem_matches + [c for c in extra_matches if c not in stem_matches]
            elif len(importable) > 1:
                importable = [importable[0]]
        for class_name, _, _ in importable:
            folder = folder_name(class_name, cs_file.name, revit_cmd_count if class_name == "RevitCommand" else 0)
            if class_name == "RevitCommand":
                revit_cmd_count += 1
            slug = to_kebab(class_name if not class_name.startswith("Cmd") else class_name[3:] or class_name)
            if class_name == "Command":
                slug = to_kebab(cs_file.stem.replace("Cmd", "") + "Command")
            commands.append({
                "className": class_name,
                "folder": folder,
                "slug": slug,
                "sourceFile": cs_file.name,
                "bcSamples": bc.get(class_name),
                "needsSplit": len(importable) > 1,
            })
    commands.sort(key=lambda c: c["folder"])
    return commands


def copy_shared() -> None:
    SHARED_DIR.mkdir(parents=True, exist_ok=True)
    props = SHARED_DIR / "Properties"
    props.mkdir(exist_ok=True)
    for name in SHARED_FILES:
        src = SRC_TBC / name
        if src.exists():
            shutil.copy2(src, SHARED_DIR / name)
    settings = SRC_TBC / "Properties" / "Settings.settings"
    designer = SRC_TBC / "Properties" / "Settings.Designer.cs"
    if settings.exists():
        shutil.copy2(settings, props / "Settings.settings")
    if designer.exists():
        shutil.copy2(designer, props / "Settings.Designer.cs")


def import_command(entry: dict) -> None:
    folder = TARGET_SRC / entry["folder"]
    folder.mkdir(parents=True, exist_ok=True)
    src_file = SRC_TBC / entry["sourceFile"]
    content = src_file.read_text(encoding="utf-8", errors="replace")
    content = remove_other_commands(content, entry["className"])
    content = transform_namespace(content, entry["folder"])
    (folder / entry["sourceFile"]).write_text(content, encoding="utf-8")

    stem = Path(entry["sourceFile"]).stem
    if stem in FORM_SAMPLES:
        for extra in FORM_SAMPLES[stem]:
            extra_src = SRC_TBC / extra
            if extra_src.exists():
                if extra.endswith(".resx"):
                    shutil.copy2(extra_src, folder / extra)
                else:
                    extra_content = extra_src.read_text(encoding="utf-8", errors="replace")
                    extra_content = transform_namespace(extra_content, entry["folder"])
                    (folder / extra).write_text(extra_content, encoding="utf-8")

    rating = estimate_mcp_rating(
        entry["className"],
        (entry.get("bcSamples") or {}).get("title", ""),
        (entry.get("bcSamples") or {}).get("description", ""),
        content,
    )
    entry["mcpRating"] = rating
    write_markdown(entry, folder, entry["slug"], rating)
    if rating >= 3:
        write_json(entry, folder, entry["slug"], rating)


def main() -> None:
    commands = scan_commands()
    MANIFEST_PATH.parent.mkdir(parents=True, exist_ok=True)
    MANIFEST_PATH.write_text(json.dumps(commands, indent=2), encoding="utf-8")
    print(f"Manifest: {len(commands)} commands -> {MANIFEST_PATH}")

    copy_shared()
    print(f"Copied shared files to {SHARED_DIR}")

    for entry in commands:
        import_command(entry)
    print(f"Imported {len(commands)} command folders under {TARGET_SRC}")


if __name__ == "__main__":
    main()
