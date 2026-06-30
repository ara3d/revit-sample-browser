using Ara3D.Logging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text;

namespace Ara3D.Bowerbird.RevitSamples;

public class CommandBimOpenSchemaVersion1 : NamedCommand
{
    public override string Name => "BIM Open Schema Exporter v1";

    public BimOpenSchemaExportSettings GetExportSettings()
    {
        return new()
        {
            Folder = BimOpenSchemaExportSettings.DefaultFolder,
            IncludeLinks = true,
            IncludeGeometry = true,
        };
    }

    public override void Execute(object arg)
    {
        var uiapp = arg as UIApplication;
        var doc = uiapp?.ActiveUIDocument?.Document;
        StringBuilder sb = new();
        var logger = Logger.Create(sb);
        doc?.ExportBimOpenSchema(GetExportSettings(), logger);
        TextDisplayForm.DisplayText(sb.ToString());
    }
}