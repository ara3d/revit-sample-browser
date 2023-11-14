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


using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.SolidSolidCut.CS
{
    /// <summary>
    /// Demonstrate how to use the SolidSolidCut API to make one solid cut another.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
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
                TaskDialog.Show("Notice", "Please open the family file SolidSolidCut.rfa, then try to run this command.");

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
    /// Demonstrate how to use the SolidSolidCut API to uncut two solids which have the cutting relationship.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
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
                TaskDialog.Show("Notice", "Please open the family file SolidSolidCut.rfa, then try to run this command.");

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
