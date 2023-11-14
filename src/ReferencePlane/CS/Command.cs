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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ReferencePlane.CS
{
    /// <summary>
    /// The entry of this sample, that supports the IExternalCommand interface.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document, "Revit.SDK.Samples.ReferencePlane");
            trans.Start();
            try
            {
                // Generate an object of Revit reference plane management.
                var refPlaneMgr = new ReferencePlaneMgr(commandData);

                using (var dlg = new ReferencePlaneForm(refPlaneMgr))
                {
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // Done some actions, ask revit to execute it.
                        trans.Commit();
                        return Result.Succeeded;
                    }
                    else
                    {
                        // Revit need to do nothing.
                        trans.RollBack();
                        return Result.Cancelled;
                    }
                }
            }
            catch (Exception e)
            {
                // Exception raised, report it by revit error reporting mechanism. 
                message = e.ToString();
                trans.RollBack();
                return Result.Failed;
            }
            
        }
    }
}
