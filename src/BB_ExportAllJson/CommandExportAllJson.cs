using Ara3D.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class CommandExportAllJson : NamedCommand
    {
        public override string Name => "Export all elements JSON";

        public UIApplication app { get; private set; }

        public override void Execute(object arg)
        {
            app = arg as UIApplication;
            if (app == null)
            {
                throw new Exception($"Passed argument {arg} is either null or not a UI application");
            }

            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;
            var outputFilePath = doc.CurrentFileName().ChangeDirectoryAndExt(Config.OutputDir, ".json");
            var elements = doc.GetElements().OrderBy(e => e.Id.Value);
            JArray array = new(elements.ToJson());
            outputFilePath.WriteAllText(array.ToString());
        }
    }
}
