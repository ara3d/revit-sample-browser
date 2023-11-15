// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS
{
    /// <summary>
    ///     This class wraps RebarShapeDefinition object.
    /// </summary>
    public abstract class RebarShapeDef
    {
        /// <summary>
        ///     Constructor, initialize the fields.
        /// </summary>
        /// <param name="shapeDef">RebarShapeDefinition object to be wrapped</param>
        public RebarShapeDef(RebarShapeDefinition shapeDef)
        {
            RebarshapeDefinition = shapeDef;
            Parameters = new List<RebarShapeParameter>();
            Constraints = new List<ConstraintOnRebarShape>();
        }

        /// <summary>
        ///     Return the real object RebarShapeDefinition.
        /// </summary>
        public RebarShapeDefinition RebarshapeDefinition { get; }

        /// <summary>
        ///     Return all the parameters.
        /// </summary>
        public List<RebarShapeParameter> Parameters { get; }

        /// <summary>
        ///     Return all the dimensions.
        /// </summary>
        public List<ConstraintOnRebarShape> Constraints { get; }

        /// <summary>
        ///     Add a parameter to RebarShapeDefinition.
        /// </summary>
        /// <param name="parameterType">
        ///     Parameter type:
        ///     (type of RebarShapeParameterDouble or type of RebarShapeParameterFormula)
        /// </param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value (double value or formula string)</param>
        /// <returns></returns>
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
            foreach (var param in Parameters) param.Commit(rvtDoc, defGroup);

            // Submit all the constraints.
            foreach (var constraint in Constraints) constraint.Commit();

            // Submit the RebarShape.
            if (RebarshapeDefinition.Complete)
                RebarshapeDefinition.CheckDefaultParameterValues(0, 0);
            else
                throw new Exception("The Rebar shape definition is not completed.");
        }

        /// <summary>
        ///     Return all the parameter types supported by RebarShape definition.
        /// </summary>
        /// <returns>All the parameter types supported by RebarShape definition</returns>
        public List<Type> AllParameterTypes()
        {
            var types = new List<Type>
            {
                typeof(RebarShapeParameterDouble),
                typeof(RebarShapeParameterFormula)
            };
            return types;
        }

        /// <summary>
        ///     Return all the constraint types supported by RebarShapeDefinition.
        /// </summary>
        /// <returns>all the constraint types supported by RebarShapeDefinition</returns>
        public virtual List<Type> AllowedConstraintTypes()
        {
            return new List<Type>();
        }
    }
}
