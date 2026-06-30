// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.PanelSchedule.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SheetImport : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            if (doc.ActiveView is not ViewSheet sheet)
            {
                message = "please go to a sheet view.";
                return Result.Failed;
            }

            FilteredElementCollector fec = new(doc);
            ElementClassFilter panelScheduleViewsAreWanted = new(typeof(PanelScheduleView));
            fec.WherePasses(panelScheduleViewsAreWanted);
            var psViews = fec.ToElements() as List<Element>;

            Transaction placePanelScheduleOnSheet = new(doc, "placePanelScheduleOnSheet");
            placePanelScheduleOnSheet.Start();

            XYZ nextOrigin = new(0.0, 0.0, 0.0);
            foreach (var element in psViews)
            {
                var psView = element as PanelScheduleView;
                if (psView.IsPanelScheduleTemplate())
                    continue;

                var onSheet = PanelScheduleSheetInstance.Create(doc, psView.Id, sheet);
                onSheet.Origin = nextOrigin;
                var bbox = onSheet.get_BoundingBox(doc.ActiveView);
                var width = bbox.Max.X - bbox.Min.X;
                nextOrigin = new XYZ(onSheet.Origin.X + width, onSheet.Origin.Y, onSheet.Origin.Z);
            }

            placePanelScheduleOnSheet.Commit();

            return Result.Succeeded;
        }
    }
}
