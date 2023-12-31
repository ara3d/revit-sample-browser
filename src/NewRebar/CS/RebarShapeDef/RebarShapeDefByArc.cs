// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.NewRebar.CS.Constraints;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.RebarShapeDef
{
    /// <summary>
    ///     This class wraps RebarShapeDefinitionByArc.
    /// </summary>
    public class RebarShapeDefByArc : RebarShapeDef
    {
        public RebarShapeDefByArc(RebarShapeDefinitionByArc arcShapeDef)
            : base(arcShapeDef)
        {
        }

        /// <summary>
        ///     Get all the constraint types supported by RebarShapeDefinitionByArc.
        /// </summary>
        /// <returns>all the constraint types supported by RebarShapeDefinitionByArc</returns>
        public override List<Type> AllowedConstraintTypes()
        {
            var allowedTypes = base.AllowedConstraintTypes();

            allowedTypes.Add(typeof(ConstraintRadius));
            allowedTypes.Add(typeof(ConstraintDiameter));
            allowedTypes.Add(typeof(ConstraintArcLength));
            allowedTypes.Add(typeof(ConstraintCircumference));
            allowedTypes.Add(typeof(ConstraintChordLength));
            allowedTypes.Add(typeof(ConstraintSagittaLength));

            return allowedTypes;
        }
    }
}
