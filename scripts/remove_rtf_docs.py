#!/usr/bin/env python3
"""Remove SDK RTF/TXT documentation now superseded by markdown command docs."""

from __future__ import annotations

import re
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
SRC = REPO / "src"
CSPROJ = SRC / "Ara3D.RevitSampleBrowser.csproj"

README_RTF = re.compile(r"ReadMe_|Readme_|help_text", re.I)
SDK_README_ROW = re.compile(r"^\| \*\*SDK ReadMe\*\* \|.*\|\s*$", re.MULTILINE)


def is_doc_rtf(path: Path) -> bool:
    return path.suffix.lower() == ".rtf" and README_RTF.search(path.name)


def remove_rtf_files() -> list[Path]:
    removed: list[Path] = []
    for path in sorted(SRC.rglob("*.rtf")):
        if is_doc_rtf(path):
            path.unlink()
            removed.append(path)
    for path in [SRC / "help.txt", SRC / "StringSearch" / "help_text.rtf"]:
        if path.exists():
            path.unlink()
            removed.append(path)
    return removed


def update_csproj() -> int:
    text = CSPROJ.read_text(encoding="utf-8")
    lines = text.splitlines(keepends=True)
    kept = [line for line in lines if not (".rtf" in line and "<None Include=" in line)]
    CSPROJ.write_text("".join(kept), encoding="utf-8")
    return sum(1 for line in lines if line not in kept)


def strip_sdk_readme_rows() -> int:
    updated = 0
    for md in SRC.rglob("*.md"):
        if md.name.startswith("_"):
            continue
        text = md.read_text(encoding="utf-8", errors="replace")
        new_text = SDK_README_ROW.sub("", text)
        if new_text != text:
            new_text = re.sub(r"\n{3,}", "\n\n", new_text)
            md.write_text(new_text, encoding="utf-8")
            updated += 1
    template = SRC / "_template.md"
    if template.exists():
        text = template.read_text(encoding="utf-8", errors="replace")
        new_text = SDK_README_ROW.sub("", text)
        if new_text != text:
            template.write_text(new_text, encoding="utf-8")
            updated += 1
    return updated


def main() -> None:
    removed = remove_rtf_files()
    csproj_lines = update_csproj()
    md_updates = strip_sdk_readme_rows()
    print(f"Removed {len(removed)} RTF/TXT documentation files")
    print(f"Removed {csproj_lines} RTF entries from csproj")
    print(f"Updated {md_updates} markdown files (removed SDK ReadMe row)")


if __name__ == "__main__":
    main()
