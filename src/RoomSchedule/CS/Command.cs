// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RoomSchedule
{
    /// <summary>
    ///     To add an external command to Autodesk Revit,
    ///     the developer must define a class which implements the IExternalCommand interface.
    ///     This class is used as the connection of Revit and external program
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Transaction tranSample = null;
            try
            {
                tranSample = new Transaction(commandData.Application.ActiveUIDocument.Document, "Sample Start");
                tranSample.Start();
                // create a form to display the information of Revit rooms and xls based rooms
                using (var infoForm = new RoomScheduleForm(commandData))
                {
                    infoForm.ShowDialog();
                }

                tranSample.Commit();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                tranSample?.RollBack();
                // if there are something wrong, give error information and return failed
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
