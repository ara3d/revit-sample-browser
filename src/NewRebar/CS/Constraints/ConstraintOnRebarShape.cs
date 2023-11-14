// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace Revit.SDK.Samples.NewRebar.CS
{
    /// <summary>
    ///     Dimension on RebarShape.
    /// </summary>
    public abstract class ConstraintOnRebarShape
    {
        /// <summary>
        ///     A wrapper of RebarShapeDefinition.
        /// </summary>
        protected RebarShapeDef m_shapeDef;

        /// <summary>
        ///     Constructor, initialize fields.
        /// </summary>
        /// <param name="def">RebarShapeDef object</param>
        protected ConstraintOnRebarShape(RebarShapeDef def)
        {
            m_shapeDef = def;
        }

        /// <summary>
        ///     Name of the constraint.
        /// </summary>
        public string Name => GetType().Name;

        /// <summary>
        ///     Update the parameter list value for property grid.
        /// </summary>
        protected void UpdateParameterTypeConverter()
        {
            TypeConverterRebarShapeParameter.RebarShapeParameters = m_shapeDef.Parameters;
        }

        /// <summary>
        ///     Commit the dimension.
        /// </summary>
        public abstract void Commit();
    }
}
