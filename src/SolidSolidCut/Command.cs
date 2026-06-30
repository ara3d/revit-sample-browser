// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace Ara3D.RevitSampleBrowser.SolidSolidCut.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Cut : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var activeDoc = commandData.Application.ActiveUIDocument.Document;

            if (!FaceAndSolidGeometry.TryGetDemoSolids(activeDoc, out var solidToBeCut, out var cuttingSolid))
            {
                TaskDialog.Show("Notice",
                    "Please open the family file SolidSolidCut.rfa, then try to run this command.");
                return Result.Succeeded;
            }

            if (!SolidSolidCutUtils.CanElementCutElement(cuttingSolid, solidToBeCut, out _))
                return Result.Succeeded;

            using Transaction transaction = new(activeDoc, "AddCutBetweenSolids");
            transaction.Start();
            SolidSolidCutUtils.AddCutBetweenSolids(activeDoc, solidToBeCut, cuttingSolid);
            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Uncut : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var activeDoc = commandData.Application.ActiveUIDocument.Document;

            if (!FaceAndSolidGeometry.TryGetDemoSolids(activeDoc, out var solidToBeCut, out var cuttingSolid))
            {
                TaskDialog.Show("Notice",
                    "Please open the family file SolidSolidCut.rfa, then try to run this command.");
                return Result.Succeeded;
            }

            using Transaction transaction = new(activeDoc, "RemoveCutBetweenSolids");
            transaction.Start();
            SolidSolidCutUtils.RemoveCutBetweenSolids(activeDoc, solidToBeCut, cuttingSolid);
            transaction.Commit();

            return Result.Succeeded;
        }
    }
}
