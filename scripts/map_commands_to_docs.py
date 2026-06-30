#!/usr/bin/env python3
"""Map IExternalCommand classes to markdown docs."""

from __future__ import annotations

import re
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
SRC = REPO / "src"

IEC = re.compile(
    r"class\s+(\w+)\s*(?::\s*|\r?\n\s*:\s*)IExternalCommand\b"
)
SKIP = {"SampleBrowserCommand"}


def sample_folder(cs: Path) -> str:
    rel = cs.relative_to(SRC)
    parts = rel.parts
    if parts[0].startswith("TBC_"):
        return parts[0]
    return parts[0]


def find_doc_for_command(cs: Path, cls: str) -> Path | None:
    folder = cs.parent
    # Same folder
    for md in folder.glob("*.md"):
        return md
    # Parent sample folder (nested commands)
    sample = SRC / sample_folder(cs)
    for md in sample.glob("*.md"):
        if cls.lower().replace("command", "") in md.stem.lower() or md.stem.lower() in cls.lower():
            return md
    for md in sample.glob("**/*.md"):
        stem = md.stem.replace("-", "").lower()
        cls_key = cls.lower().replace("command", "")
        if stem == cls_key or cls_key.startswith(stem) or stem.startswith(cls_key):
            return md
    return None


def main() -> None:
    missing: list[tuple[str, str, str]] = []
    found = 0
    for cs in sorted(SRC.rglob("*.cs")):
        if "TBC_Shared" in str(cs) or "obj" in cs.parts:
            continue
        text = cs.read_text(encoding="utf-8", errors="replace")
        for cls in IEC.findall(text):
            if cls in SKIP:
                continue
            doc = find_doc_for_command(cs, cls)
            rel = cs.relative_to(REPO).as_posix()
            if doc:
                found += 1
            else:
                missing.append((cls, rel, sample_folder(cs)))

    print(f"Commands with docs: {found}")
    print(f"Commands missing docs: {len(missing)}")
    for cls, src, folder in missing:
        print(f"  {cls:40} {src}")


if __name__ == "__main__":
    main()
