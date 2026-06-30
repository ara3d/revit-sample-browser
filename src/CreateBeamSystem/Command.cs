// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Transaction tran = new(commandData.Application.ActiveUIDocument.Document, "CreateBeamSystem");
            tran.Start();

            try
            {
                // initialize precondition data of the program
                BeamSystemData data = new(commandData);
                // display form to collect user's setting for beam system
                using (BeamSystemForm form = new(data))
                {
                    if (form.ShowDialog() != DialogResult.OK)
                    {
                        tran.RollBack();
                        return Result.Cancelled;
                    }
                }

                // create beam system using the parameters saved in BeamSystemData
                BeamSystemBuilder builder = new(data);
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
