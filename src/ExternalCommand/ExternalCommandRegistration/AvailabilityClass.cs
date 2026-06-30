// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ExternalCommand.CS.ExternalCommandRegistration
{
    /// <summary>Disables the command when a wall is selected.</summary>
    public class WallSelection : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData,
            CategorySet selectedCategories)
        {
            var iterCategory = selectedCategories.GetEnumerator();
            iterCategory.Reset();
            while (iterCategory.MoveNext())
            {
                var category = (Category)iterCategory.Current;
                if (category.Name == "Walls") return false;
            }

            return true;
        }
    }

    /// <summary>Disables the command when the active view is not 3D.</summary>
    public class View3D : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData,
            CategorySet selectedCategories)
        {
            var activeView = applicationData.ActiveUIDocument.Document.ActiveView;
            return ViewType.ThreeD == activeView.ViewType;
        }
    }
}
