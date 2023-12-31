// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS.Properties;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Size = System.Drawing.Size;

namespace Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS
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
            CreateAttachedDetailGroupPanel(application);

            return Result.Succeeded;
        }

        /// <summary>
        ///     Sets up the add-in panel for this sample.
        /// </summary>
        private void CreateAttachedDetailGroupPanel(UIControlledApplication application)
        {
            // Create the ribbon panel.
            var rp = application.CreateRibbonPanel("Attached Detail Group");

            // Create the show all detail groups pushbutton.
            var pbdShowAllDetailGroups = new PushButtonData("ShowAttachedDetailGroups", "Show Attached\nDetail Groups",
                AddAssemblyPath,
                typeof(AttachedDetailGroupShowAllCommand).FullName)
            {
                LongDescription = "Show all of the selected element group's attached detail groups that are compatible with the current view."
            };

            var pbShowAllDetailGroups = rp.AddItem(pbdShowAllDetailGroups) as PushButton;
            SetIconsForPushButton(pbShowAllDetailGroups, Resources.ShowAllDetailGroupsIcon);

            // Create the hide all detail groups pushbutton.
            var pbdHideAllDetailGroups = new PushButtonData("HideAttachedDetailGroups", "Hide Attached\nDetail Groups",
                AddAssemblyPath,
                typeof(AttachedDetailGroupHideAllCommand).FullName)
            {
                LongDescription = "Hide all of the selected element group's attached detail groups that are compatible with the current view."
            };

            var pbHideAllDetailGroups = rp.AddItem(pbdHideAllDetailGroups) as PushButton;
            SetIconsForPushButton(pbHideAllDetailGroups, Resources.HideAllDetailGroupsIcon);
        }

        /// <summary>
        ///     Utility for adding icons to the button.
        /// </summary>
        /// <param name="button">The push button.</param>
        /// <param name="icon">The icon.</param>
        private static void SetIconsForPushButton(PushButton button, Icon icon)
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
