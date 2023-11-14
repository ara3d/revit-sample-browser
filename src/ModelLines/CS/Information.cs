// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;

namespace RevitMultiSample.ModelLines.CS
{
    /// <summary>
    ///     The map class which store the data and display in informationDataGridView
    /// </summary>
    public class ModelCurveCounter
    {
        // Methods
        /// <summary>
        ///     The constructor of ModelCurveCounter
        /// </summary>
        /// <param name="typeName">The type name</param>
        public ModelCurveCounter(string typeName)
        {
            TypeName = typeName;
        }
        // Private members

        // Properties
        /// <summary>
        ///     Indicate the type name, such ModelArc, ModelLine, etc
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        ///     Indicate the number of the corresponding type which name stored in type name
        /// </summary>
        public int Number { get; set; }
    }

    /// <summary>
    ///     The map class which store the information used in elementIdComboBox comboBox in UI
    /// </summary>
    public class IdInfo
    {
        // Methods
        /// <summary>
        ///     The constructor of CreateInfo
        /// </summary>
        /// <param name="typeName">indicate model curve type</param>
        /// <param name="id">the element id</param>
        public IdInfo(string typeName, ElementId id)
        {
            Id = id; // Store the element id

            // Generate the display text
            DisplayText = typeName + " : " + id;
        }
        // Private members

        // Properties
        /// <summary>
        ///     The text displayed in the comboBox, as the DisplayMember
        /// </summary>
        public string DisplayText { get; }

        /// <summary>
        ///     The real value of the comboBox, as the ValueMember
        /// </summary>
        public ElementId Id { get; }
    }
}
