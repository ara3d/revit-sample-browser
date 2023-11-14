// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.GenerateFloor.CS
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
                    CreateFloor(data, commandData.Application.ActiveUIDocument.Document);

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


        /// <summary>
        ///     create a floor by the data obtain from revit.
        /// </summary>
        /// <param name="data">Data including the profile, level etc, which is need for create a floor.</param>
        /// <param name="doc">Retrieves an object that represents the currently active project.</param>
        public static void CreateFloor(Data data, Document doc)
        {
            var loop = new CurveLoop();
            foreach (Curve curve in data.Profile) loop.Append(curve);

            var floorLoops = new List<CurveLoop> { loop };

            Floor.Create(doc, floorLoops, data.FloorType.Id, data.Level.Id, data.Structural, null, 0.0);
        }
    }
}
