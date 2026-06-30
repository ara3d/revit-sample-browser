// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.DirectionCalculation.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class FindSouthFacingWallsWithoutProjectLocation : FindSouthFacingWalls, IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("8B29D56B-7B9A-4c79-8A38-B1C13B921877"));

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Document = revit.Application.ActiveUIDocument.Document;
            Transaction trans = new(Document, "FindSouthFacingWallsWithoutProjectLocation");
            trans.Start();
            Execute(false);

            CloseFile();

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FindSouthFacingWallsWithProjectLocation : FindSouthFacingWalls, IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("6CADE602-7F32-496c-AA37-CEE4B0EE6087"));

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Document = revit.Application.ActiveUIDocument.Document;
            Transaction trans = new(Document, "FindSouthFacingWallsWithProjectLocation");
            trans.Start();
            Execute(true);

            CloseFile();
            trans.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FindSouthFacingWindowsWithoutProjectLocation : FindSouthFacingWindows, IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("AB3588F5-1CD1-4693-9DF0-C0890C811B21"));

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Document = revit.Application.ActiveUIDocument.Document;
            Transaction trans = new(Document, "FindSouthFacingWindowsWithoutProjectLocation");
            trans.Start();
            Execute(false);

            CloseFile();

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FindSouthFacingWindowsWithProjectLocation : FindSouthFacingWindows, IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("BFECDEA2-C384-4bcc-965E-EA302BA309AA"));

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Document = revit.Application.ActiveUIDocument.Document;

            Transaction trans = new(Document, "FindSouthFacingWindowsWithProjectLocation");
            trans.Start();

            Execute(true);

            CloseFile();
            trans.Commit();
            return Result.Succeeded;
        }
    }
}
