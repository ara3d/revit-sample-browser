// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

using Ara3D.RevitSampleBrowser.Common.Parameters;
using Ara3D.RevitSampleBrowser.AreaReinParameters.CS;
namespace Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS
{
    /// <summary>
    ///     data of the AreaReinforcement
    /// </summary>
    public class AreaReinData
    {
        /// <summary>
        ///     constructor
        /// </summary>
        public AreaReinData()
        {
        }

        /// <summary>
        ///     Parameter LayoutRule of AreaReinforcement
        /// </summary>
        [Category("Construction")]
        [DefaultValue(
            LayoutRules.MaximumSpacing)]
        public LayoutRules LayoutRule { get; set; } = LayoutRules.MaximumSpacing;

        /// <summary>
        ///     set the parameters to given AreaReinforcement
        /// </summary>
        /// <param name="areaRein"></param>
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

    /// <summary>
    ///     data of AreaReinforcement which created on wall
    /// </summary>
    public class AreaReinDataOnWall : AreaReinData
    {
        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [Category("Layers")]
        public bool ExteriorMajorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [Category("Layers")]
        public bool ExteriorMinorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [Category("Layers")]
        public bool InteriorMajorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [Category("Layers")]
        public bool InteriorMinorDirection { get; set; } = true;

        /// <summary>
        ///     set the parameters to given AreaReinforcement
        /// </summary>
        /// <param name="areaRein"></param>
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

    /// <summary>
    ///     data of AreaReinforcement which created on floor
    /// </summary>
    public class AreaReinDataOnFloor : AreaReinData
    {
        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [Category("Layers")]
        public bool TopMajorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [Category("Layers")]
        public bool TopMinorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [Category("Layers")]
        public bool BottomMajorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [Category("Layers")]
        public bool BottomMinorDirection { get; set; } = true;

        /// <summary>
        ///     set the parameters to given AreaReinforcement
        /// </summary>
        /// <param name="areaRein"></param>
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
