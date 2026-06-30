using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using InvalidOperationException = Autodesk.Revit.Exceptions.InvalidOperationException;
using IOException = System.IO.IOException;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ExportImage sample.</summary>
    internal static partial class Util
    {
        internal static void SetWhiteRenderBackground(View3D view)
        {
            var rs = view.GetRenderingSettings();
            rs.BackgroundStyle = BackgroundStyle.Color;

            var cbs
                = (ColorBackgroundSettings) rs
                    .GetBackgroundSettings();

            cbs.Color = new Color(255, 0, 0);
            rs.SetBackgroundSettings(cbs);
            view.SetRenderingSettings(rs);
        }

        internal static string ExportToImage(Document doc)
        {
            var tempFileName = Path.ChangeExtension(
                Path.GetRandomFileName(), "png");

            string tempImageFile;

            try
            {
                tempImageFile = Path.Combine(
                    Path.GetTempPath(), tempFileName);
            }
            catch (IOException)
            {
                return null;
            }

            IList<ElementId> views = new List<ElementId>();

            try
            {
                var collector = new FilteredElementCollector(
                    doc);

                var viewFamilyType = collector
                    .OfClass(typeof(ViewFamilyType))
                    .OfType<ViewFamilyType>()
                    .FirstOrDefault(x =>
                        x.ViewFamily == ViewFamily.ThreeDimensional);

                var view3D = viewFamilyType != null
                    ? View3D.CreateIsometric(doc, viewFamilyType.Id)
                    : null;

                if (view3D != null)
                {
                    var white = new Color(255, 255, 255);

                    view3D.SetBackground(
                        ViewDisplayBackground.CreateGradient(
                            white, white, white));

                    views.Add(view3D.Id);

                    var graphicDisplayOptions
                        = view3D.get_Parameter(
                            BuiltInParameter.MODEL_GRAPHICS_STYLE);

                    graphicDisplayOptions.Set(6);
                }
            }
            catch (InvalidOperationException)
            {
            }

            var ieo = new ImageExportOptions
            {
                FilePath = tempImageFile,
                FitDirection = FitDirectionType.Horizontal,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_150,
                ShouldCreateWebSite = false
            };

            if (views.Count > 0)
            {
                ieo.SetViewsAndSheets(views);
                ieo.ExportRange = ExportRange.SetOfViews;
            }
            else
            {
                ieo.ExportRange = ExportRange
                    .VisibleRegionOfCurrentView;
            }

            ieo.ZoomType = ZoomFitType.FitToPage;
            ieo.ViewName = "tmp";

            if (ImageExportOptions.IsValidFileName(
                    tempImageFile))
                try
                {
                    doc.ExportImage(ieo);
                }
                catch
                {
                    return string.Empty;
                }
            else
                return string.Empty;

            var files = Directory.GetFiles(
                Path.GetTempPath(),
                $"{Path.GetFileNameWithoutExtension(tempFileName)}*.*");

            return files.Length > 0
                ? files[0]
                : string.Empty;
        }

        internal static Result ExportToImage2(Document doc)
        {
            var r = Result.Failed;

            using var tx = new Transaction(doc);
            tx.Start("Export Image");
            var filepath = ExportToImage(doc);
            tx.RollBack();

            if (0 < filepath.Length)
            {
                Process.Start(filepath);
                r = Result.Succeeded;
            }

            return r;
        }

        internal static Result ExportToImage3(Document doc)
        {
            var r = Result.Failed;

            using var tx = new Transaction(doc);
            tx.Start("Export Image");

            var desktop_path = Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop);

            var view = doc.ActiveView;

            var filepath = Path.Combine(desktop_path,
                view.Name);

            var img = new ImageExportOptions();

            img.ZoomType = ZoomFitType.FitToPage;
            img.PixelSize = 32;
            img.ImageResolution = ImageResolution.DPI_600;
            img.FitDirection = FitDirectionType.Horizontal;
            img.ExportRange = ExportRange.CurrentView;
            img.HLRandWFViewsFileType = ImageFileType.PNG;
            img.FilePath = filepath;
            img.ShadowViewsFileType = ImageFileType.PNG;

            doc.ExportImage(img);

            tx.RollBack();

            filepath = Path.ChangeExtension(
                filepath, "png");

            Process.Start(filepath);

            r = Result.Succeeded;

            return r;
        }
    }
}
