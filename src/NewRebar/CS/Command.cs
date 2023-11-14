// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace Revit.SDK.Samples.NewRebar.CS
{
    /// <summary>
    ///     This class is the entrance of this project, it implements IExternalCommand.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "External Tool");
            try
            {
                transaction.Start();
                var creator = new RebarCreator(commandData);
                creator.Execute();
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Cancelled;
            }
            finally
            {
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
