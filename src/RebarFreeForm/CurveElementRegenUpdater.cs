// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.RebarFreeForm.CS
{
    /// <summary>Flips the Updated shared param when a linked model curve changes to force free-form rebar regen.</summary>
    public class CurveElementRegenUpdater : IUpdater
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
                if (modifiedIds.Count > 0)
                {
                    FilteredElementCollector collector = new(data.GetDocument());
                    var elemBars = collector.OfClass(typeof(Rebar)).ToElements();
                    foreach (var elem in elemBars)
                    {
                        if (elem is not Rebar bar)
                            continue;
                        if (!bar.IsRebarFreeForm())
                            continue;
                        var barAccess = bar.GetFreeFormAccessor();
                        if (!barAccess.GetServerGUID()
                                .Equals(RebarUpdateServer.SampleGuid))
                            continue;
                        var paramCurveId = bar.LookupParameter(AddSharedParams.CurveIdName);
                        if (paramCurveId == null)
                            continue;
                        var id = ElementId.Parse(paramCurveId.AsString());
                        if (id == ElementId.InvalidElementId)
                            continue;
                        if (modifiedIds.Contains(id))
                        {
                            var param = bar.LookupParameter(AddSharedParams.ParamName);
                            // Toggle boolean so Revit sees a change on commit.
                            param.Set(param.AsInteger() == 0 ? 1 : 0);
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
