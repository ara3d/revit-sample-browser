using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser
{
    public class MultiSampleApplication : IExternalApplication
    {
        public static UIControlledApplication Application { get; set; }
        
        public static List<IExternalApplication> OtherApps { get; } 
            = new List<IExternalApplication>();

        public Result OnStartup(UIControlledApplication application)
        {
            Application = application;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            var results = OtherApps.Select(app => app.OnShutdown(application)).ToList();
            if (results.All(x => x == Result.Succeeded))
                return Result.Succeeded;
            return results.Any(x => x == Result.Cancelled) 
                ? Result.Cancelled 
                : Result.Failed;
        }
    }
}