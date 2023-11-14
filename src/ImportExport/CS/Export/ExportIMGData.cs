// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    internal class ExportIMGData : ExportDataWithViews
    {
        /// <summary>
        ///     String list of image type
        /// </summary>
        private List<string> m_imageType;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportIMGData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
        }

        /// <summary>
        ///     Initialize the variables
        /// </summary>
        private void Initialize()
        {
            //Image type
            m_imageType = new List<string>
            {
                "(*.bmp)",
                "(*.jpeg)",
                "(*.png)",
                "(*.tga)",
                "(*.tif)"
            };

            var tmp = new StringBuilder();
            tmp.Append(m_imageType[0] + "|*.bmp|");
            tmp.Append(m_imageType[1] + "|*.jpeg|");
            tmp.Append(m_imageType[2] + "|*.png|");
            tmp.Append(m_imageType[3] + "|*.tga|");
            tmp.Append(m_imageType[4] + "|*.tif|");

            m_filter = tmp.ToString().TrimEnd('|');
            m_title = "Export IMG";
        }


        /// <summary>
        ///     Collect the parameters and export
        /// </summary>
        /// <returns></returns>
        public override bool Export()
        {
            base.Export();
            return true;
        }
    }
}
