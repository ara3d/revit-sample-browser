// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace RevitMultiSample.NewRebar.CS
{
    /// <summary>
    ///     This class wraps a double parameter which will be the dimension of
    ///     RebarShape definition.
    /// </summary>
    internal class RebarShapeParameterDouble : RebarShapeParameter
    {
        /// <summary>
        ///     Parameter double value.
        /// </summary>
        private double m_value;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="shapeDef">RebarShapeDefinition proxy</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter double value</param>
        public RebarShapeParameterDouble(RebarShapeDef shapeDef, string name, double value)
            : base(shapeDef, name)
        {
            m_value = value;
        }

        /// <summary>
        ///     Parameter double value.
        /// </summary>
        public double Value
        {
            get => m_value;
            set => m_value = value;
        }

        /// <summary>
        ///     Add a double parameter to RebarShapeDefinition.
        /// </summary>
        /// <param name="defGroup">Definition group</param>
        public override void Commit(Document doc, DefinitionGroup defGroup)
        {
            var def = GetOrCreateDef(defGroup);
            Parameter = RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(doc, def);
            RebarShapeDef.RebarshapeDefinition.AddParameter(Parameter, m_value);
        }
    }
}
