// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.TagBeam.CS
{
    /// <summary>
    ///     A wrapper of family symbol
    /// </summary>
    public class FamilySymbolWrapper
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="tagSymbol"></param>
        public FamilySymbolWrapper(FamilySymbol familySymbol)
        {
            FamilySymbol = familySymbol;
        }

        /// <summary>
        ///     Family symbol
        /// </summary>
        public FamilySymbol FamilySymbol { get; }

        /// <summary>
        ///     Display name
        /// </summary>
        public string Name => FamilySymbol.Family.Name + " : " + FamilySymbol.Name;
    }
}
