// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.TagBeam.CS
{
    public class FamilySymbolWrapper
    {
        public FamilySymbolWrapper(FamilySymbol familySymbol)
        {
            FamilySymbol = familySymbol;
        }

        public FamilySymbol FamilySymbol { get; }

        public string Name => $"{FamilySymbol.Family.Name} : {FamilySymbol.Name}";
    }
}
