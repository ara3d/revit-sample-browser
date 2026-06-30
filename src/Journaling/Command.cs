// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.Journaling.CS
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
                Transaction tran = new(commandData.Application.ActiveUIDocument.Document, "Journaling");
                tran.Start();
                // Create a real operate class
                Journaling deal = new(commandData);
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
