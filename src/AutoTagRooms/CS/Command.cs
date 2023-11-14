// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.AutoTagRooms.CS
{
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
                //Create a transaction
                var documentTransaction =
                    new Transaction(commandData.Application.ActiveUIDocument.Document, "Document");
                documentTransaction.Start();
                // Create a new instance of class RoomsData
                var data = new RoomsData(commandData);

                DialogResult result;

                // Create a form to display the information of rooms
                using (var roomsTagForm = new AutoTagRoomsForm(data))
                {
                    result = roomsTagForm.ShowDialog();
                }

                if (result == DialogResult.OK)
                {
                    documentTransaction.Commit();
                    return Result.Succeeded;
                }

                documentTransaction.RollBack();
                return Result.Cancelled;
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
