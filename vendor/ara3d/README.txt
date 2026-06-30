Ara3D SDK vendor DLLs for Bowerbird (BB_) samples
=================================================

These DLLs are copied from a built ara3d-sdk tree. They are not committed
to source control by default.

Prerequisites
-------------
1. Clone/build ara3d-sdk:
   C:\Users\cdigg\git\studio\ara3d-sdk

2. Build the Revit samples project (Release):
   dotnet build ext\Ara3D.Bowerbird.RevitSamples\Ara3D.Bowerbird.RevitSamples.csproj -c Release

Sync
----
From the revit-sample-browser repo root:

  python scripts/sync_ara3d_vendor.py

Optional custom build output:

  python scripts/sync_ara3d_vendor.py --src "C:\path\to\bin\Release\net8.0-windows"

The sync script copies dependency DLLs listed in ara3d-sdk's refs.txt,
excluding Revit API assemblies (already under vendor/revit/2025) and
Ara3D.Revit2025.Bowerbird.dll (not required for sample-browser BB commands).
