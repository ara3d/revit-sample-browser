// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.SolidSolidCut.CS
{
    /// <summary>
    ///     Demonstrate how to use the SolidSolidCut API to make one solid cut another.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Cut : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            // NOTES: Anything can be done in this method, such as the solid-solid cut operation.

            // Get the application and document from external command data.
            var activeDoc = commandData.Application.ActiveUIDocument.Document;

            long solidToBeCutElementId = 30481; //The cube
            long cuttingSolidElementId = 30809; //The sphere

            //Get element by ElementId
            var solidToBeCut = activeDoc.GetElement(new ElementId(solidToBeCutElementId));
            var cuttingSolid = activeDoc.GetElement(new ElementId(cuttingSolidElementId));

            //If the two elements do not exist, notify user to open the family file then try this command.
            if (solidToBeCut == null || cuttingSolid == null)
            {
                TaskDialog.Show("Notice",
                    "Please open the family file SolidSolidCut.rfa, then try to run this command.");

                return Result.Succeeded;
            }

            //Check whether the cuttingSolid can cut the solidToBeCut
            if (SolidSolidCutUtils.CanElementCutElement(cuttingSolid, solidToBeCut, out _))
            {
                //cuttingSolid can cut solidToBeCut

                //Do the solid-solid cut operation
                //Start a transaction
                var transaction = new Transaction(activeDoc);
                transaction.Start("AddCutBetweenSolids(activeDoc, solidToBeCut, cuttingSolid)");

                //Let the cuttingSolid cut the solidToBeCut
                SolidSolidCutUtils.AddCutBetweenSolids(activeDoc, solidToBeCut, cuttingSolid);

                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Demonstrate how to use the SolidSolidCut API to uncut two solids which have the cutting relationship.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Uncut : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            // NOTES: Anything can be done in this method, such as the solid-solid uncut operation.

            // Get the application and document from external command data.
            var activeDoc = commandData.Application.ActiveUIDocument.Document;

            long solidToBeCutElementId = 30481; //The cube
            long cuttingSolidElementId = 30809; //The sphere

            //Get element by ElementId
            var solidToBeCut = activeDoc.GetElement(new ElementId(solidToBeCutElementId));
            var cuttingSolid = activeDoc.GetElement(new ElementId(cuttingSolidElementId));

            //If the two elements do not exist, notify user to open the family file then try this command.
            if (solidToBeCut == null || cuttingSolid == null)
            {
                TaskDialog.Show("Notice",
                    "Please open the family file SolidSolidCut.rfa, then try to run this command.");

                return Result.Succeeded;
            }

            //Remove the solid-solid cut (Uncut)
            //Start a transaction
            var transaction = new Transaction(activeDoc);
            transaction.Start("RemoveCutBetweenSolids(activeDoc, solidToBeCut, cuttingSolid)");

            //Remove the cutting relationship between solidToBeCut and cuttingSolid (Uncut)
            SolidSolidCutUtils.RemoveCutBetweenSolids(activeDoc, solidToBeCut, cuttingSolid);

            transaction.Commit();

            return Result.Succeeded;
        }
    }
}
