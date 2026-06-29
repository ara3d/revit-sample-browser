// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.DatumsModification.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DatumStyleModification : IExternalCommand
    {
        /// <summary>
        /// </summary>
        public static bool ShowLeftBubble = false;

        /// <summary>
        /// </summary>
        public static bool ShowRightBubble = false;

        /// <summary>
        /// </summary>
        public static bool AddLeftElbow = false;

        /// <summary>
        /// </summary>
        public static bool AddRightElbow = false;

        /// <summary>
        /// </summary>
        public static bool ChangeLeftEnd2D = false;

        /// <summary>
        /// </summary>
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
                //// Show UI                
                using (var settingForm = new DatumStyleSetting())
                {
                    if (settingForm.ShowDialog() == DialogResult.OK)
                        using (var tran = new Transaction(document, "StyleModification"))
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
        /// <summary>
        /// </summary>
        public static readonly Dictionary<string, DatumPlane> DatumDic = new Dictionary<string, DatumPlane>();

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

                //// Show UI                
                using (var settingForm = new AlignmentSetting())
                {
                    if (settingForm.ShowDialog() == DialogResult.OK)
                    {
                        var selectedDatum = DatumDic[settingForm.datumList.SelectedItem.ToString()];
                        var baseCurve = selectedDatum.GetCurvesInView(DatumExtentType.ViewSpecific, view).ElementAt(0);
                        var baseLine = baseCurve as Line;
                        var baseDirect = baseLine.Direction;

                        using (var tran = new Transaction(document, "DatumAlignment"))
                        {
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
                    }
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
        /// <summary>
        /// </summary>
        public static readonly Dictionary<string, ElementId> ViewDic = new Dictionary<string, ElementId>();

        /// <summary>
        ///     Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">
        ///     An object that is passed to the external application
        ///     which contains data related to the command,
        ///     such as the application object and active view.
        /// </param>
        /// <param name="message">
        ///     A message that can be set by the external application
        ///     which will be displayed if a failure or cancellation is returned by
        ///     the external command.
        /// </param>
        /// <param name="elements">
        ///     A set of elements to which the external application
        ///     can add elements that are to be highlighted in case of failure or cancellation.
        /// </param>
        /// <returns>
        ///     Return the status of the external command.
        ///     A result of Succeeded means that the API external method functioned as expected.
        ///     Cancelled can be used to signify that the user cancelled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
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

                //// Show UI                
                using (var settingForm = new PropogateSetting())
                {
                    if (settingForm.ShowDialog() == DialogResult.OK)
                    {
                        var pViewList = new List<ElementId>() as ISet<ElementId>;
                        foreach (var item in settingForm.propagationViewList.CheckedItems)
                        {
                            var selectedView = ViewDic[item.ToString()];
                            pViewList.Add(selectedView);
                        }

                        using (var tran = new Transaction(document, "propagation"))
                        {
                            tran.Start();
                            datum.PropagateToViews(view, pViewList);
                            tran.Commit();
                        }
                    }
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
