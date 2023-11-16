// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.NewRebar.CS.Constraints;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.RebarShapeDef
{
    /// <summary>
    ///     This class wraps RebarShapeDefinitionBySegments.
    /// </summary>
    internal class RebarShapeDefBySegment : RebarShapeDef
    {
        public RebarShapeDefBySegment(RebarShapeDefinitionBySegments segmentShapeDef)
            : base(segmentShapeDef)
        {
        }

        /// <summary>
        ///     Get all the constraints supported by RebarShapeDefinitionBySegments.
        /// </summary>
        /// <returns>all the constraints supported by RebarShapeDefinitionBySegments</returns>
        public override List<Type> AllowedConstraintTypes()
        {
            var allowedTypes = base.AllowedConstraintTypes();
            allowedTypes.Add(typeof(ConstraintBendDefaultRadius));
            allowedTypes.Add(typeof(ConstraintBendVariableRadius));
            allowedTypes.Add(typeof(ConstraintParallelToSegment));
            allowedTypes.Add(typeof(ConstraintToSegment));
            allowedTypes.Add(typeof(ListeningDimensionBendToBend));
            allowedTypes.Add(typeof(ListeningDimensionSegmentToBend));
            allowedTypes.Add(typeof(ListeningDimensionSegmentToSegment));
            allowedTypes.Add(typeof(RemoveParameterFromSegment));
            allowedTypes.Add(typeof(SetSegmentAs180DegreeBend));
            allowedTypes.Add(typeof(SetSegmentFixedDirection));
            allowedTypes.Add(typeof(SetSegmentVariableDirection));
            return allowedTypes;
        }
    }
}
