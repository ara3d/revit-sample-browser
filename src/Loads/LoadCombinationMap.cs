// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Text;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.Loads.CS
{
    /// <summary>
    ///     A class to store Load Combination and it's properties.
    /// </summary>
    public class LoadCombinationMap
    {
        public LoadCombinationMap(LoadCombination combination)
        {
            Name = combination.Name;
            Type = combination.Type.ToString();
            State = combination.State.ToString();
            var document = combination.Document;

            // Generate the formula field.
            var formulaString = new StringBuilder();
            var components = combination.GetComponents();
            foreach (var component in components)
            {
                formulaString.Append(component.Factor);
                formulaString.Append("*");
                formulaString.Append(document.GetElement(component.LoadCaseOrCombinationId).Name);

                if (components.IndexOf(component) < components.Count - 1) formulaString.Append(" + ");
            }

            Formula = formulaString.ToString();

            // Generate the usage field.
            var usageString = new StringBuilder();
            var usageIds = combination.GetUsageIds();
            foreach (var id in usageIds)
            {
                document.GetElement(id);
                usageString.Append(document.GetElement(id).Name);

                if (usageIds.IndexOf(id) < usageIds.Count - 1) usageString.Append(";");
            }

            Usage = usageString.ToString();
        }
        // Private Members

        public string Name { get; }

        public string Formula { get; }

        public string Type { get; }

        public string State { get; }

        public string Usage { get; set; }
    }
}
