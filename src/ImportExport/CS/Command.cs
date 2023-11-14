// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
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
                // Verify active document
                if (null == commandData.Application.ActiveUIDocument.Document)
                {
                    message = "Active view is null.";
                    return Result.Failed;
                }

                var mainData = new MainData(commandData);
                // Show the dialog
                using (var mainForm = new MainForm(mainData))
                {
                    if (mainForm.ShowDialog() == DialogResult.Cancel) return Result.Cancelled;
                }
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
