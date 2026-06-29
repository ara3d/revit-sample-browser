# SetParameterValueWithImageData

| Field | Value |
|-------|-------|
| **Sample** | Massing/ParameterValuesFromImage |
| **Class** | `SetParameterValueWithImageData` |
| **Source** | `src/Massing/ParameterValuesFromImage/Command.cs` |
| **SDK ReadMe** | `src/Massing/ParameterValuesFromImage/ReadMe_ParameterValuesFromImage.rtf` |
| **MCP rating** | 2/5 |

Maps grayscale pixel values from a companion bitmap onto divided-surface panel `Grayscale` parameters, deleting panels where the pixel is black.

## What it demonstrates

- Loading `{documentPath}_grayscale.bmp` with `System.Drawing.Bitmap`
- UV-indexed `GetPixel` mapped to divided-surface grid indices
- Setting normalized grayscale on panel instances or calling `Document.Delete` for zero values

## Prerequisites

- Mass family with `DividedSurface` and panel families containing a `Grayscale` instance parameter
- Grayscale BMP file alongside the saved family path (`doc.PathName + "_grayscale.bmp"`)

## User interaction

- No UI; fails if the bitmap or parameter is missing

## MCP notes

- Highly workflow-specific (image-driven panel culling); poor fit for generic MCP unless image path and parameter name are parameterized.
