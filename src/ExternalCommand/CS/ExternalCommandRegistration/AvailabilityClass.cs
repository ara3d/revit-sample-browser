// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ExternalCommand.CS.ExternalCommandRegistration
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommandAvailability,
    ///     determine when to enable\disable the corresponding external command by
    ///     return value from IsCommandAvailable function.
    ///     Corresponding command will be disabled when a wall selected by user in this case.
    /// </summary>
    internal class WallSelection : IExternalCommandAvailability
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

    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommandAvailability,
    ///     determine when to enable\disable the corresponding external command by
    ///     return value from IsCommandAvailable function.
    ///     Corresponding command will be disabled if active document is not a 3D view.
    /// </summary>
    internal class View3D : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData,
            CategorySet selectedCategories)
        {
            var activeView = applicationData.ActiveUIDocument.Document.ActiveView;
            return ViewType.ThreeD == activeView.ViewType;
        }
    }
}
