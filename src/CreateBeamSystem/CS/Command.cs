// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Diagnostics;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.CreateBeamSystem.CS
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
