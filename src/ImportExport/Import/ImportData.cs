// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;

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
            ImportFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public ExternalCommandData CommandData { get; }

        public string ImportFileFullName { get; set; }

        public string Filter { get; protected set; }

        public string ImportFolder { get; }

        public string Title { get; protected set; }

        public virtual bool Import()
            => ImportFileFullName == null ? throw new NullReferenceException() : true;
    }
}
