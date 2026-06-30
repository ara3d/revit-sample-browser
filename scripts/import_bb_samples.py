#!/usr/bin/env python3
"""Import Ara3D Bowerbird RevitSamples into revit-sample-browser as BB_* commands."""

from __future__ import annotations

import json
import re
import shutil
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
SRC_BB = Path(
    r"C:\Users\cdigg\git\studio\ara3d-sdk\ext\Ara3D.Bowerbird.RevitSamples"
)
TARGET_SRC = REPO / "src"
SHARED_DIR = TARGET_SRC / "BB_Shared"
MANIFEST_PATH = REPO / "scripts" / "bb-import-manifest.json"

SKIP_COMMANDS = {"AutoRun", "ChristophersTests", "BowerbirdFloorplanExporter"}

SKIP_SHARED_FILES = {
    "AutoRun.cs",
    "ChristophersTests.cs",
    "RenderMesh.cs",
    "RenderVertex.cs",
}

# Extra files copied into a command folder (beyond the primary command .cs).
COMMAND_EXTRA_FILES: dict[str, list[str]] = {
    "CommandColladaExport": ["ColladaExportContext.cs"],
    "CommandBimOpenSchemaVersion2": [
        "BIMOpenSchemaExporterForm.cs",
        "BIMOpenSchemaExporterForm.Designer.cs",
        "BIMOpenSchemaExporterForm.resx",
    ],
}

# Source files that contain only commented-out NamedCommand code.
SKIP_SOURCE_FILES = {
    "BowerbirdGeoJsonExporter.cs",
    "BowerbirdLayoutImporter.cs",
    "BackgroundForm.cs",
}

NAMED_CMD_PATTERN = re.compile(
    r"class\s+(?P<name>\w+)\b\s*(?::\s*|\r?\n\s*:\s*)NamedCommand\b",
    re.MULTILINE,
)


def to_kebab(name: str) -> str:
    s = re.sub(r"(.)([A-Z][a-z]+)", r"\1-\2", name)
    s = re.sub(r"([a-z0-9])([A-Z])", r"\1-\2", s)
    return s.replace("_", "-").lower()


def folder_name(class_name: str) -> str:
    if class_name.startswith("Command"):
        base = class_name[len("Command") :]
    else:
        base = class_name
    return f"BB_{base}"


def cmd_wrapper_name(class_name: str) -> str:
    if class_name.startswith("Command"):
        return "Cmd" + class_name[len("Command") :]
    return "Cmd" + class_name


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


def extract_class(content: str, class_name: str) -> str | None:
    pattern = re.compile(
        rf"(?P<prefix>(?:^|\n)(?:\s*\[[^\]]+\]\s*)*)"
        rf"(?P<decl>(?:public|internal|private|protected|sealed|abstract|static)\s+)*"
        rf"class\s+{re.escape(class_name)}\b[^{{]*\{{",
        re.MULTILINE,
    )
    m = pattern.search(content)
    if not m:
        return None
    brace = content.find("{", m.end() - 1)
    if brace == -1:
        return None
    end = find_brace_end(content, brace)
    start = m.start()
    if start > 0 and content[start] == "\n":
        start += 1
    return content[start : end + 1]


def scan_commands() -> list[dict]:
    commands: list[dict] = []
    for cs_file in sorted(SRC_BB.glob("*.cs")):
        if cs_file.name in SKIP_SOURCE_FILES:
            continue
        content = cs_file.read_text(encoding="utf-8", errors="replace")
        for m in NAMED_CMD_PATTERN.finditer(content):
            class_name = m.group("name")
            if class_name in SKIP_COMMANDS:
                continue
            # Skip matches inside block comments.
            if content[: m.start()].rfind("/*") > content[: m.start()].rfind("*/"):
                continue
            folder = folder_name(class_name)
            slug = to_kebab(cmd_wrapper_name(class_name)[3:])
            commands.append(
                {
                    "className": class_name,
                    "folder": folder,
                    "slug": slug,
                    "sourceFile": cs_file.name,
                    "cmdClass": cmd_wrapper_name(class_name),
                }
            )
    # Deduplicate by class name.
    seen: set[str] = set()
    unique: list[dict] = []
    for entry in commands:
        if entry["className"] in seen:
            continue
        seen.add(entry["className"])
        unique.append(entry)
    unique.sort(key=lambda c: c["folder"])
    return unique


def command_source_files(commands: list[dict]) -> set[str]:
    files: set[str] = set()
    for entry in commands:
        files.add(entry["sourceFile"])
        for extra in COMMAND_EXTRA_FILES.get(entry["className"], []):
            files.add(extra)
    return files


def patch_selected_elements_json(content: str) -> str:
    content = content.replace(
        "using Ara3D.Bowerbird.Revit;\n",
        "",
    )
    content = content.replace(
        'BowerbirdRevitApp.Instance.Schedule(_ =>\n'
        "                {\n"
        "                    app.SelectionChanged -= SelectionChanged;\n"
        '                }, "Detaching selection changed event");',
        "RevitEventScheduler.Run(_ => app.SelectionChanged -= SelectionChanged);",
    )
    return content


def patch_direct_context_ply_loader(content: str) -> str:
    extracted = extract_class(content, "CommandDirectContextPlyLoader")
    if not extracted:
        return content
    header = """using Ara3D.Geometry;
using Ara3D.IO.PLY;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.Bowerbird.RevitSamples;

"""
    return header + extracted + "\n"


def transform_command_content(entry: dict, content: str) -> str:
    class_name = entry["className"]
    if class_name == "CommandSelectedElementsJson":
        content = patch_selected_elements_json(content)
    if class_name == "CommandDirectContextPlyLoader":
        content = patch_direct_context_ply_loader(content)
    return content


def write_cmd_wrapper(entry: dict, folder: Path) -> None:
    cmd_class = entry["cmdClass"]
    named_class = entry["className"]
    text = f"""using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.Bowerbird.RevitSamples
{{
    [Transaction(TransactionMode.Manual)]
    internal class {cmd_class} : IExternalCommand
    {{
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {{
            try
            {{
                new {named_class}().Execute(commandData.Application);
                return Result.Succeeded;
            }}
            catch (Exception ex)
            {{
                message = ex.Message;
                return Result.Failed;
            }}
        }}
    }}
}}
"""
    (folder / f"{cmd_class}.cs").write_text(text, encoding="utf-8")


REVIT_EVENT_SCHEDULER = '''using System;
using Autodesk.Revit.UI;

namespace Ara3D.Bowerbird.RevitSamples
{
    /// <summary>
    /// Marshals work onto Revit's API thread without BowerbirdRevitApp.
    /// </summary>
    public static class RevitEventScheduler
    {
        private static readonly RevitEventHandler Handler = new();
        private static readonly ExternalEvent Event = ExternalEvent.Create(Handler);

        public static void Run(Action<UIApplication> action)
        {
            Handler.Action = action;
            Event.Raise();
        }

        private sealed class RevitEventHandler : IExternalEventHandler
        {
            public Action<UIApplication> Action { get; set; }

            public void Execute(UIApplication app)
                => Action?.Invoke(app);

            public string GetName()
                => "BB Revit Event Scheduler";
        }
    }
}
'''


DIRECT_CONTEXT_RENDER_TYPES_SOURCE = SRC_BB / "CommandDirectContextPlyLoader.cs"


def write_shared_helpers() -> None:
    scheduler = SHARED_DIR / "RevitEventScheduler.cs"
    scheduler.write_text(REVIT_EVENT_SCHEDULER, encoding="utf-8")

    render_types = SHARED_DIR / "DirectContextRenderTypes.cs"
    if DIRECT_CONTEXT_RENDER_TYPES_SOURCE.exists():
        content = DIRECT_CONTEXT_RENDER_TYPES_SOURCE.read_text(encoding="utf-8", errors="replace")
        mesh = extract_class(content, "RenderMesh")
        vertex = extract_class(content, "RenderVertex")
        if mesh and vertex:
            header = """using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Ara3D.Collections;
using Ara3D.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;

namespace Ara3D.Bowerbird.RevitSamples;

"""
            render_types.write_text(header + mesh + "\n\n" + vertex + "\n", encoding="utf-8")


def write_markdown(entry: dict, folder: Path) -> None:
    slug = entry["slug"]
    cmd_class = entry["cmdClass"]
    source_rel = f"src/{folder.name}/{entry['sourceFile']}"
    lines = [
        f"# {cmd_class}",
        "",
        "| Field | Value |",
        "|-------|-------|",
        f"| **Sample** | {folder.name} |",
        f"| **Class** | `{cmd_class}` |",
        f"| **NamedCommand** | `{entry['className']}` |",
        f"| **Source** | `{source_rel}` |",
        f"| **MCP rating** | 3/5 |",
        "",
        f"Bowerbird sample `{entry['className']}` imported from Ara3D.Bowerbird.RevitSamples.",
        "",
        "## What it demonstrates",
        "",
        f"- Bowerbird `NamedCommand` sample wrapped as `{cmd_class}` (`IExternalCommand`)",
        f"- Source: `{entry['sourceFile']}`",
        "",
    ]
    (folder / f"{slug}.md").write_text("\n".join(lines), encoding="utf-8")


def copy_shared(commands: list[dict]) -> None:
    SHARED_DIR.mkdir(parents=True, exist_ok=True)
    cmd_files = command_source_files(commands)
    for src in sorted(SRC_BB.iterdir()):
        if src.suffix.lower() not in {".cs", ".resx"}:
            continue
        if src.name in cmd_files or src.name in SKIP_SOURCE_FILES or src.name in SKIP_SHARED_FILES:
            continue
        if src.suffix.lower() == ".resx":
            shutil.copy2(src, SHARED_DIR / src.name)
        else:
            shutil.copy2(src, SHARED_DIR / src.name)


def import_command(entry: dict) -> None:
    folder = TARGET_SRC / entry["folder"]
    folder.mkdir(parents=True, exist_ok=True)

    src_file = SRC_BB / entry["sourceFile"]
    content = src_file.read_text(encoding="utf-8", errors="replace")
    content = transform_command_content(entry, content)
    (folder / entry["sourceFile"]).write_text(content, encoding="utf-8")

    for extra in COMMAND_EXTRA_FILES.get(entry["className"], []):
        extra_src = SRC_BB / extra
        if not extra_src.exists():
            continue
        if extra.endswith(".resx"):
            shutil.copy2(extra_src, folder / extra)
        else:
            shutil.copy2(extra_src, folder / extra)

    write_cmd_wrapper(entry, folder)
    write_markdown(entry, folder)


def main() -> None:
    commands = scan_commands()
    MANIFEST_PATH.write_text(json.dumps(commands, indent=2) + "\n", encoding="utf-8")
    print(f"Manifest: {len(commands)} commands -> {MANIFEST_PATH}")

    copy_shared(commands)
    write_shared_helpers()
    print(f"Copied shared files to {SHARED_DIR}")

    for entry in commands:
        import_command(entry)
    print(f"Imported {len(commands)} command folders under {TARGET_SRC}")


if __name__ == "__main__":
    main()
