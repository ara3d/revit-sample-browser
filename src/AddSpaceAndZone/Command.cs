// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
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
                var documentTransaction =
                    new Transaction(commandData.Application.ActiveUIDocument.Document, "Document");
                documentTransaction.Start();

                var dataManager = new DataManager(commandData);

                DialogResult result;

                using (var mainForm = new MainForm(dataManager))
                {
                    result = mainForm.ShowDialog();
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
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
