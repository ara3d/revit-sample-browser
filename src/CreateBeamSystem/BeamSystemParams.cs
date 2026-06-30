// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.ComponentModel;
using System.Diagnostics;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    public enum LayoutMethod
    {
        ClearSpacing,

        MaximumSpacing,

        FixedNumber,

        FixedDistance
    }

    public delegate void LayoutRuleChangedHandler(ref LayoutMethod layoutMethod);

    public abstract class BeamSystemParam
    {
        protected double FixedSpacing;

        protected BeamSystemJustifyType JustifyType;

        protected LayoutMethod LayoutType;

        protected int NumberOfLines;

        protected BeamSystemParam()
        {
            FixedSpacing = 2000.0;
            JustifyType = BeamSystemJustifyType.Center;
            NumberOfLines = 6;
        }

        [Browsable(false)]
        public LayoutRuleChangedHandler LayoutRuleChanged { get; set; }

        [Category("Pattern")]
        [Description("Specify the layout rule")]
        public LayoutMethod LayoutRuleMethod
        {
            get => LayoutType;
            set
            {
                if (LayoutType != value)
                    LayoutRuleChanged(ref value);
            }
        }

        [Category("Pattern")]
        [TypeConverter(typeof(BeamTypeItem))]
        [Description("Select a value for the Beam Type used in the beam system")]
        public FamilySymbol BeamType { get; set; }

        [Browsable(false)]
        public abstract LayoutRule Layout { get; }

        public static BeamSystemParam CreateInstance(LayoutMethod layoutType)
        {
            BeamSystemParam param = null;
            switch (layoutType)
            {
                case LayoutMethod.ClearSpacing:
                    param = new ClearSpacingParam();
                    break;
                case LayoutMethod.FixedDistance:
                    param = new FixedDistanceParam();
                    break;
                case LayoutMethod.FixedNumber:
                    param = new FixedNumberParam();
                    break;
                case LayoutMethod.MaximumSpacing:
                    param = new MaximumSpacingParam();
                    break;
            }

            // it is absolutely impossible unless layoutType is wrong
            Debug.Assert(null != param);
            return param;
        }

        public BeamSystemParam CloneInstance(LayoutMethod layoutType)
        {
            var param = CreateInstance(layoutType);
            param.FixedSpacing = FixedSpacing;
            param.JustifyType = JustifyType;
            param.NumberOfLines = NumberOfLines;
            param.BeamType = BeamType;
            return param;
        }

        private class ClearSpacingParam : BeamSystemParam
        {
            protected readonly LayoutRuleClearSpacing m_layout;

            public ClearSpacingParam()
            {
                m_layout = new LayoutRuleClearSpacing(FixedSpacing, base.JustifyType);
                LayoutType = LayoutMethod.ClearSpacing;
            }

            public override LayoutRule Layout => m_layout;

            [Category("Pattern")]
            [Description("representing the distance between each beam")]
            public double ClearSpacing
            {
                get => FixedSpacing;
                set
                {
                    try
                    {
                        m_layout.Spacing = value;
                        FixedSpacing = value;
                    }
                    catch
                    {
                    }
                }
            }

            [Category("Pattern")]
            [Description("This value determines the placement of the first beam"
                         + " and each subsequent beam is spaced a fixed distance from it.")]
            public BeamSystemJustifyType JustifyType
            {
                get => base.JustifyType;
                set
                {
                    m_layout.JustifyType = value;
                    base.JustifyType = value;
                }
            }
        }

        private class FixedDistanceParam : BeamSystemParam
        {
            protected readonly LayoutRuleFixedDistance m_layout;

            public FixedDistanceParam()
            {
                m_layout = new LayoutRuleFixedDistance(base.FixedSpacing, base.JustifyType);
                LayoutType = LayoutMethod.FixedDistance;
            }

            public override LayoutRule Layout => m_layout;

            [Category("Pattern")]
            [Description("allows you to specify the distance between beams"
                         + " based on the justification you specify.")]
            public double FixedSpacing
            {
                get => base.FixedSpacing;
                set
                {
                    try
                    {
                        m_layout.Spacing = value;
                        base.FixedSpacing = value;
                    }
                    catch
                    {
                    }
                }
            }

            [Category("Pattern")]
            [Description("determines the placement of the first beam in the system"
                         + " and each subsequent beam is spaced a fixed distance from that point.")]
            public BeamSystemJustifyType JustifyType
            {
                get => base.JustifyType;
                set
                {
                    m_layout.JustifyType = value;
                    base.JustifyType = value;
                }
            }
        }

        private class FixedNumberParam : BeamSystemParam
        {
            protected readonly LayoutRuleFixedNumber m_layout;

            public FixedNumberParam()
            {
                m_layout = new LayoutRuleFixedNumber(base.NumberOfLines);
                LayoutType = LayoutMethod.FixedNumber;
            }

            [Category("Pattern")]
            [Description("allows you to specify the number of beams within the beam system.")]
            public int NumberOfLines
            {
                get => base.NumberOfLines;
                set
                {
                    try
                    {
                        m_layout.NumberOfLines = value;
                        base.NumberOfLines = value;
                    }
                    catch
                    {
                    }
                }
            }

            public override LayoutRule Layout => m_layout;
        }

        private class MaximumSpacingParam : BeamSystemParam
        {
            protected readonly LayoutRuleMaximumSpacing m_layout;

            public MaximumSpacingParam()
            {
                m_layout = new LayoutRuleMaximumSpacing(FixedSpacing);
                LayoutType = LayoutMethod.MaximumSpacing;
            }

            [Category("Pattern")]
            [Description("allows you to specify the maximum distance between beams.")]
            public double MaximumSpacing
            {
                get => FixedSpacing;
                set
                {
                    try
                    {
                        m_layout.Spacing = value;
                        FixedSpacing = value;
                    }
                    catch
                    {
                    }
                }
            }

            public override LayoutRule Layout => m_layout;
        }
    }
}
