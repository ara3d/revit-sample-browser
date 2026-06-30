// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from GetElementImage by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/GetElementImage

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using BuildingCoder;

namespace Ara3D.RevitSampleBrowser.GetElementImage.CS
{
    class ImageExporter
    {
        // View data: name, distance factor, yaw (0=N), pitch (90=top).
        readonly object[][] _viewDataToExport =
        {
            new object[] { "Isometric", 1, 45, 35 },
            new object[] { "North", 1, 0, 0 },
            new object[] { "East", 1, 90, 0 },
            new object[] { "Top", 1, 0, 90, 0 }
        };

        readonly List<View> _viewsToExport;

        readonly BuiltInCategory[] _categoriesToHide =
        {
            BuiltInCategory.OST_Cameras,
            BuiltInCategory.OST_IOS_GeoSite,
            BuiltInCategory.OST_Levels,
            BuiltInCategory.OST_ProjectBasePoint
        };

        static ViewOrientation3D GetOrientationFor(
            double yawDegrees,
            double pitchDegrees)
        {
            if (Util.IsEqual(270, yawDegrees) || 270 > yawDegrees)
                yawDegrees += 90.0;

            var angleInXyPlane = yawDegrees * Math.PI / 180.0;
            var angleUpDown = pitchDegrees * Math.PI / 180.0;

            var eye = new XYZ(
                Math.Cos(angleInXyPlane),
                Math.Sin(angleInXyPlane),
                Math.Cos(angleUpDown));

            var forward = -eye;

            var left = Util.IsVertical(forward)
                ? -XYZ.BasisX
                : XYZ.BasisZ.CrossProduct(forward);

            var up = forward.CrossProduct(left);

            return new ViewOrientation3D(eye, up, forward);
        }

        public ImageExporter(Document doc)
        {
            var viewFamilyType = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

            Debug.Assert(viewFamilyType != null);

            var v = View3D.CreateIsometric(doc, viewFamilyType.Id);
            v.Name = "Isometric";

            var nViews = _viewDataToExport.Length;
            _viewsToExport = new List<View>(nViews) { v };

            for (var i = 1; i < nViews; ++i)
            {
                v = View3D.CreateIsometric(doc, viewFamilyType.Id);

                var d = _viewDataToExport[i];
                v.Name = d[0] as string;
                v.SetOrientation(GetOrientationFor((int)d[2], (int)d[3]));
                v.SaveOrientation();

                _viewsToExport.Add(v);
            }

            foreach (var v2 in _viewsToExport)
            {
                var graphicDisplayOptions = v2.get_Parameter(
                    BuiltInParameter.MODEL_GRAPHICS_STYLE);

                graphicDisplayOptions.Set(6);

                var cats = doc.Settings.Categories;

                foreach (var bic in _categoriesToHide)
                {
                    var cat = cats.get_Item(bic);

                    if (cat == null)
                        Debug.Print("{0} returns null category.", bic);
                    else
                        v2.SetCategoryHidden(cat.Id, true);
                }
            }
        }

        public string[] ExportToImage(Element e)
        {
            var doc = e.Document;

            foreach (var v in _viewsToExport)
            {
                var hideableElementIds = new FilteredElementCollector(doc, v.Id)
                    .Where(a => a.CanBeHidden(v))
                    .Select(b => b.Id)
                    .ToList();

                v.HideElements(hideableElementIds);
                v.UnhideElements(new List<ElementId> { e.Id });
            }

            doc.Regenerate();

            var dir = Path.Combine(Path.GetTempPath(), "GetElementImage");
            Directory.CreateDirectory(dir);

            var fn = e.Id.Value.ToString();
            var filepath = Path.Combine(dir, $"{fn}.png");

            var ieo = new ImageExportOptions
            {
                FilePath = filepath,
                FitDirection = FitDirectionType.Horizontal,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_150,
                ShouldCreateWebSite = false
            };

            var n = _viewsToExport.Count;

            if (n > 0)
            {
                var ids = _viewsToExport.Select(v => v.Id).ToList();
                ieo.SetViewsAndSheets(ids);
                ieo.ExportRange = ExportRange.SetOfViews;
            }
            else
            {
                ieo.ExportRange = ExportRange.VisibleRegionOfCurrentView;
            }

            ieo.ZoomType = ZoomFitType.FitToPage;
            ieo.ViewName = "tmp";

            try
            {
                doc.ExportImage(ieo);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return Directory.GetFiles(dir, $"{fn}*.*");
        }
    }
}
