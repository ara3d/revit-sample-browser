// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System.Linq;

using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.Common.Mep
{
    public static class RoutingPreferenceHelper
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
            return new FilteredElementCollector(document).OfClass(typeof(PipeType)).Any();
        }

        public static void PipesDefinedWarning()
        {
            TaskDialog.Show("RoutingPreferenceTools",
                        "At least two PipeTypes are required to run this command.  Please define another PipeType.");
        }
    }
}