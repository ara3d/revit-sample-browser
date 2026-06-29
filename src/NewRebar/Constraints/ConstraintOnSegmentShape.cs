// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;
using Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters;
using Ara3D.RevitSampleBrowser.NewRebar.CS.RebarShapeDef;
using Ara3D.RevitSampleBrowser.NewRebar.CS.TypeConverter;
using Autodesk.Revit.DB.Structure;

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

        /// <summary>
        ///     Get RebarShapeDefinitionBySegments object.
        /// </summary>
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
        ///     Bend angle field.
        /// </summary>
        private RebarShapeBendAngle m_bendAngle;

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        private int m_segment;

        /// <summary>
        ///     Bend orientation field.
        /// </summary>
        private RebarShapeVertexTurn m_turn;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="def"></param>
        public ConstraintBendDefaultRadius(RebarShapeDefBySegment def)
            : base(def)
        {
            m_turn = RebarShapeVertexTurn.Left;
            m_bendAngle = RebarShapeBendAngle.Obtuse;
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
        public RebarShapeVertexTurn Turn
        {
            get => m_turn;
            set => m_turn = value;
        }

        /// <summary>
        ///     Bend angle property.
        /// </summary>
        public RebarShapeBendAngle BendAngle
        {
            get => m_bendAngle;
            set => m_bendAngle = value;
        }

        /// <summary>
        ///     Add bend default radius constraint to RebarShapeDefinitionBySegments.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddBendDefaultRadius(
                m_segment, m_turn, m_bendAngle);
        }
    }

    /// <summary>
    ///     Variable radius dimension of bend.
    /// </summary>
    public class ConstraintBendVariableRadius : ConstraintOnSegmentShape
    {
        /// <summary>
        ///     Bend angle field.
        /// </summary>
        private RebarShapeBendAngle m_bendAngle;

        /// <summary>
        ///     Measure length including bar thickness or not.
        /// </summary>
        private bool m_measureIncludingBarThickness;

        /// <summary>
        ///     Radius dimension field.
        /// </summary>
        private RebarShapeParameter m_radiusParameter;

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        private int m_segment;

        /// <summary>
        ///     Bend orientation field.
        /// </summary>
        private RebarShapeVertexTurn m_turn;

        public ConstraintBendVariableRadius(RebarShapeDefBySegment def)
            : base(def)
        {
            m_bendAngle = RebarShapeBendAngle.Obtuse;
            m_turn = RebarShapeVertexTurn.Left;
            m_measureIncludingBarThickness = true;
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
        public RebarShapeVertexTurn Turn
        {
            get => m_turn;
            set => m_turn = value;
        }

        /// <summary>
        ///     Bend angle property.
        /// </summary>
        public RebarShapeBendAngle BendAngle
        {
            get => m_bendAngle;
            set => m_bendAngle = value;
        }

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
        public bool MeasureIncludingBarThickness
        {
            get => m_measureIncludingBarThickness;
            set => m_measureIncludingBarThickness = value;
        }

        /// <summary>
        ///     Add Dimension to constrain the bend radius.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddBendVariableRadius(
                m_segment, m_turn, m_bendAngle, m_radiusParameter.Parameter,
                m_measureIncludingBarThickness);
        }
    }

    /// <summary>
    ///     Parallel dimension to segment.
    /// </summary>
    public class ConstraintParallelToSegment : ConstraintOnSegmentShape
    {
        /// <summary>
        ///     Measure segment's length to outside of bend 0 or not.
        /// </summary>
        private bool m_measureToOutsideOfBend0;

        /// <summary>
        ///     Measure segment's length to outside of bend 1 or not.
        /// </summary>
        private bool m_measureToOutsideOfBend1;

        /// <summary>
        ///     Dimension to constrain the length of segment.
        /// </summary>
        private RebarShapeParameter m_parameter;

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        private int m_segment;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="def"></param>
        public ConstraintParallelToSegment(RebarShapeDefBySegment def)
            : base(def)
        {
            m_measureToOutsideOfBend0 = true;
            m_measureToOutsideOfBend1 = true;
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
        public bool MeasureToOutsideOfBend0
        {
            get => m_measureToOutsideOfBend0;
            set => m_measureToOutsideOfBend0 = value;
        }

        /// <summary>
        ///     Measure segment's length to outside of bend 1 or not.
        /// </summary>
        public bool MeasureToOutsideOfBend1
        {
            get => m_measureToOutsideOfBend1;
            set => m_measureToOutsideOfBend1 = value;
        }

        /// <summary>
        ///     Add Dimension to constrain the segment length.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddConstraintParallelToSegment(
                m_segment, m_parameter.Parameter, m_measureToOutsideOfBend0,
                m_measureToOutsideOfBend1);
        }
    }

    /// <summary>
    ///     Length dimension of segment in specified direction.
    /// </summary>
    public class ConstraintToSegment : ConstraintOnSegmentShape
    {
        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        private double m_constraintDirCoordX;

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        private double m_constraintDirCoordY;

        /// <summary>
        ///     Measure segment's length to outside of bend 0 or not.
        /// </summary>
        private bool m_measureToOutsideOfBend0;

        /// <summary>
        ///     Measure segment's length to outside of bend 1 or not.
        /// </summary>
        private bool m_measureToOutsideOfBend1;

        /// <summary>
        ///     Dimension to constraint the length of segment in specified direction.
        /// </summary>
        private RebarShapeParameter m_parameter;

        /// <summary>
        ///     Segment to be added constraint on.
        /// </summary>
        private int m_segment;

        /// <summary>
        ///     Sign of Z coordinate of cross product of constraint direction by segment direction.
        /// </summary>
        private int m_signOfZCoordOfCrossProductOfConstraintDirBySegmentDir;

        public ConstraintToSegment(RebarShapeDefBySegment def)
            : base(def)
        {
            m_measureToOutsideOfBend0 = true;
            m_measureToOutsideOfBend1 = false;
            m_signOfZCoordOfCrossProductOfConstraintDirBySegmentDir = -1;
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
        public double ConstraintDirCoordX
        {
            get => m_constraintDirCoordX;
            set => m_constraintDirCoordX = value;
        }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordY
        {
            get => m_constraintDirCoordY;
            set => m_constraintDirCoordY = value;
        }

        /// <summary>
        ///     Sign of Z coordinate of cross product of constraint direction by segment direction.
        /// </summary>
        public int SignOfZCoordOfCrossProductOfConstraintDirBySegmentDir
        {
            get => m_signOfZCoordOfCrossProductOfConstraintDirBySegmentDir;
            set => m_signOfZCoordOfCrossProductOfConstraintDirBySegmentDir = value;
        }

        /// <summary>
        ///     Measure segment's length to outside of bend 0 or not.
        /// </summary>
        public bool MeasureToOutsideOfBend0
        {
            get => m_measureToOutsideOfBend0;
            set => m_measureToOutsideOfBend0 = value;
        }

        /// <summary>
        ///     Measure segment's length to outside of bend 1 or not.
        /// </summary>
        public bool MeasureToOutsideOfBend1
        {
            get => m_measureToOutsideOfBend1;
            set => m_measureToOutsideOfBend1 = value;
        }

        /// <summary>
        ///     Add dimension to constrain the length of segment in the specified direction.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddConstraintToSegment(m_segment,
                m_parameter.Parameter, m_constraintDirCoordX, m_constraintDirCoordY,
                m_signOfZCoordOfCrossProductOfConstraintDirBySegmentDir,
                m_measureToOutsideOfBend0, m_measureToOutsideOfBend1);
        }
    }

    /// <summary>
    ///     Listening length dimension between two bends.
    /// </summary>
    public class ListeningDimensionBendToBend : ConstraintOnSegmentShape
    {
        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        private double m_constraintDirCoordX;

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        private double m_constraintDirCoordY;

        /// <summary>
        ///     End reference of segment 0.
        /// </summary>
        private EndReference m_end;

        /// <summary>
        ///     End reference of segment 1.
        /// </summary>
        private EndReference m_end1;

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
            m_end = EndReference.Begin;
            m_end1 = EndReference.End;
            m_constraintDirCoordX = 0;
            m_constraintDirCoordY = 0;
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
        public double ConstraintDirCoordX
        {
            get => m_constraintDirCoordX;
            set => m_constraintDirCoordX = value;
        }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordY
        {
            get => m_constraintDirCoordY;
            set => m_constraintDirCoordY = value;
        }

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
        public EndReference End0
        {
            get => m_end;
            set => m_end = value;
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
        public EndReference End1
        {
            get => m_end1;
            set => m_end1 = value;
        }

        /// <summary>
        ///     Add listening dimension to constrain the length of two bend in the specified direction.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddListeningDimensionBendToBend(
                m_parameter.Parameter, m_constraintDirCoordX, m_constraintDirCoordY,
                m_segment, (int)m_end, m_segment1, (int)m_end1);
        }
    }

    /// <summary>
    ///     Listening length dimension between a segment and a bend.
    /// </summary>
    public class ListeningDimensionSegmentToBend : ConstraintOnSegmentShape
    {
        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        private double m_constraintDirCoordX;

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        private double m_constraintDirCoordY;

        /// <summary>
        ///     End reference of segment 1.
        /// </summary>
        private EndReference m_end1;

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
            m_constraintDirCoordX = 0;
            m_constraintDirCoordY = 0;
            m_end1 = EndReference.End;
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
        public double ConstraintDirCoordX
        {
            get => m_constraintDirCoordX;
            set => m_constraintDirCoordX = value;
        }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordY
        {
            get => m_constraintDirCoordY;
            set => m_constraintDirCoordY = value;
        }

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
        public EndReference End1
        {
            get => m_end1;
            set => m_end1 = value;
        }

        /// <summary>
        ///     Add listening dimension to constrain the length between a segment and a bend
        ///     in the specified direction.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.AddListeningDimensionSegmentToBend(
                m_parameter.Parameter, m_constraintDirCoordX, m_constraintDirCoordY,
                m_segment, m_segment1, (int)m_end1);
        }
    }

    /// <summary>
    ///     Listening length dimension between two segments.
    /// </summary>
    public class ListeningDimensionSegmentToSegment : ConstraintOnSegmentShape
    {
        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        private double m_constraintDirCoordX;

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        private double m_constraintDirCoordY;

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
            m_constraintDirCoordX = 1;
            m_constraintDirCoordY = 0;
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
        public double ConstraintDirCoordX
        {
            get => m_constraintDirCoordX;
            set => m_constraintDirCoordX = value;
        }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double ConstraintDirCoordY
        {
            get => m_constraintDirCoordY;
            set => m_constraintDirCoordY = value;
        }

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
                m_parameter.Parameter, m_constraintDirCoordX, m_constraintDirCoordY,
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
        ///     If measure to outside of bend.
        /// </summary>
        private bool m_measureToOutsideOfBend;

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
            m_measureToOutsideOfBend = true;
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
        public bool MeasureToOutsideOfBend
        {
            get => m_measureToOutsideOfBend;
            set => m_measureToOutsideOfBend = value;
        }

        /// <summary>
        ///     Add a dimension of 180 degree bend for a segment.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.SetSegmentAs180DegreeBend(
                m_segment, m_radiusParameter.Parameter, m_measureToOutsideOfBend);
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

        /// <summary>
        ///     X coordinate of constraint direction.
        /// </summary>
        private double m_vecCoordX;

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        private double m_vecCoordY;

        public SetSegmentFixedDirection(RebarShapeDefBySegment def)
            : base(def)
        {
            m_vecCoordX = 1;
            m_vecCoordY = 0;
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
        public double VecCoordX
        {
            get => m_vecCoordX;
            set => m_vecCoordX = value;
        }

        /// <summary>
        ///     Y coordinate of constraint direction.
        /// </summary>
        public double VecCoordY
        {
            get => m_vecCoordY;
            set => m_vecCoordY = value;
        }

        /// <summary>
        ///     Add dimension to constrain the direction of the segment.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.SetSegmentFixedDirection(
                m_segment, m_vecCoordX, m_vecCoordY);
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

        /// <summary>
        ///     Remove the direction dimension of segment.
        /// </summary>
        public override void Commit()
        {
            GetRebarShapeDefinitionBySegments.SetSegmentVariableDirection(m_segment);
        }
    }
}
