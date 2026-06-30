// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.Common.Views
{
    public static class CustomExportHelper
    {
        public static void AddTo(IList<XYZ> to, IList<XYZ> from)
        {
            for (var ii = 0; ii < from.Count - 1; ii++)
            {
                to.Add(from[ii]);
                to.Add(from[ii + 1]);
            }
        }

        public static void DisplayExport(View view, IList<XYZ> points)
        {
            var doc = view.Document;
            using var tran = new Transaction(doc, "ExportViewGeometry");
            tran.Start("Draw Exported Lines and turn off everything but Lines");
            ViewHelper.HideAllInView(view);
            XyzMath.DrawLines(view, points, doc.Application.ShortCurveTolerance);
            tran.Commit();
        }

    }
}