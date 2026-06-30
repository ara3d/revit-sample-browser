// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from StlImport by Scott Conover, Autodesk Inc. (MIT).
// https://github.com/jeremytammik/StlImport

using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.StlImport.CS
{
    /// <summary>
    ///     Shared import options used by <see cref="StlImportCommand"/> and the
    ///     property setter commands in this sample.
    /// </summary>
    public class StlImportProperties
    {
        static StlImportProperties _properties;

        StlImportProperties()
        {
            Target = TessellatedShapeBuilderTarget.AnyGeometry;
            Fallback = TessellatedShapeBuilderFallback.Mesh;
            Binary = true;
            GraphicsStyleId = ElementId.InvalidElementId;
        }

        public static StlImportProperties GetProperties()
        {
            return _properties ??= new StlImportProperties();
        }

        public void SetModeToSolid()
        {
            Target = TessellatedShapeBuilderTarget.Solid;
            Fallback = TessellatedShapeBuilderFallback.Abort;
        }

        public void SetModeToAnyGeometry()
        {
            Target = TessellatedShapeBuilderTarget.AnyGeometry;
            Fallback = TessellatedShapeBuilderFallback.Mesh;
        }

        public void SetModeToPolymesh()
        {
            Target = TessellatedShapeBuilderTarget.Mesh;
            Fallback = TessellatedShapeBuilderFallback.Salvage;
        }

        public TessellatedShapeBuilderTarget Target { get; private set; }

        public TessellatedShapeBuilderFallback Fallback { get; private set; }

        public void SetGraphicsStyleToInvalid()
        {
            GraphicsStyleId = ElementId.InvalidElementId;
        }

        public void SetGraphicsStyleToSketch(Document doc)
        {
            var style = new FilteredElementCollector(doc)
                .OfClass(typeof(GraphicsStyle))
                .Cast<GraphicsStyle>()
                .FirstOrDefault(gs => gs.Name.Equals("<Sketch>"));

            if (style != null)
            {
                GraphicsStyleId = style.Id;
            }
        }

        public ElementId GraphicsStyleId { get; private set; }

        public void SetModeToBinary()
        {
            Binary = true;
        }

        public void SetModelToAscii()
        {
            Binary = false;
        }

        public bool Binary { get; private set; }
    }
}
