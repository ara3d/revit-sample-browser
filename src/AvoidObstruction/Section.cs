// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.AvoidObstruction.CS
{
    public class Section
    {
        private double m_endFactor;
        private double m_startFactor;

        private Section(XYZ dir)
        {
            PipeCenterLineDirection = dir;
            m_startFactor = 0;
            m_endFactor = 0;
            Refs = [];
            Pipes = [];
        }

        public XYZ PipeCenterLineDirection { get; }

        public List<Pipe> Pipes { get; }

        public XYZ Start => Refs[0].GetReference().GlobalPoint + (PipeCenterLineDirection * m_startFactor);

        public XYZ End => Refs[Refs.Count - 1].GetReference().GlobalPoint + (PipeCenterLineDirection * m_endFactor);

        public List<ReferenceWithContext> Refs { get; }

        public void Inflate(int index, double value)
        {
            switch (index)
            {
                case 0:
                    m_startFactor -= value;
                    break;
                case 1:
                    m_endFactor += value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Index should be 0 or 1.");
            }
        }

        public static List<Section> BuildSections(List<ReferenceWithContext> allrefs, XYZ dir)
        {
            List<ReferenceWithContext> buildStack = [];
            List<Section> sections = [];
            Section current = null;
            foreach (var geoRef in allrefs)
            {
                if (buildStack.Count == 0)
                {
                    current = new Section(dir);
                    sections.Add(current);
                }

                current.Refs.Add(geoRef);

                var tmp = ElementQuery.FindReferenceInList(buildStack, geoRef);
                if (tmp != null)
                    buildStack.Remove(tmp);
                else
                    buildStack.Add(geoRef);
            }

            return sections;
        }
    }
}
