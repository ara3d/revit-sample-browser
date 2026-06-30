// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SetoutPoints by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/SetoutPoints

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.SetoutPoints.CS
{
    /// <summary>
    /// Renumbers major (key) setout points with an "SOP " prefix starting at one.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CmdRenumber : IExternalCommand
    {
        private const string SopPrefix = "SOP ";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                message = "Please run this command in an active project document.";
                return Result.Failed;
            }

            var doc = uidoc.Document;
            var symbols = CmdGeomVertices.GetFamilySymbols(doc, false);

            if (symbols == null)
            {
                TaskDialog.Show(
                    "Setout Points",
                    "Setout point family not loaded, so no setout points present.");
                return Result.Succeeded;
            }

            FamilyInstanceFilter instanceFilter = new(doc, symbols[0].Id);
            var col = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_GenericModel)
                .OfClass(typeof(FamilyInstance))
                .WherePasses(instanceFilter);

            using Transaction tx = new(doc);
            tx.Start("Renumber Setout Points");

            var i = 0;
            foreach (var p in col)
                p.get_Parameter(CmdGeomVertices.ParameterPointNr).Set(SopPrefix + ++i);

            tx.Commit();

            return Result.Succeeded;
        }
    }
}
