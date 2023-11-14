//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.ComponentModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.CreateSimpleAreaRein.CS
{
    using GeoElement = GeometryElement;


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
        [CategoryAttribute("Construction")]
        [DefaultValueAttribute(
            LayoutRules.Maximum_Spacing)]
        public LayoutRules LayoutRule { get; set; } = LayoutRules.Maximum_Spacing;

        /// <summary>
        ///     set the parameters to given AreaReinforcement
        /// </summary>
        /// <param name="areaRein"></param>
        public virtual void FillIn(AreaReinforcement areaRein)
        {
            var temp = (int)LayoutRule;
            var flag = ParameterUtil.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_LAYOUT_RULE, temp);

            if (!flag)
            {
                var paraLayout = ParameterUtil.FindParaByName(
                    areaRein.Parameters, "Layout Rule");
                if (null != paraLayout) paraLayout.Set(temp);
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
        [CategoryAttribute("Layers")]
        public bool ExteriorMajorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [CategoryAttribute("Layers")]
        public bool ExteriorMinorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [CategoryAttribute("Layers")]
        public bool InteriorMajorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [CategoryAttribute("Layers")]
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
        [CategoryAttribute("Layers")]
        public bool TopMajorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [CategoryAttribute("Layers")]
        public bool TopMinorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [CategoryAttribute("Layers")]
        public bool BottomMajorDirection { get; set; } = true;

        /// <summary>
        ///     Parameter of AreaReinforcement
        /// </summary>
        [CategoryAttribute("Layers")]
        public bool BottomMinorDirection { get; set; } = true;

        /// <summary>
        ///     set the parameters to given AreaReinforcement
        /// </summary>
        /// <param name="areaRein"></param>
        public override void FillIn(AreaReinforcement areaRein)
        {
            base.FillIn(areaRein);

            ParameterUtil.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1,
                Convert.ToInt32(BottomMajorDirection));
            ParameterUtil.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2,
                Convert.ToInt32(BottomMinorDirection));
            ParameterUtil.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1,
                Convert.ToInt32(TopMajorDirection));
            ParameterUtil.SetParaInt(areaRein,
                BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2,
                Convert.ToInt32(TopMinorDirection));
        }
    }
}