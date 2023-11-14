// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.NewHostedSweep.CS
{
    /// <summary>
    ///     This class is the entrance of this project, it implements IExternalCommand.
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
                var mgr = new CreationMgr(commandData.Application.ActiveUIDocument);
                mgr.Execute();
            }
            catch (Exception e)
            {
                message += e.StackTrace;
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }
    }
}
