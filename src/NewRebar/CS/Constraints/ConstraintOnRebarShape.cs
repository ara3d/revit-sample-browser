// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace RevitMultiSample.NewRebar.CS
{
    /// <summary>
    ///     Dimension on RebarShape.
    /// </summary>
    public abstract class ConstraintOnRebarShape
    {
        /// <summary>
        ///     A wrapper of RebarShapeDefinition.
        /// </summary>
        protected readonly RebarShapeDef ShapeDef;

        /// <summary>
        ///     Constructor, initialize fields.
        /// </summary>
        /// <param name="def">RebarShapeDef object</param>
        protected ConstraintOnRebarShape(RebarShapeDef def)
        {
            ShapeDef = def;
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
            TypeConverterRebarShapeParameter.RebarShapeParameters = ShapeDef.Parameters;
        }

        /// <summary>
        ///     Commit the dimension.
        /// </summary>
        public abstract void Commit();
    }
}
