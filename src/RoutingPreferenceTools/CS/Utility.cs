// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RoutingPreferenceTools.CS
{
    internal class Validation
    {
        public static bool ValidateMep(Application application)
        {
            return application.IsPipingEnabled;
        }

        public static void MepWarning()
        {
            TaskDialog.Show("RoutingPreferenceTools", "Revit MEP is required to run this addin.");
        }

        public static bool ValidatePipesDefined(Document document)
        {
            var collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(PipeType));
            return collector.Any();
        }

        public static void PipesDefinedWarning()
        {
            TaskDialog.Show("RoutingPreferenceTools",
                "At least two PipeTypes are required to run this command.  Please define another PipeType.");
        }
    }

    internal class Convert
    {
        public static double ConvertValueDocumentUnits(double decimalFeet, Document document)
        {
            var formatOption = document.GetUnits().GetFormatOptions(SpecTypeId.PipeSize);
            return UnitUtils.ConvertFromInternalUnits(decimalFeet, formatOption.GetUnitTypeId());
        }

        public static double ConvertValueToFeet(double unitValue, Document document)
        {
            var tempVal = ConvertValueDocumentUnits(unitValue, document);
            var ratio = unitValue / tempVal;
            return unitValue * ratio;
        }
    }
}
