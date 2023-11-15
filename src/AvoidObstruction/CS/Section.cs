// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace Ara3D.RevitSampleBrowser.AvoidObstruction.CS
{
    /// <summary>
    ///     This class presents an obstruction of a Pipe.
    /// </summary>
    internal class Section
    {
        /// <summary>
        ///     Pipe centerline's direction.
        /// </summary>
        private readonly XYZ m_dir;

        /// <summary>
        ///     Extend factor in positive direction.
        /// </summary>
        private double m_endFactor;

        /// <summary>
        ///     Pipes to avoid this obstruction, it is assigned when resolving this obstruction.
        ///     Its count will be three if resolved, the three pipe constructs a "U" shape to round the obstruction.
        /// </summary>
        private readonly List<Pipe> m_pipes;

        /// <summary>
        ///     References contained in this obstruction.
        /// </summary>
        private readonly List<ReferenceWithContext> m_refs;

        /// <summary>
        ///     Extend factor in negative direction.
        /// </summary>
        private double m_startFactor;

        /// <summary>
        ///     Private constructor, just be called in static factory method BuildSections.
        /// </summary>
        /// <param name="dir">Pipe's direction</param>
        private Section(XYZ dir)
        {
            m_dir = dir;
            m_startFactor = 0;
            m_endFactor = 0;
            m_refs = new List<ReferenceWithContext>();
            m_pipes = new List<Pipe>();
        }

        /// <summary>
        ///     Pipe centerline's direction.
        /// </summary>
        public XYZ PipeCenterLineDirection => m_dir;

        /// <summary>
        ///     Pipes to avoid this obstruction, it is assigned when resolving this obstruction.
        ///     Its count will be three if resolved, the three pipe constructs a "U" shape to round the obstruction.
        /// </summary>
        public List<Pipe> Pipes => m_pipes;

        /// <summary>
        ///     Start point of this obstruction.
        /// </summary>
        public XYZ Start => m_refs[0].GetReference().GlobalPoint + m_dir * m_startFactor;

        /// <summary>
        ///     End point of this obstruction.
        /// </summary>
        public XYZ End => m_refs[m_refs.Count - 1].GetReference().GlobalPoint + m_dir * m_endFactor;

        /// <summary>
        ///     References contained in this obstruction.
        /// </summary>
        public List<ReferenceWithContext> Refs => m_refs;

        /// <summary>
        ///     Extend this obstruction's interval in one direction.
        /// </summary>
        /// <param name="index">index of direction, 0 => start, 1 => end</param>
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

        /// <summary>
        ///     Build sections for References, it's a factory method to build sections.
        ///     A section contains several points through which the ray passes the obstruction(s).
        ///     for example, a section may contain 2 points when the obstruction is stand alone,
        ///     or contain 4 points if 2 obstructions are intersects with each other in the direction of the ray.
        /// </summary>
        /// <param name="allrefs">References</param>
        /// <param name="dir">Pipe's direction</param>
        /// <returns>List of Section</returns>
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

                var tmp = Find(buildStack, geoRef);
                if (tmp != null)
                    buildStack.Remove(tmp);
                else
                    buildStack.Add(geoRef);
            }

            return sections;
        }

        /// <summary>
        ///     Judge whether a Reference is already in the list of Reference, return the founded value.
        /// </summary>
        /// <param name="arr">List of Reference</param>
        /// <param name="entry">Reference to test</param>
        /// <returns>One Reference has the same element's Id with entry</returns>
        private static ReferenceWithContext Find(List<ReferenceWithContext> arr, ReferenceWithContext entry)
        {
            foreach (var tmp in arr)
                if (tmp.GetReference().ElementId == entry.GetReference().ElementId)
                    return tmp;
            return null;
        }
    }
}
