// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.UIAPI.CS
{
    internal class ApplicationAvailabilityClass : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData,
            CategorySet selectedCategories)
        {
            var revitApplication = applicationData.Application;
            var options = ApplicationOptions.Get();

            switch (options.Availability)
            {
                case ApplicationAvailablity.ArchitectureDiscipline:
                    return revitApplication.IsArchitectureEnabled;
                case ApplicationAvailablity.StructuralAnalysis:
                    return revitApplication.IsStructuralAnalysisEnabled;
                case ApplicationAvailablity.Mep:
                    return revitApplication.IsSystemsEnabled;
            }

            return true;
        }
    }
}
