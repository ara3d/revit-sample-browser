// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Base data class which stores the basic information for import
    /// </summary>
    public class ImportData
    {
        /// <summary>
        ///     Active document
        /// </summary>
        protected Document m_activeDoc;

        /// <summary>
        ///     Revit command data
        /// </summary>
        protected ExternalCommandData m_commandData;

        /// <summary>
        ///     The filter which will be used in file saving dialog
        /// </summary>
        protected string m_filter;

        /// <summary>
        ///     File Name or Prefix to be used
        /// </summary>
        protected string m_importFileFullName;

        /// <summary>
        ///     Directory where to import the file
        /// </summary>
        protected string m_importFolder;

        /// <summary>
        ///     The format to be exported
        /// </summary>
        protected ImportFormat m_importFormat;

        /// <summary>
        ///     The title of importing dialog
        /// </summary>
        protected string m_title;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="importFormat">Format to import</param>
        public ImportData(ExternalCommandData commandData, ImportFormat importFormat)
        {
            m_commandData = commandData;
            m_activeDoc = commandData.Application.ActiveUIDocument.Document;
            m_importFormat = importFormat;
            m_filter = string.Empty;
            Initialize();
        }

        /// <summary>
        ///     Revit command data
        /// </summary>
        public ExternalCommandData CommandData => m_commandData;

        /// <summary>
        ///     File Name or Prefix to be used
        /// </summary>
        public string ImportFileFullName
        {
            get => m_importFileFullName;
            set => m_importFileFullName = value;
        }

        /// <summary>
        ///     The format to be imported
        /// </summary>
        public ImportFormat ImportFormat
        {
            get => m_importFormat;
            set => m_importFormat = value;
        }

        /// <summary>
        ///     The filter which will be used in file saving dialog
        /// </summary>
        public string Filter => m_filter;

        /// <summary>
        ///     Directory where to import the file
        /// </summary>
        public string ImportFolder
        {
            get => m_importFolder;
            set => m_importFolder = value;
        }

        /// <summary>
        ///     The title of importing dialog
        /// </summary>
        public string Title => m_title;

        /// <summary>
        ///     Initialize the variables
        /// </summary>
        private void Initialize()
        {
            //The directory into which the file will be imported
            var dllFilePath = Assembly.GetExecutingAssembly().Location;
            m_importFolder = Path.GetDirectoryName(dllFilePath);
            m_importFileFullName = string.Empty;
        }

        /// <summary>
        ///     Collect the parameters and import
        /// </summary>
        /// <returns></returns>
        public virtual bool Import()
        {
            if (m_importFileFullName == null) throw new NullReferenceException();

            return true;
        }
    }
}
