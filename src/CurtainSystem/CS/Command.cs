// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.SDK.Samples.CurtainSystem.CS.CurtainSystem;
using Revit.SDK.Samples.CurtainSystem.CS.Data;
using Revit.SDK.Samples.CurtainSystem.CS.Properties;
using Revit.SDK.Samples.CurtainSystem.CS.UI;

namespace Revit.SDK.Samples.CurtainSystem.CS
{
    /// <summary>
    ///     the entry point of the sample (to launch the sample dialog and allows further operations)
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    internal class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // data verification
            if (null == commandData.Application.ActiveUIDocument) return Result.Failed;

            var mydocument = new MyDocument(commandData);


            // check whether the mass is kind of parallelepiped
            var checker = new MassChecker(mydocument);
            var validMass = checker.CheckSelectedMass();

            if (!validMass)
            {
                message = Resources.MSG_InvalidSelection;
                return Result.Cancelled;
            }

            CurtainForm curtainForm = null;
            var transactionGroup = new TransactionGroup(commandData.Application.ActiveUIDocument.Document);
            try
            {
                transactionGroup.Start("CurtainSystemOperation");
                curtainForm = new CurtainForm(mydocument);

                if (null != curtainForm && false == curtainForm.IsDisposed) curtainForm.ShowDialog();

                transactionGroup.Commit();
            }
            catch (Exception ex)
            {
                transactionGroup.RollBack();
                message = ex.Message;
                return Result.Failed;
            }
            finally
            {
                if (null != curtainForm && false == curtainForm.IsDisposed) curtainForm.Dispose();
            }

            return Result.Succeeded;
        }
    }
}
