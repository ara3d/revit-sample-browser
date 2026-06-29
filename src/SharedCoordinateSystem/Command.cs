// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                    "Ara3D.RevitSampleBrowser.SharedCoordinateSystem");
                trans.Start();
                var data = new CoordinateSystemData(commandData);
                data.GatData();

                using (var displayForm =
                       new CoordinateSystemDataForm(data, commandData.Application.Application.Cities,
                           commandData.Application.ActiveUIDocument.Document.SiteLocation))
                {
                    if (DialogResult.OK != displayForm.ShowDialog())
                    {
                        trans.RollBack();
                        return Result.Cancelled;
                    }
                }

                trans.Commit();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
