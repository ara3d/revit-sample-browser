// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Parameters;
using Ara3D.RevitSampleBrowser.Common.Structural;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using RebarGeomHelper = Ara3D.RevitSampleBrowser.Common.Structural.RebarGeometry;

namespace Ara3D.RevitSampleBrowser.AreaReinCurve.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private AreaReinforcement m_areaRein;
        private List<AreaReinforcementCurve> m_areaReinCurves;
        private Document m_doc;

        public Result Execute(ExternalCommandData revit,
            ref string message, ElementSet elements)
        {
            Transaction trans = new(revit.Application.ActiveUIDocument.Document, "Ara3D.RevitSampleBrowser.AreaReinCurve");
            trans.Start();
            ElementSet selected = new();
            foreach (var elementId in revit.Application.ActiveUIDocument.Selection.GetElementIds())
            {
                selected.Insert(revit.Application.ActiveUIDocument.Document.GetElement(elementId));
            }

            try
            {
                m_doc = revit.Application.ActiveUIDocument.Document;

                if (!PreData(selected))
                {
                    message = "Please select only one rectangular AreaReinforcement.";
                    trans.RollBack();
                    return Result.Failed;
                }

                if (!TurnOffLayers())
                {
                    message = "Can't turn off layers as expected or can't find these layers.";
                    trans.RollBack();
                    return Result.Failed;
                }

                if (!ChangeHookType())
                {
                    message = "Can't remove HookTypes as expected.";
                    trans.RollBack();
                    return Result.Failed;
                }
            }
            catch (ApplicationException appEx)
            {
                message = appEx.ToString();
                trans.RollBack();
                return Result.Failed;
            }
            catch
            {
                message = "Unexpected error happens.";
                trans.RollBack();
                return Result.Failed;
            }

            var msg = "All layers but Major Direction Layer or Exterior Direction Layer ";
            msg += "have been turn off; ";
            msg += "Removed the Hooks from one boundary curve of the Major Direction Layer ";
            msg += "or Exterior Direction Layer.";
            TaskDialog.Show("Revit", msg);
            trans.Commit();
            return Result.Succeeded;
        }

        private bool PreData(ElementSet selected)
        {
            if (selected.Size != 1) return false;
            foreach (var o in selected)
            {
                m_areaRein = o as AreaReinforcement;
                if (null == m_areaRein) return false;
            }

            CurveArray curves = new();
            m_areaReinCurves = [];
            var curveIds = m_areaRein.GetBoundaryCurveIds();
            foreach (var o in curveIds)
            {
                if (m_doc.GetElement(o) is not AreaReinforcementCurve areaCurve)
                {
                    ApplicationException appEx = new("There is unexpected error with selected AreaReinforcement.");
                    throw appEx;
                }

                m_areaReinCurves.Add(areaCurve);
                curves.Append(areaCurve.Curve);
            }

            var flag = AreaReinforcementHelper.IsRectangular(curves);

            return flag;
        }

        private bool TurnOffLayers()
        {
            var flag = ParameterAccess.SetParaInt(m_areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1, 0);
            flag &= ParameterAccess.SetParaInt(m_areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2, 0);
            flag &= ParameterAccess.SetParaInt(m_areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2, 0);

            if (!flag)
            {
                flag = true;
                flag &= ParameterAccess.SetParaInt(m_areaRein, "Interior Major Direction", 0);
                flag &= ParameterAccess.SetParaInt(m_areaRein, "Exterior Minor Direction", 0);
                flag &= ParameterAccess.SetParaInt(m_areaRein, "Interior Minor Direction", 0);
            }

            return flag;
        }

        private bool ChangeHookType()
        {
            var line0 = m_areaReinCurves[0].Curve as Line;
            var line1 = m_areaReinCurves[1].Curve as Line;
            var temp = RebarGeomHelper.IsVertical(line0, line1) ? m_areaReinCurves[1] : m_areaReinCurves[2];
            ParameterAccess.SetParaInt(m_areaReinCurves[0],
                BuiltInParameter.REBAR_SYSTEM_OVERRIDE, -1);
            var para = m_areaReinCurves[0].get_Parameter(
                BuiltInParameter.REBAR_SYSTEM_HOOK_TYPE_TOP_DIR_1);
            var flag = ParameterAccess.SetParaNullId(para);

            ParameterAccess.SetParaInt(temp, BuiltInParameter.REBAR_SYSTEM_OVERRIDE, -1);
            para = temp.get_Parameter(
                BuiltInParameter.REBAR_SYSTEM_HOOK_TYPE_TOP_DIR_1);
            flag &= ParameterAccess.SetParaNullId(para);

            return flag;
        }
    }
}
