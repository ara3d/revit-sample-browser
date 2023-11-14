// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitMultiSample.Toposolid.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ToposolidCreation : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;
                var typeId = new FilteredElementCollector(doc).OfClass(typeof(ToposolidType)).OfType<ToposolidType>()
                    .FirstOrDefault()?.Id;
                if (typeId == null)
                {
                    TaskDialog.Show("Error", "Can not find a valid ToposolidType");
                    return Result.Failed;
                }

                var levelId = new FilteredElementCollector(doc).OfClass(typeof(Level)).OfType<Level>().FirstOrDefault()
                    ?.Id;
                if (levelId == null)
                {
                    TaskDialog.Show("Error", "Can not find a valid Level");
                    return Result.Failed;
                }

                var pt1 = XYZ.Zero;
                var pt2 = new XYZ(100, 0, 0);
                var pt3 = new XYZ(100, 100, 0);
                var pt4 = new XYZ(0, 100, 0);
                var pt5 = new XYZ(20, 50, 20);
                var pt6 = new XYZ(50, 150, 20);
                var points = new List<XYZ> { pt1, pt2, pt3, pt4, pt5, pt6 };
                var l1 = Line.CreateBound(pt1, pt2);
                var l2 = Line.CreateBound(pt2, pt3);
                var l3 = Line.CreateBound(pt3, pt4);
                var l4 = Line.CreateBound(pt4, pt1);
                var profile = CurveLoop.Create(new List<Curve> { l1, l2, l3, l4 });

                using (var transaction = new Transaction(doc, "create"))
                {
                    transaction.Start();
                    //Toposolid topo = Toposolid.Create(doc, new List<CurveLoop> { m_Profile}, typeId, levelId);
                    //Toposolid topo = Toposolid.Create(doc, m_Points, typeId, levelId);
                    var topo = Autodesk.Revit.DB.Toposolid.Create(doc, new List<CurveLoop> { profile }, points, typeId,
                        levelId);
                    topo.CreateSubDivision(doc,
                        new List<CurveLoop> { CurveLoop.CreateViaOffset(profile, -20, XYZ.BasisZ) });
                    transaction.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }

    /// <summary>
    ///     Create toposolid from dwg
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ToposolidFromDwg : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var typeId = new FilteredElementCollector(doc).OfClass(typeof(ToposolidType)).OfType<ToposolidType>()
                .FirstOrDefault()?.Id;
            if (typeId == null)
            {
                TaskDialog.Show("Error", "Can not find a valid ToposolidType");
                return Result.Failed;
            }

            var levelId = new FilteredElementCollector(doc).OfClass(typeof(Level)).OfType<Level>().FirstOrDefault()?.Id;
            if (levelId == null)
            {
                TaskDialog.Show("Error", "Can not find a valid Level");
                return Result.Failed;
            }

            var ptList = new List<XYZ>();
            var element = doc.GetElement(sel
                .PickObject(ObjectType.Element, new ImportInstanceFilter(), "pick an imported dwg file").ElementId);

            //List<Curve> curves = sel.PickObjects(ObjectType.Element, new ModelCurveFilter()).Select(e => (doc.GetElement(e.ElementId) as ModelCurve).GeometryCurve).ToList();
            //CurveLoop cl = CurveLoop.Create(curves);
            var objects = element.get_Geometry(new Options()).ToList();
            foreach (var gObject in objects)
            {
                var gInstance = gObject as GeometryInstance;
                if (gInstance != null)
                {
                    var ge = gInstance.GetSymbolGeometry();
                    var glist = ge.ToList();
                    foreach (var obj in glist)
                        switch (obj)
                        {
                            case PolyLine polyLine:
                                ptList.AddRange(polyLine.GetCoordinates());
                                break;
                            case Line line:
                                ptList.Add(line.GetEndPoint(0));
                                ptList.Add(line.GetEndPoint(1));
                                break;
                        }
                }
            }

            var xMin = ptList.Min(pt => pt.Z);
            var offsetPtList = new List<XYZ>();
            ptList.ForEach(pt => offsetPtList.Add(pt - new XYZ(0, 0, xMin)));
            using (var transaction = new Transaction(doc, "create"))
            {
                transaction.Start();
                var topo = Autodesk.Revit.DB.Toposolid.Create(doc, offsetPtList, typeId, levelId);
                //Autodesk.Revit.DB.Toposolid topo = Autodesk.Revit.DB.Toposolid.Create(doc, new List<CurveLoop> { cl }, offsetPtList, typeId, levelId);
                topo.get_Parameter(BuiltInParameter.TOPOSOLID_HEIGHTABOVELEVEL_PARAM).Set(xMin);
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Add contour setting items to toposolid type
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ContourSettingCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var topoType = new FilteredElementCollector(doc).OfClass(typeof(ToposolidType)).OfType<ToposolidType>()
                .FirstOrDefault();
            if (topoType == null)
            {
                TaskDialog.Show("Error", "Can not find a valid ToposolidType");
                return Result.Failed;
            }

            var contourSetting = topoType.GetContourSetting();
            using (var trans = new Transaction(doc, "contour"))
            {
                trans.Start();
                contourSetting.AddContourRange(1.0, 9.0, 2.0, new ElementId(BuiltInCategory.OST_ToposolidContours));
                contourSetting.AddSingleContour(10, new ElementId(BuiltInCategory.OST_ToposolidSecondaryContours));
                contourSetting.AddSingleContour(11.5, new ElementId(BuiltInCategory.OST_ToposolidSplitLines));
                contourSetting.AddSingleContour(13, new ElementId(BuiltInCategory.OST_ToposolidSplitLines));
                trans.Commit();
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Modify the current contoursetting of the toposolid type
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ContourSettingModification : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var topoType = new FilteredElementCollector(doc).OfClass(typeof(ToposolidType)).OfType<ToposolidType>()
                .FirstOrDefault();
            if (topoType == null)
            {
                TaskDialog.Show("Error", "Can not find a valid ToposolidType");
                return Result.Failed;
            }

            var contourSetting = topoType.GetContourSetting();
            var items = contourSetting.GetContourSettingItems().ToList();
            if (items.Count != 4)
            {
                TaskDialog.Show("Error", "Not expected contour setting items count");
                return Result.Failed;
            }

            using (var trans = new Transaction(doc, "contour"))
            {
                trans.Start();
                contourSetting.DisableItem(items[0]);
                //contourSetting.EnableItem(items[0]);
                //contourSetting.RemoveItem(items[1]);
                trans.Commit();
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Create a toposolid from an existing topography surface
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ToposolidFromSurface : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var typeId = new FilteredElementCollector(doc).OfClass(typeof(ToposolidType)).OfType<ToposolidType>()
                .FirstOrDefault()?.Id;
            if (typeId == null)
            {
                TaskDialog.Show("Error", "Can not find a valid ToposolidType");
                return Result.Failed;
            }

            var levelId = new FilteredElementCollector(doc).OfClass(typeof(Level)).OfType<Level>().FirstOrDefault()?.Id;
            if (levelId == null)
            {
                TaskDialog.Show("Error", "Can not find a valid Level");
                return Result.Failed;
            }

            var surface =
                doc.GetElement(sel.PickObject(ObjectType.Element, new TopographySurfaceFilter(),
                    "pick a topography surface")) as TopographySurface;

            using (var transaction = new Transaction(doc, "create"))
            {
                transaction.Start();
                var topo = Autodesk.Revit.DB.Toposolid.CreateFromTopographySurface(doc, surface.Id, typeId, levelId);
                transaction.Commit();
                topo.GetSubDivisionIds().ToList();
                //TaskDialog.Show("test", ids.Count.ToString());
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Set the SSE point visibility
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SsePointVisibility : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            using (var transaction = new Transaction(doc, "modify"))
            {
                transaction.Start();
                SSEPointVisibilitySettings.SetVisibility(doc, new ElementId(BuiltInCategory.OST_Toposolid), false);
                transaction.Commit();
            }

            SSEPointVisibilitySettings.GetVisibility(doc, new ElementId(BuiltInCategory.OST_Toposolid));
            //TaskDialog.Show("test", visible.ToString());
            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Split a toposolid by selected model curves
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SplitToposolid : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var topo =
                doc.GetElement(sel.PickObject(ObjectType.Element,
                    new ToposolidFilter())) as Autodesk.Revit.DB.Toposolid;

            var curveList = new List<Curve>();
            sel.PickObjects(ObjectType.Element, new ModelCurveFilter()).ToList()
                .ForEach(x => curveList.Add((doc.GetElement(x) as ModelCurve).GeometryCurve));
            var cl = CurveLoop.Create(curveList);

            using (var transaction = new Transaction(doc, "split"))
            {
                transaction.Start();
                topo.Split(new List<CurveLoop> { cl });
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Simplify a toposolid by reducing its inner vertices.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SimplifyToposolid : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var topo =
                doc.GetElement(sel.PickObject(ObjectType.Element,
                    new ToposolidFilter())) as Autodesk.Revit.DB.Toposolid;

            using (var transaction = new Transaction(doc, "simplify"))
            {
                transaction.Start();
                topo.Simplify(0.6);
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     ImportInstanceFilter
    /// </summary>
    public class ImportInstanceFilter : ISelectionFilter
    {
        /// <summary>
        ///     Interface implementation
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool AllowElement(Element elem)
        {
            return elem is ImportInstance;
        }

        /// <summary>
        ///     Interface implementation
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    /// <summary>
    ///     TopographySurfaceFilter
    /// </summary>
    public class TopographySurfaceFilter : ISelectionFilter
    {
        /// <summary>
        ///     Interface implementation
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool AllowElement(Element elem)
        {
            return elem is TopographySurface;
        }

        /// <summary>
        ///     Interface implementation
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    /// <summary>
    ///     ModelCurveFilter
    /// </summary>
    public class ModelCurveFilter : ISelectionFilter
    {
        /// <summary>
        ///     Interface implementation
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool AllowElement(Element elem)
        {
            return elem is ModelCurve;
        }

        /// <summary>
        ///     Interface implementation
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    /// <summary>
    ///     ToposolidFilter
    /// </summary>
    public class ToposolidFilter : ISelectionFilter
    {
        /// <summary>
        ///     Interface implementation
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool AllowElement(Element elem)
        {
            return elem is Autodesk.Revit.DB.Toposolid;
        }

        /// <summary>
        ///     Interface implementation
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
