// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Transaction transaction = new(commandData.Application.ActiveUIDocument.Document, "External Tool");
            try
            {
                transaction.Start();
                FrameReinMakerFactory factory = new(commandData);

                if (!factory.AssertData())
                {
                    message = "Please select a concrete beam or column without reinforcement.";
                    return Result.Failed;
                }

                factory.Work();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            finally
            {
                transaction.Commit();
            }
        }
    }
}
