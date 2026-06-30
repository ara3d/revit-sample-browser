// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters;
using Ara3D.RevitSampleBrowser.NewRebar.CS.RebarShapeDef;
using Ara3D.RevitSampleBrowser.NewRebar.CS.TypeConverter;
using Autodesk.Revit.DB.Structure;
using System.ComponentModel;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.Constraints
{
    /// <summary>
    ///     Segment's ends reference enum.
    /// </summary>
    public enum EndReference
    {
        /// <summary>
        ///     Segment's start reference.
        /// </summary>
        Begin = 0,

        /// <summary>
        ///     Segment's end reference.
        /// </summary>
        End = 1
    }

    /// <summary>
    ///     Constraint to be added to RebarShapeDefBySegment.
    /// </summary>
    public abstract class ConstraintOnSegmentShape : ConstraintOnRebarShape
    {
        public ConstraintOnSegmentShape(RebarShapeDefBySegment def)
            : base(def)
        {
        }

        protected RebarShapeDefinitionBySegments GetRebarShapeDefinitionBySegments =>
            ShapeDef.RebarshapeDefinition as RebarShapeDefinitionBySegments;

        /// <summary>
        ///     Update list value for property grid.
        /// </summary>
        protected void UpdateSegmentIdTypeConverter()
        {
            TypeConverterSegmentId.SegmentCount = GetRebarShapeDefinitionBySegments.NumberOfSegments;
        }
    }

    /// <summary>
    ///     Default radius dimension of bend.
    /// </summary>
    public class ConstraintBendDefaultRadius : ConstraintOnSegmentShape
    {

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        private int m_segment;

        public ConstraintBendDefaultRadius(RebarShapeDefBySegment def)
            : base(def)
        {
            Turn = RebarShapeVertexTurn.Left;
            BendAngle = RebarShapeBendAngle.Obtuse;
        }

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     Bend orientation property.
        /// </summary>
        public RebarShapeVertexTurn Turn { get; set; }

        /// <summary>
        ///     Bend angle property.
        /// </summary>
        public RebarShapeBendAngle BendAngle { get; set; }

        /// <summary>
        ///     Add bend default radius constraint to RebarShapeDefinitionBySegments.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddBendDefaultRadius(
                m_segment, Turn, BendAngle);
        }
    }

    /// <summary>
    ///     Variable radius dimension of bend.
    /// </summary>
    public class ConstraintBendVariableRadius : ConstraintOnSegmentShape
    {

        /// <summary>
        ///     Radius dimension field.
        /// </summary>
        private RebarShapeParameter m_radiusParameter;

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        private int m_segment;

        public ConstraintBendVariableRadius(RebarShapeDefBySegment def)
            : base(def)
        {
            BendAngle = RebarShapeBendAngle.Obtuse;
            Turn = RebarShapeVertexTurn.Left;
            MeasureIncludingBarThickness = true;
        }

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     Bend orientation property.
        /// </summary>
        public RebarShapeVertexTurn Turn { get; set; }

        /// <summary>
        ///     Bend angle property.
        /// </summary>
        public RebarShapeBendAngle BendAngle { get; set; }

        /// <summary>
        ///     Radius dimension property.
        /// </summary>
        [TypeConverter(typeof(TypeConverterRebarShapeParameter))]
        public RebarShapeParameter RadiusParameter
        {
            get
            {
                UpdateParameterTypeConverter();
                return m_radiusParameter;
            }
            set => m_radiusParameter = value;
        }

        /// <summary>
        ///     Measure including bar thickness or not.
        /// </summary>
        public bool MeasureIncludingBarThickness { get; set; }

        /// <summary>
        ///     Add Dimension to constrain the bend radius.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddBendVariableRadius(
                m_segment, Turn, BendAngle, m_radiusParameter.Parameter,
                MeasureIncludingBarThickness);
        }
    }

    /// <summary>
    ///     Parallel dimension to segment.
    /// </summary>
    public class ConstraintParallelToSegment : ConstraintOnSegmentShape
    {

        /// <summary>
        ///     Dimension to constrain the length of segment.
        /// </summary>
        private RebarShapeParameter m_parameter;

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        private int m_segment;

        public ConstraintParallelToSegment(RebarShapeDefBySegment def)
            : base(def)
        {
            MeasureToOutsideOfBend0 = true;
            MeasureToOutsideOfBend1 = true;
        }

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     Dimension to constrain the length of segment.
        /// </summary>
        [TypeConverter(typeof(TypeConverterRebarShapeParameter))]
        public RebarShapeParameter Parameter
        {
            get
            {
                UpdateParameterTypeConverter();
                return m_parameter;
            }
            set => m_parameter = value;
        }

        /// <summary>
        ///     Measure segment's length to outside of bend 0 or not.
        /// </summary>
        public bool MeasureToOutsideOfBend0 { get; set; }

        /// <summary>
        ///     Measure segment's length to outside of bend 1 or not.
        /// </summary>
        public bool MeasureToOutsideOfBend1 { get; set; }

        /// <summary>
        ///     Add Dimension to constrain the segment length.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddConstraintParallelToSegment(
                m_segment, m_parameter.Parameter, MeasureToOutsideOfBend0,
                MeasureToOutsideOfBend1);
        }
    }

    /// <summary>
    ///     Length dimension of segment in specified direction.
    /// </summary>
    public class ConstraintToSegment : ConstraintOnSegmentShape
    {

        /// <summary>
        ///     Dimension to constraint the length of segment in specified direction.
        /// </summary>
        private RebarShapeParameter m_parameter;

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        private int m_segment;

        public ConstraintToSegment(RebarShapeDefBySegment def)
            : base(def)
        {
            MeasureToOutsideOfBend0 = true;
            MeasureToOutsideOfBend1 = false;
            SignOfZCoordOfCrossProductOfConstraintDirBySegmentDir = -1;
        }

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     Dimension to constraint the length of segment in specified direction.
        /// </summary>
        [TypeConverter(typeof(TypeConverterRebarShapeParameter))]
        public RebarShapeParameter Parameter
        {
            get
            {
                UpdateParameterTypeConverter();
                return m_parameter;
            }
            set => m_parameter = value;
        }

        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordX { get; set; }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordY { get; set; }

        /// <summary>
        ///     Sign of Z coordinate of cross product of constraint direction by segment direction.
        /// </summary>
        public int SignOfZCoordOfCrossProductOfConstraintDirBySegmentDir { get; set; }

        /// <summary>
        ///     Measure segment's length to outside of bend 0 or not.
        /// </summary>
        public bool MeasureToOutsideOfBend0 { get; set; }

        /// <summary>
        ///     Measure segment's length to outside of bend 1 or not.
        /// </summary>
        public bool MeasureToOutsideOfBend1 { get; set; }

        /// <summary>
        ///     Add dimension to constrain the length of segment in the specified direction.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddConstraintToSegment(m_segment,
                m_parameter.Parameter, ConstraintDirCoordX, ConstraintDirCoordY,
                SignOfZCoordOfCrossProductOfConstraintDirBySegmentDir,
                MeasureToOutsideOfBend0, MeasureToOutsideOfBend1);
        }
    }

    /// <summary>
    ///     Listening length dimension between two bends.
    /// </summary>
    public class ListeningDimensionBendToBend : ConstraintOnSegmentShape
    {

        /// <summary>
        ///     Dimension to constraint the length of two bends in the specified direction.
        /// </summary>
        private RebarShapeParameter m_parameter;

        /// <summary>
        ///     Reference of segment 0.
        /// </summary>
        private int m_segment;

        /// <summary>
        ///     Reference of segment 1.
        /// </summary>
        private int m_segment1;

        public ListeningDimensionBendToBend(RebarShapeDefBySegment def)
            : base(def)
        {
            End0 = EndReference.Begin;
            End1 = EndReference.End;
            ConstraintDirCoordX = 0;
            ConstraintDirCoordY = 0;
        }

        /// <summary>
        ///     Dimension to constraint the length of two bends in the specified direction.
        /// </summary>
        [TypeConverter(typeof(TypeConverterRebarShapeParameter))]
        public RebarShapeParameter Parameter
        {
            get
            {
                UpdateParameterTypeConverter();

                return m_parameter;
            }
            set => m_parameter = value;
        }

        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordX { get; set; }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordY { get; set; }

        /// <summary>
        ///     Reference of segment 0.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment0
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     End reference of segment 0.
        /// </summary>
        public EndReference End0 { get; set; }

        /// <summary>
        ///     Reference of segment 1.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment1
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment1;
            }
            set => m_segment1 = value;
        }

        /// <summary>
        ///     End reference of segment 1.
        /// </summary>
        public EndReference End1 { get; set; }

        /// <summary>
        ///     Add listening dimension to constrain the length of two bend in the specified direction.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddListeningDimensionBendToBend(
                m_parameter.Parameter, ConstraintDirCoordX, ConstraintDirCoordY,
                m_segment, (int)End0, m_segment1, (int)End1);
        }
    }

    /// <summary>
    ///     Listening length dimension between a segment and a bend.
    /// </summary>
    public class ListeningDimensionSegmentToBend : ConstraintOnSegmentShape
    {

        /// <summary>
        ///     Dimension to constrain the length between a segment and a bend
        ///     in the specified direction.
        /// </summary>
        private RebarShapeParameter m_parameter;

        /// <summary>
        ///     Reference of segment 0.
        /// </summary>
        private int m_segment;

        /// <summary>
        ///     Reference of segment 1.
        /// </summary>
        private int m_segment1;

        public ListeningDimensionSegmentToBend(RebarShapeDefBySegment def)
            : base(def)
        {
            ConstraintDirCoordX = 0;
            ConstraintDirCoordY = 0;
            End1 = EndReference.End;
        }

        /// <summary>
        ///     Dimension to constrain the length between a segment and a bend
        ///     in the specified direction.
        /// </summary>
        [TypeConverter(typeof(TypeConverterRebarShapeParameter))]
        public RebarShapeParameter Parameter
        {
            get
            {
                UpdateParameterTypeConverter();
                return m_parameter;
            }
            set => m_parameter = value;
        }

        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordX { get; set; }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordY { get; set; }

        /// <summary>
        ///     Reference of segment 0.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment0
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     Reference of segment 1.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment1
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment1;
            }
            set => m_segment1 = value;
        }

        /// <summary>
        ///     End reference of segment 1.
        /// </summary>
        public EndReference End1 { get; set; }

        /// <summary>
        ///     Add listening dimension to constrain the length between a segment and a bend
        ///     in the specified direction.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddListeningDimensionSegmentToBend(
                m_parameter.Parameter, ConstraintDirCoordX, ConstraintDirCoordY,
                m_segment, m_segment1, (int)End1);
        }
    }

    /// <summary>
    ///     Listening length dimension between two segments.
    /// </summary>
    public class ListeningDimensionSegmentToSegment : ConstraintOnSegmentShape
    {

        /// <summary>
        ///     Dimension to constrain the perpendicular distance between two segment.
        ///     The two segment should be parallel.
        /// </summary>
        private RebarShapeParameter m_parameter;

        /// <summary>
        ///     The first segment to be constrained.
        /// </summary>
        private int m_segment;

        /// <summary>
        ///     The second segment to be constrained.
        /// </summary>
        private int m_segment1;

        public ListeningDimensionSegmentToSegment(RebarShapeDefBySegment def)
            : base(def)
        {
            ConstraintDirCoordX = 1;
            ConstraintDirCoordY = 0;
        }

        /// <summary>
        ///     Dimension to constrain the perpendicular distance between two segment.
        /// </summary>
        [TypeConverter(typeof(TypeConverterRebarShapeParameter))]
        public RebarShapeParameter Parameter
        {
            get
            {
                UpdateParameterTypeConverter();
                return m_parameter;
            }
            set => m_parameter = value;
        }

        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordX { get; set; }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordY { get; set; }

        /// <summary>
        ///     The second segment to be constrained.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment0
        {
            get
            {
                UpdateSegmentIdTypeConverter();
                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     The second segment to be constrained.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment1
        {
            get
            {
                UpdateSegmentIdTypeConverter();
                return m_segment1;
            }
            set => m_segment1 = value;
        }

        /// <summary>
        ///     Add dimension to constrain the perpendicular distance between two segment.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddListeningDimensionSegmentToSegment(
                m_parameter.Parameter, ConstraintDirCoordX, ConstraintDirCoordY,
                m_segment, m_segment1);
        }
    }

    /// <summary>
    ///     Remove a dimension from a segment.
    /// </summary>
    public class RemoveParameterFromSegment : ConstraintOnSegmentShape
    {
        /// <summary>
        ///     Dimension to be removed.
        /// </summary>
        private RebarShapeParameter m_radiusParameter;

        /// <summary>
        ///     Reference of segment.
        /// </summary>
        private int m_segment;

        public RemoveParameterFromSegment(RebarShapeDefBySegment def)
            : base(def)
        {
        }

        /// <summary>
        ///     Reference of segment.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     Dimension to be removed.
        /// </summary>
        [TypeConverter(typeof(TypeConverterRebarShapeParameter))]
        public RebarShapeParameter RadiusParameter
        {
            get
            {
                UpdateParameterTypeConverter();
                return m_radiusParameter;
            }
            set => m_radiusParameter = value;
        }

        /// <summary>
        ///     Remove dimension from Rebar shape.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.RemoveParameterFromSegment(
                m_segment, m_radiusParameter.Parameter);
        }
    }

    /// <summary>
    ///     A 180 degree bend dimension.
    /// </summary>
    public class SetSegmentAs180DegreeBend : ConstraintOnSegmentShape
    {

        /// <summary>
        ///     Dimension to constrain the bend's radius.
        /// </summary>
        private RebarShapeParameter m_radiusParameter;

        /// <summary>
        ///     Reference of segment.
        /// </summary>
        private int m_segment;

        public SetSegmentAs180DegreeBend(RebarShapeDefBySegment def)
            : base(def)
        {
            MeasureToOutsideOfBend = true;
        }

        /// <summary>
        ///     Reference of segment.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     Dimension to constrain the bend's radius.
        /// </summary>
        [TypeConverter(typeof(TypeConverterRebarShapeParameter))]
        public RebarShapeParameter RadiusParameter
        {
            get
            {
                UpdateParameterTypeConverter();
                return m_radiusParameter;
            }
            set => m_radiusParameter = value;
        }

        /// <summary>
        ///     If measure the length to outside of bend.
        /// </summary>
        public bool MeasureToOutsideOfBend { get; set; }

        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.SetSegmentAs180DegreeBend(
                m_segment, m_radiusParameter.Parameter, MeasureToOutsideOfBend);
        }
    }

    /// <summary>
    ///     Length dimension of segment in its parallel direction.
    /// </summary>
    public class SetSegmentFixedDirection : ConstraintOnSegmentShape
    {
        /// <summary>
        ///     Reference of segment.
        /// </summary>
        private int m_segment;

        public SetSegmentFixedDirection(RebarShapeDefBySegment def)
            : base(def)
        {
            VecCoordX = 1;
            VecCoordY = 0;
        }

        /// <summary>
        ///     Reference of segment.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        public double VecCoordX { get; set; }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double VecCoordY { get; set; }

        /// <summary>
        ///     Add dimension to constrain the direction of the segment.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.SetSegmentFixedDirection(
                m_segment, VecCoordX, VecCoordY);
        }
    }

    /// <summary>
    ///     Remove a dimension from a segment.
    /// </summary>
    public class SetSegmentVariableDirection : ConstraintOnSegmentShape
    {
        /// <summary>
        ///     Reference of segment.
        /// </summary>
        private int m_segment;

        public SetSegmentVariableDirection(RebarShapeDefBySegment def)
            : base(def)
        {
        }

        /// <summary>
        ///     Reference of segment.
        /// </summary>
        [TypeConverter(typeof(TypeConverterSegmentId))]
        public int Segment
        {
            get
            {
                UpdateSegmentIdTypeConverter();

                return m_segment;
            }
            set => m_segment = value;
        }

        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.SetSegmentVariableDirection(m_segment);
        }
    }
}
