#!/usr/bin/env python3
"""Generate COMMANDS.md — index of all IExternalCommand samples."""

from __future__ import annotations

import re
import subprocess
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
SRC = REPO / "src"
TODO = REPO / "todo.md"
OUTPUT = REPO / "COMMANDS.md"

ROW = re.compile(
    r"\| \[[ x~]\] \| [^\|]* \| `([^`]+)` \| \[([^\]]+)\]\((src/[^)]+\.md)\)"
)

SECTION = re.compile(r"^### (.+?) \(\d+\)\s*$", re.MULTILINE)

METADATA_END = re.compile(r"^\| \*\*MCP rating\*\* \|.*\|\s*$", re.MULTILINE)


def load_help_categories() -> dict[str, str]:
    """SDK sample categories from legacy help.txt (removed from working tree)."""
    try:
        result = subprocess.run(
            ["git", "show", "fe31deed^:src/help.txt"],
            cwd=REPO,
            capture_output=True,
            text=True,
            encoding="utf-8",
            errors="replace",
            check=True,
        )
    except (subprocess.CalledProcessError, FileNotFoundError):
        return {}

    categories: dict[str, str] = {}
    for line in result.stdout.splitlines():
        if not line.strip():
            continue
        parts = line.split("|")
        if not parts:
            continue
        name = parts[0].strip()
        for part in parts:
            if part.startswith("Category:"):
                categories[name] = part.split(":", 1)[1].strip()
                break
    return categories


def sample_folder_from_doc(doc_path: str) -> str:
    rel = Path(doc_path).relative_to("src")
    return rel.parts[0]


def infer_category(sample: str, help_cats: dict[str, str]) -> str:
    if sample.startswith("TBC_"):
        return "Building Coder"
    if sample.startswith("BB_"):
        return "Bowerbird"
    if sample.startswith("N3P_"):
        return "RevitExtensions"
    if sample.startswith("B1_"):
        return "Data Exchange"
    if sample in help_cats:
        return help_cats[sample]
    # Nested SDK samples (e.g. Events/PrintLog) — walk up path segments
    for key, cat in help_cats.items():
        if sample.startswith(key + "/") or sample == key:
            return cat
    return "SDK"


def parse_description(md_path: Path) -> str:
    text = md_path.read_text(encoding="utf-8-sig", errors="replace")
    m = METADATA_END.search(text)
    if not m:
        return ""
    rest = text[m.end() :]
    for line in rest.splitlines():
        line = line.strip()
        if not line or line.startswith("#"):
            continue
        if line.startswith("|") or line.startswith("-"):
            continue
        # First prose line after metadata table
        return line
    return ""


def truncate(text: str, max_len: int = 120) -> str:
    text = re.sub(r"\s+", " ", text).strip()
    if len(text) <= max_len:
        return text
    cut = text[: max_len - 1].rsplit(" ", 1)[0]
    return cut + "…"


def escape_cell(text: str) -> str:
    return text.replace("|", "\\|").replace("\n", " ")


def main() -> None:
    todo_text = TODO.read_text(encoding="utf-8", errors="replace")
    help_cats = load_help_categories()

    current_section = ""
    section_by_sample: dict[str, str] = {}
    for line in todo_text.splitlines():
        sm = SECTION.match(line)
        if sm:
            current_section = sm.group(1)
        else:
            rm = ROW.search(line)
            if rm and current_section:
                doc_path = rm.group(3)
                sample = sample_folder_from_doc(doc_path)
                section_by_sample[sample] = current_section

    rows: list[tuple[str, str, str, str, str]] = []
    for m in ROW.finditer(todo_text):
        cls, _label, doc_rel = m.group(1), m.group(2), m.group(3)
        md_path = REPO / doc_rel
        sample = sample_folder_from_doc(doc_rel)
        category = infer_category(sample, help_cats)
        desc = parse_description(md_path) if md_path.exists() else ""
        if not desc:
            desc = f"See [{Path(doc_rel).name}]({doc_rel})"
        rows.append((cls, sample, truncate(desc), category, doc_rel))

    rows.sort(key=lambda r: (r[3].lower(), r[1].lower(), r[0].lower()))

    lines = [
        "# Commands",
        "",
        "Index of all `IExternalCommand` samples in this repository. "
        "Per-command documentation lives under `src/<Sample>/`.",
        "",
        f"**Total:** {len(rows)} commands",
        "",
        "| Command | Sample | Description | Category | Doc |",
        "|---------|--------|-------------|----------|-----|",
    ]
    for cls, sample, desc, category, doc_rel in rows:
        link = f"[{Path(doc_rel).stem}]({doc_rel})"
        lines.append(
            f"| `{cls}` | {escape_cell(sample)} | {escape_cell(desc)} | {escape_cell(category)} | {link} |"
        )
    lines.append("")

    OUTPUT.write_text("\n".join(lines), encoding="utf-8")
    print(f"Wrote {len(rows)} commands to {OUTPUT.relative_to(REPO)}")


if __name__ == "__main__":
    main()
