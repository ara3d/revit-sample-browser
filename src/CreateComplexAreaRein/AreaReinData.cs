// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS
{
    /// <summary>
    ///     data and data manager of the AreaReinforcement
    /// </summary>
    public class AreaReinData
    {
        private readonly Document m_doc;

        /// <summary>
        ///     constructor
        /// </summary>
        public AreaReinData(Document doc)
        {
            m_doc = doc;
        }

        /// <summary>
        ///     Parameter LayoutRule of AreaReinforcement
        /// </summary>
        public LayoutRules LayoutRule { get; set; } = LayoutRules.MaximumSpacing;

        /// <summary>
        ///     set the parameters to given AreaReinforcement
        /// </summary>
        /// <param name="areaRein"></param>
        public virtual void FillIn(AreaReinforcement areaRein)
        {
            var temp = (int)LayoutRule;
            var flag = ParameterUtil.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_LAYOUT_RULE, temp);
            //if BuiltInParameter doesn't work
            if (!flag)
            {
                var paraLayout = ParameterUtil.FindParaByName(
                    areaRein.Parameters, "Layout Rule");
                paraLayout?.Set(temp);
            }

            ChangeAreaReinCurves(areaRein);
        }

        /// <summary>
        ///     On the interior 4 curves, set the override flag
        ///     and flip the hooks on the top 2 layers to "up"
        /// </summary>
        /// <param name="areaRein"></param>
        private void ChangeAreaReinCurves(AreaReinforcement areaRein)
        {
            //interior 4 curves are listed in the back of the curves, 
            //this order is decided when we create it
            var curveIds = areaRein.GetBoundaryCurveIds();
            for (var i = 4; i < 8; i++)
            {
                var areaReinCurve =
                    m_doc.GetElement(curveIds[i]) as AreaReinforcementCurve;
                //remove hooks, set the hook the top 2 layers to 'up'
                ParameterUtil.SetParaInt(areaReinCurve,
                    BuiltInParameter.REBAR_SYSTEM_OVERRIDE, -1);
                ParameterUtil.SetParaInt(areaReinCurve,
                    BuiltInParameter.REBAR_SYSTEM_HOOK_ORIENT_TOP_DIR_1,
                    (int)HookOrientation.Up);
                ParameterUtil.SetParaInt(areaReinCurve,
                    BuiltInParameter.REBAR_SYSTEM_HOOK_ORIENT_TOP_DIR_2,
                    (int)HookOrientation.Up);
            }
        }
    }
}
