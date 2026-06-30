// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.ComputedSymbolGeometry.CS
{
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
                using var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                    "Ara3D.RevitSampleBrowser.ComputedSymbolGeometry");
                trans.Start();
                new ComputedSymbolGeometry(commandData.Application.ActiveUIDocument.Document).GetInstanceGeometry();
                trans.Commit();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                message += e.StackTrace;
                return Result.Failed;
            }
        }
    }
}
