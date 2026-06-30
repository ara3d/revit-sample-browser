// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Consolidated export helpers

        internal static void SetWhiteRenderBackground(View3D view)
        {
            var rs = view.GetRenderingSettings();
            rs.BackgroundStyle = BackgroundStyle.Color;

            var cbs
                = (ColorBackgroundSettings)rs
                    .GetBackgroundSettings();

            cbs.Color = new Autodesk.Revit.DB.Color(255, 0, 0);
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

            IList<ElementId> views = [];

            try
            {
                FilteredElementCollector collector = new(
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
                    Color white = new(255, 255, 255);

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
            catch (Autodesk.Revit.Exceptions.InvalidOperationException)
            {
            }

            ImageExportOptions ieo = new()
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

            using Transaction tx = new(doc);
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

            using Transaction tx = new(doc);
            tx.Start("Export Image");

            var desktop_path = Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop);

            var view = doc.ActiveView;

            var filepath = Path.Combine(desktop_path,
                view.Name);

            ImageExportOptions img = new()
            {
                ZoomType = ZoomFitType.FitToPage,
                PixelSize = 32,
                ImageResolution = ImageResolution.DPI_600,
                FitDirection = FitDirectionType.Horizontal,
                ExportRange = ExportRange.CurrentView,
                HLRandWFViewsFileType = ImageFileType.PNG,
                FilePath = filepath,
                ShadowViewsFileType = ImageFileType.PNG
            };

            doc.ExportImage(img);

            tx.RollBack();

            filepath = Path.ChangeExtension(
                filepath, "png");

            Process.Start(filepath);

            r = Result.Succeeded;

            return r;
        }

        internal static Result ExportToIfc(Document doc)
        {
            var r = Result.Failed;

            using Transaction tx = new(doc);
            tx.Start("Export IFC");

            var desktop_path = Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop);

            IFCExportOptions opt = null;

            doc.Export(desktop_path, doc.Title, opt);

            tx.RollBack();

            r = Result.Succeeded;

            return r;
        }

        #endregion
    }
}
