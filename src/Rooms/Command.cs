// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.Rooms.CS
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
                Transaction tran = new(commandData.Application.ActiveUIDocument.Document, "Rooms");
                tran.Start();
                RoomsData data = new(commandData);

                using (RoomsInformationForm infoForm = new(data))
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
