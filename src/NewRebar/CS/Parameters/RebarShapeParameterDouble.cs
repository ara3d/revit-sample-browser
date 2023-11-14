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

using System;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.NewRebar.CS
{
    /// <summary>
    /// This class wraps a double parameter which will be the dimension of 
    /// RebarShape definition.
    /// </summary>
    class RebarShapeParameterDouble : RebarShapeParameter
    {
        /// <summary>
        /// Parameter double value.
        /// </summary>
        private double m_value;

        /// <summary>
        /// Parameter double value.
        /// </summary>
        public double Value
        {
            get => m_value;
            set => m_value = value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shapeDef">RebarShapeDefinition proxy</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter double value</param>
        public RebarShapeParameterDouble(RebarShapeDef shapeDef, string name, double value)
            : base(shapeDef, name)
        {
            m_value = value;
        }

        /// <summary>
        /// Add a double parameter to RebarShapeDefinition.
        /// </summary>
        /// <param name="defGroup">Definition group</param>
        public override void Commit(Document doc, DefinitionGroup defGroup)
        {
            var def = GetOrCreateDef(defGroup);
            m_parameterId = Autodesk.Revit.DB.Structure.RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(doc, def);
            m_rebarShapeDef.RebarshapeDefinition.AddParameter(m_parameterId, m_value);
        }
    }
}
