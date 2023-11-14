// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Journaling.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                var tran = new Transaction(commandData.Application.ActiveUIDocument.Document, "Journaling");
                tran.Start();
                // Create a real operate class
                var deal = new Journaling(commandData);
                deal.Run(); // The main deal operation
                tran.Commit();

                // if everything goes well, return succeeded.
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                // If there is something wrong, give error information and return failed
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
