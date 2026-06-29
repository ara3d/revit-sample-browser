// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.BoundaryConditions.CS
{
    public struct ConversionValue
    {
        public ConversionValue(int precision, double ratio, string unitName)
        {
            Precision = precision;
            Ratio = ratio;
            UnitName = unitName;
        }

        public int Precision { get; }
        public string UnitName { get; }
        public double Ratio { get; }
    }
}
