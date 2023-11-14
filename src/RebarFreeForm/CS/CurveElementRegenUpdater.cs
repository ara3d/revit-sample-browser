using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RvtUpdaterId = Autodesk.Revit.DB.UpdaterId;
using RvtAddinId = Autodesk.Revit.DB.AddInId;

namespace Revit.SDK.Samples.RebarFreeForm.CS
{
    /// <summary>
    ///     This updater is used to regen rebar elements whenever a curveElement that was "Selected" is changed
    /// </summary>
    internal class CurveElementRegenUpdater : IUpdater
    {
        private static AddInId m_appId;
        private static UpdaterId m_updaterId;

        public CurveElementRegenUpdater(AddInId id)
        {
            m_appId = id;
            m_updaterId = new UpdaterId(m_appId, new Guid("0935FACA-29B6-468A-95E1-D121BEE58B62"));
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
                        var bar = elem as Rebar;
                        if (bar == null)
                            continue;
                        if (!bar.IsRebarFreeForm()) // only need free form bars
                            continue;
                        var barAccess = bar.GetFreeFormAccessor();
                        if (!barAccess.GetServerGUID()
                                .Equals(RebarUpdateServer.SampleGuid)) // only use our custom FreeForm
                            continue;
                        var paramCurveId = bar.LookupParameter(AddSharedParams.m_CurveIdName);
                        if (paramCurveId == null)
                            continue;
                        var id = ElementId.Parse(paramCurveId.AsString());
                        if (id == ElementId.InvalidElementId)
                            continue;
                        if (modifiedIds.Contains(id)) // if id of line is in the rebar, then trigger regen
                        {
                            var param = bar.LookupParameter(AddSharedParams.m_paramName);
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
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "Line change updater";
        }
    }
}