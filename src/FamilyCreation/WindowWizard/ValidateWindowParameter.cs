// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public class ValidateWindowParameter
    {
        public bool IsMetric;

        private readonly double m_wallHeight = 10;
        private readonly double m_wallWidth = 10;

        public ValidateWindowParameter(double wallHeight, double wallWidth)
        {
            if (wallHeight >= 0) m_wallHeight = wallHeight;
            if (wallWidth >= 0) m_wallWidth = wallWidth;
        }

        /// <summary>
        /// Prefixes "0" so values like ".5" parse as doubles.
        /// </summary>
        public string IsDouble(string value, ref double result)
        {
            return double.TryParse($"0{value}", out result) ? string.Empty : "Please input a double value.";
        }

        public string IsWidthInRange(double value)
        {
            if (IsMetric)
            {
                value = SampleBrowserUtils.MetricToImperial(value);
                if (value >= 0.23 && value < m_wallWidth)
                    return string.Empty;
                return $"The width should be between 69 and {Convert.ToInt32(SampleBrowserUtils.ImperialToMetric(m_wallWidth))}";
            }

            if (value >= 0.4 && value < m_wallWidth)
                return string.Empty;
            return $"The width should be between 0.4 and {m_wallWidth}";
        }

        public string IsHeightInRange(double value)
        {
            if (IsMetric)
            {
                value = SampleBrowserUtils.MetricToImperial(value);
                return value >= 0.23 ? string.Empty : "The height should > 69";
            }

            return value >= 0.4 ? string.Empty : "The height should > 0.4";
        }

        public string IsInsetInRange(double value)
        {
            if (IsMetric)
                value = SampleBrowserUtils.MetricToImperial(value);
            return value >= 0 ? string.Empty : "The Inset should > 0";
        }

        public string IsSillHeightInRange(double value)
        {
            if (IsMetric)
            {
                value = SampleBrowserUtils.MetricToImperial(value);
                if (value < m_wallHeight)
                    return string.Empty;
                return $"The sillheight should be < {Convert.ToInt32(SampleBrowserUtils.ImperialToMetric(m_wallHeight))}";
            }

            if (value < m_wallHeight)
                return string.Empty;
            return $"The sillheight should be < {m_wallHeight}";
        }
    }
}
