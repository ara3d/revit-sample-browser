// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from GetElementImage by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/GetElementImage

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.GetElementImage.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        class ElementSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element e) => e is not View;

            public bool AllowReference(Reference r, XYZ p) => true;
        }

        static Result GetSelectedElements(
            UIDocument uidoc,
            ref string message,
            out ICollection<ElementId> ids)
        {
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            ids = sel.GetElementIds();

            if (ids.Count == 0)
            {
                try
                {
                    var refs = sel.PickObjects(
                        ObjectType.Element,
                        new ElementSelectionFilter(),
                        "Please select elements to export their views");

                    ids = refs.Select(r => r.ElementId).ToList();
                }
                catch (OperationCanceledException)
                {
                    return Result.Cancelled;
                }
            }

            return Result.Succeeded;
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;

            if (uidoc == null)
            {
                message = "Please run this command in an active project document.";
                return Result.Failed;
            }

            var doc = uidoc.Document;
            ICollection<ElementId> ids;

            var rc = GetSelectedElements(uidoc, ref message, out ids);
            if (rc != Result.Succeeded)
                return rc;

            using (var tx = new Transaction(doc))
            {
                tx.Start("Export PNG Element Images");

                uidoc.Selection.SetElementIds(new List<ElementId>());

                var ie = new ImageExporter(doc);
                var interactive = !app.IsJournalPlaying();

                foreach (var id in ids)
                {
                    var e = doc.GetElement(id);
                    var filepaths = ie.ExportToImage(e);

                    Debug.Print("{0}: {1}", e.Id, string.Join(", ", filepaths));

                    if (interactive)
                    {
                        foreach (var fp in filepaths)
                            Process.Start(new ProcessStartInfo(fp) { UseShellExecute = true });
                    }
                }

                tx.RollBack();
            }

            return rc;
        }
    }
}
