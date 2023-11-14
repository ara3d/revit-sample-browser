// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Reinforcement.CS
{
    /// <summary>
    ///     The entrance of this example, which create reinforcement rebars on
    ///     the selected concrete beam and column without reinforcement.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "External Tool");
            try
            {
                transaction.Start();
                // create a factory to create the corresponding FrameReinMaker
                var factory = new FrameReinMakerFactory(commandData);

                // Do some data checks, such whether the user select concrete beam or column
                if (!factory.AssertData())
                {
                    message = "Please select a concrete beam or column without reinforcement.";
                    return Result.Failed;
                }

                // Invoke work() method to create corresponding FrameReinMaker,
                // and create the reinforcement rebars
                factory.work();

                // if everything goes well, return succeeded.
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
