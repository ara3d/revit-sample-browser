// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    /// <summary>
    ///     Base data class which stores the basic information for export
    /// </summary>
    public class ExportData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportData(ExternalCommandData commandData, ExportFormat exportFormat)
        {
            ActiveDocument = commandData.Application.ActiveUIDocument.Document;
            ExportFormat = exportFormat;
            Filter = string.Empty;
            Initialize();
        }

        /// <summary>
        ///     ActiveDocument
        /// </summary>
        public Document ActiveDocument { get; }

        /// <summary>
        ///     File Name or Prefix to be used
        /// </summary>
        public string ExportFileName { get; set; }

        /// <summary>
        ///     Directory to store the exported file
        /// </summary>
        public string ExportFolder { get; set; }

        /// <summary>
        ///     ActiveDocument Name
        /// </summary>
        private string ActiveDocName { get; set; }

        /// <summary>
        ///     ActiveView Name
        /// </summary>
        public string ActiveViewName { get; private set; }

        /// <summary>
        ///     Whether current view is a 3D view
        /// </summary>
        public bool Is3DView { get; private set; }

        /// <summary>
        ///     The format to be exported
        /// </summary>
        public ExportFormat ExportFormat { get; }

        /// <summary>
        ///     The filter which will be used in file saving dialog
        /// </summary>
        public string Filter { get; protected set; }

        /// <summary>
        ///     The title of exporting dialog
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        ///     Initialize the variables
        /// </summary>
        private void Initialize()
        {
            //The directory into which the file will be exported
            var dllFilePath = Assembly.GetExecutingAssembly().Location;
            ExportFolder = Path.GetDirectoryName(dllFilePath);

            //The file name to be used by export
            ActiveDocName = ActiveDocument.Title;
            ActiveViewName = ActiveDocument.ActiveView.Name;
            var viewType = ActiveDocument.ActiveView.ViewType.ToString();
            ExportFileName = ActiveDocName + "-" + viewType + "-" + ActiveViewName + "." + GetExtension();

            //Whether current active view is 3D view
            if (ActiveDocument.ActiveView.ViewType == ViewType.ThreeD)
                Is3DView = true;
            else
                Is3DView = false;
        }

        /// <summary>
        ///     Get the extension of the file to export
        /// </summary>
        /// <returns></returns>
        private string GetExtension()
        {
            string extension = null;
            switch (ExportFormat)
            {
                case ExportFormat.Dwg:
                    extension = "dwg";
                    break;
                case ExportFormat.Dxf:
                    extension = "dxf";
                    break;
                case ExportFormat.Sat:
                    extension = "sat";
                    break;
                case ExportFormat.Dwf:
                    extension = "dwf";
                    break;
                case ExportFormat.DwFx:
                    extension = "dwfx";
                    break;
                case ExportFormat.Gbxml:
                    extension = "xml";
                    break;
                case ExportFormat.Fbx:
                    extension = "fbx";
                    break;
                case ExportFormat.Dgn:
                    extension = "dgn";
                    break;
                case ExportFormat.Image:
                    extension = "";
                    break;
                case ExportFormat.Pdf:
                    extension = "pdf";
                    break;
            }

            return extension;
        }

        /// <summary>
        ///     Collect the parameters and export
        /// </summary>
        /// <returns></returns>
        public virtual bool Export()
        {
            if (ExportFolder == null || ExportFileName == null) throw new NullReferenceException();

            return true;
        }
    }
}
