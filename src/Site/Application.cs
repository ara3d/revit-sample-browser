// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using Ara3D.RevitSampleBrowser.Site.CS.Properties;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Views;
namespace Ara3D.RevitSampleBrowser.Site.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        private static readonly string AddAssemblyPath = typeof(Application).Assembly.Location;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            CreateSitePanel(application);
            return Result.Succeeded;
        }

        private void CreateSitePanel(UIControlledApplication application)
        {
            var rp = application.CreateRibbonPanel("Site");
            var addPond = new PushButtonData("Site_Add_Pond", "Add Pond",
                AddAssemblyPath,
                typeof(SiteAddRetainingPondCommand).FullName);
            ViewHelper.SetIconsForPushButtonData(addPond, Resources.AddPond);
            _ = rp.AddItem(addPond) as PushButton;

            var moveRegion = new PushButtonData("Site_Move_Region", "Move Region",
                AddAssemblyPath,
                typeof(SiteMoveRegionAndPointsCommand).FullName);
            ViewHelper.SetIconsForPushButtonData(moveRegion, Resources.MoveRegion);

            var deleteRegion = new PushButtonData("Site_Delete_Region", "Delete Region",
                AddAssemblyPath,
                typeof(SiteDeleteRegionAndPointsCommand).FullName);
            ViewHelper.SetIconsForPushButtonData(deleteRegion, Resources.DeleteRegion);

            rp.AddStackedItems(moveRegion, deleteRegion);

            var raiseTerrain = new PushButtonData("Site_Raise_Terrain", "Raise Terrain",
                AddAssemblyPath,
                typeof(SiteRaiseTerrainInRegionCommand).FullName);
            ViewHelper.SetIconsForPushButtonData(raiseTerrain, Resources.RaiseTerrain);

            var lowerTerrain = new PushButtonData("Site_Lower_Terrain", "Lower Terrain",
                AddAssemblyPath,
                typeof(SiteLowerTerrainInRegionCommand).FullName);
            ViewHelper.SetIconsForPushButtonData(lowerTerrain, Resources.LowerTerrain);

            var normalizeTerrain = new PushButtonData("Site_Normalize_Terrain", "Normalize Terrain",
                AddAssemblyPath,
                typeof(SiteNormalizeTerrainInRegionCommand).FullName);
            ViewHelper.SetIconsForPushButtonData(normalizeTerrain, Resources.SiteNormalize);

            rp.AddStackedItems(raiseTerrain, lowerTerrain, normalizeTerrain);
        }

    }
}
