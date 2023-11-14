//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

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