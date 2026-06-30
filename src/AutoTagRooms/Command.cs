// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.AutoTagRooms.CS
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
                Transaction documentTransaction =
                    new(commandData.Application.ActiveUIDocument.Document, "Document");
                documentTransaction.Start();
                RoomsData data = new(commandData);

                DialogResult result;

                using (AutoTagRoomsForm roomsTagForm = new(data))
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
