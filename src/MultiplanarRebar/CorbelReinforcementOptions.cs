// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.MultiplanarRebar.CS
{
    /// <summary>
    ///     Represent the reinforcement options of corbel.
    ///     The options include bar type and bar counts which are collected from user via UI input.
    /// </summary>
    public class CorbelReinforcementOptions
    {
        public CorbelReinforcementOptions(Document revitDoc)
        {
            RevitDoc = revitDoc;
            var filteredElementCollector = new FilteredElementCollector(RevitDoc);
            filteredElementCollector.OfClass(typeof(RebarBarType));
            RebarBarTypes = filteredElementCollector.Cast<RebarBarType>().ToList();
        }

        public Document RevitDoc { get; set; }

        public List<RebarBarType> RebarBarTypes { get; set; }

        public RebarBarType TopBarType { get; set; }

        public RebarBarType StirrupBarType { get; set; }

        public RebarBarType MultiplanarBarType { get; set; }

        public RebarBarType HostStraightBarType { get; set; }

        public int TopBarCount { get; set; }

        public int StirrupBarCount { get; set; }
    }
}
