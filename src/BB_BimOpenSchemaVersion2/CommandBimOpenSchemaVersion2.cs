using Ara3D.BIMOpenSchema.Revit2025;
using Autodesk.Revit.UI;

namespace Ara3D.Bowerbird.RevitSamples;

public class CommandBimOpenSchemaVersion2 : NamedCommand
{
    public override string Name => "BIM Open Schema Exporter v2";

    public override void Execute(object arg)
    {
        var app = arg as UIApplication;
        BIMOpenSchemaExporterForm form = new();
        form.Show(app);
    }
}