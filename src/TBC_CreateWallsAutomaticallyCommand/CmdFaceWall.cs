#region Header

//
// CmdFaceWall.cs - demonstrate FaceWall.Create
//
// Create and insert a conceptual mass family instance, 
// then create sloped walls on all its faces.
//
// Copyright (C) 2014-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    #region Automatic Walls

    [Transaction(TransactionMode.Manual)]
    public class CreateWallsAutomaticallyCommand
        : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var cubes = Util.FindCubes(doc);

            using Transaction transaction = new(doc);
            transaction.Start("create walls");

            foreach (var cube in cubes)
            {
                var countours = Util.FindCountors(cube)
                    .SelectMany(x => x);

                var height = cube.LookupParameter("height")
                    .AsDouble();

                foreach (var countour in countours)
                {
                    var wall = Util.CreateWallForCube(cube, countour,
                        height);

                    Util.CreateDoorForWall(wall);
                }
            }

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    #endregion // Automatic Walls
}
