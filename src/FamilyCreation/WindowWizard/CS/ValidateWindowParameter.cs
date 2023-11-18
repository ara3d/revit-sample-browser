// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    /// <summary>
    ///     class is used to validate window parameters
    /// </summary>
    public class ValidateWindowParameter
    {
        /// <summary>
        ///     indicate the template file is metric or not
        /// </summary>
        public bool IsMetric;

        /// <summary>
        ///     store the wall's height
        /// </summary>
        private readonly double m_wallHeight = 10;

        /// <summary>
        ///     store the wall's width
        /// </summary>
        private readonly double m_wallWidth = 10;

        /// <summary>
        ///     constructor of ValidateWindowParameter
        /// </summary>
        /// <param name="wallHeight">wall height parameter</param>
        /// <param name="wallWidth">wall width parameter</param>
        public ValidateWindowParameter(double wallHeight, double wallWidth)
        {
            if (wallHeight >= 0) m_wallHeight = wallHeight;
            if (wallWidth >= 0) m_wallWidth = wallWidth;
        }

        /// <summary>
        ///     This method is used to check whether a value string is double type
        /// </summary>
        /// <param name="value">>the string value</param>
        /// <param name="result">the double result</param>
        /// <returns>the validation result message</returns>
        public string IsDouble(string value, ref double result)
        {
            return double.TryParse($"0{value}", out result) ? string.Empty : "Please input a double value.";
        }

        /// <summary>
        ///     This method is used to check whether the width value is out of range
        /// </summary>
        /// <param name="value">the string value</param>
        /// <returns>the validation result message</returns>
        public string IsWidthInRange(double value)
        {
            if (IsMetric)
            {
                value = Utility.MetricToImperial(value);
                if (value >= 0.23 && value < m_wallWidth)
                    return string.Empty;
                return $"The width should be between 69 and {Convert.ToInt32(Utility.ImperialToMetric(m_wallWidth))}";
            }

            if (value >= 0.4 && value < m_wallWidth)
                return string.Empty;
            return $"The width should be between 0.4 and {m_wallWidth}";
        }

        /// <summary>
        ///     This method is used to check whether the height value is out of range
        /// </summary>
        /// <param name="value">the string value</param>
        /// <returns>the validation result message</returns>
        public string IsHeightInRange(double value)
        {
            if (IsMetric)
            {
                value = Utility.MetricToImperial(value);
                return value >= 0.23 ? string.Empty : "The height should > 69";
            }

            return value >= 0.4 ? string.Empty : "The height should > 0.4";
        }

        /// <summary>
        ///     This method is used to check whether the inset value is out of range
        /// </summary>
        /// <param name="value">the string value</param>
        /// <returns>the validation result message</returns>
        public string IsInsetInRange(double value)
        {
            if (IsMetric)
                value = Utility.MetricToImperial(value);
            return value >= 0 ? string.Empty : "The Inset should > 0";
        }

        /// <summary>
        ///     This method is used to check whether the sillheight value is out of range
        /// </summary>
        /// <param name="value">the string value</param>
        /// <returns>the validation result message</returns>
        public string IsSillHeightInRange(double value)
        {
            if (IsMetric)
            {
                value = Utility.MetricToImperial(value);
                if (value < m_wallHeight)
                    return string.Empty;
                return $"The sillheight should be < {Convert.ToInt32(Utility.ImperialToMetric(m_wallHeight))}";
            }

            if (value < m_wallHeight)
                return string.Empty;
            return $"The sillheight should be < {m_wallHeight}";
        }
    }
}
