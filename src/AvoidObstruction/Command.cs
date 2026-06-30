// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.AvoidObstruction.CS
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

                Resolver resolver = new(commandData);
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
