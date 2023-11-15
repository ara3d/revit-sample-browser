// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.RebarFreeForm.CS
{
    /// <summary>
    ///     This updater is used to regen rebar elements whenever a curveElement that was "Selected" is changed
    /// </summary>
    internal class CurveElementRegenUpdater : IUpdater
    {
        private static AddInId _appId;
        private static UpdaterId _updaterId;

        public CurveElementRegenUpdater(AddInId id)
        {
            _appId = id;
            _updaterId = new UpdaterId(_appId, new Guid("0935FACA-29B6-468A-95E1-D121BEE58B62"));
        }

        public void Execute(UpdaterData data)
        {
            try
            {
                var modifiedIds = data.GetModifiedElementIds();
                if (modifiedIds.Count > 0) // if any curveElement was modified
                {
                    //get all rebar elements anf filter them, to see which need to be notified of the change
                    var collector = new FilteredElementCollector(data.GetDocument());
                    var elemBars = collector.OfClass(typeof(Rebar)).ToElements();
                    foreach (var elem in elemBars)
                    {
                        if (!(elem is Rebar bar))
                            continue;
                        if (!bar.IsRebarFreeForm()) // only need free form bars
                            continue;
                        var barAccess = bar.GetFreeFormAccessor();
                        if (!barAccess.GetServerGUID()
                                .Equals(RebarUpdateServer.SampleGuid)) // only use our custom FreeForm
                            continue;
                        var paramCurveId = bar.LookupParameter(AddSharedParams.CurveIdName);
                        if (paramCurveId == null)
                            continue;
                        var id = ElementId.Parse(paramCurveId.AsString());
                        if (id == ElementId.InvalidElementId)
                            continue;
                        if (modifiedIds.Contains(id)) // if id of line is in the rebar, then trigger regen
                        {
                            var param = bar.LookupParameter(AddSharedParams.ParamName);
                            param.Set(param.AsInteger() == 0
                                ? 1
                                : 0); // just flip the value to register a change that will trigger the regeneration of that rebar on commit.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetAdditionalInformation()
        {
            return "this is a sample updater that reacts to changing model lines to change the rebar connected to it";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Structure;
        }

        public UpdaterId GetUpdaterId()
        {
            return _updaterId;
        }

        public string GetUpdaterName()
        {
            return "Line change updater";
        }
    }
}
