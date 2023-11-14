// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


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
