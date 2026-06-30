#!/usr/bin/env python3
"""Import BIM One Excel Export/Import add-in into revit-sample-browser."""

from __future__ import annotations

import json
import shutil
import subprocess
import tempfile
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
TARGET = REPO / "src" / "B1_Shared"
MANIFEST = REPO / "scripts" / "b1-import-manifest.json"
UPSTREAM = "https://github.com/bimone/addins-excelexporterimporter.git"
SOURCE_SUBDIR = "ExcelExporterImporter"

EXCLUDE_NAMES = {
    "Button.cs",
    "Command.cs",
    "AssemblyInfo.cs",
    "Addin.addin",
    "ExcelExporterImporter.csproj",
    "app.config",
}


def upstream_commit(repo_dir: Path) -> str:
    return subprocess.check_output(
        ["git", "rev-parse", "HEAD"], cwd=repo_dir, text=True
    ).strip()


def should_copy(rel: Path) -> bool:
    if rel.name in EXCLUDE_NAMES:
        return False
    if rel.parts[:1] == ("Properties",) and rel.name == "AssemblyInfo.cs":
        return False
    return True


def copy_upstream(source_root: Path, target: Path) -> int:
    if target.exists():
        shutil.rmtree(target)
    target.mkdir(parents=True)
    count = 0
    for path in source_root.rglob("*"):
        if not path.is_file():
            continue
        rel = path.relative_to(source_root)
        if not should_copy(rel):
            continue
        dest = target / rel
        dest.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(path, dest)
        count += 1
    return count


def main() -> None:
    with tempfile.TemporaryDirectory(prefix="b1-excel-import-") as tmp:
        repo_dir = Path(tmp)
        subprocess.check_call(["git", "clone", "--depth", "1", UPSTREAM, str(repo_dir)])
        commit = upstream_commit(repo_dir)
        source_root = repo_dir / SOURCE_SUBDIR
        if not source_root.is_dir():
            raise SystemExit(f"Missing upstream folder: {source_root}")
        copied = copy_upstream(source_root, TARGET)
        MANIFEST.write_text(
            json.dumps(
                {
                    "source": UPSTREAM,
                    "commit": commit,
                    "target": str(TARGET.relative_to(REPO)).replace("\\", "/"),
                    "filesCopied": copied,
                },
                indent=2,
            )
            + "\n",
            encoding="utf-8",
        )
        print(f"Copied {copied} files from {commit} into {TARGET}")


if __name__ == "__main__":
    main()
