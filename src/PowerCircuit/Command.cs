// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.PowerCircuit.CS.Properties;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.PowerCircuit.CS
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
                if (null == commandData.Application.ActiveUIDocument.Document)
                {
                    message = Resources.ResourceManager.GetString("NullActiveDocument");
                    return Result.Failed;
                }

                if (commandData.Application.ActiveUIDocument.Selection.GetElementIds().Count == 0)
                {
                    message = Resources.ResourceManager.GetString("SelectPowerElements");
                    return Result.Failed;
                }

                CircuitOperationData optionData = new(commandData);
                using (CircuitOperationForm mainForm = new(optionData))
                {
                    if (mainForm.ShowDialog() == DialogResult.Cancel) return Result.Cancelled;
                }

                if (optionData.Operation != Operation.CreateCircuit &&
                    optionData.ElectricalSystemCount > 1)
                    using (SelectCircuitForm selectForm = new(optionData))
                    {
                        if (selectForm.ShowDialog() == DialogResult.Cancel) return Result.Cancelled;
                    }

                if (optionData.Operation == Operation.EditCircuit)
                    using (EditCircuitForm editForm = new(optionData))
                    {
                        if (editForm.ShowDialog() == DialogResult.Cancel) return Result.Cancelled;
                    }

                optionData.Operate();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
