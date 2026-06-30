// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Parameters;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System.ComponentModel;
namespace Ara3D.RevitSampleBrowser.AreaReinParameters.CS
{
    public class WallAreaReinData : IAreaReinData
    {
        private Parameter m_exteriorMajorBarType;
        private Parameter m_exteriorMajorHookOrientation;
        private Parameter m_exteriorMajorHookType;

        private Parameter m_exteriorMinorBarType;
        private Parameter m_exteriorMinorHookOrientation;
        private Parameter m_exteriorMinorHookType;

        private Parameter m_interiorMajorBarType;
        private Parameter m_interiorMajorHookOrientation;
        private Parameter m_interiorMajorHookType;

        private Parameter m_interiorMinorBarType;
        private Parameter m_interiorMinorHookOrientation;

        private Parameter m_interiorMinorHookType;

        private Parameter m_layoutRule;

        [Category("Construction")]
        public LayoutRules LayoutRule
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

        [Category("Exterior Major Layers")]
        [TypeConverter(typeof(BarTypeItem))]
        public ElementId ExteriorMajorBarType
        {
            get => m_exteriorMajorBarType.AsElementId();
            set => m_exteriorMajorBarType.Set(value);
        }

        [Category("Exterior Major Layers")]
        [TypeConverter(typeof(HookTypeItem))]
        public ElementId ExteriorMajorHookType
        {
            get => m_exteriorMajorHookType.AsElementId();
            set => m_exteriorMajorHookType.Set(value);
        }

        [Category("Exterior Major Layers")]
        public WallHookOrientations ExteriorMajorHookOrientation
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

        [Category("Exterior Minor Layers")]
        [TypeConverter(typeof(BarTypeItem))]
        public ElementId ExteriorMinorBarType
        {
            get => m_exteriorMinorBarType.AsElementId();
            set => m_exteriorMinorBarType.Set(value);
        }

        [Category("Exterior Minor Layers")]
        [TypeConverter(typeof(HookTypeItem))]
        public ElementId ExteriorMinorHookType
        {
            get => m_exteriorMinorHookType.AsElementId();
            set => m_exteriorMinorHookType.Set(value);
        }

        [Category("Exterior Minor Layers")]
        public WallHookOrientations ExteriorMinorHookOrientation
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

        [Category("Interior Major Layers")]
        [TypeConverter(typeof(BarTypeItem))]
        public ElementId InteriorMajorBarType
        {
            get => m_interiorMajorBarType.AsElementId();
            set => m_interiorMajorBarType.Set(value);
        }

        [Category("Interior Major Layers")]
        [TypeConverter(typeof(HookTypeItem))]
        public ElementId InteriorMajorHookType
        {
            get => m_interiorMajorHookType.AsElementId();
            set => m_interiorMajorHookType.Set(value);
        }

        [Category("Interior Major Layers")]
        public WallHookOrientations InteriorMajorHookOrientation
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

        [Category("Interior Minor Layers")]
        [TypeConverter(typeof(BarTypeItem))]
        public ElementId InteriorMinorBarType
        {
            get => m_interiorMinorBarType.AsElementId();
            set => m_interiorMinorBarType.Set(value);
        }

        [Category("Interior Minor Layers")]
        [TypeConverter(typeof(HookTypeItem))]
        public ElementId InteriorMinorHookType
        {
            get => m_interiorMinorHookType.AsElementId();
            set => m_interiorMinorHookType.Set(value);
        }

        [Category("Interior Minor Layers")]
        public WallHookOrientations InteriorMinorHookOrientation
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
            m_layoutRule = areaRein.get_Parameter(
                BuiltInParameter.REBAR_SYSTEM_LAYOUT_RULE);
            var flag = m_layoutRule != null;

            var paras = areaRein.Parameters;

            m_exteriorMajorBarType = ParameterAccess.FindParaByName(paras,
                "Exterior Major Bar Type");
            m_exteriorMajorHookType = ParameterAccess.FindParaByName(paras,
                "Exterior Major Hook Type");
            m_exteriorMajorHookOrientation = ParameterAccess.FindParaByName(paras,
                "Exterior Major Hook Orientation");
            flag &= m_exteriorMajorBarType != null && m_exteriorMajorHookOrientation != null
                                                   && m_exteriorMajorHookType != null;

            m_exteriorMinorBarType = ParameterAccess.FindParaByName(paras,
                "Exterior Minor Bar Type");
            m_exteriorMinorHookType = ParameterAccess.FindParaByName(paras,
                "Exterior Minor Hook Type");
            m_exteriorMinorHookOrientation = ParameterAccess.FindParaByName(paras,
                "Exterior Minor Hook Orientation");
            flag &= m_exteriorMinorBarType != null && m_exteriorMinorHookOrientation != null
                                                   && m_exteriorMinorHookType != null;

            m_interiorMajorBarType = ParameterAccess.FindParaByName(paras,
                "Interior Major Bar Type");
            m_interiorMajorHookType = ParameterAccess.FindParaByName(paras,
                "Interior Major Hook Type");
            m_interiorMajorHookOrientation = ParameterAccess.FindParaByName(paras,
                "Interior Major Hook Orientation");
            flag &= m_interiorMajorBarType != null && m_interiorMajorHookOrientation != null
                                                   && m_interiorMajorHookType != null;

            m_interiorMinorBarType = ParameterAccess.FindParaByName(paras,
                "Interior Minor Bar Type");
            m_interiorMinorHookType = ParameterAccess.FindParaByName(paras,
                "Interior Minor Hook Type");
            m_interiorMinorHookOrientation = ParameterAccess.FindParaByName(paras,
                "Interior Minor Hook Orientation");
            flag &= m_interiorMinorBarType != null && m_interiorMinorHookOrientation != null
                                                   && m_interiorMinorHookType != null;

            return flag;
        }
    }
}
