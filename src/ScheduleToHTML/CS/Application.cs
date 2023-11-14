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
using Revit.SDK.Samples.ScheduleToHTML.CS.Properties;
using Size = System.Drawing.Size;

namespace Revit.SDK.Samples.ScheduleToHTML.CS
{
    /// <summary>
    ///     Implementation of the IExternalApplication.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        /// <summary>
        ///     The path to this add-in assembly.
        /// </summary>
        private static readonly string addAssemblyPath = typeof(Application).Assembly.Location;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            CreateScheduleToHTMLPanel(application);

            return Result.Succeeded;
        }


        /// <summary>
        ///     Sets up the add-in panel for this sample.
        /// </summary>
        private void CreateScheduleToHTMLPanel(UIControlledApplication application)
        {
            var rp = application.CreateRibbonPanel("Schedule To HTML");

            var pbd = new PushButtonData("ScheduleToHTML", "Export to HTML",
                addAssemblyPath,
                typeof(ScheduleHTMLExportCommand).FullName)
            {
                LongDescription = "Export the active schedule to HTML."
            };

            var duplicateAllPB = rp.AddItem(pbd) as PushButton;
            SetIconsForPushButton(duplicateAllPB, Resources.ScheduleExport);
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
