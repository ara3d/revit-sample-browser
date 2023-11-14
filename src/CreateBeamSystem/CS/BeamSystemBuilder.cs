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


using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.CreateBeamSystem.CS
{
    /// <summary>
    ///     is used to create new instances of beam system
    /// </summary>
    public class BeamSystemBuilder
    {
        /// <summary>
        ///     the data used to create beam system
        /// </summary>
        private readonly BeamSystemData m_data;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="data">the data used to create beam system</param>
        public BeamSystemBuilder(BeamSystemData data)
        {
            m_data = data;
        }

        /// <summary>
        ///     create beam system according to given profile and property
        /// </summary>
        public void CreateBeamSystem()
        {
            var document = m_data.CommandData.Application.ActiveUIDocument.Document;
            // create curve array and insert Lines in order
            IList<Curve> curves = new List<Curve>();
            foreach (var line in m_data.Lines) curves.Add(line);
            // create beam system takes closed profile consist of lines
            var aBeamSystem = BeamSystem.Create(document, curves, document.ActiveView.SketchPlane, 0);
            // set created beam system's layout rule and beam type property
            aBeamSystem.LayoutRule = m_data.Param.Layout;
            aBeamSystem.BeamType = m_data.Param.BeamType;
        }
    }
}