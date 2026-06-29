// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewRebar.CS.TypeConverter;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.Constraints
{
    /// <summary>
    ///     Dimension on RebarShape.
    /// </summary>
    public abstract class ConstraintOnRebarShape
    {
        /// <summary>
        ///     A wrapper of RebarShapeDefinition.
        /// </summary>
        protected readonly RebarShapeDef.RebarShapeDef ShapeDef;

        /// <summary>
        ///     Constructor, initialize fields.
        /// </summary>
        /// <param name="def">RebarShapeDef object</param>
        protected ConstraintOnRebarShape(RebarShapeDef.RebarShapeDef def)
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
