// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ReferencePlane.CS
{
    /// <summary>
    ///     The entry of this sample, that supports the IExternalCommand interface.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                "RevitMultiSample.ReferencePlane");
            trans.Start();
            try
            {
                // Generate an object of Revit reference plane management.
                var refPlaneMgr = new ReferencePlaneMgr(commandData);

                using (var dlg = new ReferencePlaneForm(refPlaneMgr))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        // Done some actions, ask revit to execute it.
                        trans.Commit();
                        return Result.Succeeded;
                    }

                    // Revit need to do nothing.
                    trans.RollBack();
                    return Result.Cancelled;
                }
            }
            catch (Exception e)
            {
                // Exception raised, report it by revit error reporting mechanism. 
                message = e.ToString();
                trans.RollBack();
                return Result.Failed;
            }
        }
    }
}
