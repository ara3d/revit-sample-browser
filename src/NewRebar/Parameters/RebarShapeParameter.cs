// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters
{
    public abstract class RebarShapeParameter
    {
        /// <summary>
        ///     RebarShape definition proxy object.
        /// </summary>
        protected readonly RebarShapeDef.RebarShapeDef RebarShapeDef;

        protected RebarShapeParameter(RebarShapeDef.RebarShapeDef shapeDef, string name)
        {
            RebarShapeDef = shapeDef;
            Name = name;
            Parameter = null;
        }

        /// <summary>
        ///     Parameter, it is the result of commit.
        /// </summary>
        [Browsable(false)]
        public ElementId Parameter { get; protected set; }

        /// <summary>
        ///     Parameter name string.
        /// </summary>
        public string Name { get; }

        protected ExternalDefinition GetOrCreateDef(DefinitionGroup group)
        {
            if (!(group.Definitions.get_Item(Name) is ExternalDefinition bdef))
            {
                var externalDefinitionCreationOptions =
                    new ExternalDefinitionCreationOptions(Name, SpecTypeId.ReinforcementLength);
                bdef = group.Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;
            }

            return bdef;
        }

        /// <summary>
        ///     Yield the Parameter.
        /// </summary>
        /// <param name="defGroup">Definition group</param>
        public abstract void Commit(Document doc, DefinitionGroup defGroup);
    }
}
