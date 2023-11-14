//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PanelSchedule.CS
{
    /// <summary>
    /// Import the panel scheduel view to place on a sheet view.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class SheetImport : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            // get one sheet view to place panel schedule.
            var sheet = doc.ActiveView as ViewSheet;
            if (null == sheet)
            {
                message = "please go to a sheet view.";
                return Result.Failed;
            }

            // get all PanelScheduleView instances in the Revit document.
            var fec = new FilteredElementCollector(doc);
            var PanelScheduleViewsAreWanted = new ElementClassFilter(typeof(PanelScheduleView));
            fec.WherePasses(PanelScheduleViewsAreWanted);
            var psViews = fec.ToElements() as List<Element>;

            var placePanelScheduleOnSheet = new Transaction(doc, "placePanelScheduleOnSheet");
            placePanelScheduleOnSheet.Start();

            var nextOrigin = new XYZ(0.0, 0.0, 0.0);
            foreach (var element in psViews)
            {
                var psView = element as PanelScheduleView;
                if (psView.IsPanelScheduleTemplate())
                {
                    // ignore the PanelScheduleView instance which is a template.
                    continue;
                }

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
