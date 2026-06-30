// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.SpotDimension.CS
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
                using (SpotDimensionInfoDlg infoForm = new(commandData))
                {
                    //Highlight the selected spotdimension
                    if (infoForm.ShowDialog() == DialogResult.OK
                        && infoForm.SelectedSpotDimension != null)
                    {
                        elements.Insert(infoForm.SelectedSpotDimension);
                        message = "High light the selected SpotDimension";
                        return Result.Failed;
                    }
                }

                documentTransaction.Commit();
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
