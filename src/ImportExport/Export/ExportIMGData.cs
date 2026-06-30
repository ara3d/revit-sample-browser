// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Text;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportImgData : ExportDataWithViews
    {
        private List<string> m_imageType;

        public ExportImgData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
        }

        private void Initialize()
        {
            //Image type
            m_imageType =
            [
                "(*.bmp)",
                "(*.jpeg)",
                "(*.png)",
                "(*.tga)",
                "(*.tif)"
            ];

            StringBuilder tmp = new();
            tmp.Append($"{m_imageType[0]}|*.bmp|");
            tmp.Append($"{m_imageType[1]}|*.jpeg|");
            tmp.Append($"{m_imageType[2]}|*.png|");
            tmp.Append($"{m_imageType[3]}|*.tga|");
            tmp.Append($"{m_imageType[4]}|*.tif|");

            Filter = tmp.ToString().TrimEnd('|');
            Title = "Export IMG";
        }

        public override bool Export()
        {
            base.Export();
            return true;
        }
    }
}
