using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser
{
    public class SampleBrowserApplication : IExternalApplication
    {
        public static UIControlledApplication Application { get; set; }

        public static List<IExternalApplication> OtherApps { get; }
            = [];

        public Result OnStartup(UIControlledApplication application)
        {
            Application = application;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            var results = OtherApps.Select(app => app.OnShutdown(application)).ToList();
            return results.All(x => x == Result.Succeeded)
                ? Result.Succeeded
                : results.Any(x => x == Result.Cancelled)
                ? Result.Cancelled
                : Result.Failed;
        }
    }
}