// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.AvoidObstruction.CS
{
    /// <summary>
    ///     Implements interface IExternalCommand of Revit API.
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

                var resolver = new Resolver(commandData);
                resolver.Resolve();
            }
            catch (Exception e)
            {
                transaction.RollBack();
                message += e.ToString();
                return Result.Failed;
            }
            finally
            {
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
