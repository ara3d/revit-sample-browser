// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

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
                Transaction trans = new(commandData.Application.ActiveUIDocument.Document,
                    "Ara3D.RevitSampleBrowser.SharedCoordinateSystem");
                trans.Start();
                CoordinateSystemData data = new(commandData);
                data.GatData();

                using (CoordinateSystemDataForm displayForm =
                       new(data, commandData.Application.Application.Cities,
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
