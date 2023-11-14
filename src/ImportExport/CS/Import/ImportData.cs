// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ImportExport.CS
{
    /// <summary>
    ///     Base data class which stores the basic information for import
    /// </summary>
    public class ImportData
    {
        /// <summary>
        ///     Active document
        /// </summary>
        protected readonly Document ActiveDoc;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="importFormat">Format to import</param>
        public ImportData(ExternalCommandData commandData, ImportFormat importFormat)
        {
            CommandData = commandData;
            ActiveDoc = commandData.Application.ActiveUIDocument.Document;
            Filter = string.Empty;
            Initialize();
        }

        /// <summary>
        ///     Revit command data
        /// </summary>
        public ExternalCommandData CommandData { get; }

        /// <summary>
        ///     File Name or Prefix to be used
        /// </summary>
        public string ImportFileFullName { get; set; }

        /// <summary>
        ///     The filter which will be used in file saving dialog
        /// </summary>
        public string Filter { get; protected set; }

        /// <summary>
        ///     Directory where to import the file
        /// </summary>
        public string ImportFolder { get; private set; }

        /// <summary>
        ///     The title of importing dialog
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        ///     Initialize the variables
        /// </summary>
        private void Initialize()
        {
            //The directory into which the file will be imported
            var dllFilePath = Assembly.GetExecutingAssembly().Location;
            ImportFolder = Path.GetDirectoryName(dllFilePath);
            ImportFileFullName = string.Empty;
        }

        /// <summary>
        ///     Collect the parameters and import
        /// </summary>
        /// <returns></returns>
        public virtual bool Import()
        {
            if (ImportFileFullName == null) throw new NullReferenceException();

            return true;
        }
    }
}
