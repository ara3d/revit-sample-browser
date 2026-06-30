// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CreateAndPrintSheetsAndViews by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CreateAndPrintSheetsAndViews

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace Ara3D.RevitSampleBrowser.CreateAndPrintSheetsAndViews.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public readonly string[] product_codes = {
            "10K", "1F_Injerto", "20BS", "21BA", "22B2R",
            "31WA", "41UA", "51RA", "63RPB", "70TG",
            "72TM1", "73TM2",
        };

        bool ClassifyPart(
            List<string> report,
            Element part)
        {
            bool rc = false;
            string product_code = Util.GetProductCode(part);
            if (null != product_code
                && product_codes.Contains(product_code))
            {
                report.Add(string.Format(
                    $"{product_code}-{part.Id.Value}"));

                CmdCreateAndPrintSheetAndViews.CreateSheetAndViewsFor(part);

                rc = true;
            }
            return rc;
        }

        public static void OnDialogBoxShowing(
            object sender,
            DialogBoxShowingEventArgs e)
        {
            // https://thebuildingcoder.typepad.com/blog/2013/03/export-wall-parts-individually-to-dxf.html#3

            TaskDialogShowingEventArgs e2
                = e as TaskDialogShowingEventArgs;

            if (null != e2 && e2.DialogId.Equals(
                "TaskDialog_Really_Print_Or_Export_Temp_View_Modes"))
            {
                e.OverrideResult(
                    (int)TaskDialogResult.CommandLink2);
            }
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            uiapp.DialogBoxShowing
                += new EventHandler<DialogBoxShowingEventArgs>(
                    OnDialogBoxShowing);

            List<ElementId> ids = new FabricationPartSelector(uidoc).Ids;

            int n = ids.Count;

            if (0 == n)
            {
                return Result.Cancelled;
            }

            int nPartsProcessed = 0;
            int nPartsNotProcessed = 0;
            List<string> report = new List<string>();

            using (Transaction t = new Transaction(doc))
            {
                foreach (ElementId id in ids)
                {
                    FabricationPart part
                        = doc.GetElement(id) as FabricationPart;

                    t.Start(string.Format(
                        "Create sheet and views for part {0}",
                        id.Value));

                    if (ClassifyPart(report, part))
                    {
                        ++nPartsProcessed;
                    }
                    else
                    {
                        ++nPartsNotProcessed;
                    }
                    t.RollBack();
                }
            }

            string s = $"{nPartsProcessed} parts processed, "
                + $"{nPartsNotProcessed} not:";

            string s2 = string.Join(", ", report);
            Util.InfoMsg2(s, s2);

            return Result.Succeeded;
        }
    }
}
