// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.ComponentModel;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.CreateBeamSystem.CS
{
    /// <summary>
    ///     describes the type of beam layout method in beam system
    /// </summary>
    public enum LayoutMethod
    {
        /// <summary>
        ///     the beam's layout method in beam System is clear the spacing among beams
        /// </summary>
        ClearSpacing,

        /// <summary>
        ///     maximum the space among beams
        /// </summary>
        MaximumSpacing,

        /// <summary>
        ///     has fixed beams number and user appoint the number
        /// </summary>
        FixedNumber,

        /// <summary>
        ///     has fixed distance among beams and user appoint this distance
        /// </summary>
        FixedDistance
    }

    /// <summary>
    ///     declares a delegate for a method that takes in a LayoutMethod
    /// </summary>
    /// <param name="layoutMethod"></param>
    public delegate void LayoutRuleChangedHandler(ref LayoutMethod layoutMethod);

    /// <summary>
    ///     the properties of beam system;
    ///     can be displayed in PropertyGrid
    /// </summary>
    public abstract class BeamSystemParam
    {
        /// <summary>
        ///     space between beams; buffer for subclass
        /// </summary>
        protected double FixedSpacing;

        /// <summary>
        ///     justify type; buffer for subclass
        /// </summary>
        protected BeamSystemJustifyType JustifyType;

        /// <summary>
        ///     layout method
        /// </summary>
        protected LayoutMethod LayoutType;

        /// <summary>
        ///     number of beams
        /// </summary>
        protected int NumberOfLines;

        /// <summary>
        ///     initial general members for its subclass
        /// </summary>
        protected BeamSystemParam()
        {
            FixedSpacing = 2000.0;
            JustifyType = BeamSystemJustifyType.Center;
            NumberOfLines = 6;
        }

        /// <summary>
        ///     layout method of beam system is changed
        /// </summary>
        [Browsable(false)]
        public LayoutRuleChangedHandler LayoutRuleChanged { get; set; }

        /// <summary>
        ///     kind of layout rule
        /// </summary>
        [Category("Pattern")]
        [Description("Specify the layout rule")]
        public LayoutMethod LayoutRuleMethod
        {
            get => LayoutType;
            set
            {
                if (LayoutType != value)
                    // invokes the delegate
                    LayoutRuleChanged(ref value);
            }
        }

        /// <summary>
        ///     type of beam used to create beam system
        /// </summary>
        [Category("Pattern")]
        [TypeConverter(typeof(BeamTypeItem))]
        [Description("Select a value for the Beam Type used in the beam system")]
        public FamilySymbol BeamType { get; set; }

        /// <summary>
        ///     subclass of LayoutRule
        /// </summary>
        [Browsable(false)]
        public abstract LayoutRule Layout { get; }

        /// <summary>
        ///     create BeamSystemParam's subclass according to LayoutMethod
        /// </summary>
        /// <param name="layoutType">LayoutMethod</param>
        /// <returns>created BeamSystemParam's subclass</returns>
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

        /// <summary>
        ///     clone BeamSystemParam to one of its subclass according to LayoutMethod
        /// </summary>
        /// <param name="layoutType">LayoutMethod</param>
        /// <returns>cloned BeamSystemParam's subclass</returns>
        public BeamSystemParam CloneInstance(LayoutMethod layoutType)
        {
            // create a BeamSystemParam instance and set its properties
            var param = CreateInstance(layoutType);
            param.FixedSpacing = FixedSpacing;
            param.JustifyType = JustifyType;
            param.NumberOfLines = NumberOfLines;
            param.BeamType = BeamType;
            return param;
        }

        /// <summary>
        ///     properties related to LayoutRule when it's clear spacing
        ///     only visible for class BeamSystemParam
        /// </summary>
        private class ClearSpacingParam : BeamSystemParam
        {
            protected readonly LayoutRuleClearSpacing m_layout;

            /// <summary>
            ///     constructor
            /// </summary>
            public ClearSpacingParam()
            {
                m_layout = new LayoutRuleClearSpacing(FixedSpacing, base.JustifyType);
                LayoutType = LayoutMethod.ClearSpacing;
            }

            /// <summary>
            ///     wrapped LayoutRuleClearSpacing object
            /// </summary>
            public override LayoutRule Layout => m_layout;

            /// <summary>
            ///     FixedSpacing value of beam system
            /// </summary>
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

            /// <summary>
            ///     JustifyType value of beam system
            /// </summary>
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

        /// <summary>
        ///     properties related to LayoutRule when it's fixed distance
        ///     only visible for class BeamSystemParam
        /// </summary>
        private class FixedDistanceParam : BeamSystemParam
        {
            protected readonly LayoutRuleFixedDistance m_layout;

            /// <summary>
            ///     constructor
            /// </summary>
            public FixedDistanceParam()
            {
                m_layout = new LayoutRuleFixedDistance(base.FixedSpacing, base.JustifyType);
                LayoutType = LayoutMethod.FixedDistance;
            }

            /// <summary>
            ///     wrapped LayoutRuleFixedDistance object
            /// </summary>
            public override LayoutRule Layout => m_layout;

            /// <summary>
            ///     FixedSpacing value of beam system
            /// </summary>
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

            /// <summary>
            ///     JustifyType value of beam system
            /// </summary>
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

        /// <summary>
        ///     properties related to LayoutRule when it's fixed number
        ///     only visible for class BeamSystemParam
        /// </summary>
        private class FixedNumberParam : BeamSystemParam
        {
            protected readonly LayoutRuleFixedNumber m_layout;

            /// <summary>
            ///     constructor
            /// </summary>
            public FixedNumberParam()
            {
                m_layout = new LayoutRuleFixedNumber(base.NumberOfLines);
                LayoutType = LayoutMethod.FixedNumber;
            }

            /// <summary>
            ///     NumberOfLines value of beam system
            /// </summary>
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

            /// <summary>
            ///     wrapped LayoutRuleFixedNumber object
            /// </summary>
            public override LayoutRule Layout => m_layout;
        }

        /// <summary>
        ///     properties related to LayoutRule when it's maximum spacing
        ///     only visible for class BeamSystemParam
        /// </summary>
        private class MaximumSpacingParam : BeamSystemParam
        {
            protected readonly LayoutRuleMaximumSpacing m_layout;

            /// <summary>
            ///     constructor
            /// </summary>
            public MaximumSpacingParam()
            {
                m_layout = new LayoutRuleMaximumSpacing(FixedSpacing);
                LayoutType = LayoutMethod.MaximumSpacing;
            }

            /// <summary>
            ///     FixedSpacing value of beam system
            /// </summary>
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

            /// <summary>
            ///     wrapped LayoutRuleMaximumSpacing object
            /// </summary>
            public override LayoutRule Layout => m_layout;
        }
    }
}
