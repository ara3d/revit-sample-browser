// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Base data class which stores the basic information for export
    /// </summary>
    public class ExportData
    {
        /// <summary>
        ///     Active document
        /// </summary>
        protected readonly Document m_activeDoc;

        /// <summary>
        ///     ActiveDocument Name
        /// </summary>
        protected string m_activeDocName;

        /// <summary>
        ///     ActiveView Name
        /// </summary>
        protected string m_activeViewName;

        /// <summary>
        ///     Revit command data
        /// </summary>
        protected readonly ExternalCommandData m_commandData;

        /// <summary>
        ///     File Name or Prefix to be used
        /// </summary>
        protected string m_exportFileName;

        /// <summary>
        ///     Directory to store the exported file
        /// </summary>
        protected string m_exportFolder;

        /// <summary>
        ///     The format to be exported
        /// </summary>
        protected ExportFormat m_exportFormat;

        /// <summary>
        ///     The filter which will be used in file saving dialog
        /// </summary>
        protected string m_filter;

        /// <summary>
        ///     Whether current view is a 3D view
        /// </summary>
        protected bool m_is3DView;

        /// <summary>
        ///     The title of exporting dialog
        /// </summary>
        protected string m_title;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportData(ExternalCommandData commandData, ExportFormat exportFormat)
        {
            m_commandData = commandData;
            m_activeDoc = commandData.Application.ActiveUIDocument.Document;
            m_exportFormat = exportFormat;
            m_filter = string.Empty;
            Initialize();
        }

        /// <summary>
        ///     Revit command data
        /// </summary>
        public ExternalCommandData CommandData => m_commandData;

        /// <summary>
        ///     ActiveDocument
        /// </summary>
        public Document ActiveDocument => m_activeDoc;

        /// <summary>
        ///     File Name or Prefix to be used
        /// </summary>
        public string ExportFileName
        {
            get => m_exportFileName;
            set => m_exportFileName = value;
        }

        /// <summary>
        ///     Directory to store the exported file
        /// </summary>
        public string ExportFolder
        {
            get => m_exportFolder;
            set => m_exportFolder = value;
        }

        /// <summary>
        ///     ActiveDocument Name
        /// </summary>
        public string ActiveDocName
        {
            get => m_activeDocName;
            set => m_activeDocName = value;
        }

        /// <summary>
        ///     ActiveView Name
        /// </summary>
        public string ActiveViewName
        {
            get => m_activeViewName;
            set => m_activeViewName = value;
        }

        /// <summary>
        ///     Whether current view is a 3D view
        /// </summary>
        public bool Is3DView
        {
            get => m_is3DView;
            set => m_is3DView = value;
        }

        /// <summary>
        ///     The format to be exported
        /// </summary>
        public ExportFormat ExportFormat
        {
            get => m_exportFormat;
            set => m_exportFormat = value;
        }

        /// <summary>
        ///     The filter which will be used in file saving dialog
        /// </summary>
        public string Filter => m_filter;

        /// <summary>
        ///     The title of exporting dialog
        /// </summary>
        public string Title => m_title;

        /// <summary>
        ///     Initialize the variables
        /// </summary>
        private void Initialize()
        {
            //The directory into which the file will be exported
            var dllFilePath = Assembly.GetExecutingAssembly().Location;
            m_exportFolder = Path.GetDirectoryName(dllFilePath);

            //The file name to be used by export
            m_activeDocName = m_activeDoc.Title;
            m_activeViewName = m_activeDoc.ActiveView.Name;
            var viewType = m_activeDoc.ActiveView.ViewType.ToString();
            m_exportFileName = m_activeDocName + "-" + viewType + "-" + m_activeViewName + "." + getExtension();

            //Whether current active view is 3D view
            if (m_activeDoc.ActiveView.ViewType == ViewType.ThreeD)
                m_is3DView = true;
            else
                m_is3DView = false;
        }

        /// <summary>
        ///     Get the extension of the file to export
        /// </summary>
        /// <returns></returns>
        private string getExtension()
        {
            string extension = null;
            switch (m_exportFormat)
            {
                case ExportFormat.DWG:
                    extension = "dwg";
                    break;
                case ExportFormat.DXF:
                    extension = "dxf";
                    break;
                case ExportFormat.SAT:
                    extension = "sat";
                    break;
                case ExportFormat.DWF:
                    extension = "dwf";
                    break;
                case ExportFormat.DWFx:
                    extension = "dwfx";
                    break;
                case ExportFormat.GBXML:
                    extension = "xml";
                    break;
                case ExportFormat.FBX:
                    extension = "fbx";
                    break;
                case ExportFormat.DGN:
                    extension = "dgn";
                    break;
                case ExportFormat.Image:
                    extension = "";
                    break;
                case ExportFormat.PDF:
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
            if (m_exportFolder == null || m_exportFileName == null) throw new NullReferenceException();

            return true;
        }
    }
}
