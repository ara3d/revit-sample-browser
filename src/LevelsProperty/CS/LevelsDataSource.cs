// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.LevelsProperty.CS
{
    /// <summary>
    ///     Data source used to store a Level
    /// </summary>
    public class LevelsDataSource
    {
        /// <summary>
        ///     First column used to store Level's Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Second column to store Level's Elevation
        /// </summary>
        public double Elevation { get; set; }

        /// <summary>
        ///     Record Level's ID
        /// </summary>
        public ElementId LevelIdValue { get; set; }
    }
}
