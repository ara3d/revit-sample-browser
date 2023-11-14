// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.ComponentModel;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.NewRebar.CS
{
    /// <summary>
    ///     This class wraps an Parameter object which will be added
    ///     to RebarShape definition. This is a abstract class and it will be inherited
    ///     by two classes: RebarShapeParameterDouble and RebarShapeParameterFormula.
    /// </summary>
    public abstract class RebarShapeParameter
    {
        /// <summary>
        ///     Parameter name string.
        /// </summary>
        protected string m_name;

        /// <summary>
        ///     Parameter id. It is the result of commit.
        /// </summary>
        protected ElementId m_parameterId;

        /// <summary>
        ///     RebarShape definition proxy object.
        /// </summary>
        protected RebarShapeDef m_rebarShapeDef;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="shapeDef">RebarShapeDefinition proxy</param>
        /// <param name="name">Parameter name</param>
        protected RebarShapeParameter(RebarShapeDef shapeDef, string name)
        {
            m_rebarShapeDef = shapeDef;
            m_name = name;
            m_parameterId = null;
        }

        /// <summary>
        ///     Parameter, it is the result of commit.
        /// </summary>
        [Browsable(false)]
        public ElementId Parameter => m_parameterId;

        /// <summary>
        ///     Parameter name string.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        ///     Get a external definition if there exists one, otherwise create a new one.
        /// </summary>
        /// <param name="group">Definition group</param>
        /// <returns>External definition</returns>
        protected ExternalDefinition GetOrCreateDef(DefinitionGroup group)
        {
            var Bdef =
                group.Definitions.get_Item(m_name) as ExternalDefinition;

            if (Bdef == null)
            {
                var ExternalDefinitionCreationOptions =
                    new ExternalDefinitionCreationOptions(m_name, SpecTypeId.ReinforcementLength);
                Bdef = group.Definitions.Create(ExternalDefinitionCreationOptions) as ExternalDefinition;
            }

            return Bdef;
        }

        /// <summary>
        ///     Yield the Parameter.
        /// </summary>
        /// <param name="defGroup">Definition group</param>
        public abstract void Commit(Document doc, DefinitionGroup defGroup);
    }
}
