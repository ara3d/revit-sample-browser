// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.NewRebar.CS.Constraints;
using Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.RebarShapeDef
{
    public abstract class RebarShapeDef
    {
        public RebarShapeDef(RebarShapeDefinition shapeDef)
        {
            RebarshapeDefinition = shapeDef;
            Parameters = new List<RebarShapeParameter>();
            Constraints = new List<ConstraintOnRebarShape>();
        }

        public RebarShapeDefinition RebarshapeDefinition { get; }

        public List<RebarShapeParameter> Parameters { get; }

        public List<ConstraintOnRebarShape> Constraints { get; }

        public RebarShapeParameter AddParameter(Type parameterType, object name, object value)
        {
            var param =
                Activator.CreateInstance(parameterType, this, name, value) as RebarShapeParameter;
            Parameters.Add(param);
            return param;
        }

        /// <summary>
        ///     Add a constraint to RebarShapeDefinition.
        /// </summary>
        /// <param name="constraintType">
        ///     Type of constraint
        ///     (the class must be subclass of ConstraintOnRebarShape).
        /// </param>
        /// <returns></returns>
        public ConstraintOnRebarShape AddConstraint(Type constraintType)
        {
            var constraintIns =
                Activator.CreateInstance(constraintType, this) as ConstraintOnRebarShape;
            Constraints.Add(constraintIns);
            return constraintIns;
        }

        /// <summary>
        ///     Submit RebarShapeDefinition. All the parameters and constraints
        ///     will be added to RebarShape. The RebarShape will be added to Revit document after
        ///     successfully submitted.
        /// </summary>
        /// <param name="defGroup">Parameter definition group</param>
        public void Commit(Document rvtDoc, DefinitionGroup defGroup)
        {
            // Submit all the parameters.
            foreach (var param in Parameters)
            {
                param.Commit(rvtDoc, defGroup);
            }

            // Submit all the constraints.
            foreach (var constraint in Constraints)
            {
                constraint.Commit();
            }

            // Submit the RebarShape.
            if (RebarshapeDefinition.Complete)
                RebarshapeDefinition.CheckDefaultParameterValues(0, 0);
            else
                throw new Exception("The Rebar shape definition is not completed.");
        }

        public List<Type> AllParameterTypes() => new List<Type>
        {
            typeof(RebarShapeParameterDouble),
            typeof(RebarShapeParameterFormula)
        };

        public virtual List<Type> AllowedConstraintTypes() => new List<Type>();
    }
}
