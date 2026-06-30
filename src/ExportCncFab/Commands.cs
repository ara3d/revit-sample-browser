// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from ExportCncFab by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/ExportCncFab

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.ExportCncFab.CS
{
    [Transaction(TransactionMode.Manual)]
    public class CmdDxf : IExternalCommand
    {
        const string Caption = "Export to CNC Fabrication";

        class WallPartSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                if (e is not Part part)
                    return false;

                var cids = part.GetSourceElementOriginalCategoryIds();
                if (1 != cids.Count)
                    return false;

                var cid = cids.First();
                var bic = (BuiltInCategory)cid.Value;
                return BuiltInCategory.OST_Walls == bic;
            }

            public bool AllowReference(Reference r, XYZ p)
            {
                return true;
            }
        }

        static string _folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        static void InfoMsg(string msg)
        {
            Debug.Print(msg);
            TaskDialog.Show(Caption, msg);
        }

        static void ErrorMsg(string msg)
        {
            Debug.Print(msg);
            TaskDialog dlg = new(Caption)
            {
                MainIcon = TaskDialogIcon.TaskDialogIconWarning,
                MainInstruction = msg
            };
            dlg.Show();
        }

        static void OnDialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
        {
            // Auto-confirm "export/print with temporary view modes" so batch export is not blocked.

            if (e is TaskDialogShowingEventArgs e2 && e2.DialogId.Equals(
                    "TaskDialog_Really_Print_Or_Export_Temp_View_Modes"))
            {
                var cmdLink = (int)TaskDialogResult.CommandLink2;
                e.OverrideResult(cmdLink);
            }
        }

        public static Result Execute2(
            ExternalCommandData commandData,
            bool exportToSatFormat)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            if (null == doc)
            {
                ErrorMsg("Please run this command in a valid Revit project document.");
                return Result.Failed;
            }

            var view = doc.ActiveView;

            if (view is null or not View3D)
            {
                ErrorMsg("Please run this command in a valid 3D view.");
                return Result.Failed;
            }

            if (PartsVisibility.ShowPartsOnly != view.PartsVisibility)
            {
                ErrorMsg("Please run this command in a view displaying parts and not source elements.");
                return Result.Failed;
            }

            List<ElementId> viewIds = [view.Id];

            List<ElementId> ids = null;
            var sel = uidoc.Selection;
            var selIds = sel.GetElementIds();

            if (0 < selIds.Count)
            {
                foreach (var id in selIds)
                {
                    var e = doc.GetElement(id);

                    if (e is not Part)
                    {
                        ErrorMsg("Please pre-select only gyp wallboard parts before running this command.");
                        return Result.Failed;
                    }

                    var part = e as Part;
                    var lids = part.GetSourceElementIds();

                    if (1 != lids.Count)
                    {
                        ErrorMsg("Gyp wallboard part has multiple source elements.");
                        return Result.Failed;
                    }

                    var cids = part.GetSourceElementOriginalCategoryIds();

                    if (1 != cids.Count)
                    {
                        ErrorMsg("Gyp wallboard part has multiple source element categories.");
                        return Result.Failed;
                    }

                    var cid = cids.First();
                    var bic = (BuiltInCategory)cid.Value;

                    if (BuiltInCategory.OST_Walls != bic)
                    {
                        ErrorMsg("Please pre-select only gyp wallboard parts.");
                        return Result.Failed;
                    }

                    ids ??= new List<ElementId>(1);
                    ids.Add(e.Id);
                }

                if (null == ids)
                {
                    ErrorMsg("Please pre-select only gyp wallboard parts before running this command.");
                    return Result.Failed;
                }
            }

            if (null == ids)
            {
                IList<Reference> refs;
                try
                {
                    refs = sel.PickObjects(
                        ObjectType.Element,
                        new WallPartSelectionFilter(),
                        "Please select wall parts.");
                }
                catch (OperationCanceledException)
                {
                    return Result.Cancelled;
                }

                ids = refs.Select(r => r.ElementId).ToList();
            }

            if (0 == ids.Count)
            {
                ErrorMsg("No valid parts selected.");
                return Result.Failed;
            }

            ExportParameters exportParameters = new(doc.GetElement(ids[0]));

            if (!exportParameters.IsValid)
            {
                ErrorMsg("Please initialise the CNC fabrication export history shared parameters before launching this command.");
                return Result.Failed;
            }

            if (!Util.BrowseDirectory(ref _folder, true))
                return Result.Cancelled;

            try
            {
                uiapp.DialogBoxShowing += OnDialogBoxShowing;

                object opt = exportToSatFormat
                    ? new SATExportOptions()
                    : new DXFExportOptions();

                using TransactionGroup txg = new(doc);
                txg.Start("Export Wall Parts");

                foreach (var id in ids)
                {
                    var e = doc.GetElement(id);
                    Debug.Assert(e is Part, "expected parts only");

                    var part = e as Part;
                    var lids = part.GetSourceElementIds();
                    Debug.Assert(1 == lids.Count, "unexpected multiple part source elements.");

                    var lid = lids.First();
                    var hostId = lid.HostElementId;
                    var partId = e.Id;

                    var sort_mark = exportParameters.GetSortMarkFor(e);
                    var filename = (null == sort_mark) ? string.Empty : sort_mark + '_';
                    filename += string.Format("{0}_{1}", hostId, partId);

                    var host = doc.GetElement(hostId);
                    Debug.Assert(null != host, "expected to be able to access host element");
                    Debug.Assert(host is Wall or Part, "expected host element to be a wall or part");
                    Debug.Assert(null != host.Category, "expected host element to have a valid category");
                    Debug.Assert(
                        host.Category.Id.Value.Equals((long)BuiltInCategory.OST_Walls)
                        || host.Category.Id.Value.Equals((long)BuiltInCategory.OST_Parts),
                        "expected host element to have wall or part category");
                    Debug.Assert(ElementId.InvalidElementId != host.LevelId, "expected host element to have a valid level id");

                    if (ElementId.InvalidElementId != host.LevelId)
                    {
                        var level = doc.GetElement(host.LevelId);
                        filename = level.Name.Replace(' ', '_') + "_" + filename;
                    }

                    if (view.IsTemporaryHideIsolateActive())
                    {
                        using (Transaction tx = new(doc))
                        {
                            tx.Start("Disable Temporary Isolate");
                            view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                            tx.Commit();
                        }

                        Debug.Assert(!view.IsTemporaryHideIsolateActive(),
                            "expected to turn off temporary hide/isolate");
                    }

                    using (Transaction tx = new(doc))
                    {
                        tx.Start("Export Wall Part " + partId);
                        view.IsolateElementTemporary(partId);
                        tx.Commit();
                    }

                    if (exportToSatFormat)
                        doc.Export(_folder, filename, viewIds, (SATExportOptions)opt);
                    else
                        doc.Export(_folder, filename, viewIds, (DXFExportOptions)opt);

                    exportParameters.Add(e.Id);
                }
            }
            finally
            {
                uiapp.DialogBoxShowing -= OnDialogBoxShowing;
            }

            using (Transaction tx = new(doc))
            {
                tx.Start("Update CNC Fabrication Export History Shared Parameters");
                exportParameters.UpdateExportHistory();
                tx.Commit();
            }

            return Result.Succeeded;
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            return Execute2(commandData, false);
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdSat : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            return CmdDxf.Execute2(commandData, true);
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdCreateSharedParameters : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            ExportParameters.Create(doc);
            return Result.Succeeded;
        }
    }
}
