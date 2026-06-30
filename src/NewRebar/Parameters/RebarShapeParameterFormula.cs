// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters
{
    public class RebarShapeParameterFormula : RebarShapeParameter
    {
        /// <summary>
        ///     Parameter formula sting.
        /// </summary>
        private string m_formula;

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

        public override void Commit(Document doc, DefinitionGroup defGroup)
        {
            var def = GetOrCreateDef(defGroup);
            Parameter = RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(doc, def);
            RebarShapeDef.RebarshapeDefinition.AddFormulaParameter(Parameter, m_formula);
        }
    }
}
