// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Rooms.CS
{
    /// <summary>
    ///     To add an external command to Autodesk Revit,
    ///     the developer must define an class which implement the IExternalCommand interface.
    ///     This class is used as the connection of revit and external program
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                var tran = new Transaction(commandData.Application.ActiveUIDocument.Document, "Rooms");
                tran.Start();
                // create a new instance of class data
                var data = new RoomsData(commandData);

                // create a form to display the information of rooms
                using (var infoForm = new RoomsInformationForm(data))
                {
                    infoForm.ShowDialog();
                }

                tran.Commit();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                // If there are something wrong, give error information and return failed
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
