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


using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewOpenings.CS
{
    /// <summary>
    ///     The entrance of this example, implement the Execute method of IExternalCommand
    ///     Show how to create Opening in Revit by RevitAPI
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "External Tool");
            try
            {
                transaction.Start();

                var elems = new ElementSet();
                foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                    elems.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
                //if user have some wrong selection, give user an error message
                if (1 != elems.Size)
                {
                    message = "please selected one Object (Floor or Wall) to create Opening.";
                    return Result.Cancelled;
                }

                Element selectElem = null;
                foreach (Element e in elems) selectElem = e;

                if (!(selectElem is Wall) && !(selectElem is Floor))
                {
                    message = "please selected one Object (Floor or Wall) to create Opening.";
                    return Result.Cancelled;
                }

                try
                {
                    if (selectElem is Wall)
                    {
                        var wall = selectElem as Wall;
                        var profileWall = new ProfileWall(wall, commandData);
                        var newOpeningsForm = new NewOpeningsForm(profileWall);
                        newOpeningsForm.ShowDialog();
                    }
                    else if (selectElem is Floor)
                    {
                        var floor = selectElem as Floor;
                        var profileFloor = new ProfileFloor(floor, commandData);
                        var newOpeningsForm = new NewOpeningsForm(profileFloor);
                        newOpeningsForm.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Cancelled;
                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
            finally
            {
                transaction.Commit();
            }
        }
    }
}