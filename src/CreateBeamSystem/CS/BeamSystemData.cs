// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CreateBeamSystem.CS
{
    /// <summary>
    ///     mixed data class save the data to show in UI
    ///     and the data used to create beam system
    /// </summary>
    public class BeamSystemData
    {
        /// <summary>
        ///     all beam types loaded in current Revit project
        ///     it is declared as static only because of PropertyGrid
        /// </summary>
        private static readonly Dictionary<string, FamilySymbol> BeamTypes = new Dictionary<string, FamilySymbol>();

        /// <summary>
        ///     a number of beams that intersect end to end
        ///     so that form a profile used as beam system's profile
        /// </summary>
        private readonly List<FamilyInstance> m_beams = new List<FamilyInstance>();

        /// <summary>
        ///     buffer of ExternalCommandData
        /// </summary>
        private readonly ExternalCommandData m_commandData;

        /// <summary>
        ///     the lines compose the profile of beam system
        /// </summary>
        private List<Line> m_lines = new List<Line>();

        /// <summary>
        ///     properties of beam system
        /// </summary>
        private BeamSystemParam m_param;

        /// <summary>
        ///     constructor
        ///     if precondition in current Revit project isn't enough,
        ///     ErrorMessageException will be throw out
        /// </summary>
        /// <param name="commandData">data from Revit</param>
        public BeamSystemData(ExternalCommandData commandData)
        {
            // initialize members
            m_commandData = commandData;
            PrepareData();
            InitializeProfile(m_beams);

            m_param = BeamSystemParam.CreateInstance(LayoutMethod.ClearSpacing);
            var beamTypes = new List<FamilySymbol>(BeamTypes.Values);
            m_param.BeamType = beamTypes[0];
            m_param.LayoutRuleChanged += LayoutRuleChanged;
        }

        /// <summary>
        ///     properties of beam system
        /// </summary>
        public BeamSystemParam Param => m_param;

        /// <summary>
        ///     lines form the profile of beam system
        /// </summary>
        public ReadOnlyCollection<Line> Lines => new ReadOnlyCollection<Line>(m_lines);

        /// <summary>
        ///     buffer of ExternalCommandData
        /// </summary>
        public ExternalCommandData CommandData => m_commandData;

        /// <summary>
        ///     the data used to show in UI is updated
        /// </summary>
        public event EventHandler ParamsUpdated;

        /// <summary>
        ///     change the direction to the next line in the profile
        /// </summary>
        public void ChangeProfileDirection()
        {
            var tmp = m_lines[0];
            m_lines.RemoveAt(0);
            m_lines.Add(tmp);
        }

        /// <summary>
        ///     all beam types loaded in current Revit project
        ///     it is declared as static only because of PropertyGrid
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, FamilySymbol> GetBeamTypes()
        {
            var beamTypes = new Dictionary<string, FamilySymbol>(BeamTypes);
            return beamTypes;
        }

        /// <summary>
        ///     initialize members using data from current Revit project
        /// </summary>
        private void PrepareData()
        {
            var doc = m_commandData.Application.ActiveUIDocument;
            BeamTypes.Clear();

            // iterate all selected beams
            foreach (var elementId in doc.Selection.GetElementIds())
            {
                object obj = doc.Document.GetElement(elementId);
                if (!(obj is FamilyInstance beam)) continue;

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
                if (!(itor.Current is Family aFamily)) continue;

                foreach (var symbolId in aFamily.GetFamilySymbolIds())
                {
                    var symbol = doc.Document.GetElement(symbolId) as FamilySymbol;
                    if (null == symbol.Category) continue;

                    // add symbols to lists according to category name
                    var categoryName = symbol.Category.Name;
                    if ("Structural Framing" == categoryName)
                        BeamTypes.Add(symbol.Family.Name + ":" + symbol.Name, symbol);
                }
            }

            if (m_beams.Count == 0) throw new ErrorMessageException("Please select beams.");

            if (BeamTypes.Count == 0)
                throw new ErrorMessageException("There is no Beam families loaded in current project.");
        }

        /// <summary>
        ///     retrieve the profiles using the selected beams
        ///     ErrorMessageException will be thrown out if beams can't make a closed profile
        /// </summary>
        /// <param name="beams">beams which may form a closed profile</param>
        private void InitializeProfile(List<FamilyInstance> beams)
        {
            // retrieve collection of lines in beams
            var lines = new List<Line>();
            foreach (var beam in beams)
            {
                var locationLine = beam.Location as LocationCurve;
                var line = locationLine.Curve as Line;
                if (null == line) throw new ErrorMessageException("Please don't select any arc beam.");
                lines.Add(line);
            }

            // lines should in the same horizontal plane
            if (!GeometryUtil.InSameHorizontalPlane(lines))
                throw new ErrorMessageException("The selected beams can't form a horizontal profile.");

            // sorted lines so that all lines are intersect end to end
            m_lines = GeometryUtil.SortLines(lines);
            // lines can't make a closed profile
            if (null == m_lines) throw new ErrorMessageException("The selected beams can't form a closed profile.");
        }

        /// <summary>
        ///     layout rule of beam system has changed
        /// </summary>
        /// <param name="layoutMethod">changed method</param>
        private void LayoutRuleChanged(ref LayoutMethod layoutMethod)
        {
            // create BeamSystemParams instance according to changed LayoutMethod
            m_param = m_param.CloneInstance(layoutMethod);

            // raise DataUpdated event
            OnParamsUpdated(new EventArgs());

            // rebind delegate
            m_param.LayoutRuleChanged += LayoutRuleChanged;
        }

        /// <summary>
        ///     the data used to show in UI is updated
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnParamsUpdated(EventArgs e)
        {
            ParamsUpdated?.Invoke(this, e);
        }
    }
}
