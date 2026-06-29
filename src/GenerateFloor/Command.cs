// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.GenerateFloor.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var tran = new Transaction(commandData.Application.ActiveUIDocument.Document, "Generate Floor");
            tran.Start();

            try
            {
                if (null == commandData) throw new ArgumentNullException(nameof(commandData));

                var data = new Data();
                data.ObtainData(commandData);

                var dlg = new GenerateFloorForm(data);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SampleBrowserUtils.CreateFloor(data, commandData.Application.ActiveUIDocument.Document);

                    tran.Commit();
                    return Result.Succeeded;
                }

                tran.RollBack();
                return Result.Cancelled;
            }
            catch (Exception e)
            {
                message = e.Message;
                tran.RollBack();
                return Result.Failed;
            }
        }
    }
}
