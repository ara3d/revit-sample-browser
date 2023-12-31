// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters
{
    /// <summary>
    ///     This class wraps a formula parameter which will be the dimension of
    ///     RebarShape definition.
    /// </summary>
    public class RebarShapeParameterFormula : RebarShapeParameter
    {
        /// <summary>
        ///     Parameter formula sting.
        /// </summary>
        private string m_formula;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="shapeDef">RebarShapeDefinition proxy</param>
        /// <param name="name">Parameter name</param>
        /// <param name="formula">Parameter formula sting</param>
        public RebarShapeParameterFormula(RebarShapeDef.RebarShapeDef shapeDef, string name, string formula)
            : base(shapeDef, name)
        {
            m_formula = formula;
        }

        /// <summary>
        ///     Parameter formula sting.
        /// </summary>
        public string Formula
        {
            get => m_formula;
            set => m_formula = value;
        }

        /// <summary>
        ///     Add a formula parameter to RebarShapeDefinition.
        /// </summary>
        /// <param name="defGroup">Definition group</param>
        public override void Commit(Document doc, DefinitionGroup defGroup)
        {
            var def = GetOrCreateDef(defGroup);
            Parameter = RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(doc, def);
            RebarShapeDef.RebarshapeDefinition.AddFormulaParameter(Parameter, m_formula);
        }
    }
}
