using System.Linq;
using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;
using Nice3point.Revit.Extensions.UI;

namespace Ara3D.RevitSampleBrowser.N3P_View.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_View : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uidoc = data.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var view = doc.ActiveView;

            N3POutput.Header("Nice3point View extensions");

            N3POutput.Line("Active view", view.Name);

            var visibleWalls = doc.CollectElements(view)
                .OfClass<Wall>()
                .VisibleInView(view)
                .Count();
            N3POutput.Line("Walls visible in view", visibleWalls);

            var selectable = doc.CollectElements()
                .OfClass<Wall>()
                .SelectableInView(view)
                .Count();
            N3POutput.Line("Walls selectable in view", selectable);

            var manager = view.GetSpatialFieldManager();
            N3POutput.Line("GetSpatialFieldManager()", manager != null ? "present" : "none");

            return Result.Succeeded;
        }
    }
}
