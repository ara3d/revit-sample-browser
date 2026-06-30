// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    public class BeamSystemBuilder
    {
        private readonly BeamSystemData m_data;

        public BeamSystemBuilder(BeamSystemData data)
        {
            m_data = data;
        }

        public void CreateBeamSystem()
        {
            var document = m_data.CommandData.Application.ActiveUIDocument.Document;
            // create curve array and insert Lines in order
            IList<Curve> curves = new List<Curve>();
            foreach (var line in m_data.Lines)
            {
                curves.Add(line);
            }

            // create beam system takes closed profile consist of lines
            var aBeamSystem = BeamSystem.Create(document, curves, document.ActiveView.SketchPlane, 0);
            // set created beam system's layout rule and beam type property
            aBeamSystem.LayoutRule = m_data.Param.Layout;
            aBeamSystem.BeamType = m_data.Param.BeamType;
        }
    }
}
