// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.AreaReinParameters.CS;
using Ara3D.RevitSampleBrowser.Common.Parameters;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.ComponentModel;
namespace Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS
{
    public class AreaReinData
    {
        public AreaReinData()
        {
        }

        [Category("Construction")]
        [DefaultValue(
            LayoutRules.MaximumSpacing)]
        public LayoutRules LayoutRule { get; set; } = LayoutRules.MaximumSpacing;

        public virtual void FillIn(AreaReinforcement areaRein)
        {
            var temp = (int)LayoutRule;
            var flag = ParameterAccess.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_LAYOUT_RULE, temp);

            if (!flag)
            {
                var paraLayout = ParameterAccess.FindParaByName(
                    areaRein.Parameters, "Layout Rule");
                paraLayout?.Set(temp);
            }
        }
    }

    public class AreaReinDataOnWall : AreaReinData
    {
        [Category("Layers")]
        public bool ExteriorMajorDirection { get; set; } = true;

        [Category("Layers")]
        public bool ExteriorMinorDirection { get; set; } = true;

        [Category("Layers")]
        public bool InteriorMajorDirection { get; set; } = true;

        [Category("Layers")]
        public bool InteriorMinorDirection { get; set; } = true;

        public override void FillIn(AreaReinforcement areaRein)
        {
            base.FillIn(areaRein);

            foreach (Parameter para in areaRein.Parameters)
            {
                if (para.Definition.Name == "Exterior Major Direction")
                    para.Set(Convert.ToInt32(ExteriorMajorDirection));

                if (para.Definition.Name == "Interior Major Direction")
                    para.Set(Convert.ToInt32(InteriorMajorDirection));

                if (para.Definition.Name == "Exterior Minor Direction")
                    para.Set(Convert.ToInt32(ExteriorMinorDirection));

                if (para.Definition.Name == "Interior Minor Direction")
                    para.Set(Convert.ToInt32(InteriorMinorDirection));
            }
        }
    }

    public class AreaReinDataOnFloor : AreaReinData
    {
        [Category("Layers")]
        public bool TopMajorDirection { get; set; } = true;

        [Category("Layers")]
        public bool TopMinorDirection { get; set; } = true;

        [Category("Layers")]
        public bool BottomMajorDirection { get; set; } = true;

        [Category("Layers")]
        public bool BottomMinorDirection { get; set; } = true;

        public override void FillIn(AreaReinforcement areaRein)
        {
            base.FillIn(areaRein);

            ParameterAccess.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1,
                Convert.ToInt32(BottomMajorDirection));
            ParameterAccess.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2,
                Convert.ToInt32(BottomMinorDirection));
            ParameterAccess.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1,
                Convert.ToInt32(TopMajorDirection));
            ParameterAccess.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2,
                Convert.ToInt32(TopMinorDirection));
        }
    }
}
