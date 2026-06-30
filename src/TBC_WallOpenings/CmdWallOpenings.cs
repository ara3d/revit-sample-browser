#region Header

//
// CmdWallOpenings.cs - determine wall opening side faces and report their start and end points along location line
//
// Copyright (C) 2015-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdWallOpenings : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            if (null == doc)
            {
                message = "Please run this command in a valid document.";
                return Result.Failed;
            }

            if (doc.ActiveView is not View3D view)
            {
                message = "Please run this command in a 3D view.";
                return Result.Failed;
            }

            var e = Util.SelectSingleElementOfType(
                uidoc, typeof(Wall), "wall", true);

            var openings = Util.GetWallOpenings(
                e as Wall, view);

            var n = openings.Count;

            var msg = $"{n} opening{Util.PluralSuffix(n)} found{Util.DotOrColon(n)}";

            Util.InfoMsg2(msg, string.Join(
                "\r\n", openings));

            return Result.Succeeded;
        }

        #region Determine walls in linked file intersecting pipe

        public void GetWalls(UIDocument uidoc)
        {
            var doc = uidoc.Document;

            var pipeRef = uidoc.Selection.PickObject(
                ObjectType.Element);

            var pipeElem = doc.GetElement(pipeRef);

            var lc = pipeElem.Location as LocationCurve;
            var curve = lc.Curve;

            var reference1 = new ReferenceComparer();

            ElementFilter filter = new ElementCategoryFilter(
                BuiltInCategory.OST_Walls);

            var collector
                = new FilteredElementCollector(doc);

            Func<View3D, bool> isNotTemplate = v3 => !v3.IsTemplate;
            var view3D = collector
                .OfClass(typeof(View3D))
                .Cast<View3D>()
                .First(isNotTemplate);

            var refIntersector
                = new ReferenceIntersector(
                    filter, FindReferenceTarget.Element, view3D)
                {
                    FindReferencesInRevitLinks = true
                };
            var referenceWithContext
                = refIntersector.Find(
                    curve.GetEndPoint(0),
                    (curve as Line).Direction);

            IList<Reference> references
                = referenceWithContext
                    .Select(p => p.GetReference())
                    .Distinct(reference1)
                    .Where(p => p.GlobalPoint.DistanceTo(
                        curve.GetEndPoint(0)) < curve.Length)
                    .ToList();

            IList<Element> walls = [];
            foreach (var reference in references)
            {
                var instance = doc.GetElement(reference)
                    as RevitLinkInstance;
                var linkDoc = instance.GetLinkDocument();
                var element = linkDoc.GetElement(reference.LinkedElementId);
                walls.Add(element);
            }

            TaskDialog.Show("Count of wall", walls.Count.ToString());
        }

        public class ReferenceComparer : IEqualityComparer<Reference>
        {
            public bool Equals(Reference x, Reference y)
            {
                return x.ElementId == y.ElementId && x.LinkedElementId == y.LinkedElementId;
            }

            public int GetHashCode(Reference obj)
            {
                var hashName = obj.ElementId.GetHashCode();
                var hashId = obj.LinkedElementId.GetHashCode();
                return hashId ^ hashId;
            }
        }

        public string GetFaceRefRepresentation(
            Wall wall,
            Document doc,
            RevitLinkInstance instance)
        {
            var faceRef = HostObjectUtils.GetSideFaces(
                wall, ShellLayerType.Exterior).FirstOrDefault();
            var stRef = faceRef.CreateLinkReference(instance);
            var stable = stRef.ConvertToStableRepresentation(doc);
            return stable;
        }

        #endregion // Determine walls in linked file intersecting pipe

        #region Find Beams and Slabs intersecting Columns

        // https://forums.autodesk.com/t5/revit-api-forum/ray-projection-not-picking-up-beams/m-p/10388868
        private void AdjustColumnHeightsUsingBoundingBox(
            Document doc,
            IList<ElementId> ids)
        {
            var view = doc.ActiveView;

            var allColumns = 0;
            var successColumns = 0;

            if (view is View3D)
            {
                using (var tx = new Transaction(doc))
                {
                    tx.Start("Adjust Column Heights");

                    foreach (var elemId in ids)
                    {
                        var elem = doc.GetElement(elemId);

                        if ((BuiltInCategory)elem.Category.Id.Value
                            == BuiltInCategory.OST_StructuralColumns)
                        {
                            allColumns++;

                            var column = elem as FamilyInstance;

                            var builtInCats = new List<BuiltInCategory>
                            {
                                BuiltInCategory.OST_Floors,
                                BuiltInCategory.OST_StructuralFraming
                            };
                            var beamSlabFilter
                                = new ElementMulticategoryFilter(builtInCats);

                            var bb = elem.get_BoundingBox(view);
                            var myOutLn = new Outline(bb.Min, bb.Max + (100 * XYZ.BasisZ));
                            var bbFilter
                                = new BoundingBoxIntersectsFilter(myOutLn);

                            var collector
                                = new FilteredElementCollector(doc)
                                    .WherePasses(beamSlabFilter)
                                    .WherePasses(bbFilter);

                            var intersectingBeams = new List<Element>();
                            var intersectingSlabs = new List<Element>();

                            if (ColumnAttachment.GetColumnAttachment(
                                column, 1) != null)
                            {
                                var color = new Color(0, 255, 0);
                                var ogs = new OverrideGraphicSettings();
                                ogs.SetProjectionLineColor(color);
                                view.SetElementOverrides(elem.Id, ogs);
                            }
                            else
                            {
                                foreach (var e in collector)
                                    switch (e.Category.Name)
                                    {
                                        case "Structural Framing":
                                            intersectingBeams.Add(e);
                                            break;
                                        case "Floors":
                                            intersectingSlabs.Add(e);
                                            break;
                                    }

                                if (intersectingBeams.Any())
                                {
                                    var lowestBottomElem = intersectingBeams.First();
                                    foreach (var beam in intersectingBeams)
                                    {
                                        var thisBeamBB = beam.get_BoundingBox(view);
                                        var currentLowestBB = lowestBottomElem.get_BoundingBox(view);
                                        if (thisBeamBB.Min.Z < currentLowestBB.Min.Z) lowestBottomElem = beam;
                                    }

                                    ColumnAttachment.AddColumnAttachment(
                                        doc, column, lowestBottomElem, 1,
                                        ColumnAttachmentCutStyle.None,
                                        ColumnAttachmentJustification.Minimum,
                                        0);
                                    successColumns++;
                                }
                                else if (intersectingSlabs.Any())
                                {
                                    var lowestBottomElem = intersectingSlabs.First();
                                    foreach (var slab in intersectingSlabs)
                                    {
                                        var thisSlabBB = slab.get_BoundingBox(view);
                                        var currentLowestBB = lowestBottomElem.get_BoundingBox(view);
                                        if (thisSlabBB.Min.Z < currentLowestBB.Min.Z) lowestBottomElem = slab;
                                    }

                                    ColumnAttachment.AddColumnAttachment(
                                        doc, column, lowestBottomElem, 1,
                                        ColumnAttachmentCutStyle.None,
                                        ColumnAttachmentJustification.Minimum,
                                        0);
                                    successColumns++;
                                }
                                else
                                {
                                    var color = new Color(255, 0, 0);
                                    var ogs = new OverrideGraphicSettings();
                                    ogs.SetProjectionLineColor(color);
                                    view.SetElementOverrides(elem.Id, ogs);
                                }
                            }
                        }
                    }

                    tx.Commit();
                }

                TaskDialog.Show("Columns Changed",
                    $"{successColumns} of {allColumns} Columns Changed");
            }
            else
            {
                TaskDialog.Show("Revit", "Run Script in 3D View.");
            }
        }

        private void AdjustColumnHeightsUsingReferenceIntersector(
            Document doc,
            IList<ElementId> ids)
        {
            if (doc.ActiveView is not View3D view)
                throw new Exception(
                    "Please run this command in a 3D view.");

            var allColumns = 0;
            var successColumns = 0;

            using var tx = new Transaction(doc);
            tx.Start("Attach Columns Tops");

            foreach (var elemId in ids)
            {
                var elem = doc.GetElement(elemId);

                if ((BuiltInCategory)elem.Category.Id.Value
                    == BuiltInCategory.OST_StructuralColumns)
                {
                    allColumns++;

                    var column = elem as FamilyInstance;

                    var builtInCats = new List<BuiltInCategory>
                    {
                        BuiltInCategory.OST_Floors,
                        BuiltInCategory.OST_StructuralFraming
                    };
                    var filter
                        = new ElementMulticategoryFilter(builtInCats);

                    if (ColumnAttachment.GetColumnAttachment(column, 1) != null) ColumnAttachment.RemoveColumnAttachment(column, 1);

                    var elemBB = elem.get_BoundingBox(view);

                    var elemLoc = (elem.Location as LocationPoint).Point;
                    var elemCenter = new XYZ(elemLoc.X, elemLoc.Y, elemLoc.Z + 0.1);
                    var b1 = new XYZ(elemBB.Min.X, elemBB.Min.Y, elemBB.Min.Z + 0.1);
                    var b2 = new XYZ(elemBB.Max.X, elemBB.Max.Y, elemBB.Min.Z + 0.1);
                    var b3 = new XYZ(elemBB.Min.X, elemBB.Max.Y, elemBB.Min.Z + 0.1);
                    var b4 = new XYZ(elemBB.Max.X, elemBB.Min.Y, elemBB.Min.Z + 0.1);

                    var points = new List<XYZ>(5)
                    {
                        b1,
                        b2,
                        b3,
                        b4,
                        elemCenter
                    };

                    var refI = new ReferenceIntersector(
                        filter, FindReferenceTarget.All, view);

                    var rayd = XYZ.BasisZ;
                    ReferenceWithContext refC = null;
                    foreach (var pt in points)
                    {
                        refC = refI.FindNearest(pt, rayd);
                        if (refC != null) break;
                    }

                    if (refC != null)
                    {
                        var reference = refC.GetReference();
                        var id = reference.ElementId;
                        var e = doc.GetElement(id);

                        ColumnAttachment.AddColumnAttachment(
                            doc, column, e, 1,
                            ColumnAttachmentCutStyle.None,
                            ColumnAttachmentJustification.Minimum,
                            0);

                        successColumns++;
                    }
                    else
                    {
                        var color = new Color(255, 0, 0);
                        var ogs = new OverrideGraphicSettings();
                        ogs.SetProjectionLineColor(color);
                        view.SetElementOverrides(elem.Id, ogs);
                    }
                }
            }

            tx.Commit();
        }

        #endregion // Find Beams and Slabs intersecting Columns
    }
}
