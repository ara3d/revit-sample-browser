// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;
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
            Transaction tran = new(commandData.Application.ActiveUIDocument.Document, "Generate Floor");
            tran.Start();

            try
            {
                if (null == commandData) throw new ArgumentNullException(nameof(commandData));

                Data data = new();
                data.ObtainData(commandData);

                GenerateFloorForm dlg = new(data);

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
