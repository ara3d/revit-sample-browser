# _utils.cs Analysis Report

Analysis of **101** `_utils.cs` files across the Revit sample browser.

## Summary

| Metric | Count |
|--------|------:|
| `_utils.cs` files | 101 |
| Total methods (all types in `_utils.cs`) | 584 |
| `_Utils` static helper methods | 435 |
| Methods with analyzable body | 543 |
| Exact duplicate body groups | 77 |
| Duplicate method bodies (excess copies) | 126 |
| Near-duplicate pairs (≥85% similarity) | 55 |
| Methods used by exactly one command/caller | 216 |
| Methods used by multiple commands/callers | 59 |
| Methods with no detected `_Utils.` usage | 309 |

> **Notes:** Duplicate detection compares normalized method bodies (whitespace/comments stripped). Near-duplicates share ≥85% body similarity but are not identical. Usage is detected via `_Utils.<Method>` references from non-`_utils` `.cs` files within the same sample folder. A *command* is a class implementing `IExternalCommand` or `IExternalApplication`; when absent, the calling file path is listed.

## Exact Duplicate Method Bodies

### Group 1 — `CrossProduct` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.CrossProduct` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.CrossProduct` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.CrossProduct` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.CrossProduct` — `Truss/_utils.cs`

```csharp
{ return new Vector4(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X); }
```

### Group 2 — `CrossProduct` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.CrossProduct` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.CrossProduct` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.CrossProduct` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.CrossProduct` — `Truss/_utils.cs`

```csharp
{ return new Vector4(va.Y * vb.Z - va.Z * vb.Y, va.Z * vb.X - va.X * vb.Z, va.X * vb.Y - va.Y * vb.X); }
```

### Group 3 — `DotMatrix` (4 copies)

- `Ara3D.RevitSampleBrowser.AreaReinCurve.CS._Utils.DotMatrix` — `AreaReinCurve/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.DotMatrix` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.DotMatrix` — `CreateSimpleAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewRebar.CS._Utils.DotMatrix` — `NewRebar/_utils.cs`

```csharp
=> p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
```

### Group 4 — `DotProduct` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.DotProduct` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.DotProduct` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.DotProduct` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.DotProduct` — `Truss/_utils.cs`

```csharp
{ return X * v.X + Y * v.Y + Z * v.Z; }
```

### Group 5 — `DotProduct` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.DotProduct` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.DotProduct` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.DotProduct` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.DotProduct` — `Truss/_utils.cs`

```csharp
{ return va.X * vb.X + va.Y * vb.Y + va.Z * vb.Z; }
```

### Group 6 — `Identity` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.Identity` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Matrix4.Identity` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.Identity` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.Identity` — `Truss/_utils.cs`

```csharp
{ for (var i = 0; i < 4; i++) for (var j = 0; j < 4; j++) m_matrix[i, j] = 0.0f; m_matrix[0, 0] = 1.0f; m_matrix[1, 1] = 1.0f; m_matrix[2, 2] = 1.0f; m_matrix[3, 3] = 1.0f; }
```

### Group 7 — `Matrix4` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.Matrix4` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Matrix4.Matrix4` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.Matrix4` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.Matrix4` — `Truss/_utils.cs`

```csharp
{ m_type = MatrixType.Normal; Identity(); }
```

### Group 8 — `Matrix4` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.Matrix4` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Matrix4.Matrix4` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.Matrix4` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.Matrix4` — `Truss/_utils.cs`

```csharp
{ m_type = MatrixType.Rotation; Identity(); m_matrix[0, 0] = xAxis.X; m_matrix[0, 1] = xAxis.Y; m_matrix[0, 2] = xAxis.Z; m_matrix[1, 0] = yAxis.X; m_matrix[1, 1] = yAxis.Y; m_matrix[1, 2] = yAxis.Z; m_matrix[2, 0] = zAxis.X; m_matrix[2, 1] = zAxis.Y; m_matrix[2, 2] = zAxis.Z; }
```

### Group 9 — `Multiply` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.Multiply` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Matrix4.Multiply` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.Multiply` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.Multiply` — `Truss/_utils.cs`

```csharp
{ var result = new Matrix4(); for (var i = 0; i < 4; i++) for (var j = 0; j < 4; j++) result[i, j] = left[i, 0] * right[0, j] + left[i, 1] * right[1, j] + left[i, 2] * right[2, j] + left[i, 3] * right[3, j]; return result; }
```

### Group 10 — `Normalize` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.Normalize` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.Normalize` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.Normalize` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.Normalize` — `Truss/_utils.cs`

```csharp
{ var length = Length(); if (length == 0) length = 1; X /= length; Y /= length; Z /= length; }
```

### Group 11 — `RotationInverse` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.RotationInverse` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Matrix4.RotationInverse` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.RotationInverse` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.RotationInverse` — `Truss/_utils.cs`

```csharp
{ return new Matrix4(new Vector4(this[0, 0], this[1, 0], this[2, 0]), new Vector4(this[0, 1], this[1, 1], this[2, 1]), new Vector4(this[0, 2], this[1, 2], this[2, 2])); }
```

### Group 12 — `ScaleInverse` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.ScaleInverse` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Matrix4.ScaleInverse` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.ScaleInverse` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.ScaleInverse` — `Truss/_utils.cs`

```csharp
{ return new Matrix4(1 / m_matrix[0, 0]); }
```

### Group 13 — `SubXyz` (4 copies)

- `Ara3D.RevitSampleBrowser.AreaReinCurve.CS._Utils.SubXyz` — `AreaReinCurve/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.SubXyz` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.SubXyz` — `CreateSimpleAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewRebar.CS._Utils.SubXyz` — `NewRebar/_utils.cs`

```csharp
=> new XYZ(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
```

### Group 14 — `Transform` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.Transform` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Matrix4.TransForm` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.Transform` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.Transform` — `Truss/_utils.cs`

```csharp
{ return new Vector4(point.X * this[0, 0] + point.Y * this[1, 0] + point.Z * this[2, 0] + point.W * this[3, 0], point.X * this[0, 1] + point.Y * this[1, 1] + point.Z * this[2, 1] + point.W * this[3, 1], point.X * this[0, 2] + point.Y * this[1, 2] + point.Z * this[2, 2] + point.W * this[3, 2]); }
```

### Group 15 — `TranslationInverse` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.TranslationInverse` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Matrix4.TransLationInverse` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.TranslationInverse` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.TranslationInverse` — `Truss/_utils.cs`

```csharp
{ return new Matrix4(new Vector4(-this[3, 0], -this[3, 1], -this[3, 2])); }
```

### Group 16 — `Vector4` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.Vector4` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.Vector4` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.Vector4` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.Vector4` — `Truss/_utils.cs`

```csharp
{ X = x; Y = y; Z = z; }
```

### Group 17 — `operator *` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.operator *` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.operator *` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.operator *` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.operator *` — `Truss/_utils.cs`

```csharp
{ return new Vector4(v.X * factor, v.Y * factor, v.Z * factor); }
```

### Group 18 — `operator +` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.operator +` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.operator +` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.operator +` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.operator +` — `Truss/_utils.cs`

```csharp
{ return new Vector4(va.X + vb.X, va.Y + vb.Y, va.Z + vb.Z); }
```

### Group 19 — `operator -` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.operator -` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.operator -` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.operator -` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.operator -` — `Truss/_utils.cs`

```csharp
{ return new Vector4(va.X - vb.X, va.Y - vb.Y, va.Z - vb.Z); }
```

### Group 20 — `operator /` (4 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.operator /` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.operator /` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.operator /` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.operator /` — `Truss/_utils.cs`

```csharp
{ return new Vector4(v.X / factor, v.Y / factor, v.Z / factor); }
```

### Group 21 — `AllowReference` (3 copies)

- `Ara3D.RevitSampleBrowser.Site.CS._Utils.AllowReference` — `Site/_utils.cs`
- `Ara3D.RevitSampleBrowser.Site.CS.SubRegionSelectionFilter.AllowReference` — `Site/_utils.cs`
- `Ara3D.RevitSampleBrowser.Site.CS.TopographySurfaceSelectionFilter.AllowReference` — `Site/_utils.cs`

```csharp
=> false;
```

### Group 22 — `Inverse` (3 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.Inverse` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.Inverse` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.Inverse` — `Truss/_utils.cs`

```csharp
{ switch (m_type) { case MatrixType.Rotation: return RotationInverse(); case MatrixType.Translation: return TranslationInverse(); case MatrixType.RotationAndTranslation: return Multiply(TranslationInverse(), RotationInverse()); case MatrixType.Scale: return ScaleInverse(); case MatrixType.Normal:...
```

### Group 23 — `IsEqual` (3 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.IsEqual` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.IsEqual` — `CreateSimpleAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewRebar.CS._Utils.IsEqual` — `NewRebar/_utils.cs`

```csharp
=> Math.Abs(d1 - d2) < Precision;
```

### Group 24 — `IsVertical` (3 copies)

- `Ara3D.RevitSampleBrowser.AreaReinCurve.CS._Utils.IsVertical` — `AreaReinCurve/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.IsVertical` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.IsVertical` — `CreateSimpleAreaRein/_utils.cs`

```csharp
=> Math.Abs(DotMatrix(SubXyz(line1.GetEndPoint(0), line1.GetEndPoint(1)), SubXyz(line2.GetEndPoint(0), line2.GetEndPoint(1)))) < Precision;
```

### Group 25 — `Length` (3 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.Length` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.Length` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.Length` — `ShaftHolePuncher/_utils.cs`

```csharp
{ return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }
```

### Group 26 — `Matrix4` (3 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.Matrix4` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.Matrix4` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.Matrix4` — `Truss/_utils.cs`

```csharp
{ m_type = MatrixType.Translation; Identity(); m_matrix[3, 0] = origin.X; m_matrix[3, 1] = origin.Y; m_matrix[3, 2] = origin.Z; }
```

### Group 27 — `Matrix4` (3 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.Matrix4` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.Matrix4` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.Matrix4` — `Truss/_utils.cs`

```csharp
{ m_type = MatrixType.RotationAndTranslation; Identity(); m_matrix[0, 0] = xAxis.X; m_matrix[0, 1] = xAxis.Y; m_matrix[0, 2] = xAxis.Z; m_matrix[1, 0] = yAxis.X; m_matrix[1, 1] = yAxis.Y; m_matrix[1, 2] = yAxis.Z; m_matrix[2, 0] = zAxis.X; m_matrix[2, 1] = zAxis.Y; m_matrix[2, 2] = zAxis.Z; m_mat...
```

### Group 28 — `Matrix4` (3 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Matrix4.Matrix4` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Matrix4.Matrix4` — `ShaftHolePuncher/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Matrix4.Matrix4` — `Truss/_utils.cs`

```csharp
{ m_type = MatrixType.Scale; Identity(); m_matrix[0, 0] = scale; m_matrix[1, 1] = scale; m_matrix[2, 2] = scale; }
```

### Group 29 — `Vector4` (3 copies)

- `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS.Vector4.Vector4` — `CurtainWallGrid/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.Vector4` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS.Vector4.Vector4` — `ShaftHolePuncher/_utils.cs`

```csharp
{ X = (float)v.X; Y = (float)v.Y; Z = (float)v.Z; }
```

### Group 30 — `AddXyz` (2 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.AddXyz` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.NewRebar.CS._Utils.AddXyz` — `NewRebar/_utils.cs`

```csharp
=> new XYZ(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
```

### Group 31 — `AddXyz` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.AddXyz` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.AddXyz` — `Reinforcement/_utils.cs`

```csharp
{ var x = p1.X + p2.X; var y = p1.Y + p2.Y; var z = p1.Z + p2.Z; return new XYZ(x, y, z); }
```

### Group 32 — `AllowElement` (2 copies)

- `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS._Utils.AllowElement` — `AppearanceAssetEditing/_utils.cs`
- `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS.IsPaintedFaceSelectionFilter.AllowElement` — `AppearanceAssetEditing/_utils.cs`

```csharp
{ m_selectedDocument = element.Document; return true; }
```

### Group 33 — `AllowElement` (2 copies)

- `Ara3D.RevitSampleBrowser.Site.CS._Utils.AllowElement` — `Site/_utils.cs`
- `Ara3D.RevitSampleBrowser.Site.CS.SubRegionSelectionFilter.AllowElement` — `Site/_utils.cs`

```csharp
=> element is TopographySurface ts && ts.IsSiteSubRegion;
```

### Group 34 — `AllowReference` (2 copies)

- `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS._Utils.AllowReference` — `AppearanceAssetEditing/_utils.cs`
- `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS.IsPaintedFaceSelectionFilter.AllowReference` — `AppearanceAssetEditing/_utils.cs`

```csharp
{ if (m_selectedDocument == null) throw new Exception("AllowElement was never called for this reference..."); var element = m_selectedDocument.GetElement(refer); var face = element.GetGeometryObjectFromReference(refer) as Face; return m_selectedDocument.IsPainted(element.Id, face); }
```

### Group 35 — `ConvertFromBitmap` (2 copies)

- `Ara3D.RevitSampleBrowser.UIAPI.CS._Utils.ConvertFromBitmap` — `UIAPI/_utils.cs`
- `Ara3D.RevitSampleBrowser.UIAPI.CS._Utils.GetBitmapAsImageSource` — `UIAPI/_utils.cs`

```csharp
=> Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
```

### Group 36 — `CrossMatrix` (2 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.CrossMatrix` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.CrossMatrix` — `CreateSimpleAreaRein/_utils.cs`

```csharp
=> new XYZ(p1.Z * p2.Y - p1.Y * p2.Z, p1.X * p2.Z - p1.Z * p2.X, p1.Y * p2.X - p1.X * p2.Y);
```

### Group 37 — `CrossMatrix` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.CrossMatrix` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.CrossMatrix` — `Reinforcement/_utils.cs`

```csharp
{ var u1 = p1.X; var u2 = p1.Y; var u3 = p1.Z; var v1 = p2.X; var v2 = p2.Y; var v3 = p2.Z; var x = v3 * u2 - v2 * u3; var y = v1 * u3 - v3 * u1; var z = v2 * u1 - v1 * u2; return new XYZ(x, y, z); }
```

### Group 38 — `Distribute` (2 copies)

- `Ara3D.RevitSampleBrowser.FamilyCreation.CS._Utils.Distribute` — `FamilyCreation/_utils.cs`
- `Ara3D.RevitSampleBrowser.ReferencePlane.CS._Utils.Distribute` — `ReferencePlane/_utils.cs`

```csharp
{ var count = mesh.Vertices.Count; startPoint = mesh.Vertices[0]; endPoint = mesh.Vertices[count / 3]; thirdPnt = mesh.Vertices[count / 3 * 2]; }
```

### Group 39 — `DotMatrix` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.DotMatrix` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.DotMatrix` — `Reinforcement/_utils.cs`

```csharp
{ var v1 = p1.X; var v2 = p1.Y; var v3 = p1.Z; var u1 = p2.X; var u2 = p2.Y; var u3 = p2.Z; return v1 * u1 + v2 * u2 + v3 * u3; }
```

### Group 40 — `Equal` (2 copies)

- `Ara3D.RevitSampleBrowser.FamilyCreation.CS._Utils.Equal` — `FamilyCreation/_utils.cs`
- `Ara3D.RevitSampleBrowser.ReferencePlane.CS._Utils.Equal` — `ReferencePlane/_utils.cs`

```csharp
=> Math.Abs(vectorA.X - vectorB.X) < Precision && Math.Abs(vectorA.Y - vectorB.Y) < Precision;
```

### Group 41 — `FindParaByName` (2 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.FindParaByName` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.FindParaByName` — `CreateSimpleAreaRein/_utils.cs`

```csharp
{ foreach (Parameter para in paras) if (para.Definition.Name == name) return para; return null; }
```

### Group 42 — `FindParameter` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.FindParameter` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.FindParameter` — `Reinforcement/_utils.cs`

```csharp
=> parameters.Cast<Parameter>().FirstOrDefault(p => p.Definition.Name == name);
```

### Group 43 — `GetFaces` (2 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.GetFaces` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.GetFaces` — `CreateSimpleAreaRein/_utils.cs`

```csharp
{ var geoOptions = elem.Document.Application.Create.NewGeometryOptions(); geoOptions.ComputeReferences = true; foreach (var obj in elem.get_Geometry(geoOptions)) if (obj is Solid solid) return solid.Faces; return null; }
```

### Group 44 — `GetHookOrient` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.GetHookOrient` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.GetHookOrient` — `Reinforcement/_utils.cs`

```csharp
{ var tempVec = normal; for (var i = 0; i < 4; i++) { tempVec = CrossMatrix(tempVec, curveVec); if (IsSameDirection(tempVec, hookVec)) { switch (i) { case 0: return RebarHookOrientation.Right; case 2: return RebarHookOrientation.Left; } } } throw new Exception("Can't find the hook orient accordin...
```

### Group 45 — `GetLength` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.GetLength` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.GetLength` — `Reinforcement/_utils.cs`

```csharp
{ var x = vector.X; var y = vector.Y; var z = vector.Z; return Math.Sqrt(x * x + y * y + z * z); }
```

### Group 46 — `GetPoints` (2 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.GetPoints` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.GetPoints` — `CreateSimpleAreaRein/_utils.cs`

```csharp
=> face.Triangulate().Vertices as List<XYZ> ?? new List<XYZ>();
```

### Group 47 — `ImperialDutRatio` (2 copies)

- `Ara3D.RevitSampleBrowser.GridCreation.CS._Utils.ImperialDutRatio` — `GridCreation/_utils.cs`
- `Ara3D.RevitSampleBrowser.ImportExport.CS._Utils.ImperialDutRatio` — `ImportExport/_utils.cs`

```csharp
{ if (unit == UnitTypeId.Feet) return 1; if (unit == UnitTypeId.FeetFractionalInches) return 1; if (unit == UnitTypeId.Inches) return 12; if (unit == UnitTypeId.FractionalInches) return 12; if (unit == UnitTypeId.Meters) return 0.3048; if (unit == UnitTypeId.Centimeters) return 30.48; if (unit ==...
```

### Group 48 — `IsEqual` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.IsEqual` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.IsEqual` — `Reinforcement/_utils.cs`

```csharp
{ var diff = Math.Abs(d1 - d2); return diff < Precision; }
```

### Group 49 — `IsEqual` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.IsEqual` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.IsEqual` — `Reinforcement/_utils.cs`

```csharp
{ var flag = true; flag = flag && IsEqual(first.X, second.X); flag = flag && IsEqual(first.Y, second.Y); flag = flag && IsEqual(first.Z, second.Z); return flag; }
```

### Group 50 — `IsHorizontalFace` (2 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.IsHorizontalFace` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.IsHorizontalFace` — `CreateSimpleAreaRein/_utils.cs`

```csharp
{ var points = GetPoints(face); if (points.Count < 4) return false; return IsEqual(points[0].Z, points[1].Z) && IsEqual(points[1].Z, points[2].Z) && IsEqual(points[2].Z, points[3].Z) && IsEqual(points[3].Z, points[0].Z); }
```

### Group 51 — `IsInRightDir` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.IsInRightDir` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.IsInRightDir` — `Reinforcement/_utils.cs`

```csharp
{ var eps = 1.0e-8; if (Math.Abs(normal.X) <= eps) { if (normal.Y > 0) return false; return true; } if (normal.X > 0) return true; if (normal.X < 0) return false; return true; }
```

### Group 52 — `IsOppositeDirection` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.IsOppositeDirection` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.IsOppositeDirection` — `Reinforcement/_utils.cs`

```csharp
{ var first = UnitVector(firstVec); var second = UnitVector(secondVec); var dot = DotMatrix(first, second); return IsEqual(dot, -1); }
```

### Group 53 — `IsParallel` (2 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.IsParallel` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.IsParallel` — `CreateSimpleAreaRein/_utils.cs`

```csharp
{ var points = GetPoints(face); if (points.Count < 3) return false; var cross = CrossMatrix(SubXyz(points[0], points[1]), SubXyz(points[1], points[2])); return DotMatrix(cross, SubXyz(line.GetEndPoint(0), line.GetEndPoint(1))) < Precision; }
```

### Group 54 — `IsRectangular` (2 copies)

- `Ara3D.RevitSampleBrowser.AreaReinCurve.CS._Utils.IsRectangular` — `AreaReinCurve/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.IsRectangular` — `CreateComplexAreaRein/_utils.cs`

```csharp
=> IsRectangular(curves.Cast<Curve>().ToList());
```

### Group 55 — `IsRectangular` (2 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.IsRectangular` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.IsRectangular` — `CreateSimpleAreaRein/_utils.cs`

```csharp
{ if (curves.Count != 4) return false; var lines = new Line[4]; for (var i = 0; i < 4; i++) { lines[i] = curves[i] as Line; if (lines[i] == null) return false; } var verticalLines = new Line[2]; Line paraLine = null; var index = 0; for (var i = 1; i < 4; i++) if (IsVertical(lines[0], lines[i])) v...
```

### Group 56 — `IsSameDirection` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.IsSameDirection` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.IsSameDirection` — `Reinforcement/_utils.cs`

```csharp
{ var first = UnitVector(firstVec); var second = UnitVector(secondVec); var dot = DotMatrix(first, second); return IsEqual(dot, 1); }
```

### Group 57 — `IsVertical` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.IsVertical` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.IsVertical` — `Reinforcement/_utils.cs`

```csharp
{ var points = face.Triangulate().Vertices as List<XYZ>; if (3 > points.Count) return false; var first = points[0]; var second = points[1]; var third = points[2]; var lineStart = line.GetEndPoint(0); var lineEnd = line.GetEndPoint(1); if (null != faceTrans) { first = TransformPoint(first, faceTra...
```

### Group 58 — `MultiplyVector` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.MultiplyVector` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.MultiplyVector` — `Reinforcement/_utils.cs`

```csharp
{ var x = vector.X * rate; var y = vector.Y * rate; var z = vector.Z * rate; return new XYZ(x, y, z); }
```

### Group 59 — `OffsetPoint` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.OffsetPoint` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.OffsetPoint` — `Reinforcement/_utils.cs`

```csharp
{ var directUnit = UnitVector(direction); var offsetVect = MultiplyVector(directUnit, offset); return AddXyz(point, offsetVect); }
```

### Group 60 — `SetParaInt` (2 copies)

- `Ara3D.RevitSampleBrowser.AreaReinCurve.CS._Utils.SetParaInt` — `AreaReinCurve/_utils.cs`
- `Ara3D.RevitSampleBrowser.AreaReinParameters.CS._Utils.SetParaInt` — `AreaReinParameters/_utils.cs`

```csharp
{ var findPara = FindParaByName(elem.Parameters, paraName); if (findPara == null || findPara.IsReadOnly) return false; findPara.Set(value); return true; }
```

### Group 61 — `SetParaInt` (2 copies)

- `Ara3D.RevitSampleBrowser.AreaReinCurve.CS._Utils.SetParaInt` — `AreaReinCurve/_utils.cs`
- `Ara3D.RevitSampleBrowser.AreaReinParameters.CS._Utils.SetParaInt` — `AreaReinParameters/_utils.cs`

```csharp
{ var para = elem.get_Parameter(paraIndex); if (para == null || para.IsReadOnly) return false; para.Set(value); return true; }
```

### Group 62 — `SetParaInt` (2 copies)

- `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS._Utils.SetParaInt` — `CreateComplexAreaRein/_utils.cs`
- `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS._Utils.SetParaInt` — `CreateSimpleAreaRein/_utils.cs`

```csharp
{ var para = elem.get_Parameter(paraIndex); if (para == null) return false; para.Set(value); return true; }
```

### Group 63 — `SetParaNullId` (2 copies)

- `Ara3D.RevitSampleBrowser.AreaReinCurve.CS._Utils.SetParaNullId` — `AreaReinCurve/_utils.cs`
- `Ara3D.RevitSampleBrowser.AreaReinParameters.CS._Utils.SetParaNullId` — `AreaReinParameters/_utils.cs`

```csharp
{ if (para.IsReadOnly) return false; para.Set(ElementId.InvalidElementId); return true; }
```

### Group 64 — `SetParaNullId` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SetParaNullId` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SetParaNullId` — `Reinforcement/_utils.cs`

```csharp
{ var id = ElementId.InvalidElementId; if (!parameter.IsReadOnly) { parameter.Set(id); return true; } return false; }
```

### Group 65 — `SetParameter` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SetParameter` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SetParameter` — `Reinforcement/_utils.cs`

```csharp
{ var parameters = element.Parameters; var findParameter = FindParameter(parameters, parameterName); if (null == findParameter) return false; if (!findParameter.IsReadOnly) { var parameterType = findParameter.StorageType; if (StorageType.Integer != parameterType) throw new Exception("The types of...
```

### Group 66 — `SetParameter` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SetParameter` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SetParameter` — `Reinforcement/_utils.cs`

```csharp
{ var parameters = element.Parameters; var findParameter = FindParameter(parameters, parameterName); if (null == findParameter) return false; if (!findParameter.IsReadOnly) { var parameterType = findParameter.StorageType; if (StorageType.Double != parameterType) throw new Exception("The types of ...
```

### Group 67 — `SetParameter` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SetParameter` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SetParameter` — `Reinforcement/_utils.cs`

```csharp
{ var parameters = element.Parameters; var findParameter = FindParameter(parameters, parameterName); if (null == findParameter) return false; if (!findParameter.IsReadOnly) { var parameterType = findParameter.StorageType; if (StorageType.String != parameterType) throw new Exception("The types of ...
```

### Group 68 — `SetParameter` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SetParameter` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SetParameter` — `Reinforcement/_utils.cs`

```csharp
{ var parameters = element.Parameters; var findParameter = FindParameter(parameters, parameterName); if (null == findParameter) return false; if (!findParameter.IsReadOnly) { var parameterType = findParameter.StorageType; if (StorageType.ElementId != parameterType) throw new Exception("The types ...
```

### Group 69 — `SetParameter` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SetParameter` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SetParameter` — `Reinforcement/_utils.cs`

```csharp
{ var parameter = element.get_Parameter(paraIndex); if (null == parameter) return false; if (!parameter.IsReadOnly) { var parameterType = parameter.StorageType; if (StorageType.Integer != parameterType) throw new Exception("The types of value and parameter are different!"); parameter.Set(value); ...
```

### Group 70 — `SetParameter` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SetParameter` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SetParameter` — `Reinforcement/_utils.cs`

```csharp
{ var parameter = element.get_Parameter(paraIndex); if (null == parameter) return false; if (!parameter.IsReadOnly) { var parameterType = parameter.StorageType; if (StorageType.Double != parameterType) throw new Exception("The types of value and parameter are different!"); parameter.Set(value); r...
```

### Group 71 — `SetParameter` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SetParameter` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SetParameter` — `Reinforcement/_utils.cs`

```csharp
{ var parameter = element.get_Parameter(paraIndex); if (null == parameter) return false; if (!parameter.IsReadOnly) { var parameterType = parameter.StorageType; if (StorageType.String != parameterType) throw new Exception("The types of value and parameter are different!"); parameter.Set(value); r...
```

### Group 72 — `SetParameter` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SetParameter` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SetParameter` — `Reinforcement/_utils.cs`

```csharp
{ var parameter = element.get_Parameter(paraIndex); if (null == parameter) return false; if (!parameter.IsReadOnly) { var parameterType = parameter.StorageType; if (StorageType.ElementId != parameterType) throw new Exception("The types of value and parameter are different!"); parameter.Set(value)...
```

### Group 73 — `SubXyz` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.SubXyz` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.SubXyz` — `Reinforcement/_utils.cs`

```csharp
{ var x = p1.X - p2.X; var y = p1.Y - p2.Y; var z = p1.Z - p2.Z; return new XYZ(x, y, z); }
```

### Group 74 — `TransformPoint` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.TransformPoint` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.TransformPoint` — `Reinforcement/_utils.cs`

```csharp
{ var x = point.X; var y = point.Y; var z = point.Z; var b0 = transform.get_Basis(0); var b1 = transform.get_Basis(1); var b2 = transform.get_Basis(2); var origin = transform.Origin; var xTemp = x * b0.X + y * b1.X + z * b2.X + origin.X; var yTemp = x * b0.Y + y * b1.Y + z * b2.Y + origin.Y; var ...
```

### Group 75 — `UnitVector` (2 copies)

- `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS._Utils.UnitVector` — `RebarContainerAnyShapeType/_utils.cs`
- `Ara3D.RevitSampleBrowser.Reinforcement.CS._Utils.UnitVector` — `Reinforcement/_utils.cs`

```csharp
{ var length = GetLength(vector); var x = vector.X / length; var y = vector.Y / length; var z = vector.Z / length; return new XYZ(x, y, z); }
```

### Group 76 — `Vector4` (2 copies)

- `Ara3D.RevitSampleBrowser.NewOpenings.CS.Vector4.Vector4` — `NewOpenings/_utils.cs`
- `Ara3D.RevitSampleBrowser.Truss.CS.Vector4.Vector4` — `Truss/_utils.cs`

```csharp
{ X = v.X; Y = v.Y; Z = v.Z; }
```

### Group 77 — `XyzToString` (2 copies)

- `Ara3D.RevitSampleBrowser.DirectionCalculation.CS._Utils.XyzToString` — `DirectionCalculation/_utils.cs`
- `Ara3D.RevitSampleBrowser.FindReferencesByDirection.CS._Utils.XyzToString` — `FindReferencesByDirection/_utils.cs`

```csharp
=> $"( {point.X}, {point.Y}, {point.Z})";
```

## Near-Duplicate Method Bodies

Method pairs with ≥85% normalized body similarity but different hashes.

| Similarity | Method A | Method B |
|-----------:|----------|----------|
| 99.1% | `Site/_Utils.AllowElement` | `Site/TopographySurfaceSelectionFilter.AllowElement` |
| 99.1% | `Site/SubRegionSelectionFilter.AllowElement` | `Site/TopographySurfaceSelectionFilter.AllowElement` |
| 98.6% | `CurtainWallGrid/_Utils.CovertToApi` | `ImportExport/_Utils.CovertToApi` |
| 98.5% | `CurtainWallGrid/_Utils.CovertFromApi` | `ImportExport/_Utils.CovertFromApi` |
| 98.3% | `RebarContainerAnyShapeType/_Utils.SetParameter` | `RebarContainerAnyShapeType/_Utils.SetParameter` |
| 98.3% | `RebarContainerAnyShapeType/_Utils.SetParameter` | `Reinforcement/_Utils.SetParameter` |
| 98.3% | `Reinforcement/_Utils.SetParameter` | `Reinforcement/_Utils.SetParameter` |
| 98.0% | `AreaReinCurve/_Utils.FindParaByName` | `CreateComplexAreaRein/_Utils.FindParaByName` |
| 98.0% | `AreaReinCurve/_Utils.FindParaByName` | `CreateSimpleAreaRein/_Utils.FindParaByName` |
| 97.8% | `FamilyCreation/_Utils.GetVector` | `ReferencePlane/_Utils.GetVector` |
| 96.2% | `DirectionCalculation/_Utils.XyzToString` | `Journaling/_Utils.XyzToString` |
| 96.2% | `FindReferencesByDirection/_Utils.XyzToString` | `Journaling/_Utils.XyzToString` |
| 94.4% | `CurtainWallGrid/_Utils.ImperialDutRatio` | `GridCreation/_Utils.ImperialDutRatio` |
| 94.4% | `CurtainWallGrid/_Utils.ImperialDutRatio` | `ImportExport/_Utils.ImperialDutRatio` |
| 92.6% | `CurtainWallGrid/Vector4.Length` | `Truss/Vector4.Length` |
| 92.6% | `NewOpenings/Vector4.Length` | `Truss/Vector4.Length` |
| 92.6% | `ShaftHolePuncher/Vector4.Length` | `Truss/Vector4.Length` |
| 92.5% | `FamilyCreation/_Utils.IsVerticalEdge` | `ReferencePlane/_Utils.IsVerticalEdge` |
| 91.8% | `AreaReinCurve/_Utils.SetParaInt` | `CreateComplexAreaRein/_Utils.SetParaInt` |
| 91.8% | `AreaReinCurve/_Utils.SetParaInt` | `CreateSimpleAreaRein/_Utils.SetParaInt` |
| 91.8% | `AreaReinParameters/_Utils.SetParaInt` | `CreateComplexAreaRein/_Utils.SetParaInt` |
| 91.8% | `AreaReinParameters/_Utils.SetParaInt` | `CreateSimpleAreaRein/_Utils.SetParaInt` |
| 87.0% | `CurtainWallGrid/Vector4.CrossProduct` | `CurtainWallGrid/Vector4.CrossProduct` |
| 87.0% | `CurtainWallGrid/Vector4.CrossProduct` | `NewOpenings/Vector4.CrossProduct` |
| 87.0% | `CurtainWallGrid/Vector4.CrossProduct` | `ShaftHolePuncher/Vector4.CrossProduct` |
| 87.0% | `CurtainWallGrid/Vector4.CrossProduct` | `Truss/Vector4.CrossProduct` |
| 87.0% | `NewOpenings/Vector4.CrossProduct` | `NewOpenings/Vector4.CrossProduct` |
| 87.0% | `NewOpenings/Vector4.CrossProduct` | `ShaftHolePuncher/Vector4.CrossProduct` |
| 87.0% | `NewOpenings/Vector4.CrossProduct` | `Truss/Vector4.CrossProduct` |
| 87.0% | `ShaftHolePuncher/Vector4.CrossProduct` | `ShaftHolePuncher/Vector4.CrossProduct` |
| 87.0% | `ShaftHolePuncher/Vector4.CrossProduct` | `Truss/Vector4.CrossProduct` |
| 87.0% | `Truss/Vector4.CrossProduct` | `Truss/Vector4.CrossProduct` |
| 86.7% | `CurtainWallGrid/Vector4.DotProduct` | `CurtainWallGrid/Vector4.DotProduct` |
| 86.7% | `CurtainWallGrid/Vector4.DotProduct` | `NewOpenings/Vector4.DotProduct` |
| 86.7% | `CurtainWallGrid/Vector4.DotProduct` | `ShaftHolePuncher/Vector4.DotProduct` |
| 86.7% | `CurtainWallGrid/Vector4.DotProduct` | `Truss/Vector4.DotProduct` |
| 86.7% | `NewOpenings/Vector4.DotProduct` | `NewOpenings/Vector4.DotProduct` |
| 86.7% | `NewOpenings/Vector4.DotProduct` | `ShaftHolePuncher/Vector4.DotProduct` |
| 86.7% | `NewOpenings/Vector4.DotProduct` | `Truss/Vector4.DotProduct` |
| 86.7% | `ShaftHolePuncher/Vector4.DotProduct` | `ShaftHolePuncher/Vector4.DotProduct` |
| 86.7% | `ShaftHolePuncher/Vector4.DotProduct` | `Truss/Vector4.DotProduct` |
| 86.7% | `Truss/Vector4.DotProduct` | `Truss/Vector4.DotProduct` |
| 85.5% | `CurtainWallGrid/Matrix4.Matrix4` | `CurtainWallGrid/Matrix4.Matrix4` |
| 85.5% | `CurtainWallGrid/Matrix4.Matrix4` | `NewOpenings/Matrix4.Matrix4` |
| 85.5% | `CurtainWallGrid/Matrix4.Matrix4` | `ShaftHolePuncher/Matrix4.Matrix4` |
| 85.5% | `CurtainWallGrid/Matrix4.Matrix4` | `Truss/Matrix4.Matrix4` |
| 85.5% | `NewOpenings/Matrix4.Matrix4` | `NewOpenings/Matrix4.Matrix4` |
| 85.5% | `NewOpenings/Matrix4.Matrix4` | `ShaftHolePuncher/Matrix4.Matrix4` |
| 85.5% | `NewOpenings/Matrix4.Matrix4` | `Truss/Matrix4.Matrix4` |
| 85.5% | `ShaftHolePuncher/Matrix4.Matrix4` | `ShaftHolePuncher/Matrix4.Matrix4` |
| 85.5% | `ShaftHolePuncher/Matrix4.Matrix4` | `Truss/Matrix4.Matrix4` |
| 85.5% | `Truss/Matrix4.Matrix4` | `Truss/Matrix4.Matrix4` |
| 85.2% | `CurtainWallGrid/Matrix4.Inverse` | `NewOpenings/Matrix4.Inverse` |
| 85.2% | `NewOpenings/Matrix4.Inverse` | `ShaftHolePuncher/Matrix4.Inverse` |
| 85.2% | `NewOpenings/Matrix4.Inverse` | `Truss/Matrix4.Inverse` |

## Usage: Multiple Commands vs Single Command

### Used by multiple commands/callers (59)

| Method | Namespace | Callers |
|--------|-----------|---------|
| `_Utils.GetSelectedObject` | `Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS` | ContextualAnalyticalModel/AddAssociation.cs (AddAssociation)<br>ContextualAnalyticalModel/CreateAreaLoadWithRefPoint.cs (CreateAreaLoadWithRefPoint)<br>ContextualAnalyticalModel/CustomAreaLoad.cs (CreateCustomAreaLoad)<br>ContextualAnalyticalModel/CustomLineLoad.cs (CreateCustomLineLoad)<br>ContextualAnalyticalModel/CustomPointLoad.cs (CreateCustomPointLoad)<br>ContextualAnalyticalModel/RemoveAssociation.cs (RemoveAssociation) |
| `_Utils.MyMessageBox` | `Ara3D.RevitSampleBrowser.ViewPrinter.CS` | ViewPrinter/Command.cs (Command)<br>ViewPrinter/PrintMgrForm.cs<br>ViewPrinter/PrintSTP.cs<br>ViewPrinter/PrintSetupForm.cs<br>ViewPrinter/ViewSheets.cs |
| `_Utils.ValidateLabel` | `Ara3D.RevitSampleBrowser.GridCreation.CS` | GridCreation/CreateOrthogonalGridsForm.cs<br>GridCreation/CreateRadialAndArcGridsForm.cs<br>GridCreation/CreateWithSelectedCurvesForm.cs |
| `_Utils.GetNumberOfRowsAndColumns` | `Ara3D.RevitSampleBrowser.PanelSchedule.CS` | PanelSchedule/CSVTranslator.cs<br>PanelSchedule/HTMLTranslator.cs<br>PanelSchedule/Translator.cs |
| `_Utils.ReplaceIllegalCharacters` | `Ara3D.RevitSampleBrowser.PanelSchedule.CS` | PanelSchedule/CSVTranslator.cs<br>PanelSchedule/HTMLTranslator.cs<br>PanelSchedule/Translator.cs |
| `_Utils.GetLength` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/BeamGeometrySupport.cs<br>RebarContainerAnyShapeType/ColumnGeometrySupport.cs<br>RebarContainerAnyShapeType/GeometrySupport.cs |
| `_Utils.OffsetPoint` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/BeamGeometrySupport.cs<br>RebarContainerAnyShapeType/ColumnGeometrySupport.cs<br>RebarContainerAnyShapeType/GeometrySupport.cs |
| `_Utils.GetLength` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/BeamGeometrySupport.cs<br>Reinforcement/ColumnGeometrySupport.cs<br>Reinforcement/GeometrySupport.cs |
| `_Utils.OffsetPoint` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/BeamGeometrySupport.cs<br>Reinforcement/ColumnGeometrySupport.cs<br>Reinforcement/GeometrySupport.cs |
| `_Utils.ShareParameterExists` | `Ara3D.RevitSampleBrowser.RoomSchedule.CS` | RoomSchedule/EventsReactor.cs<br>RoomSchedule/RoomScheduleForm.cs<br>RoomSchedule/RoomsData.cs |
| `_Utils.ConvertValueDocumentUnits` | `Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS` | RoutingPreferenceTools/RoutingPreferenceAnalysis/Analyzer.cs<br>RoutingPreferenceTools/RoutingPreferenceAnalysis/MainWindow.xaml.cs<br>RoutingPreferenceTools/RoutingPreferenceBuilder/RoutingPreferenceBuilder.cs |
| `_Utils.MepWarning` | `Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS` | RoutingPreferenceTools/RoutingPreferenceAnalysis/Command.cs (Command)<br>RoutingPreferenceTools/RoutingPreferenceBuilder/CommandReadPreferences.cs (CommandReadPreferences)<br>RoutingPreferenceTools/RoutingPreferenceBuilder/CommandWritePreferences.cs (CommandWritePreferences) |
| `_Utils.PipesDefinedWarning` | `Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS` | RoutingPreferenceTools/RoutingPreferenceAnalysis/Command.cs (Command)<br>RoutingPreferenceTools/RoutingPreferenceBuilder/CommandReadPreferences.cs (CommandReadPreferences)<br>RoutingPreferenceTools/RoutingPreferenceBuilder/CommandWritePreferences.cs (CommandWritePreferences) |
| `_Utils.ValidateMep` | `Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS` | RoutingPreferenceTools/RoutingPreferenceAnalysis/Command.cs (Command)<br>RoutingPreferenceTools/RoutingPreferenceBuilder/CommandReadPreferences.cs (CommandReadPreferences)<br>RoutingPreferenceTools/RoutingPreferenceBuilder/CommandWritePreferences.cs (CommandWritePreferences) |
| `_Utils.ValidatePipesDefined` | `Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS` | RoutingPreferenceTools/RoutingPreferenceAnalysis/Command.cs (Command)<br>RoutingPreferenceTools/RoutingPreferenceBuilder/CommandReadPreferences.cs (CommandReadPreferences)<br>RoutingPreferenceTools/RoutingPreferenceBuilder/CommandWritePreferences.cs (CommandWritePreferences) |
| `_Utils.GetAverageElevation` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteAddRetainingPondCommand.cs (SiteAddRetainingPondCommand)<br>Site/SiteMoveRegionAndPointsCommand.cs (SiteMoveRegionAndPointsCommand)<br>Site/SiteNormalizeTerrainInRegionCommand.cs (SiteNormalizeTerrainInRegionCommand) |
| `_Utils.GetNonBoundaryPoints` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteAddRetainingPondCommand.cs (SiteAddRetainingPondCommand)<br>Site/SiteDeleteRegionAndPointsCommand.cs (SiteDeleteRegionAndPointsCommand)<br>Site/SiteMoveRegionAndPointsCommand.cs (SiteMoveRegionAndPointsCommand) |
| `_Utils.GetTopographySurfaceHost` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteDeleteRegionAndPointsCommand.cs (SiteDeleteRegionAndPointsCommand)<br>Site/SiteMoveRegionAndPointsCommand.cs (SiteMoveRegionAndPointsCommand)<br>Site/SiteNormalizeTerrainInRegionCommand.cs (SiteNormalizeTerrainInRegionCommand) |
| `_Utils.PickSubregion` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteDeleteRegionAndPointsCommand.cs (SiteDeleteRegionAndPointsCommand)<br>Site/SiteMoveRegionAndPointsCommand.cs (SiteMoveRegionAndPointsCommand)<br>Site/SiteNormalizeTerrainInRegionCommand.cs (SiteNormalizeTerrainInRegionCommand) |
| `_Utils.ShowMessage` | `Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS` | AddSpaceAndZone/DataManager.cs<br>AddSpaceAndZone/SpaceManager.cs |
| `_Utils.GetSelectedModelGroup` | `Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS` | AttachedDetailGroup/AttachedDetailGroupHideAllCommand.cs (AttachedDetailGroupHideAllCommand)<br>AttachedDetailGroup/AttachedDetailGroupShowAllCommand.cs (AttachedDetailGroupShowAllCommand) |
| `_Utils.CreateRectangleLoop` | `Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS` | ContextualAnalyticalModel/CreateAreaLoadWithRefPoint.cs (CreateAreaLoadWithRefPoint)<br>ContextualAnalyticalModel/CustomAreaLoad.cs (CreateCustomAreaLoad) |
| `_Utils.CovertFromApi` | `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS` | CurtainWallGrid/GridDrawing.cs<br>CurtainWallGrid/WallDrawing.cs |
| `_Utils.GetUnitLabel` | `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS` | CurtainWallGrid/GridDrawing.cs<br>CurtainWallGrid/WallDrawing.cs |
| `_Utils.GetRevitDbEventName` | `Ara3D.RevitSampleBrowser.Events.CS` | Events/CancelSave/LogManager.cs<br>Events/EventsMonitor/LogManager.cs |
| `_Utils.TitleNoExt` | `Ara3D.RevitSampleBrowser.Events.CS` | Events/CancelSave/LogManager.cs<br>Events/SelectionChanged/SelectionChangedEventArgsExtension.cs |
| `_Utils.NewGuid` | `Ara3D.RevitSampleBrowser.ExtensibleStorageManager.CS` | ExtensibleStorageManager/ExtensibleStorageManager/User/StorageCommand.cs<br>ExtensibleStorageManager/ExtensibleStorageManager/User/UICommand.xaml.cs |
| `_Utils.MetricToImperial` | `Ara3D.RevitSampleBrowser.FamilyCreation.CS` | FamilyCreation/WindowWizard/DoubleHungWinCreation.cs<br>FamilyCreation/WindowWizard/ValidateWindowParameter.cs |
| `_Utils.SelectConnection` | `Ara3D.RevitSampleBrowser.GenericStructuralConnection.CS` | GenericStructuralConnection/DetailedStructuralConnectionOps.cs<br>GenericStructuralConnection/GenericStructuralConnectionOps.cs |
| `_Utils.SelectConnectionElements` | `Ara3D.RevitSampleBrowser.GenericStructuralConnection.CS` | GenericStructuralConnection/DetailedStructuralConnectionOps.cs<br>GenericStructuralConnection/GenericStructuralConnectionOps.cs |
| `_Utils.ExecuteCreateBRepCommand` | `Ara3D.RevitSampleBrowser.GeometryAPI.CS` | GeometryAPI/UpdateExternallyTaggedBRep/CreateBRep.cs (CreateBRep)<br>GeometryAPI/UpdateExternallyTaggedBRep/UpdateBRep.cs (UpdateBRep) |
| `_Utils.CovertFromApi` | `Ara3D.RevitSampleBrowser.GridCreation.CS` | GridCreation/CreateOrthogonalGridsForm.cs<br>GridCreation/CreateRadialAndArcGridsForm.cs |
| `_Utils.CovertToApi` | `Ara3D.RevitSampleBrowser.GridCreation.CS` | GridCreation/CreateOrthogonalGridsForm.cs<br>GridCreation/CreateRadialAndArcGridsForm.cs |
| `_Utils.ValidateCoord` | `Ara3D.RevitSampleBrowser.GridCreation.CS` | GridCreation/CreateOrthogonalGridsForm.cs<br>GridCreation/CreateRadialAndArcGridsForm.cs |
| `_Utils.ValidateLabels` | `Ara3D.RevitSampleBrowser.GridCreation.CS` | GridCreation/CreateOrthogonalGridsForm.cs<br>GridCreation/CreateRadialAndArcGridsForm.cs |
| `_Utils.ValidateLength` | `Ara3D.RevitSampleBrowser.GridCreation.CS` | GridCreation/CreateOrthogonalGridsForm.cs<br>GridCreation/CreateRadialAndArcGridsForm.cs |
| `_Utils.ValidateNumbers` | `Ara3D.RevitSampleBrowser.GridCreation.CS` | GridCreation/CreateOrthogonalGridsForm.cs<br>GridCreation/CreateRadialAndArcGridsForm.cs |
| `_Utils.CovertFromApi` | `Ara3D.RevitSampleBrowser.LevelsProperty.CS` | LevelsProperty/Command.cs (Command)<br>LevelsProperty/LevelsForm.cs |
| `_Utils.FlipHandAndFace` | `Ara3D.RevitSampleBrowser.ModelessDialog.CS` | ModelessDialog/ModelessForm_ExternalEvent/RequestHandler.cs<br>ModelessDialog/ModelessForm_IdlingEvent/RequestHandler.cs |
| `_Utils.MakeLeft` | `Ara3D.RevitSampleBrowser.ModelessDialog.CS` | ModelessDialog/ModelessForm_ExternalEvent/RequestHandler.cs<br>ModelessDialog/ModelessForm_IdlingEvent/RequestHandler.cs |
| `_Utils.MakeRight` | `Ara3D.RevitSampleBrowser.ModelessDialog.CS` | ModelessDialog/ModelessForm_ExternalEvent/RequestHandler.cs<br>ModelessDialog/ModelessForm_IdlingEvent/RequestHandler.cs |
| `_Utils.ModifySelectedDoors` | `Ara3D.RevitSampleBrowser.ModelessDialog.CS` | ModelessDialog/ModelessForm_ExternalEvent/RequestHandler.cs<br>ModelessDialog/ModelessForm_IdlingEvent/RequestHandler.cs |
| `_Utils.TurnIn` | `Ara3D.RevitSampleBrowser.ModelessDialog.CS` | ModelessDialog/ModelessForm_ExternalEvent/RequestHandler.cs<br>ModelessDialog/ModelessForm_IdlingEvent/RequestHandler.cs |
| `_Utils.TurnOut` | `Ara3D.RevitSampleBrowser.ModelessDialog.CS` | ModelessDialog/ModelessForm_ExternalEvent/RequestHandler.cs<br>ModelessDialog/ModelessForm_IdlingEvent/RequestHandler.cs |
| `_Utils.GetXElement` | `Ara3D.RevitSampleBrowser.PointCloudEngine.CS` | PointCloudEngine/PointCloudAccessBase.cs<br>PointCloudEngine/PointCloudCellStorage.cs |
| `_Utils.GetXElement` | `Ara3D.RevitSampleBrowser.PointCloudEngine.CS` | PointCloudEngine/PointCloudAccessBase.cs<br>PointCloudEngine/PointCloudCellStorage.cs |
| `_Utils.IsEqual` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/GeomData.cs<br>RebarContainerAnyShapeType/GeometrySupport.cs |
| `_Utils.IsEqual` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/GeomData.cs<br>RebarContainerAnyShapeType/GeometrySupport.cs |
| `_Utils.SubXyz` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/BeamFramReinMaker.cs<br>RebarContainerAnyShapeType/GeometrySupport.cs |
| `_Utils.IsEqual` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/GeomData.cs<br>Reinforcement/GeometrySupport.cs |
| `_Utils.IsEqual` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/GeomData.cs<br>Reinforcement/GeometrySupport.cs |
| `_Utils.SubXyz` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/BeamFramReinMaker.cs<br>Reinforcement/GeometrySupport.cs |
| `_Utils.GetProperty` | `Ara3D.RevitSampleBrowser.RoomSchedule.CS` | RoomSchedule/EventsReactor.cs<br>RoomSchedule/RoomsData.cs |
| `_Utils.ConvertValueToFeet` | `Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS` | RoutingPreferenceTools/RoutingPreferenceAnalysis/Analyzer.cs<br>RoutingPreferenceTools/RoutingPreferenceBuilder/RoutingPreferenceBuilder.cs |
| `_Utils.ChangeSubregionAndPointsElevation` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteLowerTerrainInRegionCommand.cs (SiteLowerTerrainInRegionCommand)<br>Site/SiteRaiseTerrainInRegionCommand.cs (SiteRaiseTerrainInRegionCommand) |
| `_Utils.GetPointsFromSubregionExact` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteMoveRegionAndPointsCommand.cs (SiteMoveRegionAndPointsCommand)<br>Site/SiteNormalizeTerrainInRegionCommand.cs (SiteNormalizeTerrainInRegionCommand) |
| `_Utils.PickPointNearToposurface` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteAddRetainingPondCommand.cs (SiteAddRetainingPondCommand)<br>Site/SiteMoveRegionAndPointsCommand.cs (SiteMoveRegionAndPointsCommand) |
| `_Utils.ShowWarningMessageBox` | `Ara3D.RevitSampleBrowser.ViewTemplateCreation.CS` | ViewTemplateCreation/Command.cs (Command)<br>ViewTemplateCreation/ViewTemplateCreationForm.cs |
| `_Utils.CalculateControlPoints` | `Ara3D.RevitSampleBrowser.WinderStairs.CS` | WinderStairs/Command.cs (Command)<br>WinderStairs/WinderUpdater.cs |

### Used by exactly one command/caller (216)

| Method | Namespace | Caller |
|--------|-----------|--------|
| `_Utils.Rescale` | `Ara3D.RevitSampleBrowser.AllViews.CS` | AllViews/AllViews.cs (Command) |
| `IsPaintedFaceSelectionFilter.AllowElement` | `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS` | AppearanceAssetEditing/Application.cs (Application) |
| `IsPaintedFaceSelectionFilter.AllowReference` | `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS` | AppearanceAssetEditing/Application.cs (Application) |
| `_Utils.ColorsEqual` | `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS` | AppearanceAssetEditing/Application.cs (Application) |
| `_Utils.LimitValue` | `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS` | AppearanceAssetEditing/Application.cs (Application) |
| `_Utils.Log` | `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS` | AppearanceAssetEditing/Application.cs (Application) |
| `_Utils.IsRectangular` | `Ara3D.RevitSampleBrowser.AreaReinCurve.CS` | AreaReinCurve/AreaReinCurve.cs (Command) |
| `_Utils.IsRectangular` | `Ara3D.RevitSampleBrowser.AreaReinCurve.CS` | AreaReinCurve/AreaReinCurve.cs (Command) |
| `_Utils.IsVertical` | `Ara3D.RevitSampleBrowser.AreaReinCurve.CS` | AreaReinCurve/AreaReinCurve.cs (Command) |
| `_Utils.SetParaInt` | `Ara3D.RevitSampleBrowser.AreaReinCurve.CS` | AreaReinCurve/AreaReinCurve.cs (Command) |
| `_Utils.SetParaInt` | `Ara3D.RevitSampleBrowser.AreaReinCurve.CS` | AreaReinCurve/AreaReinCurve.cs (Command) |
| `_Utils.SetParaNullId` | `Ara3D.RevitSampleBrowser.AreaReinCurve.CS` | AreaReinCurve/AreaReinCurve.cs (Command) |
| `_Utils.FindParaByName` | `Ara3D.RevitSampleBrowser.AreaReinParameters.CS` | AreaReinParameters/WallAreaReinData.cs |
| `_Utils.HideAllAttachedDetailGroups` | `Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS` | AttachedDetailGroup/AttachedDetailGroupHideAllCommand.cs (AttachedDetailGroupHideAllCommand) |
| `_Utils.ShowAllAttachedDetailGroups` | `Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS` | AttachedDetailGroup/AttachedDetailGroupShowAllCommand.cs (AttachedDetailGroupShowAllCommand) |
| `_Utils.CompareReferencesWithContext` | `Ara3D.RevitSampleBrowser.AvoidObstruction.CS` | AvoidObstruction/Detector.cs |
| `_Utils.CopyParameters` | `Ara3D.RevitSampleBrowser.AvoidObstruction.CS` | AvoidObstruction/Resolver.cs |
| `_Utils.FindConnectedTo` | `Ara3D.RevitSampleBrowser.AvoidObstruction.CS` | AvoidObstruction/Resolver.cs |
| `_Utils.FindConnector` | `Ara3D.RevitSampleBrowser.AvoidObstruction.CS` | AvoidObstruction/Resolver.cs |
| `_Utils.FindReferenceInList` | `Ara3D.RevitSampleBrowser.AvoidObstruction.CS` | AvoidObstruction/Section.cs |
| `_Utils.GetClosestSectionsToOrigin` | `Ara3D.RevitSampleBrowser.AvoidObstruction.CS` | AvoidObstruction/Resolver.cs |
| `_Utils.InReferenceArray` | `Ara3D.RevitSampleBrowser.AvoidObstruction.CS` | AvoidObstruction/Detector.cs |
| `_Utils.PerpendicularDirs` | `Ara3D.RevitSampleBrowser.AvoidObstruction.CS` | AvoidObstruction/Resolver.cs |
| `_Utils.GetTargetFolderUrn` | `Ara3D.RevitSampleBrowser.CloudAPISample.CS` | CloudAPISample/Samples/Migration/MigrationToBim360.cs |
| `_Utils.GenerateRandomColor` | `Ara3D.RevitSampleBrowser.ColorFill.CS` | ColorFill/ColorFillMgr.cs |
| `_Utils.GetSelectedObjects` | `Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS` | ContextualAnalyticalModel/AddCustomAssociation.cs (AddCustomAssociation) |
| `_Utils.Add` | `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS` | CreateBeamSystem/Line2D.cs |
| `_Utils.Dot` | `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS` | CreateBeamSystem/Line2D.cs |
| `_Utils.FindIntersection` | `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS` | CreateBeamSystem/Line2D.cs |
| `_Utils.GetMax` | `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS` | CreateBeamSystem/Line2D.cs |
| `_Utils.GetMin` | `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS` | CreateBeamSystem/Line2D.cs |
| `_Utils.InSameHorizontalPlane` | `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS` | CreateBeamSystem/BeamSystemData.cs |
| `_Utils.Multiply` | `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS` | CreateBeamSystem/Line2D.cs |
| `_Utils.SortLines` | `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS` | CreateBeamSystem/BeamSystemData.cs |
| `_Utils.Subtract` | `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS` | CreateBeamSystem/Line2D.cs |
| `_Utils.FindParaByName` | `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS` | CreateComplexAreaRein/AreaReinData.cs |
| `_Utils.GetFaces` | `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS` | CreateComplexAreaRein/GeomHelper.cs |
| `_Utils.GetLength` | `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS` | CreateComplexAreaRein/GeomHelper.cs |
| `_Utils.GetScaledLine` | `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS` | CreateComplexAreaRein/GeomHelper.cs |
| `_Utils.GetXyParallelLine` | `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS` | CreateComplexAreaRein/GeomHelper.cs |
| `_Utils.IsHorizontalFace` | `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS` | CreateComplexAreaRein/GeomHelper.cs |
| `_Utils.IsRectangular` | `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS` | CreateComplexAreaRein/GeomHelper.cs |
| `_Utils.IsRectangular` | `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS` | CreateComplexAreaRein/GeomHelper.cs |
| `_Utils.SetParaInt` | `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS` | CreateComplexAreaRein/AreaReinData.cs |
| `_Utils.FindParaByName` | `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS` | CreateSimpleAreaRein/AreaReinData.cs |
| `_Utils.GetFaces` | `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS` | CreateSimpleAreaRein/GeomHelper.cs |
| `_Utils.IsHorizontalFace` | `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS` | CreateSimpleAreaRein/GeomHelper.cs |
| `_Utils.IsParallel` | `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS` | CreateSimpleAreaRein/GeomHelper.cs |
| `_Utils.IsRectangular` | `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS` | CreateSimpleAreaRein/GeomHelper.cs |
| `_Utils.SetParaInt` | `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS` | CreateSimpleAreaRein/AreaReinData.cs |
| `_Utils.LoadTrianglesData` | `Ara3D.RevitSampleBrowser.CreateTrianglesTopography.CS` | CreateTrianglesTopography/Command.cs (Command) |
| `_Utils.FindDirection` | `Ara3D.RevitSampleBrowser.CreateViewSection.CS` | CreateViewSection/Command.cs (Command, CreateDraftingView) |
| `_Utils.FindFloorViewDirection` | `Ara3D.RevitSampleBrowser.CreateViewSection.CS` | CreateViewSection/Command.cs (Command, CreateDraftingView) |
| `_Utils.FindMidPoint` | `Ara3D.RevitSampleBrowser.CreateViewSection.CS` | CreateViewSection/Command.cs (Command, CreateDraftingView) |
| `_Utils.FindMiddlePoint` | `Ara3D.RevitSampleBrowser.CreateViewSection.CS` | CreateViewSection/Command.cs (Command, CreateDraftingView) |
| `_Utils.FindRightDirection` | `Ara3D.RevitSampleBrowser.CreateViewSection.CS` | CreateViewSection/Command.cs (Command, CreateDraftingView) |
| `_Utils.FindUpDirection` | `Ara3D.RevitSampleBrowser.CreateViewSection.CS` | CreateViewSection/Command.cs (Command, CreateDraftingView) |
| `_Utils.AreFaceNormalsPaired` | `Ara3D.RevitSampleBrowser.CurtainSystem.CS` | CurtainSystem/CurtainSystem/MassChecker.cs |
| `_Utils.ComputeCrossProduct` | `Ara3D.RevitSampleBrowser.CurtainSystem.CS` | CurtainSystem/CurtainSystem/MassChecker.cs |
| `_Utils.IsLinesParallel` | `Ara3D.RevitSampleBrowser.CurtainSystem.CS` | CurtainSystem/CurtainSystem/MassChecker.cs |
| `_Utils.IsLinesParallel` | `Ara3D.RevitSampleBrowser.CurtainSystem.CS` | CurtainSystem/CurtainSystem/MassChecker.cs |
| `_Utils.CalculateAlignedCurve` | `Ara3D.RevitSampleBrowser.DatumsModification.CS` | DatumsModification/DatumsModificationCmd.cs (DatumStyleModification, DatumAlignment, DatumPropagation) |
| `_Utils.CalculateLeader` | `Ara3D.RevitSampleBrowser.DatumsModification.CS` | DatumsModification/DatumsModificationCmd.cs (DatumStyleModification, DatumAlignment, DatumPropagation) |
| `_Utils.FormatParameterValue` | `Ara3D.RevitSampleBrowser.DeckProperties.CS` | DeckProperties/Command.cs (Command) |
| `_Utils.ComputeLeaderPosition` | `Ara3D.RevitSampleBrowser.DimensionLeaderEnd.CS` | DimensionLeaderEnd/Command.cs (MoveHorizontally, MoveToPickedPoint) |
| `_Utils.CloseFile` | `Ara3D.RevitSampleBrowser.DirectionCalculation.CS` | DirectionCalculation/FindSouthFacing.cs |
| `_Utils.CollectExteriorWalls` | `Ara3D.RevitSampleBrowser.DirectionCalculation.CS` | DirectionCalculation/FindSouthFacingWalls.cs |
| `_Utils.CollectWindows` | `Ara3D.RevitSampleBrowser.DirectionCalculation.CS` | DirectionCalculation/FindSouthFacingWindows.cs |
| `_Utils.GetExteriorWallDirection` | `Ara3D.RevitSampleBrowser.DirectionCalculation.CS` | DirectionCalculation/FindSouthFacingWalls.cs |
| `_Utils.GetWindowDirection` | `Ara3D.RevitSampleBrowser.DirectionCalculation.CS` | DirectionCalculation/FindSouthFacingWindows.cs |
| `_Utils.IsSouthFacing` | `Ara3D.RevitSampleBrowser.DirectionCalculation.CS` | DirectionCalculation/FindSouthFacing.cs |
| `_Utils.TransformByProjectLocation` | `Ara3D.RevitSampleBrowser.DirectionCalculation.CS` | DirectionCalculation/FindSouthFacing.cs |
| `_Utils.Write` | `Ara3D.RevitSampleBrowser.DirectionCalculation.CS` | DirectionCalculation/FindSouthFacing.cs |
| `_Utils.ShowDialog` | `Ara3D.RevitSampleBrowser.DisableCommand.CS` | DisableCommand/Application.cs (Application) |
| `_Utils.GetApplicationResourcesPath` | `Ara3D.RevitSampleBrowser.DockableDialogs.CS` | DockableDialogs/Application/ThisApplication.cs (ThisApplication) |
| `_Utils.GetAssemblyFullName` | `Ara3D.RevitSampleBrowser.DockableDialogs.CS` | DockableDialogs/Application/ThisApplication.cs (ThisApplication) |
| `_Utils.Message` | `Ara3D.RevitSampleBrowser.DockableDialogs.CS` | DockableDialogs/MainPage/MainPage.xaml.cs |
| `_Utils.AddSharedParameters` | `Ara3D.RevitSampleBrowser.DoorSwing.CS` | DoorSwing/DoorSwingData.cs |
| `_Utils.DuplicateDraftingViews` | `Ara3D.RevitSampleBrowser.DuplicateViews.CS` | DuplicateViews/DuplicateAcrossDocumentsCommand.cs (DuplicateAcrossDocumentsCommand) |
| `_Utils.DuplicateSchedules` | `Ara3D.RevitSampleBrowser.DuplicateViews.CS` | DuplicateViews/DuplicateAcrossDocumentsCommand.cs (DuplicateAcrossDocumentsCommand) |
| `_Utils.CreateElementFilterFromFilterRules` | `Ara3D.RevitSampleBrowser.ElementFilterSample.CS` | ElementFilterSample/ViewFiltersForm.cs |
| `_Utils.CreateFilterRuleBuilder` | `Ara3D.RevitSampleBrowser.ElementFilterSample.CS` | ElementFilterSample/ViewFiltersForm.cs |
| `_Utils.GetConjunctionOfFilterRulesFromElementFilter` | `Ara3D.RevitSampleBrowser.ElementFilterSample.CS` | ElementFilterSample/ViewFiltersForm.cs |
| `_Utils.GetViewFilters` | `Ara3D.RevitSampleBrowser.ElementFilterSample.CS` | ElementFilterSample/ViewFiltersForm.cs |
| `_Utils.GetRevitUiEventName` | `Ara3D.RevitSampleBrowser.Events.CS` | Events/SelectionChanged/SelectionChangedEventArgsExtension.cs |
| `_Utils.FindSampleSettings` | `Ara3D.RevitSampleBrowser.ExportPDFSettingsSample.CS` | ExportPDFSettingsSample/Application.cs (ExportPdfSettingsSampleApplication, CreateExportPdfSettingsCommand, ModifyExportPdfSettingsCommand, AddNamingRuleCommand, MofidyNamingRuleCommand, DeleteNamingRuleCommand) |
| `_Utils.DoesAnyStorageExist` | `Ara3D.RevitSampleBrowser.ExtensibleStorageUtility.CS` | ExtensibleStorageUtility/DeleteStorage.cs (DeleteStorage) |
| `_Utils.GetElementsWithAllSchemas` | `Ara3D.RevitSampleBrowser.ExtensibleStorageUtility.CS` | ExtensibleStorageUtility/QueryStorage.cs (QueryStorage) |
| `_Utils.IsValidDbKey` | `Ara3D.RevitSampleBrowser.ExternalResourceServer.CS` | ExternalResourceServer/ExternalResourceDBServer/SampleExternalResourceDBServer.cs |
| `_Utils.LoadKeynoteEntries` | `Ara3D.RevitSampleBrowser.ExternalResourceServer.CS` | ExternalResourceServer/ExternalResourceDBServer/SampleExternalResourceDBServer.cs |
| `_Utils.FormatNumber` | `Ara3D.RevitSampleBrowser.FabricationPartLayout.CS` | FabricationPartLayout/PartInfo.cs (PartInfo) |
| `_Utils.IsACoupling` | `Ara3D.RevitSampleBrowser.FabricationPartLayout.CS` | FabricationPartLayout/PartRenumber.cs (PartRenumber) |
| `_Utils.IsADuct` | `Ara3D.RevitSampleBrowser.FabricationPartLayout.CS` | FabricationPartLayout/PartRenumber.cs (PartRenumber) |
| `_Utils.IsAPipe` | `Ara3D.RevitSampleBrowser.FabricationPartLayout.CS` | FabricationPartLayout/PartRenumber.cs (PartRenumber) |
| `_Utils.GetExtrusionFace` | `Ara3D.RevitSampleBrowser.FamilyCreation.CS` | FamilyCreation/WindowWizard/DoubleHungWinCreation.cs |
| `_Utils.GetWallFace` | `Ara3D.RevitSampleBrowser.FamilyCreation.CS` | FamilyCreation/WindowWizard/DoubleHungWinCreation.cs |
| `_Utils.ImperialToMetric` | `Ara3D.RevitSampleBrowser.FamilyCreation.CS` | FamilyCreation/WindowWizard/ValidateWindowParameter.cs |
| `_Utils.ResolveDirectoryPath` | `Ara3D.RevitSampleBrowser.FamilyParametersOrder.CS` | FamilyParametersOrder/SortFamilyFilesParamsForm.cs |
| `_Utils.DrawProfile` | `Ara3D.RevitSampleBrowser.FoundationSlab.CS` | FoundationSlab/FoundationSlabForm.cs |
| `_Utils.GetFloorProfile` | `Ara3D.RevitSampleBrowser.FoundationSlab.CS` | FoundationSlab/SlabData.cs |
| `_Utils.IsPlanarFloor` | `Ara3D.RevitSampleBrowser.FoundationSlab.CS` | FoundationSlab/SlabData.cs |
| `_Utils.CheckTotalNumber` | `Ara3D.RevitSampleBrowser.FrameBuilder.CS` | FrameBuilder/FrameData.cs |
| `_Utils.CreateMatrix` | `Ara3D.RevitSampleBrowser.FrameBuilder.CS` | FrameBuilder/FrameBuilder.cs |
| `_Utils.DuplicateSymbol` | `Ara3D.RevitSampleBrowser.FrameBuilder.CS` | FrameBuilder/CreateFrameForm.cs |
| `_Utils.RefreshListControl` | `Ara3D.RevitSampleBrowser.FrameBuilder.CS` | FrameBuilder/CreateFrameForm.cs |
| `_Utils.CreateNegativeBlock` | `Ara3D.RevitSampleBrowser.FreeFormElement.CS` | FreeFormElement/CreateNegativeBlockCommand.cs (CreateNegativeBlockCommand) |
| `_Utils.FindGenericModelTemplate` | `Ara3D.RevitSampleBrowser.FreeFormElement.CS` | FreeFormElement/CreateNegativeBlockCommand.cs (CreateNegativeBlockCommand) |
| `_Utils.GetTargetSolids` | `Ara3D.RevitSampleBrowser.FreeFormElement.CS` | FreeFormElement/CreateNegativeBlockCommand.cs (CreateNegativeBlockCommand) |
| `_Utils.IsCurveInXyPlane` | `Ara3D.RevitSampleBrowser.FreeFormElement.CS` | FreeFormElement/CreateNegativeBlockCommand.cs (CreateNegativeBlockCommand) |
| `_Utils.SupportsLoopUtilities` | `Ara3D.RevitSampleBrowser.FreeFormElement.CS` | FreeFormElement/CreateNegativeBlockCommand.cs (CreateNegativeBlockCommand) |
| `_Utils.CreateFloor` | `Ara3D.RevitSampleBrowser.GenerateFloor.CS` | GenerateFloor/Command.cs (Command) |
| `_Utils.FindMinMax` | `Ara3D.RevitSampleBrowser.GenerateFloor.CS` | GenerateFloor/Data.cs |
| `_Utils.WallFilter` | `Ara3D.RevitSampleBrowser.GenerateFloor.CS` | GenerateFloor/Data.cs |
| `_Utils.CreateExternallyTaggedPodium` | `Ara3D.RevitSampleBrowser.GeometryAPI.CS` | GeometryAPI/UpdateExternallyTaggedBRep/UpdateBRep.cs (UpdateBRep) |
| `_Utils.DockablePanesExist` | `Ara3D.RevitSampleBrowser.GetSetDefaultTypes.CS` | GetSetDefaultTypes/ThisCommand.cs (ThisCommand) |
| `_Utils.ValidateDegrees` | `Ara3D.RevitSampleBrowser.GridCreation.CS` | GridCreation/CreateRadialAndArcGridsForm.cs |
| `_Utils.SelectMarker` | `Ara3D.RevitSampleBrowser.InCanvasControlAPI.CS` | InCanvasControlAPI/IssueSelectHandler.cs |
| `_Utils.CreateGraphicsData` | `Ara3D.RevitSampleBrowser.InPlaceMembers.CS` | InPlaceMembers/Command.cs (Command) |
| `_Utils.GetSpecialData` | `Ara3D.RevitSampleBrowser.Journaling.CS` | Journaling/Journaling.cs |
| `_Utils.StringToXyz` | `Ara3D.RevitSampleBrowser.Journaling.CS` | Journaling/Journaling.cs |
| `_Utils.XyzToString` | `Ara3D.RevitSampleBrowser.Journaling.CS` | Journaling/Journaling.cs |
| `_Utils.FindLoadCaseByName` | `Ara3D.RevitSampleBrowser.Loads.CS` | Loads/LoadCombinationDeal.cs |
| `_Utils.FindUsageByName` | `Ara3D.RevitSampleBrowser.Loads.CS` | Loads/LoadCombinationDeal.cs |
| `_Utils.MakeArc` | `Ara3D.RevitSampleBrowser.Massing.CS` | Massing/NewForm/Command.cs (MakeExtrusionForm, MakeCapForm, MakeRevolveForm, MakeSweptBlendForm, MakeLoftForm) |
| `_Utils.MakeLine` | `Ara3D.RevitSampleBrowser.Massing.CS` | Massing/NewForm/Command.cs (MakeExtrusionForm, MakeCapForm, MakeRevolveForm, MakeSweptBlendForm, MakeLoftForm) |
| `_Utils.MakeLine` | `Ara3D.RevitSampleBrowser.Massing.CS` | Massing/NewForm/Command.cs (MakeExtrusionForm, MakeCapForm, MakeRevolveForm, MakeSweptBlendForm, MakeLoftForm) |
| `_Utils.GetElementById` | `Ara3D.RevitSampleBrowser.ModelLines.CS` | ModelLines/ModelLines.cs |
| `_Utils.GetSketchPlaneById` | `Ara3D.RevitSampleBrowser.ModelLines.CS` | ModelLines/ModelLines.cs |
| `_Utils.ParseReference` | `Ara3D.RevitSampleBrowser.MultiThreading.WorkThread.CS` | MultiThreading/WorkThread/FaceAnalyzer.cs |
| `_Utils.GetOrCreateDef` | `Ara3D.RevitSampleBrowser.MultiplanarRebar.CS` | MultiplanarRebar/CorbelFrame.cs |
| `_Utils.GetOrCreateDef` | `Ara3D.RevitSampleBrowser.MultiplanarRebar.CS` | MultiplanarRebar/CorbelFrame.cs |
| `_Utils.GetOrCreateDef` | `Ara3D.RevitSampleBrowser.MultiplanarRebar.CS` | MultiplanarRebar/CorbelFrame.cs |
| `_Utils.ParseCorbelGeometry` | `Ara3D.RevitSampleBrowser.MultiplanarRebar.CS` | MultiplanarRebar/CorbelFrame.cs |
| `_Utils.GetSpatialFieldManager` | `Ara3D.RevitSampleBrowser.NetworkPressureLossReport.CS` | NetworkPressureLossReport/AVFViewer.cs |
| `_Utils.ProjectToTrackball` | `Ara3D.RevitSampleBrowser.NewHostedSweep.CS` | NewHostedSweep/Geom/TrackBall.cs |
| `_Utils.IsEqual` | `Ara3D.RevitSampleBrowser.NewRebar.CS` | NewRebar/Geom/GeometrySupport.cs |
| `_Utils.IsEqual` | `Ara3D.RevitSampleBrowser.NewRebar.CS` | NewRebar/Geom/GeometrySupport.cs |
| `_Utils.IsOppositeDirection` | `Ara3D.RevitSampleBrowser.NewRebar.CS` | NewRebar/Geom/GeometrySupport.cs |
| `_Utils.IsSameDirection` | `Ara3D.RevitSampleBrowser.NewRebar.CS` | NewRebar/Geom/GeometrySupport.cs |
| `_Utils.IsVertical` | `Ara3D.RevitSampleBrowser.NewRebar.CS` | NewRebar/Geom/GeometrySupport.cs |
| `_Utils.OffsetPoint` | `Ara3D.RevitSampleBrowser.NewRebar.CS` | NewRebar/Geom/GeometrySupport.cs |
| `_Utils.SubXyz` | `Ara3D.RevitSampleBrowser.NewRebar.CS` | NewRebar/Geom/GeometrySupport.cs |
| `_Utils.TransformPoint` | `Ara3D.RevitSampleBrowser.NewRebar.CS` | NewRebar/Geom/GeometrySupport.cs |
| `_Utils.ParseBracketedId` | `Ara3D.RevitSampleBrowser.NewRoof.CS` | NewRoof/RoofForms/CustomTypeConverter.cs |
| `_Utils.Translate` | `Ara3D.RevitSampleBrowser.NewRoof.CS` | NewRoof/RoofForms/FootPrintRoofWrapper.cs |
| `_Utils.FormatParameterLine` | `Ara3D.RevitSampleBrowser.ParameterUtils.CS` | ParameterUtils/Command.cs (Command) |
| `_Utils.CollectRuleInfo` | `Ara3D.RevitSampleBrowser.PerformanceAdviserControl.CS` | PerformanceAdviserControl/UICommand.cs (UiCommand) |
| `_Utils.CheckSelectedElement` | `Ara3D.RevitSampleBrowser.PlaceFamilyInstanceByFace.CS` | PlaceFamilyInstanceByFace/FamilyInstanceCreator.cs |
| `_Utils.Project` | `Ara3D.RevitSampleBrowser.PlaceFamilyInstanceByFace.CS` | PlaceFamilyInstanceByFace/FamilyInstanceCreator.cs |
| `_Utils.FindProperFamilySymbol` | `Ara3D.RevitSampleBrowser.PlacementOptions.CS` | PlacementOptions/Command.cs (Command) |
| `_Utils.GetBoolean` | `Ara3D.RevitSampleBrowser.PointCloudEngine.CS` | PointCloudEngine/PointCloudCellStorage.cs |
| `_Utils.GetColor` | `Ara3D.RevitSampleBrowser.PointCloudEngine.CS` | PointCloudEngine/PointCloudCellStorage.cs |
| `_Utils.GetColorXElement` | `Ara3D.RevitSampleBrowser.PointCloudEngine.CS` | PointCloudEngine/PointCloudCellStorage.cs |
| `_Utils.GetDouble` | `Ara3D.RevitSampleBrowser.PointCloudEngine.CS` | PointCloudEngine/PointCloudAccessBase.cs |
| `_Utils.GetXyz` | `Ara3D.RevitSampleBrowser.PointCloudEngine.CS` | PointCloudEngine/PointCloudCellStorage.cs |
| `_Utils.IsElementBelongsToCircuit` | `Ara3D.RevitSampleBrowser.PowerCircuit.CS` | PowerCircuit/CircuitOperationData.cs |
| `_Utils.ShowErrorMessage` | `Ara3D.RevitSampleBrowser.PowerCircuit.CS` | PowerCircuit/CircuitOperationData.cs |
| `_Utils.VerifyUnusedConnectors` | `Ara3D.RevitSampleBrowser.PowerCircuit.CS` | PowerCircuit/CircuitOperationData.cs |
| `_Utils.AngleStringToDouble` | `Ara3D.RevitSampleBrowser.ProjectInfo.CS` | ProjectInfo/Converters/AngleConverter.cs |
| `_Utils.DoubleEquals` | `Ara3D.RevitSampleBrowser.ProjectInfo.CS` | ProjectInfo/Wrappers/SiteLocationWrapper.cs |
| `_Utils.DoubleToAngleString` | `Ara3D.RevitSampleBrowser.ProjectInfo.CS` | ProjectInfo/Converters/AngleConverter.cs |
| `_Utils.TimeZoneDoubleToString` | `Ara3D.RevitSampleBrowser.ProjectInfo.CS` | ProjectInfo/Wrappers/SiteLocationWrapper.cs |
| `_Utils.GetHookOrient` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/BeamFramReinMaker.cs |
| `_Utils.IsInRightDir` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/BeamFramReinMaker.cs |
| `_Utils.IsOppositeDirection` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/GeometrySupport.cs |
| `_Utils.IsSameDirection` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/GeometrySupport.cs |
| `_Utils.IsVertical` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/GeometrySupport.cs |
| `_Utils.TransformPoint` | `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS` | RebarContainerAnyShapeType/GeometrySupport.cs |
| `_Utils.AddSharedTestParameter` | `Ara3D.RevitSampleBrowser.RebarFreeForm.CS` | RebarFreeForm/AddSharedParams.cs (AddSharedParams) |
| `_Utils.GetOffsetFromConstraintAtTarget` | `Ara3D.RevitSampleBrowser.RebarFreeForm.CS` | RebarFreeForm/RebarUpdateServer.cs |
| `_Utils.Distribute` | `Ara3D.RevitSampleBrowser.ReferencePlane.CS` | ReferencePlane/ReferencePlaneMgr.cs |
| `_Utils.GetBottomFace` | `Ara3D.RevitSampleBrowser.ReferencePlane.CS` | ReferencePlane/ReferencePlaneMgr.cs |
| `_Utils.GetDistance` | `Ara3D.RevitSampleBrowser.ReferencePlane.CS` | ReferencePlane/ReferencePlaneMgr.cs |
| `_Utils.GetLength` | `Ara3D.RevitSampleBrowser.ReferencePlane.CS` | ReferencePlane/ReferencePlaneMgr.cs |
| `_Utils.GetHookOrient` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/BeamFramReinMaker.cs |
| `_Utils.IsInRightDir` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/BeamFramReinMaker.cs |
| `_Utils.IsOppositeDirection` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/GeometrySupport.cs |
| `_Utils.IsSameDirection` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/GeometrySupport.cs |
| `_Utils.IsVertical` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/GeometrySupport.cs |
| `_Utils.TransformPoint` | `Ara3D.RevitSampleBrowser.Reinforcement.CS` | Reinforcement/GeometrySupport.cs |
| `_Utils.GetRoomAndSpaceElements` | `Ara3D.RevitSampleBrowser.RoofsRooms.CS` | RoofsRooms/Command.cs (Command) |
| `_Utils.FindParameter` | `Ara3D.RevitSampleBrowser.RotateFramingObjects.CS` | RotateFramingObjects/RotateFramingObjects.cs (RotateFramingObjects) |
| `_Utils.CreateAndAddSchedules` | `Ara3D.RevitSampleBrowser.ScheduleCreation.CS` | ScheduleCreation/Command.cs (Command) |
| `_Utils.MoveElement` | `Ara3D.RevitSampleBrowser.Selections.CS` | Selections/SelectionManager.cs |
| `_Utils.ConvertFrom` | `Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS` | SharedCoordinateSystem/CoordinateSystemDataForm.cs |
| `_Utils.ConvertTo` | `Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS` | SharedCoordinateSystem/CoordinateSystemDataForm.cs |
| `_Utils.DealPrecision` | `Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS` | SharedCoordinateSystem/CoordinateSystemData.cs |
| `_Utils.DoubleToString` | `Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS` | SharedCoordinateSystem/CoordinateSystemDataForm.cs |
| `_Utils.StringToDouble` | `Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS` | SharedCoordinateSystem/CoordinateSystemDataForm.cs |
| `_Utils.MakeFromViewportClick` | `Ara3D.RevitSampleBrowser.SheetToView3D.CS` | SheetToView3D/SheetToView3D.cs (Command) |
| `_Utils.GeneratePondPointsSurrounding` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteAddRetainingPondCommand.cs (SiteAddRetainingPondCommand) |
| `_Utils.GetCenterOf` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteMoveRegionAndPointsCommand.cs (SiteMoveRegionAndPointsCommand) |
| `_Utils.MoveXyzToElevation` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteMoveRegionAndPointsCommand.cs (SiteMoveRegionAndPointsCommand) |
| `_Utils.PickTopographySurface` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/SiteAddRetainingPondCommand.cs (SiteAddRetainingPondCommand) |
| `_Utils.SetIconsForPushButtonData` | `Ara3D.RevitSampleBrowser.Site.CS` | Site/Application.cs (Application) |
| `_Utils.TryGetDemoSolids` | `Ara3D.RevitSampleBrowser.SolidSolidCut.CS` | SolidSolidCut/Command.cs (Cut, Uncut) |
| `_Utils.AddDataRow` | `Ara3D.RevitSampleBrowser.SpotDimension.CS` | SpotDimension/SpotDimensionParams.cs |
| `_Utils.CreateTable` | `Ara3D.RevitSampleBrowser.SpotDimension.CS` | SpotDimension/SpotDimensionParams.cs |
| `_Utils.FormatFractionalInches` | `Ara3D.RevitSampleBrowser.SpotDimension.CS` | SpotDimension/SpotDimensionParams.cs |
| `_Utils.FindLongestEndpointConnection` | `Ara3D.RevitSampleBrowser.StairsAutomation.CS` | StairsAutomation/LandingComponents/StairsRectangleLandingComponent.cs |
| `_Utils.FindTargetLevels` | `Ara3D.RevitSampleBrowser.StairsAutomation.CS` | StairsAutomation/StairsAutomationUtility.cs |
| `_Utils.ProjectCurveToElevation` | `Ara3D.RevitSampleBrowser.StairsAutomation.CS` | StairsAutomation/LandingComponents/StairsRectangleLandingComponent.cs |
| `_Utils.TransformCurve` | `Ara3D.RevitSampleBrowser.StairsAutomation.CS` | StairsAutomation/RunComponents/TransformedStairsComponent.cs |
| `_Utils.TransformCurves` | `Ara3D.RevitSampleBrowser.StairsAutomation.CS` | StairsAutomation/RunComponents/TransformedStairsComponent.cs |
| `_Utils.FindFamilySymbol` | `Ara3D.RevitSampleBrowser.StructSample.CS` | StructSample/Command.cs (Command) |
| `_Utils.AddTagSymbolByCategory` | `Ara3D.RevitSampleBrowser.TagBeam.CS` | TagBeam/TagBeamData.cs |
| `_Utils.GetSelectedBeams` | `Ara3D.RevitSampleBrowser.TagBeam.CS` | TagBeam/TagBeamData.cs |
| `_Utils.TryGetFirstToposolidTypeAndLevel` | `Ara3D.RevitSampleBrowser.Toposolid.CS` | Toposolid/Command.cs (ToposolidCreation, ToposolidFromDwg, ContourSettingCreation, ContourSettingModification, ToposolidFromSurface, SsePointVisibility, SplitToposolid, SimplifyToposolid) |
| `_Utils.ExtractSystemFromConnectors` | `Ara3D.RevitSampleBrowser.TraverseSystem.CS` | TraverseSystem/Command.cs (Command) |
| `_Utils.GetConnectedConnector` | `Ara3D.RevitSampleBrowser.TraverseSystem.CS` | TraverseSystem/TraversalTree.cs |
| `_Utils.ConvertFromBitmap` | `Ara3D.RevitSampleBrowser.UIAPI.CS` | UIAPI/ExternalApplication.cs (ExternalApp, CalcCommand) |
| `_Utils.GetBitmapAsImageSource` | `Ara3D.RevitSampleBrowser.UIAPI.CS` | UIAPI/OptionsDialog/UserControl3.xaml.cs |
| `_Utils.GetDocumentDisplayName` | `Ara3D.RevitSampleBrowser.UIAPI.CS` | UIAPI/PreviewControl/PreviewModel.cs |
| `_Utils.CenterOnScreen` | `Ara3D.RevitSampleBrowser.VersionChecking.CS` | VersionChecking/VersionCheckingForm.cs |
| `_Utils.ShowInformationMessageBox` | `Ara3D.RevitSampleBrowser.ViewTemplateCreation.CS` | ViewTemplateCreation/ViewTemplateCreationForm.cs |
| `_Utils.CalculateMaxStepsCount` | `Ara3D.RevitSampleBrowser.WinderStairs.CS` | WinderStairs/Command.cs (Command) |

### No detected usage (309)

- `_Utils.AllowElement` (public) — `AppearanceAssetEditing/_utils.cs`
- `_Utils.AllowReference` (public) — `AppearanceAssetEditing/_utils.cs`
- `_Utils.DotMatrix` (non-public) — `AreaReinCurve/_utils.cs`
- `_Utils.FindParaByName` (public) — `AreaReinCurve/_utils.cs`
- `_Utils.SubXyz` (non-public) — `AreaReinCurve/_utils.cs`
- `_Utils.SetParaInt` (public) — `AreaReinParameters/_utils.cs`
- `_Utils.SetParaInt` (public) — `AreaReinParameters/_utils.cs`
- `_Utils.SetParaNullId` (public) — `AreaReinParameters/_utils.cs`
- `_Utils.AddNewUnit` (non-public) — `BoundaryConditions/_utils.cs`
- `_Utils._Utils` (non-public) — `BoundaryConditions/_utils.cs`
- `_Utils.CompareDouble` (public) — `CreateBeamSystem/_utils.cs`
- `_Utils.CompareXyz` (public) — `CreateBeamSystem/_utils.cs`
- `_Utils.ConvertTo2DLine` (non-public) — `CreateBeamSystem/_utils.cs`
- `_Utils.AddXyz` (non-public) — `CreateComplexAreaRein/_utils.cs`
- `_Utils.CrossMatrix` (non-public) — `CreateComplexAreaRein/_utils.cs`
- `_Utils.DotMatrix` (non-public) — `CreateComplexAreaRein/_utils.cs`
- `_Utils.GetPoints` (public) — `CreateComplexAreaRein/_utils.cs`
- `_Utils.IsEqual` (non-public) — `CreateComplexAreaRein/_utils.cs`
- `_Utils.IsParallel` (public) — `CreateComplexAreaRein/_utils.cs`
- `_Utils.IsVertical` (public) — `CreateComplexAreaRein/_utils.cs`
- `_Utils.MultiXyz` (non-public) — `CreateComplexAreaRein/_utils.cs`
- `_Utils.SubXyz` (non-public) — `CreateComplexAreaRein/_utils.cs`
- `_Utils.CrossMatrix` (non-public) — `CreateSimpleAreaRein/_utils.cs`
- `_Utils.DotMatrix` (non-public) — `CreateSimpleAreaRein/_utils.cs`
- `_Utils.GetPoints` (public) — `CreateSimpleAreaRein/_utils.cs`
- `_Utils.IsEqual` (non-public) — `CreateSimpleAreaRein/_utils.cs`
- `_Utils.IsVertical` (non-public) — `CreateSimpleAreaRein/_utils.cs`
- `_Utils.SubXyz` (non-public) — `CreateSimpleAreaRein/_utils.cs`
- `_Utils.ParseTrianglesData` (public) — `CreateTrianglesTopography/_utils.cs`
- `_Utils.FindDistance` (public) — `CreateViewSection/_utils.cs`
- `_Utils.FindWallViewDirection` (public) — `CreateViewSection/_utils.cs`
- `Matrix4.Identity` (public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Inverse` (public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Multiply` (public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.RotationInverse` (public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.ScaleInverse` (public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.Transform` (public) — `CurtainWallGrid/_utils.cs`
- `Matrix4.TranslationInverse` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.CrossProduct` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.CrossProduct` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.DotProduct` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.DotProduct` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Length` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Normalize` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Vector4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Vector4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Vector4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Vector4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Vector4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Vector4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Vector4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Vector4.Vector4` (non-public) — `CurtainWallGrid/_utils.cs`
- `Vector4.operator *` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.operator +` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.operator -` (public) — `CurtainWallGrid/_utils.cs`
- `Vector4.operator /` (public) — `CurtainWallGrid/_utils.cs`
- `ViewComparer.Compare` (public) — `CurtainWallGrid/_utils.cs`
- `WallTypeComparer.Compare` (public) — `CurtainWallGrid/_utils.cs`
- `_Utils.CovertToApi` (public) — `CurtainWallGrid/_utils.cs`
- `_Utils.ImperialDutRatio` (non-public) — `CurtainWallGrid/_utils.cs`
- `_Utils.AddTo` (public) — `CustomExporter/_utils.cs`
- `_Utils.DisplayExport` (public) — `CustomExporter/_utils.cs`
- `_Utils.DrawLines` (non-public) — `CustomExporter/_utils.cs`
- `_Utils.GetAppropriatePlane` (non-public) — `CustomExporter/_utils.cs`
- `_Utils.HideAllInView` (non-public) — `CustomExporter/_utils.cs`
- `_Utils.IsExterior` (public) — `DirectionCalculation/_utils.cs`
- `_Utils.XyzToString` (non-public) — `DirectionCalculation/_utils.cs`
- `_Utils.GetAssemblyPath` (public) — `DockableDialogs/_utils.cs`
- `_Utils.AccessOrCreateSharedParameterFile` (non-public) — `DoorSwing/_utils.cs`
- `_Utils.AlreadyAddedSharedParameter` (non-public) — `DoorSwing/_utils.cs`
- `HideAndAcceptDuplicateTypeNamesHandler.OnDuplicateTypeNamesFound` (public) — `DuplicateViews/_utils.cs`
- `HidePasteDuplicateTypesPreprocessor.PreprocessFailures` (public) — `DuplicateViews/_utils.cs`
- `_Utils.DuplicateDetailingAcrossViews` (non-public) — `DuplicateViews/_utils.cs`
- `_Utils.DuplicateElementsAcrossDocuments` (non-public) — `DuplicateViews/_utils.cs`
- `_Utils.GetEvaluatorCriteriaName` (non-public) — `ElementFilterSample/_utils.cs`
- `_Utils.GetEvaluatorCriteriaName` (non-public) — `ElementFilterSample/_utils.cs`
- `_Utils.ReflectToInnerRule` (public) — `ElementFilterSample/_utils.cs`
- `_Utils.ElementsWithStorage` (non-public) — `ExtensibleStorageUtility/_utils.cs`
- `_Utils.GetElementsWithSchema` (non-public) — `ExtensibleStorageUtility/_utils.cs`
- `_Utils.PrintElementInfo` (non-public) — `ExtensibleStorageUtility/_utils.cs`
- `_Utils.CreateRectangularWallCurves` (public) — `ExternalCommand/_utils.cs`
- `_Utils.AverageY` (non-public) — `FamilyCreation/_utils.cs`
- `_Utils.Distribute` (public) — `FamilyCreation/_utils.cs`
- `_Utils.Equal` (public) — `FamilyCreation/_utils.cs`
- `_Utils.GetElementFace` (non-public) — `FamilyCreation/_utils.cs`
- `_Utils.GetExteriorFace` (non-public) — `FamilyCreation/_utils.cs`
- `_Utils.GetInteriorFace` (non-public) — `FamilyCreation/_utils.cs`
- `_Utils.GetSolidFaces` (non-public) — `FamilyCreation/_utils.cs`
- `_Utils.GetVector` (public) — `FamilyCreation/_utils.cs`
- `_Utils.IsVerticalEdge` (public) — `FamilyCreation/_utils.cs`
- `_Utils.FindColumnsWithin` (public) — `FindReferencesByDirection/_utils.cs`
- `_Utils.Get3DView` (public) — `FindReferencesByDirection/_utils.cs`
- `_Utils.GetElevationForRay` (public) — `FindReferencesByDirection/_utils.cs`
- `_Utils.GetNormalToWallAt` (public) — `FindReferencesByDirection/_utils.cs`
- `_Utils.GetTangentAt` (public) — `FindReferencesByDirection/_utils.cs`
- `_Utils.GetWallDeltaAt` (public) — `FindReferencesByDirection/_utils.cs`
- `_Utils.XyzToString` (public) — `FindReferencesByDirection/_utils.cs`
- `_Utils.DrawLine` (non-public) — `FoundationSlab/_utils.cs`
- `_Utils.GetMaxBBox` (non-public) — `FoundationSlab/_utils.cs`
- `_Utils.GetTransformMatrix` (non-public) — `FoundationSlab/_utils.cs`
- `_Utils.CreateReversedCurve` (non-public) — `FreeFormElement/_utils.cs`
- `_Utils.GetContiguousCurvesFromSelectedCurveElements` (public) — `FreeFormElement/_utils.cs`
- `_Utils.CreateDirectShapeWithExternallyTaggedBRep` (public) — `GeometryAPI/_utils.cs`
- `_Utils.GetDefaultElementType` (public) — `GetSetDefaultTypes/_utils.cs`
- `_Utils.ImperialDutRatio` (non-public) — `GridCreation/_utils.cs`
- `_Utils.ShowWarningMessage` (public) — `GridCreation/_utils.cs`
- `_Utils.ValidateDegree` (public) — `GridCreation/_utils.cs`
- `_Utils.ValidateNotNull` (public) — `GridCreation/_utils.cs`
- `_Utils.ValidateNumber` (public) — `GridCreation/_utils.cs`
- `_Utils.CovertFromApi` (public) — `ImportExport/_utils.cs`
- `_Utils.CovertToApi` (public) — `ImportExport/_utils.cs`
- `_Utils.ImperialDutRatio` (non-public) — `ImportExport/_utils.cs`
- `_Utils.CovertToApi` (public) — `LevelsProperty/_utils.cs`
- `_Utils.DoorOperation` (public) — `ModelessDialog/_utils.cs`
- `_Utils.ConstructCorbelFrame` (non-public) — `MultiplanarRebar/_utils.cs`
- `_Utils.GetCommonVertex` (non-public) — `MultiplanarRebar/_utils.cs`
- `_Utils.GetDistance` (non-public) — `MultiplanarRebar/_utils.cs`
- `_Utils.GetElementSolid` (non-public) — `MultiplanarRebar/_utils.cs`
- `_Utils.GetNormalOutside` (non-public) — `MultiplanarRebar/_utils.cs`
- `_Utils.GetSharedParameterFile` (public) — `MultiplanarRebar/_utils.cs`
- `_Utils.IsTrapezoid` (non-public) — `MultiplanarRebar/_utils.cs`
- `Matrix4.Identity` (public) — `NewOpenings/_utils.cs`
- `Matrix4.Inverse` (public) — `NewOpenings/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `NewOpenings/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `NewOpenings/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `NewOpenings/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `NewOpenings/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `NewOpenings/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `NewOpenings/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `NewOpenings/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `NewOpenings/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `NewOpenings/_utils.cs`
- `Matrix4.Multiply` (public) — `NewOpenings/_utils.cs`
- `Matrix4.RotationInverse` (public) — `NewOpenings/_utils.cs`
- `Matrix4.ScaleInverse` (public) — `NewOpenings/_utils.cs`
- `Matrix4.TransForm` (public) — `NewOpenings/_utils.cs`
- `Matrix4.TransLationInverse` (public) — `NewOpenings/_utils.cs`
- `Vector4.CrossProduct` (public) — `NewOpenings/_utils.cs`
- `Vector4.CrossProduct` (public) — `NewOpenings/_utils.cs`
- `Vector4.DotProduct` (public) — `NewOpenings/_utils.cs`
- `Vector4.DotProduct` (public) — `NewOpenings/_utils.cs`
- `Vector4.Length` (public) — `NewOpenings/_utils.cs`
- `Vector4.Normalize` (public) — `NewOpenings/_utils.cs`
- `Vector4.Vector4` (non-public) — `NewOpenings/_utils.cs`
- `Vector4.Vector4` (non-public) — `NewOpenings/_utils.cs`
- `Vector4.Vector4` (non-public) — `NewOpenings/_utils.cs`
- `Vector4.Vector4` (non-public) — `NewOpenings/_utils.cs`
- `Vector4.Vector4` (non-public) — `NewOpenings/_utils.cs`
- `Vector4.Vector4` (non-public) — `NewOpenings/_utils.cs`
- `Vector4.Vector4` (non-public) — `NewOpenings/_utils.cs`
- `Vector4.Vector4` (non-public) — `NewOpenings/_utils.cs`
- `Vector4.Vector4` (non-public) — `NewOpenings/_utils.cs`
- `Vector4.operator *` (public) — `NewOpenings/_utils.cs`
- `Vector4.operator +` (public) — `NewOpenings/_utils.cs`
- `Vector4.operator -` (public) — `NewOpenings/_utils.cs`
- `Vector4.operator /` (public) — `NewOpenings/_utils.cs`
- `_Utils.AddXyz` (public) — `NewRebar/_utils.cs`
- `_Utils.DotMatrix` (non-public) — `NewRebar/_utils.cs`
- `_Utils.GetLength` (public) — `NewRebar/_utils.cs`
- `_Utils.MultiplyVector` (public) — `NewRebar/_utils.cs`
- `_Utils.UnitVector` (public) — `NewRebar/_utils.cs`
- `_Utils.FormatElementIdParameter` (non-public) — `ParameterUtils/_utils.cs`
- `_Utils.InquireGeometry` (public) — `PlaceFamilyInstanceByFace/_utils.cs`
- `_Utils.GetInteger` (public) — `PointCloudEngine/_utils.cs`
- `_Utils.TimeZoneStringToDouble` (public) — `ProjectInfo/_utils.cs`
- `_Utils.AddXyz` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.CrossMatrix` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.DotMatrix` (non-public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.FindParameter` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.MultiplyVector` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.SetParaNullId` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.SetParameter` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.SetParameter` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.SetParameter` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.SetParameter` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.SetParameter` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.SetParameter` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.SetParameter` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.SetParameter` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.UnitVector` (public) — `RebarContainerAnyShapeType/_utils.cs`
- `_Utils.ShareParameterExists` (public) — `RebarFreeForm/_utils.cs`
- `_Utils.Equal` (non-public) — `ReferencePlane/_utils.cs`
- `_Utils.GetVector` (public) — `ReferencePlane/_utils.cs`
- `_Utils.IsVerticalEdge` (non-public) — `ReferencePlane/_utils.cs`
- `_Utils.IsVerticalFace` (non-public) — `ReferencePlane/_utils.cs`
- `_Utils.AddXyz` (public) — `Reinforcement/_utils.cs`
- `_Utils.CrossMatrix` (public) — `Reinforcement/_utils.cs`
- `_Utils.DotMatrix` (non-public) — `Reinforcement/_utils.cs`
- `_Utils.FindParameter` (public) — `Reinforcement/_utils.cs`
- `_Utils.MultiplyVector` (public) — `Reinforcement/_utils.cs`
- `_Utils.SetParaNullId` (public) — `Reinforcement/_utils.cs`
- `_Utils.SetParameter` (public) — `Reinforcement/_utils.cs`
- `_Utils.SetParameter` (public) — `Reinforcement/_utils.cs`
- `_Utils.SetParameter` (public) — `Reinforcement/_utils.cs`
- `_Utils.SetParameter` (public) — `Reinforcement/_utils.cs`
- `_Utils.SetParameter` (public) — `Reinforcement/_utils.cs`
- `_Utils.SetParameter` (public) — `Reinforcement/_utils.cs`
- `_Utils.SetParameter` (public) — `Reinforcement/_utils.cs`
- `_Utils.SetParameter` (public) — `Reinforcement/_utils.cs`
- `_Utils.UnitVector` (public) — `Reinforcement/_utils.cs`
- `_Utils.AddScheduleToNewSheet` (non-public) — `ScheduleCreation/_utils.cs`
- `_Utils.CreateSchedules` (non-public) — `ScheduleCreation/_utils.cs`
- `_Utils.ShouldSkip` (non-public) — `ScheduleCreation/_utils.cs`
- `Matrix4.Identity` (public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Inverse` (public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Multiply` (public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.RotationInverse` (public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.ScaleInverse` (public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.Transform` (public) — `ShaftHolePuncher/_utils.cs`
- `Matrix4.TranslationInverse` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.CrossProduct` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.CrossProduct` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.DotProduct` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.DotProduct` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Length` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Normalize` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Vector4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Vector4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Vector4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Vector4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Vector4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Vector4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Vector4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.Vector4` (non-public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.operator *` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.operator +` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.operator -` (public) — `ShaftHolePuncher/_utils.cs`
- `Vector4.operator /` (public) — `ShaftHolePuncher/_utils.cs`
- `_Utils.DealDecimalNumber` (public) — `SharedCoordinateSystem/_utils.cs`
- `_Utils.ParseFromString` (non-public) — `SharedCoordinateSystem/_utils.cs`
- `_Utils.ValueConversion` (non-public) — `SharedCoordinateSystem/_utils.cs`
- `_Utils.CalculateClickAsModelRay` (public) — `SheetToView3D/_utils.cs`
- `_Utils.Create3DView` (public) — `SheetToView3D/_utils.cs`
- `_Utils.GetViewPlanCutPlane` (public) — `SheetToView3D/_utils.cs`
- `_Utils.GetViewportAtClick` (public) — `SheetToView3D/_utils.cs`
- `_Utils.IsPointInsideCurveLoop` (public) — `SheetToView3D/_utils.cs`
- `_Utils.MakeSheetToModelTransform` (public) — `SheetToView3D/_utils.cs`
- `_Utils.ProjectPointOnPlane` (public) — `SheetToView3D/_utils.cs`
- `SubRegionSelectionFilter.AllowElement` (public) — `Site/_utils.cs`
- `SubRegionSelectionFilter.AllowReference` (public) — `Site/_utils.cs`
- `TopographySurfaceSelectionFilter.AllowElement` (public) — `Site/_utils.cs`
- `TopographySurfaceSelectionFilter.AllowReference` (public) — `Site/_utils.cs`
- `_Utils.AllowElement` (public) — `Site/_utils.cs`
- `_Utils.AllowReference` (public) — `Site/_utils.cs`
- `_Utils.GenerateCircleSurrounding` (non-public) — `Site/_utils.cs`
- `_Utils.GetPointsFromSubregionRough` (public) — `Site/_utils.cs`
- `_Utils.GetSmallIcon` (public) — `Site/_utils.cs`
- `_Utils.GetStdIcon` (public) — `Site/_utils.cs`
- `_Utils.ComputeParameter` (public) — `StairsAutomation/_utils.cs`
- `_Utils.ProjectCurvesToElevation` (public) — `StairsAutomation/_utils.cs`
- `_Utils.CollectPointsFromImportInstance` (public) — `Toposolid/_utils.cs`
- `Matrix4.Identity` (public) — `Truss/_utils.cs`
- `Matrix4.Inverse` (public) — `Truss/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `Truss/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `Truss/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `Truss/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `Truss/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `Truss/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `Truss/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `Truss/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `Truss/_utils.cs`
- `Matrix4.Matrix4` (non-public) — `Truss/_utils.cs`
- `Matrix4.Multiply` (public) — `Truss/_utils.cs`
- `Matrix4.RotationInverse` (public) — `Truss/_utils.cs`
- `Matrix4.ScaleInverse` (public) — `Truss/_utils.cs`
- `Matrix4.Transform` (public) — `Truss/_utils.cs`
- `Matrix4.TranslationInverse` (public) — `Truss/_utils.cs`
- `Vector4.CrossProduct` (public) — `Truss/_utils.cs`
- `Vector4.CrossProduct` (public) — `Truss/_utils.cs`
- `Vector4.DotProduct` (public) — `Truss/_utils.cs`
- `Vector4.DotProduct` (public) — `Truss/_utils.cs`
- `Vector4.Length` (public) — `Truss/_utils.cs`
- `Vector4.Normalize` (public) — `Truss/_utils.cs`
- `Vector4.Vector4` (non-public) — `Truss/_utils.cs`
- `Vector4.Vector4` (non-public) — `Truss/_utils.cs`
- `Vector4.Vector4` (non-public) — `Truss/_utils.cs`
- `Vector4.Vector4` (non-public) — `Truss/_utils.cs`
- `Vector4.Vector4` (non-public) — `Truss/_utils.cs`
- `Vector4.Vector4` (non-public) — `Truss/_utils.cs`
- `Vector4.Vector4` (non-public) — `Truss/_utils.cs`
- `Vector4.Vector4` (non-public) — `Truss/_utils.cs`
- `Vector4.operator *` (public) — `Truss/_utils.cs`
- `Vector4.operator +` (public) — `Truss/_utils.cs`
- `Vector4.operator -` (public) — `Truss/_utils.cs`
- `Vector4.operator /` (public) — `Truss/_utils.cs`
- `_Utils.CalculateControlPoints2` (non-public) — `WinderStairs/_utils.cs`
- `_Utils.CalculateOffset` (non-public) — `WinderStairs/_utils.cs`
- `_Utils.CheckOrientation` (non-public) — `WinderStairs/_utils.cs`
- `_Utils.HasCommonEndPoint` (non-public) — `WinderStairs/_utils.cs`
- `SampleBrowserUtils.GetSourceFolder` (public) — `_utils.cs`
- `SampleBrowserUtils.NormalizeSampleNamespace` (public) — `_utils.cs`

## All _utils.cs Files

### `AddSpaceAndZone/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ShowMessage` | `public static void ShowMessage(string message)` |

### `AllViews/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.AllViews.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `Rescale` | `public static void Rescale(View view, double x, double y)` |

### `AppearanceAssetEditing/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS`
- **Types:** `IsPaintedFaceSelectionFilter`, `_Utils`
- **Method count:** 7

| Type | Method | Signature |
|------|--------|-----------|
| `IsPaintedFaceSelectionFilter` | `AllowElement` | `public bool AllowElement(Element element)` |
| `IsPaintedFaceSelectionFilter` | `AllowReference` | `public bool AllowReference(Reference refer, XYZ point)` |
| `_Utils` | `AllowElement` | `public bool AllowElement(Element element)` |
| `_Utils` | `AllowReference` | `public bool AllowReference(Reference refer, XYZ point)` |
| `_Utils` | `ColorsEqual` | `public static bool ColorsEqual(Color color1, Color color2)` |
| `_Utils` | `LimitValue` | `public static int LimitValue(int value)` |
| `_Utils` | `Log` | `public static void Log(string msg)` |

### `AreaReinCurve/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.AreaReinCurve.CS`
- **Types:** `_Utils`
- **Method count:** 9

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `DotMatrix` | `private static double DotMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `FindParaByName` | `public static Parameter FindParaByName(ParameterSet paras, string name)` |
| `_Utils` | `IsRectangular` | `public static bool IsRectangular(CurveArray curves)` |
| `_Utils` | `IsRectangular` | `public static bool IsRectangular(IList<Curve> curves)` |
| `_Utils` | `IsVertical` | `public static bool IsVertical(Line line1, Line line2)` |
| `_Utils` | `SetParaInt` | `public static bool SetParaInt(Element elem, string paraName, int value)` |
| `_Utils` | `SetParaInt` | `public static bool SetParaInt(Element elem, BuiltInParameter paraIndex, int value)` |
| `_Utils` | `SetParaNullId` | `public static bool SetParaNullId(Parameter para)` |
| `_Utils` | `SubXyz` | `private static XYZ SubXyz(XYZ p1, XYZ p2)` |

### `AreaReinParameters/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.AreaReinParameters.CS`
- **Types:** `_Utils`
- **Method count:** 4

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FindParaByName` | `public static Parameter FindParaByName(ParameterSet paras, string name)` |
| `_Utils` | `SetParaInt` | `public static bool SetParaInt(Element elem, string paraName, int value)` |
| `_Utils` | `SetParaInt` | `public static bool SetParaInt(Element elem, BuiltInParameter paraIndex, int value)` |
| `_Utils` | `SetParaNullId` | `public static bool SetParaNullId(Parameter para)` |

### `AttachedDetailGroup/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetSelectedModelGroup` | `public static bool GetSelectedModelGroup(UIDocument uiDoc, out Group modelGroup, out string error...` |
| `_Utils` | `HideAllAttachedDetailGroups` | `public static void HideAllAttachedDetailGroups(Group modelGroup, Document doc, View view)` |
| `_Utils` | `ShowAllAttachedDetailGroups` | `public static void ShowAllAttachedDetailGroups(Group modelGroup, Document doc, View view)` |

### `AvoidObstruction/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.AvoidObstruction.CS`
- **Types:** `_Utils`
- **Method count:** 8

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CompareReferencesWithContext` | `public static int CompareReferencesWithContext(ReferenceWithContext a, ReferenceWithContext b)` |
| `_Utils` | `CopyParameters` | `public static void CopyParameters(Pipe source, Pipe target)` |
| `_Utils` | `FindConnectedTo` | `public static Connector FindConnectedTo(Pipe pipe, XYZ conXyz)` |
| `_Utils` | `FindConnector` | `public static Connector FindConnector(Pipe pipe, XYZ conXyz)` |
| `_Utils` | `FindReferenceInList` | `public static ReferenceWithContext FindReferenceInList(List<ReferenceWithContext> arr, ReferenceW...` |
| `_Utils` | `GetClosestSectionsToOrigin` | `public static ReferenceWithContext[] GetClosestSectionsToOrigin(List<ReferenceWithContext> refs)` |
| `_Utils` | `InReferenceArray` | `public static bool InReferenceArray(List<ReferenceWithContext> arr, ReferenceWithContext entry)` |
| `_Utils` | `PerpendicularDirs` | `public static List<XYZ> PerpendicularDirs(XYZ dir, int count)` |

### `BoundaryConditions/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.BoundaryConditions.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddNewUnit` | `private static void AddNewUnit(int precision, double ratio, string unitName, string key)` |
| `_Utils` | `_Utils` | `static _Utils()` |

### `CloudAPISample/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.CloudAPISample.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetTargetFolderUrn` | `public static FolderLocation GetTargetFolderUrn(IList<MigrationRule> rules, string directory, str...` |

### `ColorFill/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ColorFill.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GenerateRandomColor` | `public static Color GenerateRandomColor(int seed)` |

### `ContextualAnalyticalModel/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CreateRectangleLoop` | `public static CurveLoop CreateRectangleLoop(XYZ start, XYZ end)` |
| `_Utils` | `GetSelectedObject` | `public static ElementId GetSelectedObject(UIDocument uiDoc, string msg)` |
| `_Utils` | `GetSelectedObjects` | `public static ISet<ElementId> GetSelectedObjects(UIDocument uiDoc, string msg)` |

### `CreateBeamSystem/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.CreateBeamSystem.CS`
- **Types:** `_Utils`
- **Method count:** 12

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `Add` | `public static PointF Add(PointF f1, PointF f2)` |
| `_Utils` | `CompareDouble` | `public static bool CompareDouble(double d1, double d2)` |
| `_Utils` | `CompareXyz` | `public static bool CompareXyz(XYZ pnt1, XYZ pnt2)` |
| `_Utils` | `ConvertTo2DLine` | `private static Line2D ConvertTo2DLine(Line line)` |
| `_Utils` | `Dot` | `public static float Dot(PointF pnt1, PointF pnt2)` |
| `_Utils` | `FindIntersection` | `public static int FindIntersection(float u0, float u1, float v0, float v1, ref float[] w)` |
| `_Utils` | `GetMax` | `public static float GetMax(float f1, float f2)` |
| `_Utils` | `GetMin` | `public static float GetMin(float f1, float f2)` |
| `_Utils` | `InSameHorizontalPlane` | `public static bool InSameHorizontalPlane(List<Line> lines)` |
| `_Utils` | `Multiply` | `public static PointF Multiply(float f, PointF pnt)` |
| `_Utils` | `SortLines` | `public static List<Line> SortLines(List<Line> originLines)` |
| `_Utils` | `Subtract` | `public static PointF Subtract(PointF f1, PointF f2)` |

### `CreateComplexAreaRein/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS`
- **Types:** `_Utils`
- **Method count:** 18

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddXyz` | `private static XYZ AddXyz(XYZ p1, XYZ p2)` |
| `_Utils` | `CrossMatrix` | `private static XYZ CrossMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `DotMatrix` | `private static double DotMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `FindParaByName` | `public static Parameter FindParaByName(ParameterSet paras, string name)` |
| `_Utils` | `GetFaces` | `public static FaceArray GetFaces(Element elem)` |
| `_Utils` | `GetLength` | `public static double GetLength(Line line)` |
| `_Utils` | `GetPoints` | `public static List<XYZ> GetPoints(Face face)` |
| `_Utils` | `GetScaledLine` | `public static Line GetScaledLine(Line inLine, double scale)` |
| `_Utils` | `GetXyParallelLine` | `public static Line GetXyParallelLine(Line inLine, double distance)` |
| `_Utils` | `IsEqual` | `private static bool IsEqual(double d1, double d2)` |
| `_Utils` | `IsHorizontalFace` | `public static bool IsHorizontalFace(Face face)` |
| `_Utils` | `IsParallel` | `public static bool IsParallel(Face face, Line line)` |
| `_Utils` | `IsRectangular` | `public static bool IsRectangular(CurveArray curves)` |
| `_Utils` | `IsRectangular` | `public static bool IsRectangular(IList<Curve> curves)` |
| `_Utils` | `IsVertical` | `public static bool IsVertical(Line line1, Line line2)` |
| `_Utils` | `MultiXyz` | `private static XYZ MultiXyz(XYZ p1, double para)` |
| `_Utils` | `SetParaInt` | `public static bool SetParaInt(Element elem, BuiltInParameter paraIndex, int value)` |
| `_Utils` | `SubXyz` | `private static XYZ SubXyz(XYZ p1, XYZ p2)` |

### `CreateSimpleAreaRein/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS`
- **Types:** `_Utils`
- **Method count:** 12

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CrossMatrix` | `private static XYZ CrossMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `DotMatrix` | `private static double DotMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `FindParaByName` | `public static Parameter FindParaByName(ParameterSet paras, string name)` |
| `_Utils` | `GetFaces` | `public static FaceArray GetFaces(Element elem)` |
| `_Utils` | `GetPoints` | `public static List<XYZ> GetPoints(Face face)` |
| `_Utils` | `IsEqual` | `private static bool IsEqual(double d1, double d2)` |
| `_Utils` | `IsHorizontalFace` | `public static bool IsHorizontalFace(Face face)` |
| `_Utils` | `IsParallel` | `public static bool IsParallel(Face face, Line line)` |
| `_Utils` | `IsRectangular` | `public static bool IsRectangular(IList<Curve> curves)` |
| `_Utils` | `IsVertical` | `private static bool IsVertical(Line line1, Line line2)` |
| `_Utils` | `SetParaInt` | `public static bool SetParaInt(Element elem, BuiltInParameter paraIndex, int value)` |
| `_Utils` | `SubXyz` | `private static XYZ SubXyz(XYZ p1, XYZ p2)` |

### `CreateTrianglesTopography/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.CreateTrianglesTopography.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `LoadTrianglesData` | `public static TrianglesData LoadTrianglesData()` |
| `_Utils` | `ParseTrianglesData` | `public static TrianglesData ParseTrianglesData(string jsonString)` |

### `CreateViewSection/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.CreateViewSection.CS`
- **Types:** `_Utils`
- **Method count:** 8

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FindDirection` | `public static XYZ FindDirection(XYZ first, XYZ second)` |
| `_Utils` | `FindDistance` | `public static double FindDistance(XYZ first, XYZ second)` |
| `_Utils` | `FindFloorViewDirection` | `public static XYZ FindFloorViewDirection(CurveArray curveArray)` |
| `_Utils` | `FindMidPoint` | `public static XYZ FindMidPoint(XYZ first, XYZ second)` |
| `_Utils` | `FindMiddlePoint` | `public static XYZ FindMiddlePoint(CurveArray curveArray)` |
| `_Utils` | `FindRightDirection` | `public static XYZ FindRightDirection(XYZ viewDirection)` |
| `_Utils` | `FindUpDirection` | `public static XYZ FindUpDirection(XYZ viewDirection)` |
| `_Utils` | `FindWallViewDirection` | `public static XYZ FindWallViewDirection(CurveArray curveArray)` |

### `CurtainSystem/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.CurtainSystem.CS`
- **Types:** `_Utils`
- **Method count:** 4

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AreFaceNormalsPaired` | `public static bool AreFaceNormalsPaired(List<Vector4> normals)` |
| `_Utils` | `ComputeCrossProduct` | `public static Vector4 ComputeCrossProduct(Edge edgeA, Edge edgeB)` |
| `_Utils` | `IsLinesParallel` | `public static bool IsLinesParallel(Vector4 vec4A, Vector4 vec4B)` |
| `_Utils` | `IsLinesParallel` | `public static bool IsLinesParallel(Edge edgeA, Edge edgeB)` |

### `CurtainWallGrid/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.CurtainWallGrid.CS`
- **Types:** `Matrix4`, `Vector4`, `ViewComparer`, `WallTypeComparer`, `_Utils`
- **Method count:** 40

| Type | Method | Signature |
|------|--------|-----------|
| `Matrix4` | `Identity` | `public void Identity()` |
| `Matrix4` | `Inverse` | `public Matrix4 Inverse()` |
| `Matrix4` | `Matrix4` | `public Matrix4()` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis)` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 origin)` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis, Vector4 origin)` |
| `Matrix4` | `Matrix4` | `public Matrix4(float scale)` |
| `Matrix4` | `Matrix4` | `return new Matrix4(new Vector4(this[0, 0], this[1, 0], this[2, 0]), new Vector4(this[0, 1], this[...` |
| `Matrix4` | `Matrix4` | `return new Matrix4(new Vector4(-this[3, 0], -this[3, 1], -this[3, 2]))` |
| `Matrix4` | `Matrix4` | `return new Matrix4()` |
| `Matrix4` | `Matrix4` | `return new Matrix4(1 / m_matrix[0, 0])` |
| `Matrix4` | `Multiply` | `public static Matrix4 Multiply(Matrix4 left, Matrix4 right)` |
| `Matrix4` | `RotationInverse` | `public Matrix4 RotationInverse()` |
| `Matrix4` | `ScaleInverse` | `public Matrix4 ScaleInverse()` |
| `Matrix4` | `Transform` | `public Vector4 Transform(Vector4 point)` |
| `Matrix4` | `TranslationInverse` | `public Matrix4 TranslationInverse()` |
| `Vector4` | `CrossProduct` | `public Vector4 CrossProduct(Vector4 v)` |
| `Vector4` | `CrossProduct` | `public static Vector4 CrossProduct(Vector4 va, Vector4 vb)` |
| `Vector4` | `DotProduct` | `public float DotProduct(Vector4 v)` |
| `Vector4` | `DotProduct` | `public static float DotProduct(Vector4 va, Vector4 vb)` |
| `Vector4` | `Length` | `public float Length()` |
| `Vector4` | `Normalize` | `public void Normalize()` |
| `Vector4` | `Vector4` | `public Vector4(float x, float y, float z)` |
| `Vector4` | `Vector4` | `public Vector4(XYZ v)` |
| `Vector4` | `Vector4` | `return new Vector4(va.X + vb.X, va.Y + vb.Y, va.Z + vb.Z)` |
| `Vector4` | `Vector4` | `return new Vector4(va.X - vb.X, va.Y - vb.Y, va.Z - vb.Z)` |
| `Vector4` | `Vector4` | `return new Vector4(v.X * factor, v.Y * factor, v.Z * factor)` |
| `Vector4` | `Vector4` | `return new Vector4(v.X / factor, v.Y / factor, v.Z / factor)` |
| `Vector4` | `Vector4` | `return new Vector4(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X)` |
| `Vector4` | `Vector4` | `return new Vector4(va.Y * vb.Z - va.Z * vb.Y, va.Z * vb.X - va.X * vb.Z, va.X * vb.Y - va.Y * vb.X)` |
| `Vector4` | `operator *` | `public static Vector4 operator *(Vector4 v, float factor)` |
| `Vector4` | `operator +` | `public static Vector4 operator +(Vector4 va, Vector4 vb)` |
| `Vector4` | `operator -` | `public static Vector4 operator -(Vector4 va, Vector4 vb)` |
| `Vector4` | `operator /` | `public static Vector4 operator /(Vector4 v, float factor)` |
| `ViewComparer` | `Compare` | `public int Compare(View x, View y)` |
| `WallTypeComparer` | `Compare` | `public int Compare(WallType x, WallType y)` |
| `_Utils` | `CovertFromApi` | `public static double CovertFromApi(ForgeTypeId to, double value)` |
| `_Utils` | `CovertToApi` | `public static double CovertToApi(double value, ForgeTypeId from)` |
| `_Utils` | `GetUnitLabel` | `public static string GetUnitLabel(ForgeTypeId unitTypeId)` |
| `_Utils` | `ImperialDutRatio` | `private static double ImperialDutRatio(ForgeTypeId unitTypeId)` |

### `CustomExporter/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.CustomExporter.CS`
- **Types:** `_Utils`
- **Method count:** 5

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddTo` | `public static void AddTo(IList<XYZ> to, IList<XYZ> from)` |
| `_Utils` | `DisplayExport` | `public static void DisplayExport(View view, IList<XYZ> points)` |
| `_Utils` | `DrawLines` | `private static void DrawLines(View view, IList<XYZ> points, double tolerance)` |
| `_Utils` | `GetAppropriatePlane` | `private static Plane GetAppropriatePlane(View view)` |
| `_Utils` | `HideAllInView` | `private static void HideAllInView(View view)` |

### `DatumsModification/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.DatumsModification.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CalculateAlignedCurve` | `public static Curve CalculateAlignedCurve(Curve curve, Curve baseCurve, XYZ baseDirect)` |
| `_Utils` | `CalculateLeader` | `public static Leader CalculateLeader(Leader leader, bool addElbow)` |

### `DeckProperties/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.DeckProperties.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FormatParameterValue` | `public static string FormatParameterValue(Parameter parameter)` |

### `DimensionLeaderEnd/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.DimensionLeaderEnd.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ComputeLeaderPosition` | `public static XYZ ComputeLeaderPosition(XYZ direction, XYZ origin, double delta)` |

### `DirectionCalculation/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.DirectionCalculation.CS`
- **Types:** `_Utils`
- **Method count:** 10

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CloseFile` | `public static void CloseFile()` |
| `_Utils` | `CollectExteriorWalls` | `public static IEnumerable<Wall> CollectExteriorWalls(Document document)` |
| `_Utils` | `CollectWindows` | `public static IEnumerable<FamilyInstance> CollectWindows(Document document)` |
| `_Utils` | `GetExteriorWallDirection` | `public static XYZ GetExteriorWallDirection(Wall wall)` |
| `_Utils` | `GetWindowDirection` | `public static XYZ GetWindowDirection(FamilyInstance window)` |
| `_Utils` | `IsExterior` | `public static bool IsExterior(ElementType wallType)` |
| `_Utils` | `IsSouthFacing` | `public static bool IsSouthFacing(XYZ direction)` |
| `_Utils` | `TransformByProjectLocation` | `public static XYZ TransformByProjectLocation(Document document, XYZ direction)` |
| `_Utils` | `Write` | `public static void Write(string label, Curve curve)` |
| `_Utils` | `XyzToString` | `private static string XyzToString(XYZ point)` |

### `DisableCommand/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.DisableCommand.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ShowDialog` | `public static void ShowDialog(string title, string message)` |

### `DockableDialogs/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.DockableDialogs.CS`
- **Types:** `_Utils`
- **Method count:** 4

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetApplicationResourcesPath` | `public static string GetApplicationResourcesPath()` |
| `_Utils` | `GetAssemblyFullName` | `public static string GetAssemblyFullName()` |
| `_Utils` | `GetAssemblyPath` | `public static string GetAssemblyPath()` |
| `_Utils` | `Message` | `public static void Message(string message, int level = 0)` |

### `DoorSwing/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.DoorSwing.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AccessOrCreateSharedParameterFile` | `private static DefinitionFile AccessOrCreateSharedParameterFile(Application app)` |
| `_Utils` | `AddSharedParameters` | `public static void AddSharedParameters(UIApplication app)` |
| `_Utils` | `AlreadyAddedSharedParameter` | `private static bool AlreadyAddedSharedParameter(Document doc, string paraName, BuiltInCategory bo...` |

### `DuplicateViews/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.DuplicateViews.CS`
- **Types:** `HideAndAcceptDuplicateTypeNamesHandler`, `HidePasteDuplicateTypesPreprocessor`, `_Utils`
- **Method count:** 6

| Type | Method | Signature |
|------|--------|-----------|
| `HideAndAcceptDuplicateTypeNamesHandler` | `OnDuplicateTypeNamesFound` | `public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)` |
| `HidePasteDuplicateTypesPreprocessor` | `PreprocessFailures` | `public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)` |
| `_Utils` | `DuplicateDetailingAcrossViews` | `private static int DuplicateDetailingAcrossViews(View fromView, View toView)` |
| `_Utils` | `DuplicateDraftingViews` | `public static int DuplicateDraftingViews(Document fromDocument, IEnumerable<ViewDrafting> views, ...` |
| `_Utils` | `DuplicateElementsAcrossDocuments` | `private static Dictionary<ElementId, ElementId> DuplicateElementsAcrossDocuments(Document fromDoc...` |
| `_Utils` | `DuplicateSchedules` | `public static void DuplicateSchedules(Document fromDocument, IEnumerable<ViewSchedule> views, Doc...` |

### `ElementFilterSample/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ElementFilterSample.CS`
- **Types:** `_Utils`
- **Method count:** 7

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CreateElementFilterFromFilterRules` | `public static ElementFilter CreateElementFilterFromFilterRules(IList<FilterRule> filterRules)` |
| `_Utils` | `CreateFilterRuleBuilder` | `public static FilterRuleBuilder CreateFilterRuleBuilder(BuiltInParameter param, FilterRule rule)` |
| `_Utils` | `GetConjunctionOfFilterRulesFromElementFilter` | `public static IList<FilterRule> GetConjunctionOfFilterRulesFromElementFilter(ElementFilter elemFi...` |
| `_Utils` | `GetEvaluatorCriteriaName` | `private static string GetEvaluatorCriteriaName(FilterStringRuleEvaluator fsre, bool inverted)` |
| `_Utils` | `GetEvaluatorCriteriaName` | `private static string GetEvaluatorCriteriaName(FilterNumericRuleEvaluator fsre, bool inverted)` |
| `_Utils` | `GetViewFilters` | `public static ICollection<ParameterFilterElement> GetViewFilters(Document doc)` |
| `_Utils` | `ReflectToInnerRule` | `public static FilterRule ReflectToInnerRule(FilterRule srcRule, out bool inverted)` |

### `Events/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.Events.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetRevitDbEventName` | `public static string GetRevitDbEventName(Type type)` |
| `_Utils` | `GetRevitUiEventName` | `public static string GetRevitUiEventName(Type type)` |
| `_Utils` | `TitleNoExt` | `public static string TitleNoExt(string orgTitle)` |

### `ExportPDFSettingsSample/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ExportPDFSettingsSample.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FindSampleSettings` | `public static ExportPDFSettings FindSampleSettings(Document doc)` |

### `ExtensibleStorageManager/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ExtensibleStorageManager.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `NewGuid` | `public static Guid NewGuid()` |

### `ExtensibleStorageUtility/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ExtensibleStorageUtility.CS`
- **Types:** `_Utils`
- **Method count:** 5

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `DoesAnyStorageExist` | `public static bool DoesAnyStorageExist(Document doc)` |
| `_Utils` | `ElementsWithStorage` | `private static List<ElementId> ElementsWithStorage(Document doc, Schema schema)` |
| `_Utils` | `GetElementsWithAllSchemas` | `public static string GetElementsWithAllSchemas(Document doc)` |
| `_Utils` | `GetElementsWithSchema` | `private static string GetElementsWithSchema(Document doc, Schema schema)` |
| `_Utils` | `PrintElementInfo` | `private static string PrintElementInfo(ElementId id, Document document)` |

### `ExternalCommand/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ExternalCommand.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CreateRectangularWallCurves` | `public static IList<Curve> CreateRectangularWallCurves(double length = 60, double width = 40)` |

### `ExternalResourceServer/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ExternalResourceServer.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `IsValidDbKey` | `public static bool IsValidDbKey(string key)` |
| `_Utils` | `LoadKeynoteEntries` | `public static void LoadKeynoteEntries(string key, ref KeyBasedTreeEntriesLoadContent kdrlc)` |

### `FabricationPartLayout/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.FabricationPartLayout.CS`
- **Types:** `_Utils`
- **Method count:** 4

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FormatNumber` | `public static string FormatNumber(Document doc, double number, ForgeTypeId specTypeId)` |
| `_Utils` | `IsACoupling` | `public static bool IsACoupling(FabricationPart fabPart)` |
| `_Utils` | `IsADuct` | `public static bool IsADuct(FabricationPart fabPart)` |
| `_Utils` | `IsAPipe` | `public static bool IsAPipe(FabricationPart fabPart)` |

### `FamilyCreation/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.FamilyCreation.CS`
- **Types:** `_Utils`
- **Method count:** 13

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AverageY` | `private static double AverageY(Face face)` |
| `_Utils` | `Distribute` | `public static void Distribute(Mesh mesh, ref XYZ startPoint, ref XYZ endPoint, ref XYZ thirdPnt)` |
| `_Utils` | `Equal` | `public static bool Equal(XYZ vectorA, XYZ vectorB)` |
| `_Utils` | `GetElementFace` | `private static Face GetElementFace(Element element, View view, bool exterior)` |
| `_Utils` | `GetExteriorFace` | `private static Face GetExteriorFace(FaceArray faces)` |
| `_Utils` | `GetExtrusionFace` | `public static Face GetExtrusionFace(Extrusion extrusion, View view, bool extOrInt)` |
| `_Utils` | `GetInteriorFace` | `private static Face GetInteriorFace(FaceArray faces)` |
| `_Utils` | `GetSolidFaces` | `private static FaceArray GetSolidFaces(Element element, View view)` |
| `_Utils` | `GetVector` | `public static XYZ GetVector(XYZ startPoint, XYZ endPoint)` |
| `_Utils` | `GetWallFace` | `public static Face GetWallFace(Wall wall, View view, bool extOrInt)` |
| `_Utils` | `ImperialToMetric` | `public static double ImperialToMetric(double value)` |
| `_Utils` | `IsVerticalEdge` | `public static bool IsVerticalEdge(Edge edge)` |
| `_Utils` | `MetricToImperial` | `public static double MetricToImperial(double value)` |

### `FamilyParametersOrder/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.FamilyParametersOrder.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ResolveDirectoryPath` | `public static string ResolveDirectoryPath(string path)` |

### `FindReferencesByDirection/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.FindReferencesByDirection.CS`
- **Types:** `_Utils`
- **Method count:** 7

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FindColumnsWithin` | `public static void FindColumnsWithin(IList<ReferenceWithContext> references, double proximity, Wa...` |
| `_Utils` | `Get3DView` | `public static View3D Get3DView(Document document, string viewName = "{3D}")` |
| `_Utils` | `GetElevationForRay` | `public static double GetElevationForRay(Document document, Wall wall)` |
| `_Utils` | `GetNormalToWallAt` | `public static XYZ GetNormalToWallAt(Wall wall, LocationCurve curve, double parameter)` |
| `_Utils` | `GetTangentAt` | `public static XYZ GetTangentAt(Curve curve, double parameter)` |
| `_Utils` | `GetWallDeltaAt` | `public static XYZ GetWallDeltaAt(Wall wall, LocationCurve locationCurve, double parameter)` |
| `_Utils` | `XyzToString` | `public static string XyzToString(XYZ point)` |

### `FoundationSlab/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.FoundationSlab.CS`
- **Types:** `_Utils`
- **Method count:** 6

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `DrawLine` | `private static void DrawLine(Pen pen, Graphics graphic, CurveArray curveArray)` |
| `_Utils` | `DrawProfile` | `public static void DrawProfile(Graphics graphic, RectangleF rclip, Collection<RegularSlab> baseSl...` |
| `_Utils` | `GetFloorProfile` | `public static CurveArray GetFloorProfile(Floor floor, UIApplication revit)` |
| `_Utils` | `GetMaxBBox` | `private static RectangleF GetMaxBBox(Collection<RegularSlab> baseSlabList)` |
| `_Utils` | `GetTransformMatrix` | `private static Matrix GetTransformMatrix(RectangleF rclip, RectangleF rBox)` |
| `_Utils` | `IsPlanarFloor` | `public static bool IsPlanarFloor(BoundingBoxXYZ bbXyz, Floor floor, UIApplication revit)` |

### `FrameBuilder/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.FrameBuilder.CS`
- **Types:** `_Utils`
- **Method count:** 4

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CheckTotalNumber` | `public static void CheckTotalNumber(int number)` |
| `_Utils` | `CreateMatrix` | `public static UV[,] CreateMatrix(int xNumber, int yNumber, double distance)` |
| `_Utils` | `DuplicateSymbol` | `public static bool DuplicateSymbol(FrameTypesMgr typesMgr, object symbol)` |
| `_Utils` | `RefreshListControl` | `public static void RefreshListControl(ListControl list, FrameTypesMgr typesMgr)` |

### `FreeFormElement/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.FreeFormElement.CS`
- **Types:** `_Utils`
- **Method count:** 7

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CreateNegativeBlock` | `public static FailureCondition CreateNegativeBlock(Element targetElement, IList<Reference> bounda...` |
| `_Utils` | `CreateReversedCurve` | `private static Curve CreateReversedCurve(Curve orig)` |
| `_Utils` | `FindGenericModelTemplate` | `public static string FindGenericModelTemplate(string familyPath)` |
| `_Utils` | `GetContiguousCurvesFromSelectedCurveElements` | `public static IList<Curve> GetContiguousCurvesFromSelectedCurveElements(Document doc, IList<Refer...` |
| `_Utils` | `GetTargetSolids` | `public static IList<Solid> GetTargetSolids(Element element)` |
| `_Utils` | `IsCurveInXyPlane` | `public static bool IsCurveInXyPlane(Curve curve)` |
| `_Utils` | `SupportsLoopUtilities` | `public static bool SupportsLoopUtilities(Curve curve)` |

### `GenerateFloor/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.GenerateFloor.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CreateFloor` | `public static void CreateFloor(Data data, Document doc)` |
| `_Utils` | `FindMinMax` | `public static void FindMinMax(XYZ point, ref double xMin, ref double xMax, ref double yMin, ref d...` |
| `_Utils` | `WallFilter` | `public static ElementSet WallFilter(ElementSet miscellanea)` |

### `GenericStructuralConnection/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.GenericStructuralConnection.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `SelectConnection` | `public static StructuralConnectionHandler SelectConnection(UIDocument document)` |
| `_Utils` | `SelectConnectionElements` | `public static List<ElementId> SelectConnectionElements(UIDocument document)` |

### `GeometryAPI/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.GeometryAPI.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CreateDirectShapeWithExternallyTaggedBRep` | `public static DirectShape CreateDirectShapeWithExternallyTaggedBRep(Document document, Externally...` |
| `_Utils` | `CreateExternallyTaggedPodium` | `public static ExternallyTaggedBRep CreateExternallyTaggedPodium(double width, double height, doub...` |
| `_Utils` | `ExecuteCreateBRepCommand` | `public static Result ExecuteCreateBRepCommand(Document document)` |

### `GetSetDefaultTypes/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.GetSetDefaultTypes.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `DockablePanesExist` | `public static bool DockablePanesExist()` |
| `_Utils` | `GetDefaultElementType` | `public static ElementType GetDefaultElementType(Document document, ElementTypeGroup group)` |

### `GridCreation/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.GridCreation.CS`
- **Types:** `_Utils`
- **Method count:** 13

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CovertFromApi` | `public static double CovertFromApi(ForgeTypeId to, double value)` |
| `_Utils` | `CovertToApi` | `public static double CovertToApi(double value, ForgeTypeId from)` |
| `_Utils` | `ImperialDutRatio` | `private static double ImperialDutRatio(ForgeTypeId unit)` |
| `_Utils` | `ShowWarningMessage` | `public static void ShowWarningMessage(string message, string caption)` |
| `_Utils` | `ValidateCoord` | `public static bool ValidateCoord(WinFormsControl coordCtrl)` |
| `_Utils` | `ValidateDegree` | `public static bool ValidateDegree(WinFormsControl degreeCtrl)` |
| `_Utils` | `ValidateDegrees` | `public static bool ValidateDegrees(WinFormsControl startDegree, WinFormsControl endDegree)` |
| `_Utils` | `ValidateLabel` | `public static bool ValidateLabel(WinFormsControl labelCtrl, ArrayList allLabels)` |
| `_Utils` | `ValidateLabels` | `public static bool ValidateLabels(WinFormsControl label1Ctrl, WinFormsControl label2Ctrl)` |
| `_Utils` | `ValidateLength` | `public static bool ValidateLength(WinFormsControl lengthCtrl, string typeName, bool canBeZero)` |
| `_Utils` | `ValidateNotNull` | `public static bool ValidateNotNull(WinFormsControl control, string typeName)` |
| `_Utils` | `ValidateNumber` | `public static bool ValidateNumber(WinFormsControl numberCtrl)` |
| `_Utils` | `ValidateNumbers` | `public static bool ValidateNumbers(WinFormsControl number1Ctrl, WinFormsControl number2Ctrl)` |

### `ImportExport/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ImportExport.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CovertFromApi` | `public static double CovertFromApi(ForgeTypeId to, double value)` |
| `_Utils` | `CovertToApi` | `public static double CovertToApi(double value, ForgeTypeId from)` |
| `_Utils` | `ImperialDutRatio` | `private static double ImperialDutRatio(ForgeTypeId unit)` |

### `InCanvasControlAPI/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.InCanvasControlAPI.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `SelectMarker` | `public static void SelectMarker(Document document, int controlIndex)` |

### `InPlaceMembers/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.InPlaceMembers.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CreateGraphicsData` | `public static GraphicsData CreateGraphicsData(AnalyticalElement model)` |

### `Journaling/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.Journaling.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetSpecialData` | `public static string GetSpecialData(IDictionary<string, string> dataMap, string key)` |
| `_Utils` | `StringToXyz` | `public static XYZ StringToXyz(string pointString)` |
| `_Utils` | `XyzToString` | `public static string XyzToString(XYZ point)` |

### `LevelsProperty/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.LevelsProperty.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CovertFromApi` | `public static double CovertFromApi(ForgeTypeId to, double value)` |
| `_Utils` | `CovertToApi` | `public static double CovertToApi(double value, ForgeTypeId from)` |

### `Loads/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.Loads.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FindLoadCaseByName` | `public static LoadCase FindLoadCaseByName(IEnumerable<LoadCase> loadCases, string name)` |
| `_Utils` | `FindUsageByName` | `public static LoadUsage FindUsageByName(IEnumerable<LoadUsage> usages, string name)` |

### `Massing/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.Massing.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `MakeArc` | `public static ModelCurve MakeArc(UIApplication app, XYZ ptA, XYZ ptB, XYZ ptC)` |
| `_Utils` | `MakeLine` | `public static ModelCurve MakeLine(UIApplication app, XYZ ptA, XYZ ptB)` |
| `_Utils` | `MakeLine` | `public static ModelCurve MakeLine(UIApplication app, XYZ ptA, XYZ ptB, XYZ norm)` |

### `ModelLines/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ModelLines.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetElementById` | `public static Element GetElementById(Document document, ElementId id)` |
| `_Utils` | `GetSketchPlaneById` | `public static SketchPlane GetSketchPlaneById(Document document, ElementId id)` |

### `ModelessDialog/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ModelessDialog.CS`
- **Types:** `_Utils`
- **Method count:** 7

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `DoorOperation` | `delegate void DoorOperation(...)` |
| `_Utils` | `FlipHandAndFace` | `public static void FlipHandAndFace(FamilyInstance door)` |
| `_Utils` | `MakeLeft` | `public static void MakeLeft(FamilyInstance door)` |
| `_Utils` | `MakeRight` | `public static void MakeRight(FamilyInstance door)` |
| `_Utils` | `ModifySelectedDoors` | `public static void ModifySelectedDoors(UIApplication uiapp, string text, DoorOperation operation)` |
| `_Utils` | `TurnIn` | `public static void TurnIn(FamilyInstance door)` |
| `_Utils` | `TurnOut` | `public static void TurnOut(FamilyInstance door)` |

### `MultiThreading/WorkThread/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.MultiThreading.WorkThread.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ParseReference` | `public static Reference ParseReference(Document document, string stableRepresentation)` |

### `MultiplanarRebar/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.MultiplanarRebar.CS`
- **Types:** `_Utils`
- **Method count:** 11

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ConstructCorbelFrame` | `private static CorbelFrame ConstructCorbelFrame(FamilyInstance corbel, Edge depthEdge, Edge leftE...` |
| `_Utils` | `GetCommonVertex` | `private static XYZ GetCommonVertex(Edge edge1, Edge edge2)` |
| `_Utils` | `GetDistance` | `private static double GetDistance(PlanarFace face1, PlanarFace face2)` |
| `_Utils` | `GetElementSolid` | `private static Solid GetElementSolid(Element element)` |
| `_Utils` | `GetNormalOutside` | `private static XYZ GetNormalOutside(Face face)` |
| `_Utils` | `GetOrCreateDef` | `public static ElementId GetOrCreateDef(string name, Document revitDoc)` |
| `_Utils` | `GetOrCreateDef` | `public static ExternalDefinition GetOrCreateDef(string name, Application revitApp)` |
| `_Utils` | `GetOrCreateDef` | `public static ExternalDefinition GetOrCreateDef(string name, string groupName, Application revitApp)` |
| `_Utils` | `GetSharedParameterFile` | `public static DefinitionFile GetSharedParameterFile(Application revitApp)` |
| `_Utils` | `IsTrapezoid` | `private static bool IsTrapezoid(XYZ hostNormal, PlanarFace corbelBottomFace, Edge bottomEdge, out...` |
| `_Utils` | `ParseCorbelGeometry` | `public static CorbelFrame ParseCorbelGeometry(FamilyInstance corbel)` |

### `NetworkPressureLossReport/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.NetworkPressureLossReport.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetSpatialFieldManager` | `public static SpatialFieldManager GetSpatialFieldManager(View view)` |

### `NewHostedSweep/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.NewHostedSweep.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ProjectToTrackball` | `public static XYZ ProjectToTrackball(double width, double height, Point point)` |

### `NewOpenings/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.NewOpenings.CS`
- **Types:** `Matrix4`, `Vector4`
- **Method count:** 35

| Type | Method | Signature |
|------|--------|-----------|
| `Matrix4` | `Identity` | `public void Identity()` |
| `Matrix4` | `Inverse` | `public Matrix4 Inverse()` |
| `Matrix4` | `Matrix4` | `public Matrix4()` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis)` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 origin)` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis, Vector4 origin)` |
| `Matrix4` | `Matrix4` | `public Matrix4(float scale)` |
| `Matrix4` | `Matrix4` | `return new Matrix4(new Vector4(this[0, 0], this[1, 0], this[2, 0]), new Vector4(this[0, 1], this[...` |
| `Matrix4` | `Matrix4` | `return new Matrix4(new Vector4(-this[3, 0], -this[3, 1], -this[3, 2]))` |
| `Matrix4` | `Matrix4` | `return new Matrix4()` |
| `Matrix4` | `Matrix4` | `return new Matrix4(1 / m_matrix[0, 0])` |
| `Matrix4` | `Multiply` | `public static Matrix4 Multiply(Matrix4 left, Matrix4 right)` |
| `Matrix4` | `RotationInverse` | `public Matrix4 RotationInverse()` |
| `Matrix4` | `ScaleInverse` | `public Matrix4 ScaleInverse()` |
| `Matrix4` | `TransForm` | `public Vector4 TransForm(Vector4 point)` |
| `Matrix4` | `TransLationInverse` | `public Matrix4 TransLationInverse()` |
| `Vector4` | `CrossProduct` | `public Vector4 CrossProduct(Vector4 v)` |
| `Vector4` | `CrossProduct` | `public static Vector4 CrossProduct(Vector4 va, Vector4 vb)` |
| `Vector4` | `DotProduct` | `public float DotProduct(Vector4 v)` |
| `Vector4` | `DotProduct` | `public static float DotProduct(Vector4 va, Vector4 vb)` |
| `Vector4` | `Length` | `public float Length()` |
| `Vector4` | `Normalize` | `public void Normalize()` |
| `Vector4` | `Vector4` | `public Vector4(float x, float y, float z)` |
| `Vector4` | `Vector4` | `public Vector4(Vector4 v)` |
| `Vector4` | `Vector4` | `public Vector4(XYZ v)` |
| `Vector4` | `Vector4` | `return new Vector4(va.X + vb.X, va.Y + vb.Y, va.Z + vb.Z)` |
| `Vector4` | `Vector4` | `return new Vector4(va.X - vb.X, va.Y - vb.Y, va.Z - vb.Z)` |
| `Vector4` | `Vector4` | `return new Vector4(v.X * factor, v.Y * factor, v.Z * factor)` |
| `Vector4` | `Vector4` | `return new Vector4(v.X / factor, v.Y / factor, v.Z / factor)` |
| `Vector4` | `Vector4` | `return new Vector4(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X)` |
| `Vector4` | `Vector4` | `return new Vector4(va.Y * vb.Z - va.Z * vb.Y, va.Z * vb.X - va.X * vb.Z, va.X * vb.Y - va.Y * vb.X)` |
| `Vector4` | `operator *` | `public static Vector4 operator *(Vector4 v, float factor)` |
| `Vector4` | `operator +` | `public static Vector4 operator +(Vector4 va, Vector4 vb)` |
| `Vector4` | `operator -` | `public static Vector4 operator -(Vector4 va, Vector4 vb)` |
| `Vector4` | `operator /` | `public static Vector4 operator /(Vector4 v, float factor)` |

### `NewRebar/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.NewRebar.CS`
- **Types:** `_Utils`
- **Method count:** 13

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddXyz` | `public static XYZ AddXyz(XYZ p1, XYZ p2)` |
| `_Utils` | `DotMatrix` | `private static double DotMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `GetLength` | `public static double GetLength(XYZ vector)` |
| `_Utils` | `IsEqual` | `public static bool IsEqual(double d1, double d2)` |
| `_Utils` | `IsEqual` | `public static bool IsEqual(XYZ first, XYZ second)` |
| `_Utils` | `IsOppositeDirection` | `public static bool IsOppositeDirection(XYZ firstVec, XYZ secondVec)` |
| `_Utils` | `IsSameDirection` | `public static bool IsSameDirection(XYZ firstVec, XYZ secondVec)` |
| `_Utils` | `IsVertical` | `public static bool IsVertical(Face face, Line line, Transform faceTrans, Transform lineTrans)` |
| `_Utils` | `MultiplyVector` | `public static XYZ MultiplyVector(XYZ vector, double rate)` |
| `_Utils` | `OffsetPoint` | `public static XYZ OffsetPoint(XYZ point, XYZ direction, double offset)` |
| `_Utils` | `SubXyz` | `public static XYZ SubXyz(XYZ p1, XYZ p2)` |
| `_Utils` | `TransformPoint` | `public static XYZ TransformPoint(XYZ point, Transform transform)` |
| `_Utils` | `UnitVector` | `public static XYZ UnitVector(XYZ vector)` |

### `NewRoof/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.NewRoof.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ParseBracketedId` | `public static string ParseBracketedId(string text)` |
| `_Utils` | `Translate` | `public static PointF Translate(XYZ pointXyz, BoundingBoxXYZ boundingbox)` |

### `PanelSchedule/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.PanelSchedule.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetNumberOfRowsAndColumns` | `public static void GetNumberOfRowsAndColumns(Document doc, PanelScheduleView psView, SectionType ...` |
| `_Utils` | `ReplaceIllegalCharacters` | `public static string ReplaceIllegalCharacters(string stringWithIllegalChar)` |

### `ParameterUtils/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ParameterUtils.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FormatElementIdParameter` | `private static string FormatElementIdParameter(string name, Parameter param, Document document)` |
| `_Utils` | `FormatParameterLine` | `public static string FormatParameterLine(Parameter param, Document document)` |

### `PerformanceAdviserControl/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.PerformanceAdviserControl.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CollectRuleInfo` | `public static List<RuleInfo> CollectRuleInfo(PerformanceAdviser performanceAdviser)` |

### `PlaceFamilyInstanceByFace/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.PlaceFamilyInstanceByFace.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CheckSelectedElement` | `public static bool CheckSelectedElement(RevitElement elem, UIDocument revitDoc, List<Face> faceLi...` |
| `_Utils` | `InquireGeometry` | `public static bool InquireGeometry(GeoElement geoElement, RevitElement elem, List<Face> faceList,...` |
| `_Utils` | `Project` | `public static XYZ Project(List<XYZ> xyzArray, XYZ point)` |

### `PlacementOptions/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.PlacementOptions.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FindProperFamilySymbol` | `public static List<FamilySymbol> FindProperFamilySymbol(Document document, BuiltInCategory category)` |

### `PointCloudEngine/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.PointCloudEngine.CS`
- **Types:** `_Utils`
- **Method count:** 8

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetBoolean` | `public static bool GetBoolean(XElement element)` |
| `_Utils` | `GetColor` | `public static int GetColor(XElement element)` |
| `_Utils` | `GetColorXElement` | `public static XElement GetColorXElement(int color, string name)` |
| `_Utils` | `GetDouble` | `public static double GetDouble(XElement element)` |
| `_Utils` | `GetInteger` | `public static int GetInteger(XElement element)` |
| `_Utils` | `GetXElement` | `public static XElement GetXElement(XYZ point, string name)` |
| `_Utils` | `GetXElement` | `public static XElement GetXElement(object obj, string name)` |
| `_Utils` | `GetXyz` | `public static XYZ GetXyz(XElement element)` |

### `PowerCircuit/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.PowerCircuit.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `IsElementBelongsToCircuit` | `public static bool IsElementBelongsToCircuit(MEPModel mepModel, ElectricalSystem selectedElectric...` |
| `_Utils` | `ShowErrorMessage` | `public static void ShowErrorMessage(string message)` |
| `_Utils` | `VerifyUnusedConnectors` | `public static bool VerifyUnusedConnectors(FamilyInstance fi)` |

### `ProjectInfo/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ProjectInfo.CS`
- **Types:** `_Utils`
- **Method count:** 5

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AngleStringToDouble` | `public static double AngleStringToDouble(string value)` |
| `_Utils` | `DoubleEquals` | `public static bool DoubleEquals(double x, double y)` |
| `_Utils` | `DoubleToAngleString` | `public static string DoubleToAngleString(double value)` |
| `_Utils` | `TimeZoneDoubleToString` | `public static string TimeZoneDoubleToString(double timeZone, string[] timeZones)` |
| `_Utils` | `TimeZoneStringToDouble` | `public static double TimeZoneStringToDouble(string value)` |

### `RebarContainerAnyShapeType/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS`
- **Types:** `_Utils`
- **Method count:** 26

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddXyz` | `public static XYZ AddXyz(XYZ p1, XYZ p2)` |
| `_Utils` | `CrossMatrix` | `public static XYZ CrossMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `DotMatrix` | `private static double DotMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `FindParameter` | `public static Parameter FindParameter(ParameterSet parameters, string name)` |
| `_Utils` | `GetHookOrient` | `public static RebarHookOrientation GetHookOrient(XYZ curveVec, XYZ normal, XYZ hookVec)` |
| `_Utils` | `GetLength` | `public static double GetLength(XYZ vector)` |
| `_Utils` | `IsEqual` | `public static bool IsEqual(double d1, double d2)` |
| `_Utils` | `IsEqual` | `public static bool IsEqual(XYZ first, XYZ second)` |
| `_Utils` | `IsInRightDir` | `public static bool IsInRightDir(XYZ normal)` |
| `_Utils` | `IsOppositeDirection` | `public static bool IsOppositeDirection(XYZ firstVec, XYZ secondVec)` |
| `_Utils` | `IsSameDirection` | `public static bool IsSameDirection(XYZ firstVec, XYZ secondVec)` |
| `_Utils` | `IsVertical` | `public static bool IsVertical(Face face, Line line, Transform faceTrans, Transform lineTrans)` |
| `_Utils` | `MultiplyVector` | `public static XYZ MultiplyVector(XYZ vector, double rate)` |
| `_Utils` | `OffsetPoint` | `public static XYZ OffsetPoint(XYZ point, XYZ direction, double offset)` |
| `_Utils` | `SetParaNullId` | `public static bool SetParaNullId(Parameter parameter)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, string parameterName, int value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, string parameterName, double value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, string parameterName, string value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, string parameterName, ref ElementId value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, BuiltInParameter paraIndex, int value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, BuiltInParameter paraIndex, double value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, BuiltInParameter paraIndex, string value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, BuiltInParameter paraIndex, ref ElementId value)` |
| `_Utils` | `SubXyz` | `public static XYZ SubXyz(XYZ p1, XYZ p2)` |
| `_Utils` | `TransformPoint` | `public static XYZ TransformPoint(XYZ point, Transform transform)` |
| `_Utils` | `UnitVector` | `public static XYZ UnitVector(XYZ vector)` |

### `RebarFreeForm/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.RebarFreeForm.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddSharedTestParameter` | `public static bool AddSharedTestParameter(ExternalCommandData commandData, string paramName, Forg...` |
| `_Utils` | `GetOffsetFromConstraintAtTarget` | `public static bool GetOffsetFromConstraintAtTarget(RebarUpdateCurvesData updateData, RebarConstra...` |
| `_Utils` | `ShareParameterExists` | `public static bool ShareParameterExists(Document doc, string paramName)` |

### `ReferencePlane/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ReferencePlane.CS`
- **Types:** `_Utils`
- **Method count:** 8

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `Distribute` | `public static void Distribute(Mesh mesh, ref XYZ startPoint, ref XYZ endPoint, ref XYZ thirdPnt)` |
| `_Utils` | `Equal` | `private static bool Equal(XYZ vectorA, XYZ vectorB)` |
| `_Utils` | `GetBottomFace` | `public static Face GetBottomFace(FaceArray faces)` |
| `_Utils` | `GetDistance` | `public static double GetDistance(double start, double end)` |
| `_Utils` | `GetLength` | `public static double GetLength(XYZ startPoint, XYZ endPoint)` |
| `_Utils` | `GetVector` | `public static XYZ GetVector(XYZ startPoint, XYZ endPoint)` |
| `_Utils` | `IsVerticalEdge` | `private static bool IsVerticalEdge(Edge edge)` |
| `_Utils` | `IsVerticalFace` | `private static bool IsVerticalFace(Face face)` |

### `Reinforcement/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.Reinforcement.CS`
- **Types:** `_Utils`
- **Method count:** 26

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddXyz` | `public static XYZ AddXyz(XYZ p1, XYZ p2)` |
| `_Utils` | `CrossMatrix` | `public static XYZ CrossMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `DotMatrix` | `private static double DotMatrix(XYZ p1, XYZ p2)` |
| `_Utils` | `FindParameter` | `public static Parameter FindParameter(ParameterSet parameters, string name)` |
| `_Utils` | `GetHookOrient` | `public static RebarHookOrientation GetHookOrient(XYZ curveVec, XYZ normal, XYZ hookVec)` |
| `_Utils` | `GetLength` | `public static double GetLength(XYZ vector)` |
| `_Utils` | `IsEqual` | `public static bool IsEqual(double d1, double d2)` |
| `_Utils` | `IsEqual` | `public static bool IsEqual(XYZ first, XYZ second)` |
| `_Utils` | `IsInRightDir` | `public static bool IsInRightDir(XYZ normal)` |
| `_Utils` | `IsOppositeDirection` | `public static bool IsOppositeDirection(XYZ firstVec, XYZ secondVec)` |
| `_Utils` | `IsSameDirection` | `public static bool IsSameDirection(XYZ firstVec, XYZ secondVec)` |
| `_Utils` | `IsVertical` | `public static bool IsVertical(Face face, Line line, Transform faceTrans, Transform lineTrans)` |
| `_Utils` | `MultiplyVector` | `public static XYZ MultiplyVector(XYZ vector, double rate)` |
| `_Utils` | `OffsetPoint` | `public static XYZ OffsetPoint(XYZ point, XYZ direction, double offset)` |
| `_Utils` | `SetParaNullId` | `public static bool SetParaNullId(Parameter parameter)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, string parameterName, int value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, string parameterName, double value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, string parameterName, string value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, string parameterName, ref ElementId value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, BuiltInParameter paraIndex, int value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, BuiltInParameter paraIndex, double value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, BuiltInParameter paraIndex, string value)` |
| `_Utils` | `SetParameter` | `public static bool SetParameter(Element element, BuiltInParameter paraIndex, ref ElementId value)` |
| `_Utils` | `SubXyz` | `public static XYZ SubXyz(XYZ p1, XYZ p2)` |
| `_Utils` | `TransformPoint` | `public static XYZ TransformPoint(XYZ point, Transform transform)` |
| `_Utils` | `UnitVector` | `public static XYZ UnitVector(XYZ vector)` |

### `RoofsRooms/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.RoofsRooms.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetRoomAndSpaceElements` | `public static List<Element> GetRoomAndSpaceElements(Document document)` |

### `RoomSchedule/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.RoomSchedule.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `GetProperty` | `public static string GetProperty(Document activeDoc, Room room, BuiltInParameter paraEnum, bool u...` |
| `_Utils` | `ShareParameterExists` | `public static bool ShareParameterExists(Room roomObj, string paramName, ref Parameter sharedParam)` |

### `RotateFramingObjects/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.RotateFramingObjects.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FindParameter` | `public static string FindParameter(string parameterName, FamilyInstance familyInstance)` |

### `RoutingPreferenceTools/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS`
- **Types:** `_Utils`
- **Method count:** 6

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ConvertValueDocumentUnits` | `public static double ConvertValueDocumentUnits(double decimalFeet, Document document)` |
| `_Utils` | `ConvertValueToFeet` | `public static double ConvertValueToFeet(double unitValue, Document document)` |
| `_Utils` | `MepWarning` | `public static void MepWarning()` |
| `_Utils` | `PipesDefinedWarning` | `public static void PipesDefinedWarning()` |
| `_Utils` | `ValidateMep` | `public static bool ValidateMep(Application application)` |
| `_Utils` | `ValidatePipesDefined` | `public static bool ValidatePipesDefined(Document document)` |

### `ScheduleCreation/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ScheduleCreation.CS`
- **Types:** `_Utils`
- **Method count:** 4

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddScheduleToNewSheet` | `private static void AddScheduleToNewSheet(Document document, ViewSchedule schedule)` |
| `_Utils` | `CreateAndAddSchedules` | `public static void CreateAndAddSchedules(UIDocument uiDocument)` |
| `_Utils` | `CreateSchedules` | `private static ICollection<ViewSchedule> CreateSchedules(UIDocument uiDocument)` |
| `_Utils` | `ShouldSkip` | `private static bool ShouldSkip(ElementId parameterId)` |

### `Selections/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.Selections.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `MoveElement` | `public static void MoveElement(Document document, Element elem, ref XYZ pickedPoint, XYZ targetPo...` |

### `ShaftHolePuncher/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS`
- **Types:** `Matrix4`, `Vector4`
- **Method count:** 34

| Type | Method | Signature |
|------|--------|-----------|
| `Matrix4` | `Identity` | `public void Identity()` |
| `Matrix4` | `Inverse` | `public Matrix4 Inverse()` |
| `Matrix4` | `Matrix4` | `public Matrix4()` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis)` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 origin)` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis, Vector4 origin)` |
| `Matrix4` | `Matrix4` | `public Matrix4(float scale)` |
| `Matrix4` | `Matrix4` | `return new Matrix4(new Vector4(this[0, 0], this[1, 0], this[2, 0]), new Vector4(this[0, 1], this[...` |
| `Matrix4` | `Matrix4` | `return new Matrix4(new Vector4(-this[3, 0], -this[3, 1], -this[3, 2]))` |
| `Matrix4` | `Matrix4` | `return new Matrix4()` |
| `Matrix4` | `Matrix4` | `return new Matrix4(1 / m_matrix[0, 0])` |
| `Matrix4` | `Multiply` | `public static Matrix4 Multiply(Matrix4 left, Matrix4 right)` |
| `Matrix4` | `RotationInverse` | `public Matrix4 RotationInverse()` |
| `Matrix4` | `ScaleInverse` | `public Matrix4 ScaleInverse()` |
| `Matrix4` | `Transform` | `public Vector4 Transform(Vector4 point)` |
| `Matrix4` | `TranslationInverse` | `public Matrix4 TranslationInverse()` |
| `Vector4` | `CrossProduct` | `public Vector4 CrossProduct(Vector4 v)` |
| `Vector4` | `CrossProduct` | `public static Vector4 CrossProduct(Vector4 va, Vector4 vb)` |
| `Vector4` | `DotProduct` | `public float DotProduct(Vector4 v)` |
| `Vector4` | `DotProduct` | `public static float DotProduct(Vector4 va, Vector4 vb)` |
| `Vector4` | `Length` | `public float Length()` |
| `Vector4` | `Normalize` | `public void Normalize()` |
| `Vector4` | `Vector4` | `public Vector4(float x, float y, float z)` |
| `Vector4` | `Vector4` | `public Vector4(XYZ v)` |
| `Vector4` | `Vector4` | `return new Vector4(va.X + vb.X, va.Y + vb.Y, va.Z + vb.Z)` |
| `Vector4` | `Vector4` | `return new Vector4(va.X - vb.X, va.Y - vb.Y, va.Z - vb.Z)` |
| `Vector4` | `Vector4` | `return new Vector4(v.X * factor, v.Y * factor, v.Z * factor)` |
| `Vector4` | `Vector4` | `return new Vector4(v.X / factor, v.Y / factor, v.Z / factor)` |
| `Vector4` | `Vector4` | `return new Vector4(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X)` |
| `Vector4` | `Vector4` | `return new Vector4(va.Y * vb.Z - va.Z * vb.Y, va.Z * vb.X - va.X * vb.Z, va.X * vb.Y - va.Y * vb.X)` |
| `Vector4` | `operator *` | `public static Vector4 operator *(Vector4 v, float factor)` |
| `Vector4` | `operator +` | `public static Vector4 operator +(Vector4 va, Vector4 vb)` |
| `Vector4` | `operator -` | `public static Vector4 operator -(Vector4 va, Vector4 vb)` |
| `Vector4` | `operator /` | `public static Vector4 operator /(Vector4 v, float factor)` |

### `SharedCoordinateSystem/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS`
- **Types:** `_Utils`
- **Method count:** 8

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ConvertFrom` | `public static CityInfoString ConvertFrom(CityInfo cityInfo)` |
| `_Utils` | `ConvertTo` | `public static CityInfo ConvertTo(CityInfoString cityInfoString)` |
| `_Utils` | `DealDecimalNumber` | `public static string DealDecimalNumber(string value, int number)` |
| `_Utils` | `DealPrecision` | `public static double DealPrecision(double value, int precision)` |
| `_Utils` | `DoubleToString` | `public static string DoubleToString(double value, ValueType valueType)` |
| `_Utils` | `ParseFromString` | `private static bool ParseFromString(string value, ValueType valueType, out double result)` |
| `_Utils` | `StringToDouble` | `public static bool StringToDouble(string value, ValueType valueType, out double newValue)` |
| `_Utils` | `ValueConversion` | `private static void ValueConversion(double value, ValueType valueType, bool isDoubleToString, out...` |

### `SheetToView3D/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.SheetToView3D.CS`
- **Types:** `_Utils`
- **Method count:** 8

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CalculateClickAsModelRay` | `public static XYZ CalculateClickAsModelRay(Viewport viewport, XYZ click)` |
| `_Utils` | `Create3DView` | `public static View3D Create3DView(Document doc, XYZ eyePosition, XYZ upDir, XYZ forwardDir)` |
| `_Utils` | `GetViewPlanCutPlane` | `public static Plane GetViewPlanCutPlane(ViewPlan plan)` |
| `_Utils` | `GetViewportAtClick` | `public static Viewport GetViewportAtClick(ViewSheet viewSheet, XYZ click)` |
| `_Utils` | `IsPointInsideCurveLoop` | `public static bool IsPointInsideCurveLoop(XYZ point, CurveLoop curveloop)` |
| `_Utils` | `MakeFromViewportClick` | `public static Result MakeFromViewportClick(UIDocument uidoc)` |
| `_Utils` | `MakeSheetToModelTransform` | `public static Transform MakeSheetToModelTransform(Transform trfModelToProjection, Transform trfPr...` |
| `_Utils` | `ProjectPointOnPlane` | `public static XYZ ProjectPointOnPlane(Plane plane, XYZ point)` |

### `Site/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.Site.CS`
- **Types:** `SubRegionSelectionFilter`, `TopographySurfaceSelectionFilter`, `_Utils`
- **Method count:** 22

| Type | Method | Signature |
|------|--------|-----------|
| `SubRegionSelectionFilter` | `AllowElement` | `public bool AllowElement(Element element)` |
| `SubRegionSelectionFilter` | `AllowReference` | `public bool AllowReference(Reference refer, XYZ point)` |
| `TopographySurfaceSelectionFilter` | `AllowElement` | `public bool AllowElement(Element element)` |
| `TopographySurfaceSelectionFilter` | `AllowReference` | `public bool AllowReference(Reference refer, XYZ point)` |
| `_Utils` | `AllowElement` | `public bool AllowElement(Element element)` |
| `_Utils` | `AllowReference` | `public bool AllowReference(Reference refer, XYZ point)` |
| `_Utils` | `ChangeSubregionAndPointsElevation` | `public static void ChangeSubregionAndPointsElevation(UIDocument uiDoc, double elevationDelta)` |
| `_Utils` | `GenerateCircleSurrounding` | `private static void GenerateCircleSurrounding(IList<XYZ> points, XYZ center, double deltaElevatio...` |
| `_Utils` | `GeneratePondPointsSurrounding` | `public static IList<XYZ> GeneratePondPointsSurrounding(XYZ center, double maxRadius)` |
| `_Utils` | `GetAverageElevation` | `public static double GetAverageElevation(IList<XYZ> existingPoints)` |
| `_Utils` | `GetCenterOf` | `public static XYZ GetCenterOf(Element element)` |
| `_Utils` | `GetNonBoundaryPoints` | `public static IList<XYZ> GetNonBoundaryPoints(TopographySurface toposurface)` |
| `_Utils` | `GetPointsFromSubregionExact` | `public static IList<XYZ> GetPointsFromSubregionExact(TopographySurface subregion)` |
| `_Utils` | `GetPointsFromSubregionRough` | `public static IList<XYZ> GetPointsFromSubregionRough(TopographySurface subregion)` |
| `_Utils` | `GetSmallIcon` | `public static BitmapSource GetSmallIcon(Icon icon)` |
| `_Utils` | `GetStdIcon` | `public static BitmapSource GetStdIcon(Icon icon)` |
| `_Utils` | `GetTopographySurfaceHost` | `public static TopographySurface GetTopographySurfaceHost(TopographySurface subregion)` |
| `_Utils` | `MoveXyzToElevation` | `public static XYZ MoveXyzToElevation(XYZ input, double elevation)` |
| `_Utils` | `PickPointNearToposurface` | `public static XYZ PickPointNearToposurface(UIDocument uiDoc, TopographySurface toposurface, strin...` |
| `_Utils` | `PickSubregion` | `public static TopographySurface PickSubregion(UIDocument uiDoc)` |
| `_Utils` | `PickTopographySurface` | `public static TopographySurface PickTopographySurface(UIDocument uiDoc)` |
| `_Utils` | `SetIconsForPushButtonData` | `public static void SetIconsForPushButtonData(PushButtonData button, Icon icon)` |

### `SolidSolidCut/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.SolidSolidCut.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `TryGetDemoSolids` | `public static bool TryGetDemoSolids(Document doc, out Element solidToBeCut, out Element cuttingSo...` |

### `SpotDimension/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.SpotDimension.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddDataRow` | `public static void AddDataRow(string parameterName, string value, DataTable parameterTable)` |
| `_Utils` | `CreateTable` | `public static DataTable CreateTable()` |
| `_Utils` | `FormatFractionalInches` | `public static string FormatFractionalInches(double value, string formatter = "#0.000")` |

### `StairsAutomation/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.StairsAutomation.CS`
- **Types:** `_Utils`
- **Method count:** 7

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ComputeParameter` | `public static double ComputeParameter(Curve curve, XYZ point)` |
| `_Utils` | `FindLongestEndpointConnection` | `public static Line FindLongestEndpointConnection(Line line1, Line line2)` |
| `_Utils` | `FindTargetLevels` | `public static Tuple<Level, Level, Level> FindTargetLevels(Document doc, string name1, string name...` |
| `_Utils` | `ProjectCurveToElevation` | `public static Curve ProjectCurveToElevation(Curve curve, double elevation)` |
| `_Utils` | `ProjectCurvesToElevation` | `public static IList<Curve> ProjectCurvesToElevation(IList<Curve> curves, double elevation)` |
| `_Utils` | `TransformCurve` | `public static Curve TransformCurve(Curve input, Transform trf)` |
| `_Utils` | `TransformCurves` | `public static IList<Curve> TransformCurves(IList<Curve> inputs, Transform trf)` |

### `StructSample/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.StructSample.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `FindFamilySymbol` | `public static FamilySymbol FindFamilySymbol(Document doc, string familyName, string symbolName)` |

### `TagBeam/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.TagBeam.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `AddTagSymbolByCategory` | `public static void AddTagSymbolByCategory(FamilySymbol tagSymbol, List<FamilySymbolWrapper> categ...` |
| `_Utils` | `GetSelectedBeams` | `public static List<FamilyInstance> GetSelectedBeams(UIDocument revitDoc)` |

### `Toposolid/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.Toposolid.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CollectPointsFromImportInstance` | `public static List<XYZ> CollectPointsFromImportInstance(ImportInstance import)` |
| `_Utils` | `TryGetFirstToposolidTypeAndLevel` | `public static bool TryGetFirstToposolidTypeAndLevel(Document doc, out ElementId typeId, out Eleme...` |

### `TraverseSystem/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.TraverseSystem.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ExtractSystemFromConnectors` | `public static MEPSystem ExtractSystemFromConnectors(ConnectorSet connectors)` |
| `_Utils` | `GetConnectedConnector` | `public static Connector GetConnectedConnector(Connector connector)` |

### `Truss/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.Truss.CS`
- **Types:** `Matrix4`, `Vector4`
- **Method count:** 34

| Type | Method | Signature |
|------|--------|-----------|
| `Matrix4` | `Identity` | `public void Identity()` |
| `Matrix4` | `Inverse` | `public Matrix4 Inverse()` |
| `Matrix4` | `Matrix4` | `public Matrix4()` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis)` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 origin)` |
| `Matrix4` | `Matrix4` | `public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis, Vector4 origin)` |
| `Matrix4` | `Matrix4` | `public Matrix4(double scale)` |
| `Matrix4` | `Matrix4` | `return new Matrix4(new Vector4(this[0, 0], this[1, 0], this[2, 0]), new Vector4(this[0, 1], this[...` |
| `Matrix4` | `Matrix4` | `return new Matrix4(new Vector4(-this[3, 0], -this[3, 1], -this[3, 2]))` |
| `Matrix4` | `Matrix4` | `return new Matrix4()` |
| `Matrix4` | `Matrix4` | `return new Matrix4(1 / m_matrix[0, 0])` |
| `Matrix4` | `Multiply` | `public static Matrix4 Multiply(Matrix4 left, Matrix4 right)` |
| `Matrix4` | `RotationInverse` | `public Matrix4 RotationInverse()` |
| `Matrix4` | `ScaleInverse` | `public Matrix4 ScaleInverse()` |
| `Matrix4` | `Transform` | `public Vector4 Transform(Vector4 point)` |
| `Matrix4` | `TranslationInverse` | `public Matrix4 TranslationInverse()` |
| `Vector4` | `CrossProduct` | `public Vector4 CrossProduct(Vector4 v)` |
| `Vector4` | `CrossProduct` | `public static Vector4 CrossProduct(Vector4 va, Vector4 vb)` |
| `Vector4` | `DotProduct` | `public double DotProduct(Vector4 v)` |
| `Vector4` | `DotProduct` | `public static double DotProduct(Vector4 va, Vector4 vb)` |
| `Vector4` | `Length` | `public double Length()` |
| `Vector4` | `Normalize` | `public void Normalize()` |
| `Vector4` | `Vector4` | `public Vector4(double x, double y, double z)` |
| `Vector4` | `Vector4` | `public Vector4(XYZ v)` |
| `Vector4` | `Vector4` | `return new Vector4(va.X + vb.X, va.Y + vb.Y, va.Z + vb.Z)` |
| `Vector4` | `Vector4` | `return new Vector4(va.X - vb.X, va.Y - vb.Y, va.Z - vb.Z)` |
| `Vector4` | `Vector4` | `return new Vector4(v.X * factor, v.Y * factor, v.Z * factor)` |
| `Vector4` | `Vector4` | `return new Vector4(v.X / factor, v.Y / factor, v.Z / factor)` |
| `Vector4` | `Vector4` | `return new Vector4(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X)` |
| `Vector4` | `Vector4` | `return new Vector4(va.Y * vb.Z - va.Z * vb.Y, va.Z * vb.X - va.X * vb.Z, va.X * vb.Y - va.Y * vb.X)` |
| `Vector4` | `operator *` | `public static Vector4 operator *(Vector4 v, double factor)` |
| `Vector4` | `operator +` | `public static Vector4 operator +(Vector4 va, Vector4 vb)` |
| `Vector4` | `operator -` | `public static Vector4 operator -(Vector4 va, Vector4 vb)` |
| `Vector4` | `operator /` | `public static Vector4 operator /(Vector4 v, double factor)` |

### `UIAPI/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.UIAPI.CS`
- **Types:** `_Utils`
- **Method count:** 3

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ConvertFromBitmap` | `public static BitmapSource ConvertFromBitmap(Bitmap bitmap)` |
| `_Utils` | `GetBitmapAsImageSource` | `public static ImageSource GetBitmapAsImageSource(Bitmap bitmap)` |
| `_Utils` | `GetDocumentDisplayName` | `public static string GetDocumentDisplayName(Document dbDoc)` |

### `VersionChecking/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.VersionChecking.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CenterOnScreen` | `public static void CenterOnScreen(Form form)` |

### `ViewPrinter/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ViewPrinter.CS`
- **Types:** `_Utils`
- **Method count:** 1

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `MyMessageBox` | `public static void MyMessageBox(string text)` |

### `ViewTemplateCreation/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.ViewTemplateCreation.CS`
- **Types:** `_Utils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `ShowInformationMessageBox` | `public static void ShowInformationMessageBox(string message)` |
| `_Utils` | `ShowWarningMessageBox` | `public static void ShowWarningMessageBox(string message)` |

### `WinderStairs/_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser.WinderStairs.CS`
- **Types:** `_Utils`
- **Method count:** 6

| Type | Method | Signature |
|------|--------|-----------|
| `_Utils` | `CalculateControlPoints` | `public static IList<XYZ> CalculateControlPoints(Document rvtDoc, IList<ElementId> crvElements)` |
| `_Utils` | `CalculateControlPoints2` | `private static IList<XYZ> CalculateControlPoints2(Document rvtDoc, IList<ElementId> elements)` |
| `_Utils` | `CalculateMaxStepsCount` | `public static IList<uint> CalculateMaxStepsCount(IList<XYZ> controlPoints, double runWidth, doubl...` |
| `_Utils` | `CalculateOffset` | `private static IList<XYZ> CalculateOffset(IList<XYZ> controlPoints, double offset)` |
| `_Utils` | `CheckOrientation` | `private static bool CheckOrientation(IList<XYZ> controlPoints)` |
| `_Utils` | `HasCommonEndPoint` | `private static bool HasCommonEndPoint(Curve crv1, Curve crv2, out XYZ common, out int index1, out...` |

### `_utils.cs`

- **Namespace:** `Ara3D.RevitSampleBrowser`
- **Types:** `SampleBrowserUtils`
- **Method count:** 2

| Type | Method | Signature |
|------|--------|-----------|
| `SampleBrowserUtils` | `GetSourceFolder` | `public static string GetSourceFolder([CallerFilePath] string callerFilePath = null)` |
| `SampleBrowserUtils` | `NormalizeSampleNamespace` | `public static string NormalizeSampleNamespace(string typeNamespace)` |
