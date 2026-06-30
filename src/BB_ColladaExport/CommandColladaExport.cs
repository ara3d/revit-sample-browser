using Ara3D.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class CommandColladaExport : NamedCommand
    {
        public override string Name => "Export Collada";

        public override void Execute(object argument)
        {
            if (argument is not UIApplication uiapp)
                throw new Exception("Argument is not a UIApplication");

            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            if (doc.ActiveView is not View3D view3D)
                throw new Exception("You must be in 3D view to export.");


            var outputFilePath = doc.CurrentFileName().ChangeDirectoryAndExt(Config.OutputDir, ".dae");
            using (StreamWriter textWriter = new(outputFilePath.CreateWrite()))
            {
                ColladaExportContext context = new(doc, textWriter);
                CustomExporter exporter = new(doc, context)
                {
                    IncludeGeometricObjects = true,
                    ShouldStopOnError = false
                };
                exporter.Export(view3D);
            }

            outputFilePath.OpenFileInExplorer();
        }
    }
}
