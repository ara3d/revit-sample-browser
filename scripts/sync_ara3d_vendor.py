#!/usr/bin/env python3
"""Copy Ara3D SDK dependency DLLs into vendor/ara3d for revit-sample-browser."""

from __future__ import annotations

import argparse
import json
import shutil
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
SDK_ROOT = Path(r"C:\Users\cdigg\git\studio\ara3d-sdk")
DEFAULT_SRC = (
    SDK_ROOT
    / "ext"
    / "Ara3D.Bowerbird.RevitSamples"
    / "bin"
    / "Debug"
    / "net8.0-windows"
)
VENDOR_DIR = REPO / "vendor" / "ara3d"
REFS_FILE = SDK_ROOT / "ext" / "Ara3D.Bowerbird.RevitSamples" / "refs.txt"

SKIP_PREFIXES = (
    "Revit",
    "AdWindows",
    "UIFramework",
    "mscorlib",
    "netstandard",
    "System.",
    "System",
    "Presentation",
    "WindowsBase",
)

SKIP_EXACT = {
    "Ara3D.Revit2025.Bowerbird.dll",
    "Ara3D.Bowerbird.RevitSamples.dll",
}


def should_skip(name: str) -> bool:
    if name in SKIP_EXACT:
        return True
    return any(name.startswith(p) for p in SKIP_PREFIXES)


def load_ref_names() -> list[str]:
    if not REFS_FILE.exists():
        return []
    names: list[str] = []
    for line in REFS_FILE.read_text(encoding="utf-8").splitlines():
        name = line.strip()
        if name.endswith(".dll") and not should_skip(name):
            names.append(name)
    return names


def copy_dll(src: Path, vendor_dir: Path) -> bool:
    if not src.is_file():
        return False
    dest = vendor_dir / src.name
    if not dest.exists() or src.stat().st_mtime > dest.stat().st_mtime:
        shutil.copy2(src, dest)
        return True
    return False


def collect_from_sdk_build_outputs(vendor_dir: Path) -> int:
    copied = 0
    for dll in SDK_ROOT.rglob("Ara3D.*.dll"):
        if "obj" in dll.parts:
            continue
        if should_skip(dll.name):
            continue
        if copy_dll(dll, vendor_dir):
            copied += 1
    return copied


def collect_from_deps_json(src_dir: Path, vendor_dir: Path) -> int:
    copied = 0
    for deps_path in src_dir.glob("*.deps.json"):
        try:
            data = json.loads(deps_path.read_text(encoding="utf-8"))
        except json.JSONDecodeError:
            continue
        targets = data.get("targets", {})
        for target in targets.values():
            if not isinstance(target, dict):
                continue
            for entry in target.values():
                if not isinstance(entry, dict):
                    continue
                runtime = entry.get("runtime") or {}
                for rel_path in runtime:
                    if not rel_path.endswith(".dll"):
                        continue
                    dll_name = Path(rel_path).name
                    if should_skip(dll_name):
                        continue
                    local = src_dir / dll_name
                    if local.exists() and copy_dll(local, vendor_dir):
                        copied += 1
                        continue
                    nuget_root = Path.home() / ".nuget" / "packages"
                    pkg_key = entry.get("name") or ""
                    # runtime keys look like lib/net8.0/Foo.dll — search nuget cache by dll name
                    for pkg_dir in nuget_root.glob("*/*"):
                        candidate = pkg_dir / rel_path.replace("/", "\\")
                        if candidate.exists() and copy_dll(candidate, vendor_dir):
                            copied += 1
                            break
                        flat = list(pkg_dir.rglob(dll_name))
                        if flat and copy_dll(flat[0], vendor_dir):
                            copied += 1
                            break
    return copied


def sync(src_dir: Path, vendor_dir: Path) -> int:
    vendor_dir.mkdir(parents=True, exist_ok=True)
    copied = 0
    missing: list[str] = []

    if src_dir.is_dir():
        for name in load_ref_names():
            if copy_dll(src_dir / name, vendor_dir):
                copied += 1
            elif not (vendor_dir / name).exists():
                missing.append(name)
        for src in sorted(src_dir.glob("*.dll")):
            if should_skip(src.name):
                continue
            if copy_dll(src, vendor_dir):
                copied += 1
        copied += collect_from_deps_json(src_dir, vendor_dir)

    copied += collect_from_sdk_build_outputs(vendor_dir)

    print(f"Synced DLLs to {vendor_dir} ({copied} file operations)")
    still_missing = [n for n in load_ref_names() if not (vendor_dir / n).exists()]
    if still_missing:
        print(f"Warning: {len(still_missing)} referenced DLLs still missing:")
        for name in still_missing[:15]:
            print(f"  - {name}")
        if len(still_missing) > 15:
            print(f"  ... and {len(still_missing) - 15} more")
    return 1 if still_missing else 0


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--src", type=Path, default=DEFAULT_SRC)
    args = parser.parse_args()
    return sync(args.src, VENDOR_DIR)


if __name__ == "__main__":
    raise SystemExit(main())
