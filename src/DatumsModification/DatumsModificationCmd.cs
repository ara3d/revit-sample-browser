// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using View = Autodesk.Revit.DB.View;
namespace Ara3D.RevitSampleBrowser.DatumsModification.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DatumStyleModification : IExternalCommand
    {
        public static bool ShowLeftBubble = false;

        public static bool ShowRightBubble = false;

        public static bool AddLeftElbow = false;

        public static bool AddRightElbow = false;

        public static bool ChangeLeftEnd2D = false;

        public static bool ChangeRightEnd2D = false;

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                var datums = commandData.Application.ActiveUIDocument.Selection.GetElementIds();
                var view = commandData.Application.ActiveUIDocument.ActiveView;
                if (datums == null || datums.Count == 0)
                    return Result.Cancelled;
                using DatumStyleSetting settingForm = new();
                if (settingForm.ShowDialog() == DialogResult.OK)
                    using (Transaction tran = new(document, "StyleModification"))
                    {
                        tran.Start();
                        foreach (var datumRef in datums)
                        {
                            var datum = document.GetElement(datumRef) as DatumPlane;
                            if (ShowLeftBubble)
                                datum.ShowBubbleInView(DatumEnds.End0, view);
                            else
                                datum.HideBubbleInView(DatumEnds.End0, view);
                            if (ShowRightBubble)
                                datum.ShowBubbleInView(DatumEnds.End1, view);
                            else
                                datum.HideBubbleInView(DatumEnds.End1, view);
                            if (ChangeLeftEnd2D)
                                datum.SetDatumExtentType(DatumEnds.End0, view, DatumExtentType.ViewSpecific);
                            else
                                datum.SetDatumExtentType(DatumEnds.End0, view, DatumExtentType.Model);
                            if (ChangeRightEnd2D)
                                datum.SetDatumExtentType(DatumEnds.End1, view, DatumExtentType.ViewSpecific);
                            else
                                datum.SetDatumExtentType(DatumEnds.End1, view, DatumExtentType.Model);
                            if (AddLeftElbow && datum.GetLeader(DatumEnds.End0, view) == null)
                            {
                                datum.AddLeader(DatumEnds.End0, view);
                            }
                            else if (datum.GetLeader(DatumEnds.End0, view) != null)
                            {
                                var leader = datum.GetLeader(DatumEnds.End0, view);
                                leader = SampleBrowserUtils.CalculateLeader(leader, AddLeftElbow);
                                datum.SetLeader(DatumEnds.End0, view, leader);
                            }

                            if (AddRightElbow && datum.GetLeader(DatumEnds.End1, view) == null)
                            {
                                datum.AddLeader(DatumEnds.End1, view);
                            }
                            else if (datum.GetLeader(DatumEnds.End1, view) != null)
                            {
                                var leader = datum.GetLeader(DatumEnds.End1, view);
                                leader = SampleBrowserUtils.CalculateLeader(leader, AddRightElbow);
                                datum.SetLeader(DatumEnds.End1, view, leader);
                            }
                        }

                        tran.Commit();
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DatumAlignment : IExternalCommand
    {
        public static readonly Dictionary<string, DatumPlane> DatumDic = [];

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                var view = commandData.Application.ActiveUIDocument.ActiveView;
                DatumDic.Clear();
                var datums = commandData.Application.ActiveUIDocument.Selection.GetElementIds();
                if (datums == null || datums.Count == 0)
                    return Result.Cancelled;

                foreach (var datumRef in datums)
                {
                    var datum = document.GetElement(datumRef) as DatumPlane;
                    if (!DatumDic.Keys.Contains(datum.Name)) DatumDic.Add(datum.Name, datum);
                }

                using AlignmentSetting settingForm = new();
                if (settingForm.ShowDialog() == DialogResult.OK)
                {
                    var selectedDatum = DatumDic[settingForm.datumList.SelectedItem.ToString()];
                    var baseCurve = selectedDatum.GetCurvesInView(DatumExtentType.ViewSpecific, view).ElementAt(0);
                    var baseLine = baseCurve as Line;
                    var baseDirect = baseLine.Direction;

                    using Transaction tran = new(document, "DatumAlignment");
                    tran.Start();

                    foreach (var datum in DatumDic.Values)
                    {
                        var curve = datum
                            .GetCurvesInView(datum.GetDatumExtentTypeInView(DatumEnds.End0, view), view)
                            .ElementAt(0);
                        var newCurve = XyzMath.CalculateAlignedCurve(curve, baseLine, baseDirect);
                        datum.SetCurveInView(datum.GetDatumExtentTypeInView(DatumEnds.End0, view), view,
                            newCurve);
                    }

                    tran.Commit();
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DatumPropagation : IExternalCommand
    {
        public static readonly Dictionary<string, ElementId> ViewDic = [];

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                var view = commandData.Application.ActiveUIDocument.ActiveView;
                ViewDic.Clear();
                var datumRef = commandData.Application.ActiveUIDocument.Selection.GetElementIds().First();
                if (datumRef == null)
                    return Result.Cancelled;

                var datum = document.GetElement(datumRef) as DatumPlane;
                var viewList = datum.GetPropagationViews(view);
                foreach (var id in viewList)
                {
                    var pView = document.GetElement(id) as View;
                    if (!ViewDic.Keys.Contains($"{pView.ViewType} : {pView.Name}"))
                        ViewDic.Add($"{pView.ViewType} : {pView.Name}", id);
                }

                using PropogateSetting settingForm = new();
                if (settingForm.ShowDialog() == DialogResult.OK)
                {
                    var pViewList = new List<ElementId>() as ISet<ElementId>;
                    foreach (var item in settingForm.propagationViewList.CheckedItems)
                    {
                        var selectedView = ViewDic[item.ToString()];
                        pViewList.Add(selectedView);
                    }

                    using Transaction tran = new(document, "propagation");
                    tran.Start();
                    datum.PropagateToViews(view, pViewList);
                    tran.Commit();
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
}
