// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace RevitMultiSample.Loads.CS
{
    /// <summary>
    ///     The map class which store the data and display in formulaDataGridView
    /// </summary>
    public class FormulaMap
    {
        // Methods
        /// <summary>
        ///     Default constructor of FormulaMap
        /// </summary>
        public FormulaMap()
        {
            Factor = 0;
        }

        /// <summary>
        ///     constructor with the case name
        /// </summary>
        /// <param name="caseName">The value set to Case Property</param>
        public FormulaMap(string caseName)
        {
            Factor = 1;
            Case = caseName;
        }

        /// <summary>
        ///     Constructor with the factor and case name
        /// </summary>
        /// <param name="factor">The value set to Factor Property</param>
        /// <param name="caseName">The value set to Case Property</param>
        public FormulaMap(double factor, string caseName)
        {
            Factor = factor;
            Case = caseName;
        }
        // Private Members

        /// <summary>
        ///     Factor
        /// </summary>
        public double Factor { get; set; }

        /// <summary>
        ///     Load Case
        /// </summary>
        public string Case { get; set; }
    }
}
