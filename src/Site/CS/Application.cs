//
// (C) Copyright 2003-2019 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

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

using System;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Revit.SDK.Samples.Site.CS
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalApplication
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        #region IExternalApplication Members
        /// <summary>
        /// Implements the OnShutdown event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// Implements the OnStartup event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            CreateSitePanel(application);
            return Result.Succeeded;
        }

        #endregion

        private void CreateSitePanel(UIControlledApplication application)
        {
            var rp = application.CreateRibbonPanel("Site");
            var addPond = new PushButtonData("Site_Add_Pond", "Add Pond",
                                                            addAssemblyPath,
                                                            typeof(SiteAddRetainingPondCommand).FullName);
            SetIconsForPushButtonData(addPond, Properties.Resources.AddPond);
            _ = rp.AddItem(addPond) as PushButton;

            var moveRegion = new PushButtonData("Site_Move_Region", "Move Region",
                                                            addAssemblyPath,
                                                            typeof(SiteMoveRegionAndPointsCommand).FullName);
            SetIconsForPushButtonData(moveRegion, Properties.Resources.MoveRegion);

            var deleteRegion = new PushButtonData("Site_Delete_Region", "Delete Region",
                                                            addAssemblyPath,
                                                            typeof(SiteDeleteRegionAndPointsCommand).FullName);
            SetIconsForPushButtonData(deleteRegion, Properties.Resources.DeleteRegion);

            rp.AddStackedItems(moveRegion, deleteRegion);

            var raiseTerrain = new PushButtonData("Site_Raise_Terrain", "Raise Terrain",
                                                            addAssemblyPath,
                                                            typeof(SiteRaiseTerrainInRegionCommand).FullName);
            SetIconsForPushButtonData(raiseTerrain, Properties.Resources.RaiseTerrain);

            var lowerTerrain = new PushButtonData("Site_Lower_Terrain", "Lower Terrain",
                                                            addAssemblyPath,
                                                            typeof(SiteLowerTerrainInRegionCommand).FullName);
            SetIconsForPushButtonData(lowerTerrain, Properties.Resources.LowerTerrain);

            var normalizeTerrain = new PushButtonData("Site_Normalize_Terrain", "Normalize Terrain",
                                                            addAssemblyPath,
                                                            typeof(SiteNormalizeTerrainInRegionCommand).FullName);
            SetIconsForPushButtonData(normalizeTerrain, Properties.Resources.SiteNormalize);

            rp.AddStackedItems(raiseTerrain, lowerTerrain, normalizeTerrain);
        }

        /// <summary>
        /// Utility for adding icons to the button.
        /// </summary>
        /// <param name="button">The push button.</param>
        /// <param name="icon">The icon.</param>
        private static void SetIconsForPushButtonData(PushButtonData button, System.Drawing.Icon icon)
        {
            button.LargeImage = GetStdIcon(icon);
            button.Image = GetSmallIcon(icon);
        }

        /// <summary>
        /// Gets the standard sized icon as a BitmapSource.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <returns>The BitmapSource.</returns>
        private static BitmapSource GetStdIcon(System.Drawing.Icon icon)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Gets the small sized icon as a BitmapSource.
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <returns>The BitmapSource.</returns>
        private static BitmapSource GetSmallIcon(System.Drawing.Icon icon)
        {
            var smallIcon = new System.Drawing.Icon(icon, new System.Drawing.Size(16, 16));
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                smallIcon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// The path to this add-in assembly.
        /// </summary>
        static string addAssemblyPath = typeof(Application).Assembly.Location;
    }
}
