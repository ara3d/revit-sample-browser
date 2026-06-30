#region Header

//
// CmdWallOpeningProfiles.cs - determine and display all wall opening face edges including elevation profile lines
//
// Copyright (C) 2015-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdWallOpeningProfiles : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var commandResult = Result.Succeeded;
            var cats = doc.Settings.Categories;

            var catDoorsId = cats.get_Item(
                BuiltInCategory.OST_Doors).Id;

            var catWindowsId = cats.get_Item(
                BuiltInCategory.OST_Windows).Id;

            try
            {
                var selectedIds = uidoc.Selection
                    .GetElementIds().ToList();

                using var trans = new Transaction(doc);
                trans.Start("Cmd: GetOpeningProfiles");

                var newIds = new List<ElementId>();

                foreach (var selectedId in selectedIds)
                    if (doc.GetElement(selectedId) is Wall wall)
                    {
                        var faceList = new List<PlanarFace>();

                        var insertIds = wall.FindInserts(
                            true, false, false, false).ToList();

                        foreach (var insertId in insertIds)
                        {
                            var elem = doc.GetElement(insertId);

                            switch (elem)
                            {
                                case FamilyInstance inst:
                                {
                                    var catType = inst.Category
                                        .CategoryType;

                                    var cat = inst.Category;

                                    if (catType == CategoryType.Model
                                        && (cat.Id == catDoorsId
                                            || cat.Id == catWindowsId))
                                        faceList.AddRange(
                                            Util.GetWallOpeningPlanarFaces(
                                                wall, insertId));
                                    break;
                                }
                                case Opening:
                                    faceList.AddRange(
                                        Util.GetWallOpeningPlanarFaces(
                                            wall, insertId));
                                    break;
                            }
                        }

                        foreach (var face in faceList)
                        {
                            //Plane facePlane = new Plane(
                            //  face.ComputeNormal( UV.Zero ), 
                            //  face.Origin ); // 2016

                            var facePlane = Plane.CreateByNormalAndOrigin(
                                face.ComputeNormal(UV.Zero),
                                face.Origin); // 2017

                            var sketchPlane
                                = SketchPlane.Create(doc, facePlane);

                            foreach (var curveLoop in
                                face.GetEdgesAsCurveLoops())
                            foreach (var curve in curveLoop)
                            {
                                var modelCurve = doc.Create
                                    .NewModelCurve(curve, sketchPlane);

                                newIds.Add(modelCurve.Id);
                            }
                        }
                    }

                if (newIds.Count > 0)
                {
                    var activeView = uidoc.ActiveGraphicalView;
                    activeView.IsolateElementsTemporary(newIds);
                }

                trans.Commit();
            }

            #region Exception Handling

            catch (ExternalApplicationException e)
            {
                message = e.Message;
                Debug.WriteLine(
                    $"Exception Encountered (Application)\n{e.Message}\nStack Trace: {e.StackTrace}");

                commandResult = Result.Failed;
            }
            catch (OperationCanceledException e)
            {
                Debug.WriteLine($"Operation cancelled. {e.Message}");

                message = "Operation cancelled.";

                commandResult = Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                Debug.WriteLine(
                    $"Exception Encountered (General)\n{e.Message}\nStack Trace: {e.StackTrace}");

                commandResult = Result.Failed;
            }

            #endregion

            return commandResult;
        }

    }
}