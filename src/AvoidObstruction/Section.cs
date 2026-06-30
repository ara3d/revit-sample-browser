// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

using Ara3D.RevitSampleBrowser.Common.Documents;
namespace Ara3D.RevitSampleBrowser.AvoidObstruction.CS
{
    public class Section
    {
        private readonly XYZ m_dir;

        private double m_endFactor;

        private readonly List<Pipe> m_pipes;

        private readonly List<ReferenceWithContext> m_refs;

        private double m_startFactor;

        private Section(XYZ dir)
        {
            m_dir = dir;
            m_startFactor = 0;
            m_endFactor = 0;
            m_refs = new List<ReferenceWithContext>();
            m_pipes = new List<Pipe>();
        }

        public XYZ PipeCenterLineDirection => m_dir;

        public List<Pipe> Pipes => m_pipes;

        public XYZ Start => m_refs[0].GetReference().GlobalPoint + m_dir * m_startFactor;

        public XYZ End => m_refs[m_refs.Count - 1].GetReference().GlobalPoint + m_dir * m_endFactor;

        public List<ReferenceWithContext> Refs => m_refs;

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
            var buildStack = new List<ReferenceWithContext>();
            var sections = new List<Section>();
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
