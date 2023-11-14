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


using System.ComponentModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.AreaReinParameters.CS
{
    /// <summary>
    ///     can be the datasource of propertygrid
    /// </summary>
    internal class WallAreaReinData : IAreaReinData
    {
        //exterior major layer
        private Parameter m_exteriorMajorBarType;
        private Parameter m_exteriorMajorHookOrientation;
        private Parameter m_exteriorMajorHookType;

        //exterior minor layer
        private Parameter m_exteriorMinorBarType;
        private Parameter m_exteriorMinorHookOrientation;
        private Parameter m_exteriorMinorHookType;

        //interior major layer
        private Parameter m_interiorMajorBarType;
        private Parameter m_interiorMajorHookOrientation;
        private Parameter m_interiorMajorHookType;

        //interior minor layer
        private Parameter m_interiorMinorBarType;
        private Parameter m_interiorMinorHookOrientation;

        private Parameter m_interiorMinorHookType;

        //member
        private Parameter m_layoutRule;

        [Category("Construction")]
        public LayoutRules Layout_Rule
        {
            get
            {
                var index = m_layoutRule.AsInteger();
                return (LayoutRules)index;
            }
            set
            {
                var index = (int)value;
                m_layoutRule.Set(index);
            }
        }

        [CategoryAttribute("Exterior Major Layers")]
        [TypeConverter(typeof(BarTypeItem))]
        public ElementId Exterior_Major_Bar_Type
        {
            get => m_exteriorMajorBarType.AsElementId();
            set => m_exteriorMajorBarType.Set(value);
        }

        [CategoryAttribute("Exterior Major Layers")]
        [TypeConverter(typeof(HookTypeItem))]
        public ElementId Exterior_Major_Hook_Type
        {
            get => m_exteriorMajorHookType.AsElementId();
            set => m_exteriorMajorHookType.Set(value);
        }

        [CategoryAttribute("Exterior Major Layers")]
        public WallHookOrientations Exterior_Major_Hook_Orientation
        {
            get
            {
                var index = m_exteriorMajorHookOrientation.AsInteger();
                return (WallHookOrientations)index;
            }
            set
            {
                var index = (int)value;
                m_exteriorMajorHookOrientation.Set(index);
            }
        }

        [CategoryAttribute("Exterior Minor Layers")]
        [TypeConverter(typeof(BarTypeItem))]
        public ElementId Exterior_Minor_Bar_Type
        {
            get => m_exteriorMinorBarType.AsElementId();
            set => m_exteriorMinorBarType.Set(value);
        }

        [CategoryAttribute("Exterior Minor Layers")]
        [TypeConverter(typeof(HookTypeItem))]
        public ElementId Exterior_Minor_Hook_Type
        {
            get => m_exteriorMinorHookType.AsElementId();
            set => m_exteriorMinorHookType.Set(value);
        }

        [CategoryAttribute("Exterior Minor Layers")]
        public WallHookOrientations Exterior_Minor_Hook_Orientation
        {
            get
            {
                var index = m_exteriorMinorHookOrientation.AsInteger();
                return (WallHookOrientations)index;
            }
            set
            {
                var index = (int)value;
                m_exteriorMinorHookOrientation.Set(index);
            }
        }

        [CategoryAttribute("Interior Major Layers")]
        [TypeConverter(typeof(BarTypeItem))]
        public ElementId Interior_Major_Bar_Type
        {
            get => m_interiorMajorBarType.AsElementId();
            set => m_interiorMajorBarType.Set(value);
        }

        [CategoryAttribute("Interior Major Layers")]
        [TypeConverter(typeof(HookTypeItem))]
        public ElementId Interior_Major_Hook_Type
        {
            get => m_interiorMajorHookType.AsElementId();
            set => m_interiorMajorHookType.Set(value);
        }

        [CategoryAttribute("Interior Major Layers")]
        public WallHookOrientations Interior_Major_Hook_Orientation
        {
            get
            {
                var index = m_interiorMajorHookOrientation.AsInteger();
                return (WallHookOrientations)index;
            }
            set
            {
                var index = (int)value;
                m_interiorMajorHookOrientation.Set(index);
            }
        }

        [CategoryAttribute("Interior Minor Layers")]
        [TypeConverter(typeof(BarTypeItem))]
        public ElementId Interior_Minor_Bar_Type
        {
            get => m_interiorMinorBarType.AsElementId();
            set => m_interiorMinorBarType.Set(value);
        }

        [CategoryAttribute("Interior Minor Layers")]
        [TypeConverter(typeof(HookTypeItem))]
        public ElementId Interior_Minor_Hook_Type
        {
            get => m_interiorMinorHookType.AsElementId();
            set => m_interiorMinorHookType.Set(value);
        }

        [CategoryAttribute("Interior Minor Layers")]
        public WallHookOrientations Interior_Minor_Hook_Orientation
        {
            get
            {
                var index = m_interiorMinorHookOrientation.AsInteger();
                return (WallHookOrientations)index;
            }
            set
            {
                var index = (int)value;
                m_interiorMinorHookOrientation.Set(index);
            }
        }

        public bool FillInData(AreaReinforcement areaRein)
        {
            //member
            m_layoutRule = areaRein.get_Parameter(
                BuiltInParameter.REBAR_SYSTEM_LAYOUT_RULE);
            var flag = m_layoutRule != null;

            var paras = areaRein.Parameters;

            //exterior major layer
            m_exteriorMajorBarType = ParameterUtil.FindParaByName(paras,
                "Exterior Major Bar Type");
            m_exteriorMajorHookType = ParameterUtil.FindParaByName(paras,
                "Exterior Major Hook Type");
            m_exteriorMajorHookOrientation = ParameterUtil.FindParaByName(paras,
                "Exterior Major Hook Orientation");
            flag &= m_exteriorMajorBarType != null && m_exteriorMajorHookOrientation != null
                                                   && m_exteriorMajorHookType != null;

            //exterior minor layer
            m_exteriorMinorBarType = ParameterUtil.FindParaByName(paras,
                "Exterior Minor Bar Type");
            m_exteriorMinorHookType = ParameterUtil.FindParaByName(paras,
                "Exterior Minor Hook Type");
            m_exteriorMinorHookOrientation = ParameterUtil.FindParaByName(paras,
                "Exterior Minor Hook Orientation");
            flag &= m_exteriorMinorBarType != null && m_exteriorMinorHookOrientation != null
                                                   && m_exteriorMinorHookType != null;

            //interior major layer
            m_interiorMajorBarType = ParameterUtil.FindParaByName(paras,
                "Interior Major Bar Type");
            m_interiorMajorHookType = ParameterUtil.FindParaByName(paras,
                "Interior Major Hook Type");
            m_interiorMajorHookOrientation = ParameterUtil.FindParaByName(paras,
                "Interior Major Hook Orientation");
            flag &= m_interiorMajorBarType != null && m_interiorMajorHookOrientation != null
                                                   && m_interiorMajorHookType != null;

            //interior minor layer
            m_interiorMinorBarType = ParameterUtil.FindParaByName(paras,
                "Interior Minor Bar Type");
            m_interiorMinorHookType = ParameterUtil.FindParaByName(paras,
                "Interior Minor Hook Type");
            m_interiorMinorHookOrientation = ParameterUtil.FindParaByName(paras,
                "Interior Minor Hook Orientation");
            flag &= m_interiorMinorBarType != null && m_interiorMinorHookOrientation != null
                                                   && m_interiorMinorHookType != null;

            return flag;
        }
    }
}