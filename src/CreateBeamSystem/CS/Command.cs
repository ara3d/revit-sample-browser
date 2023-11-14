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
using System.Diagnostics;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CreateBeamSystem.CS
{
    /// <summary>
    ///     external applications' only entry point class that supports the IExternalCommand interface
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var tran = new Transaction(commandData.Application.ActiveUIDocument.Document, "CreateBeamSystem");
            tran.Start();

            try
            {
                GeometryUtil.CreApp = commandData.Application.Application.Create;

                // initialize precondition data of the program
                var data = new BeamSystemData(commandData);
                // display form to collect user's setting for beam system
                using (var form = new BeamSystemForm(data))
                {
                    if (form.ShowDialog() != DialogResult.OK)
                    {
                        tran.RollBack();
                        return Result.Cancelled;
                    }
                }

                // create beam system using the parameters saved in BeamSystemData
                var builder = new BeamSystemBuilder(data);
                builder.CreateBeamSystem();
            }
            catch (ErrorMessageException errorEx)
            {
                // checked exception need to show in error messagebox
                message = errorEx.Message;
                tran.RollBack();
                return Result.Failed;
            }
            catch (Exception ex)
            {
                // unchecked exception cause command failed
                message = "Command is failed for unexpected reason.";
                Trace.WriteLine(ex.ToString());
                tran.RollBack();
                return Result.Failed;
            }

            tran.Commit();
            return Result.Succeeded;
        }
    }
}