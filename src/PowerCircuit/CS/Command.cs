// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.SDK.Samples.PowerCircuit.CS.Properties;

namespace Revit.SDK.Samples.PowerCircuit.CS
{
    /// <summary>
    ///     To add an external command to Autodesk Revit
    ///     the developer should implement an object that
    ///     supports the IExternalCommand interface.
    /// </summary>
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
                // Quit if active document is null
                if (null == commandData.Application.ActiveUIDocument.Document)
                {
                    message = Resources.ResourceManager.GetString("NullActiveDocument");
                    return Result.Failed;
                }

                // Quit if no elements selected
                if (commandData.Application.ActiveUIDocument.Selection.GetElementIds().Count == 0)
                {
                    message = Resources.ResourceManager.GetString("SelectPowerElements");
                    return Result.Failed;
                }

                // Collect information from selected elements and show operation dialog
                var optionData = new CircuitOperationData(commandData);
                using (var mainForm = new CircuitOperationForm(optionData))
                {
                    if (mainForm.ShowDialog() == DialogResult.Cancel) return Result.Cancelled;
                }

                // Show the dialog for user to select a circuit if more than one circuit available
                if (optionData.Operation != Operation.CreateCircuit &&
                    optionData.ElectricalSystemCount > 1)
                    using (var selectForm = new SelectCircuitForm(optionData))
                    {
                        if (selectForm.ShowDialog() == DialogResult.Cancel) return Result.Cancelled;
                    }

                // If user choose to edit circuit, display the circuit editing dialog
                if (optionData.Operation == Operation.EditCircuit)
                    using (var editForm = new EditCircuitForm(optionData))
                    {
                        if (editForm.ShowDialog() == DialogResult.Cancel) return Result.Cancelled;
                    }

                // Perform the operation
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
