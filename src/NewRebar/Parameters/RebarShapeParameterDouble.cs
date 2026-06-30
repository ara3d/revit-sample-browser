// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters
{
    public class RebarShapeParameterDouble : RebarShapeParameter
    {
        /// <summary>
        ///     Parameter double value.
        /// </summary>
        private double m_value;

        public RebarShapeParameterDouble(RebarShapeDef.RebarShapeDef shapeDef, string name, double value)
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

        public override void Commit(Document doc, DefinitionGroup defGroup)
        {
            var def = GetOrCreateDef(defGroup);
            Parameter = RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(doc, def);
            RebarShapeDef.RebarshapeDefinition.AddParameter(Parameter, m_value);
        }
    }
}
