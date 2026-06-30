// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.UIAPI.CS
{
    public class ApplicationAvailabilityClass : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData,
            CategorySet selectedCategories)
        {
            var revitApplication = applicationData.Application;
            var options = ApplicationOptions.Get();

            return options.Availability switch
            {
                ApplicationAvailablity.ArchitectureDiscipline => revitApplication.IsArchitectureEnabled,
                ApplicationAvailablity.StructuralAnalysis => revitApplication.IsStructuralAnalysisEnabled,
                ApplicationAvailablity.Mep => revitApplication.IsSystemsEnabled,
                _ => true,
            };
        }
    }
}
