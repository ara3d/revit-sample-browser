#region Header

//
// CmdCropToRoom.cs - set 3D view crop box to room extents
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdCropToRoom : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            if (doc.ActiveView is not View3D view3d)
            {
                message = "Please activate a 3D view"
                          + " before running this command.";

                return Result.Failed;
            }

            using var t = new Transaction(doc);
            t.Start("Crop to Room");

            // get the 3d view crop box:

            var bb = view3d.CropBox;

            // get the transform from the current view
            // to the 3D model:

            var transform = bb.Transform;

            // get the transform from the 3D model
            // to the current view:

            var transformInverse = transform.Inverse;

            // get all rooms in the model:

            var collector
                = new FilteredElementCollector(doc);

            collector.OfClass(typeof(Room));
            var rooms = collector.ToElements();
            var n = rooms.Count;

            var room = 0 < n
                ? rooms[Util.BumpRoomIndex(n)] as Room
                : null;

            if (null == room)
            {
                message = "No room element found in project.";
                return Result.Failed;
            }

            // Collect all vertices of room closed shell
            // to determine its extents:

            var e = room.ClosedShell;
            var vertices = new List<XYZ>();

            //foreach( GeometryObject o in e.Objects ) // 2012

            foreach (var o in e) // 2013
                if (o is Solid solid)
                    // Iterate over all the edges of all solids:

                    foreach (Edge edge in solid.Edges)
                    foreach (var p in edge.Tessellate())
                        // Collect all vertices,
                        // including duplicates:

                        vertices.Add(p);

            var verticesIn3dView = new List<XYZ>();

            foreach (var p in vertices)
                verticesIn3dView.Add(
                    transformInverse.OfPoint(p));

            // Ignore the Z coorindates and find the
            // min and max X and Y in the 3d view:

            double xMin = 0, yMin = 0, xMax = 0, yMax = 0;

            var first = true;
            foreach (var p in verticesIn3dView)
                if (first)
                {
                    xMin = p.X;
                    yMin = p.Y;
                    xMax = p.X;
                    yMax = p.Y;
                    first = false;
                }
                else
                {
                    if (xMin > p.X)
                        xMin = p.X;
                    if (yMin > p.Y)
                        yMin = p.Y;
                    if (xMax < p.X)
                        xMax = p.X;
                    if (yMax < p.Y)
                        yMax = p.Y;
                }

            // Grow the crop box by one twentieth of its
            // size to include the walls of the room:

            var d = 0.05 * (xMax - xMin);
            xMin = xMin - d;
            xMax = xMax + d;

            d = 0.05 * (yMax - yMin);
            yMin = yMin - d;
            yMax = yMax + d;

            bb.Max = new XYZ(xMax, yMax, bb.Max.Z);
            bb.Min = new XYZ(xMin, yMin, bb.Min.Z);

            view3d.CropBox = bb;

            // Change the crop view setting manually or
            // programmatically to see the result:

            view3d.CropBoxActive = true;
            view3d.CropBoxVisible = true;
            t.Commit();

            return Result.Succeeded;
        }
    }
}