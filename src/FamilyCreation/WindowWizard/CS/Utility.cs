// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    /// <summary>
    ///     A common class for users to get some specified element
    /// </summary>
    public class Utility
    {
        
        /// <summary>
        ///     This function is used to convert from metric to imperial
        /// </summary>
        /// <param name="value">the metric value</param>
        /// <returns>the result</returns>
        public static double MetricToImperial(double value)
        {
            return value / 304.8; //* 0.00328;
        }

        /// <summary>
        ///     This function is used to convert from imperial to metric
        /// </summary>
        /// <param name="value">the imperial value</param>
        /// <returns>the result</returns>
        public static double ImperialToMetric(double value)
        {
            return value * 304.8;
        }
    }
}
