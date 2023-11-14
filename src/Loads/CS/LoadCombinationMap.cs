// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Text;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.Loads.CS
{
    /// <summary>
    ///     A class to store Load Combination and it's properties.
    /// </summary>
    public class LoadCombinationMap
    {
        /// <summary>
        ///     Default constructor of LoadCombinationMap
        /// </summary>
        /// <param name="combination">the reference of LoadCombination</param>
        public LoadCombinationMap(LoadCombination combination)
        {
            Name = combination.Name;
            Type = combination.Type.ToString();
            State = combination.State.ToString();
            var m_document = combination.Document;

            // Generate the formula field.
            var formulaString = new StringBuilder();
            var components = combination.GetComponents();
            foreach (var component in components)
            {
                formulaString.Append(component.Factor);
                formulaString.Append("*");
                formulaString.Append(m_document.GetElement(component.LoadCaseOrCombinationId).Name);

                if (components.IndexOf(component) < components.Count - 1) formulaString.Append(" + ");
            }

            Formula = formulaString.ToString();

            // Generate the usage field.
            var usageString = new StringBuilder();
            var usageIds = combination.GetUsageIds();
            foreach (var id in usageIds)
            {
                m_document.GetElement(id);
                usageString.Append(m_document.GetElement(id).Name);

                if (usageIds.IndexOf(id) < usageIds.Count - 1) usageString.Append(";");
            }

            Usage = usageString.ToString();
        }
        // Private Members

        /// <summary>
        ///     Name property of LoadCombinationMap
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Formula property of LoadCombinationMap
        /// </summary>
        public string Formula { get; }

        /// <summary>
        ///     Type property of LoadCombinationMap
        /// </summary>
        public string Type { get; }

        /// <summary>
        ///     State property of LoadCombinationMap
        /// </summary>
        public string State { get; }

        /// <summary>
        ///     Usage property of LoadCombinationMap
        /// </summary>
        public string Usage { get; set; }
    }
}
