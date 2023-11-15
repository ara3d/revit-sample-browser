// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ExternalCommandRegistration.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand, create a wall
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalCommandCreateWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.ExternalCommandRegistration");
            trans.Start();
            var uiDoc = commandData.Application.ActiveUIDocument;
            var curves = new List<Curve>();
            //create rectangular curve: wall length: 60 , wall width: 40
            var line1 = Line.CreateBound(new XYZ(0, 0, 0),
                new XYZ(0, 60, 0));
            var line2 = Line.CreateBound(new XYZ(0, 60, 0),
                new XYZ(0, 60, 40));
            var line3 = Line.CreateBound(new XYZ(0, 60, 40),
                new XYZ(0, 0, 40));
            var line4 = Line.CreateBound(new XYZ(0, 0, 40),
                new XYZ(0, 0, 0));
            curves.Add(line1);
            curves.Add(line2);
            curves.Add(line3);
            curves.Add(line4);
            //create wall
            Wall.Create(uiDoc.Document, curves, false);

            trans.Commit();
            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand, show a message box
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExternalCommand3DView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            TaskDialog.Show("External Command Registration Sample", "Hello, 3D View!");

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication,
    ///     show message box when Revit start up and shut down
    /// </summary>
    [Regeneration(RegenerationOption.Manual)]
    public class ExternalApplicationClass : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            TaskDialog.Show("External command Registration Sample", "Revit is starting up.");
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            TaskDialog.Show("External command Registration Sample", "Revit is shutting down.");
            return Result.Succeeded;
        }
    }
}
