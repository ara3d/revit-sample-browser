// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS.Properties;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Size = System.Drawing.Size;

namespace Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS
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
            CreateAttachedDetailGroupPanel(application);

            return Result.Succeeded;
        }

        private void CreateAttachedDetailGroupPanel(UIControlledApplication application)
        {
            var rp = application.CreateRibbonPanel("Attached Detail Group");

            PushButtonData pbdShowAllDetailGroups = new("ShowAttachedDetailGroups", "Show Attached\nDetail Groups",
                AddAssemblyPath,
                typeof(AttachedDetailGroupShowAllCommand).FullName)
            {
                LongDescription = "Show all of the selected element group's attached detail groups that are compatible with the current view."
            };

            var pbShowAllDetailGroups = rp.AddItem(pbdShowAllDetailGroups) as PushButton;
            SetIconsForPushButton(pbShowAllDetailGroups, Resources.ShowAllDetailGroupsIcon);

            PushButtonData pbdHideAllDetailGroups = new("HideAttachedDetailGroups", "Hide Attached\nDetail Groups",
                AddAssemblyPath,
                typeof(AttachedDetailGroupHideAllCommand).FullName)
            {
                LongDescription = "Hide all of the selected element group's attached detail groups that are compatible with the current view."
            };

            var pbHideAllDetailGroups = rp.AddItem(pbdHideAllDetailGroups) as PushButton;
            SetIconsForPushButton(pbHideAllDetailGroups, Resources.HideAllDetailGroupsIcon);
        }

        private static void SetIconsForPushButton(PushButton button, Icon icon)
        {
            button.LargeImage = GetStdIcon(icon);
            button.Image = GetSmallIcon(icon);
        }

        private static BitmapSource GetStdIcon(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private static BitmapSource GetSmallIcon(Icon icon)
        {
            Icon smallIcon = new(icon, new Size(16, 16));
            return Imaging.CreateBitmapSourceFromHIcon(
                smallIcon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
