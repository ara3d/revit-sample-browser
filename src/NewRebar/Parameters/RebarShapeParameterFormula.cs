// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters
{
    public class RebarShapeParameterFormula : RebarShapeParameter
    {
        public RebarShapeParameterFormula(RebarShapeDef.RebarShapeDef shapeDef, string name, string formula)
            : base(shapeDef, name)
        {
            Formula = formula;
        }

        /// <summary>
        ///     Parameter formula sting.
        /// </summary>
        public string Formula { get; set; }

        public override void Commit(Document doc, DefinitionGroup defGroup)
        {
            var def = GetOrCreateDef(defGroup);
            Parameter = RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(doc, def);
            RebarShapeDef.RebarshapeDefinition.AddFormulaParameter(Parameter, Formula);
        }
    }
}
