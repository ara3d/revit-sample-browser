// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewRebar.CS.Constraints;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.RebarShapeDef
{
    public class RebarShapeDefByArc : RebarShapeDef
    {
        public RebarShapeDefByArc(RebarShapeDefinitionByArc arcShapeDef)
            : base(arcShapeDef)
        {
        }

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
