// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Geometry;
namespace Ara3D.RevitSampleBrowser.ExternalCommand.CS.ExternalCommandRegistration
{
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
            var curves = XyzMath.CreateRectangularWallCurves();
            Wall.Create(uiDoc.Document, curves, false);

            trans.Commit();
            return Result.Succeeded;
        }
    }

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
