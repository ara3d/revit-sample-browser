// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

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

using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RevitMultiSample.Site.CS.Properties;
using Size = System.Drawing.Size;

namespace RevitMultiSample.Site.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        /// <summary>
        ///     The path to this add-in assembly.
        /// </summary>
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
            SetIconsForPushButtonData(addPond, Resources.AddPond);
            _ = rp.AddItem(addPond) as PushButton;

            var moveRegion = new PushButtonData("Site_Move_Region", "Move Region",
                AddAssemblyPath,
                typeof(SiteMoveRegionAndPointsCommand).FullName);
            SetIconsForPushButtonData(moveRegion, Resources.MoveRegion);

            var deleteRegion = new PushButtonData("Site_Delete_Region", "Delete Region",
                AddAssemblyPath,
                typeof(SiteDeleteRegionAndPointsCommand).FullName);
            SetIconsForPushButtonData(deleteRegion, Resources.DeleteRegion);

            rp.AddStackedItems(moveRegion, deleteRegion);

            var raiseTerrain = new PushButtonData("Site_Raise_Terrain", "Raise Terrain",
                AddAssemblyPath,
                typeof(SiteRaiseTerrainInRegionCommand).FullName);
            SetIconsForPushButtonData(raiseTerrain, Resources.RaiseTerrain);

            var lowerTerrain = new PushButtonData("Site_Lower_Terrain", "Lower Terrain",
                AddAssemblyPath,
                typeof(SiteLowerTerrainInRegionCommand).FullName);
            SetIconsForPushButtonData(lowerTerrain, Resources.LowerTerrain);

            var normalizeTerrain = new PushButtonData("Site_Normalize_Terrain", "Normalize Terrain",
                AddAssemblyPath,
                typeof(SiteNormalizeTerrainInRegionCommand).FullName);
            SetIconsForPushButtonData(normalizeTerrain, Resources.SiteNormalize);

            rp.AddStackedItems(raiseTerrain, lowerTerrain, normalizeTerrain);
        }

        /// <summary>
        ///     Utility for adding icons to the button.
        /// </summary>
        /// <param name="button">The push button.</param>
        /// <param name="icon">The icon.</param>
        private static void SetIconsForPushButtonData(PushButtonData button, Icon icon)
        {
            button.LargeImage = GetStdIcon(icon);
            button.Image = GetSmallIcon(icon);
        }

        /// <summary>
        ///     Gets the standard sized icon as a BitmapSource.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <returns>The BitmapSource.</returns>
        private static BitmapSource GetStdIcon(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        ///     Gets the small sized icon as a BitmapSource.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <returns>The BitmapSource.</returns>
        private static BitmapSource GetSmallIcon(Icon icon)
        {
            var smallIcon = new Icon(icon, new Size(16, 16));
            return Imaging.CreateBitmapSourceFromHIcon(
                smallIcon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
