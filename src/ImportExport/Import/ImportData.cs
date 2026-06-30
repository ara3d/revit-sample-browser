// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Import
{
    public class ImportData
    {
        protected readonly Document ActiveDoc;

        public ImportData(ExternalCommandData commandData, ImportFormat importFormat)
        {
            CommandData = commandData;
            ActiveDoc = commandData.Application.ActiveUIDocument.Document;
            Filter = string.Empty;
            Initialize();
        }

        public ExternalCommandData CommandData { get; }

        public string ImportFileFullName { get; set; }

        public string Filter { get; protected set; }

        public string ImportFolder { get; private set; }

        public string Title { get; protected set; }

        private void Initialize()
        {
            //The directory into which the file will be imported
            var dllFilePath = Assembly.GetExecutingAssembly().Location;
            ImportFolder = Path.GetDirectoryName(dllFilePath);
            ImportFileFullName = string.Empty;
        }

        public virtual bool Import()
        {
            if (ImportFileFullName == null) throw new NullReferenceException();

            return true;
        }
    }
}
