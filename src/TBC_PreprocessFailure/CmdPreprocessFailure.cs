#region Header

//
// CmdPreprocessFailure.cs - suppress warning message by implementing the IFailuresPreprocessor interface
//
// Copyright (C) 2010-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Suppresses the unbounded-room warning via IFailuresPreprocessor (Harry Mattison).</summary>
    [Transaction(TransactionMode.Manual)]
    internal class CmdPreprocessFailure : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var doc = commandData.Application
                .ActiveUIDocument.Document;

            using var t = new Transaction(doc);
            var collector
                = new FilteredElementCollector(doc);

            collector.OfClass(typeof(Level));
            var level = collector.FirstElement() as Level;

            t.Start("Create unbounded room");

            var failOpt
                = t.GetFailureHandlingOptions();

            failOpt.SetFailuresPreprocessor(
                new RoomWarningSwallower());

            t.SetFailureHandlingOptions(failOpt);

            doc.Create.NewRoom(level, new UV(0, 0));

            t.Commit();

            return Result.Succeeded;
        }
    }
}
