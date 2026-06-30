// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportData
    {
        public ExportData(ExternalCommandData commandData, ExportFormat exportFormat)
        {
            ActiveDocument = commandData.Application.ActiveUIDocument.Document;
            ExportFormat = exportFormat;
            Filter = string.Empty;
            Initialize();
        }

        public Document ActiveDocument { get; }

        public string ExportFileName { get; set; }

        public string ExportFolder { get; set; }

        private string ActiveDocName { get; set; }

        public string ActiveViewName { get; private set; }

        public bool Is3DView { get; private set; }

        public ExportFormat ExportFormat { get; }

        public string Filter { get; protected set; }

        public string Title { get; protected set; }

        private void Initialize()
        {
            var dllFilePath = Assembly.GetExecutingAssembly().Location;
            ExportFolder = Path.GetDirectoryName(dllFilePath);
            ActiveDocName = ActiveDocument.Title;
            ActiveViewName = ActiveDocument.ActiveView.Name;
            var viewType = ActiveDocument.ActiveView.ViewType.ToString();
            ExportFileName = $"{ActiveDocName}-{viewType}-{ActiveViewName}.{GetExtension()}";
            if (ActiveDocument.ActiveView.ViewType == ViewType.ThreeD)
                Is3DView = true;
            else
                Is3DView = false;
        }

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

        public virtual bool Export()
        {
            if (ExportFolder == null || ExportFileName == null) throw new NullReferenceException();

            return true;
        }
    }
}
