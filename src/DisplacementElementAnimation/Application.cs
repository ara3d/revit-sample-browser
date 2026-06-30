// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using Ara3D.RevitSampleBrowser.DisplacementElementAnimation.CS.Properties;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Size = System.Drawing.Size;

namespace Ara3D.RevitSampleBrowser.DisplacementElementAnimation.CS
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
            CreateDisplacementPanel(application);

            return Result.Succeeded;
        }

        private void CreateDisplacementPanel(UIControlledApplication application)
        {
            var rp = application.CreateRibbonPanel("Displacement");
            PushButtonData setupMonitor = new("Displacement_Animiation", "Displacement Animation",
                AddAssemblyPath,
                typeof(DisplacementStructureModelAnimatorCommand).FullName);
            var setupMonitorPb = rp.AddItem(setupMonitor) as PushButton;

            SetIconsForPushButton(setupMonitorPb, Resources.DisplacementPlay);
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
