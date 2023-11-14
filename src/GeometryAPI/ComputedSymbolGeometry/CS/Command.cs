// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ComputedSymbolGeometry.CS
{
    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     this class controls the class which subscribes handle events and the events' information UI.
    ///     like a bridge between them.
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
                var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                    "Revit.SDK.Samples.ComputedSymbolGeometry");
                trans.Start();
                // create a ComputedSymbolGeometry object 
                var computedSymGeo = new ComputedSymbolGeometry(commandData.Application.ActiveUIDocument.Document);
                // execute method to get and show geometry of all instances
                computedSymGeo.GetInstanceGeometry();
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
