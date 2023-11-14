// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.ComponentModel;
using Autodesk.Revit.DB.Structure;

namespace RevitMultiSample.NewRebar.CS
{
    /// <summary>
    ///     Dimension on RebarShapeDefByArc.
    /// </summary>
    internal abstract class ConstraintOnArcShape : ConstraintOnRebarShape
    {
        /// <summary>
        ///     Dimension to constrain the arc shape.
        /// </summary>
        private RebarShapeParameter m_rebarShapeParameter;

        protected ConstraintOnArcShape(RebarShapeDefByArc def)
            : base(def)
        {
        }

        /// <summary>
        ///     Dimension to constrain the arc shape.
        /// </summary>
        [DisplayName("RebarShape parameter")]
        [TypeConverter(typeof(TypeConverterRebarShapeParameter))]
        [ReadOnly(false)]
        protected RebarShapeParameter RebarShapeParameter
        {
            get
            {
                UpdateParameterTypeConverter();
                return m_rebarShapeParameter;
            }
        }

        /// <summary>
        ///     Get RebarShapeDefinitionByArc object.
        /// </summary>
        protected RebarShapeDefinitionByArc GetRebarShapeDefinitionByArc =>
            ShapeDef.RebarshapeDefinition as RebarShapeDefinitionByArc;
    }

    /// <summary>
    ///     Arc length dimension.
    /// </summary>
    internal class ConstraintArcLength : ConstraintOnArcShape
    {
        public ConstraintArcLength(RebarShapeDefByArc def)
            : base(def)
        {
        }

        /// <summary>
        ///     Add dimension to constrain the arc length.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionByArc.AddConstraintArcLength(
                RebarShapeParameter.Parameter);
        }
    }

    /// <summary>
    ///     Arc chord length dimension.
    /// </summary>
    internal class ConstraintChordLength : ConstraintOnArcShape
    {
        public ConstraintChordLength(RebarShapeDefByArc def)
            : base(def)
        {
        }

        /// <summary>
        ///     Add dimension to constrain the arc chord length.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionByArc.AddConstraintChordLength(
                RebarShapeParameter.Parameter);
        }
    }

    /// <summary>
    ///     Arc circumference dimension.
    /// </summary>
    internal class ConstraintCircumference : ConstraintOnArcShape
    {
        public ConstraintCircumference(RebarShapeDefByArc def)
            : base(def)
        {
            ArcReferenceType = RebarShapeArcReferenceType.External;
        }

        /// <summary>
        ///     Arc reference type.
        /// </summary>
        private RebarShapeArcReferenceType ArcReferenceType { get; }

        /// <summary>
        ///     Add dimension to constrain the arc circumference.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionByArc.AddConstraintCircumference(
                RebarShapeParameter.Parameter, ArcReferenceType);
        }
    }

    /// <summary>
    ///     Arc diameter dimension.
    /// </summary>
    internal class ConstraintDiameter : ConstraintOnArcShape
    {
        public ConstraintDiameter(RebarShapeDefByArc def)
            : base(def)
        {
            ArcReferenceType = RebarShapeArcReferenceType.External;
        }

        /// <summary>
        ///     Arc reference type.
        /// </summary>
        private RebarShapeArcReferenceType ArcReferenceType { get; }

        /// <summary>
        ///     Add dimension to constrain arc diameter.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionByArc.AddConstraintDiameter(
                RebarShapeParameter.Parameter, ArcReferenceType);
        }
    }

    /// <summary>
    ///     Arc radius dimension.
    /// </summary>
    internal class ConstraintRadius : ConstraintOnArcShape
    {
        public ConstraintRadius(RebarShapeDefByArc def)
            : base(def)
        {
            ArcReferenceType = RebarShapeArcReferenceType.External;
        }

        /// <summary>
        ///     Arc reference type.
        /// </summary>
        private RebarShapeArcReferenceType ArcReferenceType { get; }

        /// <summary>
        ///     Add dimension to constrain the radius of arc.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionByArc.AddConstraintRadius(
                RebarShapeParameter.Parameter, ArcReferenceType);
        }
    }

    /// <summary>
    ///     Arc Sagittarius length dimension.
    /// </summary>
    internal class ConstraintSagittaLength : ConstraintOnArcShape
    {
        public ConstraintSagittaLength(RebarShapeDefByArc def)
            : base(def)
        {
        }

        /// <summary>
        ///     Add dimension to constrain the Sagittarius length of arc.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionByArc.AddConstraintSagittaLength(
                RebarShapeParameter.Parameter);
        }
    }
}
