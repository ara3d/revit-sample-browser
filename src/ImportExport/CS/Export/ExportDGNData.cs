//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using System.IO;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    /// Data class which stores the main information for exporting dgn format
    /// </summary>
    public class ExportDGNData : ExportDataWithViews
    {
                /// <summary>
        /// String list of Layer Settings used in UI
        /// </summary>
        List<string> m_layerMapping;
        /// <summary>
        /// String list of layer settings values defined in Revit 
        /// </summary>
        List<string> m_enumLayerMapping;
        /// <summary>
        /// String list of exported format version defined in Revit 
        /// </summary>
        List<string> m_exportFileVersions;
      
        /// <summary>
        /// Layer setting option to export
        /// </summary>
        private string m_exportLayerMapping;
        /// <summary>
        /// Exported format version
        /// </summary>
        private string m_exportFileVersion;
        /// <summary>
        /// Whether to hide scope box
        /// </summary>
        private bool m_hideScopeBox;
        /// <summary>
        /// Whether to hide unreference view tags
        /// </summary>
        private bool m_hideUnreferenceViewTags;
        /// <summary>
        /// Whether to hide reference plane
        /// </summary>
        private bool m_hideReferencePlane;
        
                /// <summary>
        /// Layer setting option to export
        /// </summary>
        public string ExportLayerMapping
        {
            get => m_exportLayerMapping;
            set => m_exportLayerMapping = value;
        }

        /// <summary>
        /// Whether to hide scope box
        /// </summary>
        public bool HideScopeBox
        {
            get => m_hideScopeBox;
            set => m_hideScopeBox = value;
        }

        /// <summary>
        /// Whether to hide unreference view tags
        /// </summary>
        public bool HideUnreferenceViewTags
        {
            get => m_hideUnreferenceViewTags;
            set => m_hideUnreferenceViewTags = value;
        }

        /// <summary>
        /// Whether to hide reference plane
        /// </summary>
        public bool HideReferencePlane
        {
            get => m_hideReferencePlane;
            set => m_hideReferencePlane = value;
        }

        /// <summary>
        /// Exported format version
        /// </summary>
        public string ExportFileVersion
        {
            get => m_exportFileVersion;
            set => m_exportFileVersion = value;
        }

        /// <summary>
        /// String collection of Layer Settings used in UI
        /// </summary>
        public ReadOnlyCollection<string> LayerMapping => new ReadOnlyCollection<string>(m_layerMapping);

        /// <summary>
        /// String list of exported format version defined in Revit 
        /// </summary>
        public List<string> ExportFileVersions => m_exportFileVersions;

        /// <summary>
        /// String collection of layer settings values defined in Revit  
        /// </summary>
        public ReadOnlyCollection<string> EnumLayerMapping => new ReadOnlyCollection<string>(m_enumLayerMapping);

        
                /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportDGNData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the variables
        /// </summary>
        private void Initialize()
        {
            //Layer Settings:
            m_layerMapping = new List<string>();
            m_enumLayerMapping = new List<string>();
            m_layerMapping.Add("AIA - American Institute of Architects standard");
            m_enumLayerMapping.Add("AIA");
            m_layerMapping.Add("ISO13567 - ISO standard 13567");
            m_enumLayerMapping.Add("ISO13567");
            m_layerMapping.Add("CP83 - Singapore standard 83");
            m_enumLayerMapping.Add("CP83");
            m_layerMapping.Add("BS1192 - British standard 1192");
            m_enumLayerMapping.Add("BS1192");

            //Export format:
            m_exportFileVersions = new List<string>();
            m_exportFileVersions.Add("MicroStation V8 Format");
            m_exportFileVersions.Add("MicroStation V7 Format");

            m_filter = "Microstation DGN Files |*.dgn";
            m_title = "Export DGN";
        }

        /// <summary>
        /// Collect the parameters and export
        /// </summary>
        /// <returns></returns>
        public override bool Export()
        {
            base.Export();

            ICollection<ElementId> views = new List<ElementId>();
            if (m_currentViewOnly)
            {
                views.Add(m_activeDoc.ActiveView.Id);
            }
            else
            {
                foreach (View view in m_selectViewsData.SelectedViews)
                {
                   views.Add(view.Id);
                }
            }

            //parameter : DWGExportOptions dwgExportOptions
            var dgnExportOptions = new DGNExportOptions();
            // default values
            dgnExportOptions.FileVersion = DGNFileFormat.DGNVersion8;
            m_exportLayerMapping = m_enumLayerMapping[0];

            // set values from selected options
            dgnExportOptions.LayerMapping = m_exportLayerMapping;
            if (m_exportFileVersion == "MicroStation V7 Format")
            {
                dgnExportOptions.FileVersion = DGNFileFormat.DGNVersion7;
            }
            else
            {
                dgnExportOptions.FileVersion = DGNFileFormat.DGNVersion8;
            }
            dgnExportOptions.HideScopeBox = m_hideScopeBox;
            dgnExportOptions.HideUnreferenceViewTags = m_hideUnreferenceViewTags;
            dgnExportOptions.HideReferencePlane = m_hideReferencePlane;
            var mainModule = Process.GetCurrentProcess().MainModule;
            var RevitFolder = Path.GetDirectoryName( mainModule.FileName );
            dgnExportOptions.SeedName = Path.Combine(RevitFolder, @"ACADInterop\V8-Imperial-Seed3D.dgn");

            //Export
            var exported = m_activeDoc.Export(m_exportFolder, m_exportFileName, views, dgnExportOptions);
            return exported;
        }
            }
}
