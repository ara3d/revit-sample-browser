//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ExternalCommandRegistration.CS
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
                "Revit.SDK.Samples.ExternalCommandRegistration");
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