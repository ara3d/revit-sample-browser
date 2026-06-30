// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    public class BeamSystemData
    {
        private static readonly Dictionary<string, FamilySymbol> BeamTypes = [];

        private readonly List<FamilyInstance> m_beams = [];
        private List<Line> m_lines = [];

        public BeamSystemData(ExternalCommandData commandData)
        {
            // initialize members
            CommandData = commandData;
            PrepareData();
            InitializeProfile(m_beams);

            Param = BeamSystemParam.CreateInstance(LayoutMethod.ClearSpacing);
            List<FamilySymbol> beamTypes = [.. BeamTypes.Values];
            Param.BeamType = beamTypes[0];
            Param.LayoutRuleChanged += LayoutRuleChanged;
        }

        public BeamSystemParam Param { get; private set; }

        public ReadOnlyCollection<Line> Lines => new(m_lines);

        public ExternalCommandData CommandData { get; }

        public event EventHandler ParamsUpdated;

        public void ChangeProfileDirection()
        {
            var tmp = m_lines[0];
            m_lines.RemoveAt(0);
            m_lines.Add(tmp);
        }

        public static Dictionary<string, FamilySymbol> GetBeamTypes()
        {
            Dictionary<string, FamilySymbol> beamTypes = new(BeamTypes);
            return beamTypes;
        }

        private void PrepareData()
        {
            var doc = CommandData.Application.ActiveUIDocument;
            BeamTypes.Clear();

            // iterate all selected beams
            foreach (var elementId in doc.Selection.GetElementIds())
            {
                object obj = doc.Document.GetElement(elementId);
                if (obj is not FamilyInstance beam) continue;

                // add beam to lists according to category name
                var categoryName = beam.Category.Name;
                if ("Structural Framing" == categoryName
                    && beam.StructuralType == StructuralType.Beam)
                    m_beams.Add(beam);
            }

            //iterate all beam types
            var itor = new FilteredElementCollector(doc.Document).OfClass(typeof(Family)).GetElementIterator();
            itor.Reset();
            while (itor.MoveNext())
            {
                // get Family to get FamilySymbols
                if (itor.Current is not Family aFamily) continue;

                foreach (var symbolId in aFamily.GetFamilySymbolIds())
                {
                    var symbol = doc.Document.GetElement(symbolId) as FamilySymbol;
                    if (null == symbol.Category) continue;

                    // add symbols to lists according to category name
                    var categoryName = symbol.Category.Name;
                    if ("Structural Framing" == categoryName)
                        BeamTypes.Add($"{symbol.Family.Name}:{symbol.Name}", symbol);
                }
            }

            if (m_beams.Count == 0) throw new ErrorMessageException("Please select beams.");

            if (BeamTypes.Count == 0)
                throw new ErrorMessageException("There is no Beam families loaded in current project.");
        }

        private void InitializeProfile(List<FamilyInstance> beams)
        {
            // retrieve collection of lines in beams
            List<Line> lines = [];
            foreach (var beam in beams)
            {
                var locationLine = beam.Location as LocationCurve;
                var line = locationLine.Curve as Line;
                if (null == line) throw new ErrorMessageException("Please don't select any arc beam.");
                lines.Add(line);
            }

            // lines should in the same horizontal plane
            if (!XyzMath.InSameHorizontalPlane(lines))
                throw new ErrorMessageException("The selected beams can't form a horizontal profile.");

            m_lines = XyzMath.SortLines(lines);
            // lines can't make a closed profile
            if (null == m_lines) throw new ErrorMessageException("The selected beams can't form a closed profile.");
        }

        private void LayoutRuleChanged(ref LayoutMethod layoutMethod)
        {
            // create BeamSystemParams instance according to changed LayoutMethod
            Param = Param.CloneInstance(layoutMethod);

            // raise DataUpdated event
            OnParamsUpdated(new EventArgs());

            // rebind delegate
            Param.LayoutRuleChanged += LayoutRuleChanged;
        }

        protected virtual void OnParamsUpdated(EventArgs e)
        {
            ParamsUpdated?.Invoke(this, e);
        }
    }
}
