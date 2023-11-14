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
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
   /// <summary>
   /// Data class which stores the main information for exporting dxf format
   /// </summary>
   public class ExportSATData : ExportDataWithViews
   {
      #region Class Member Variables
      /// <summary>
      /// String list of AutoCAD versions
      /// </summary>
      private List<string> m_fileVersion;

      /// <summary>
      /// List of Autodesk.Revit.DB.ACADVersion defined in Revit
      /// </summary>
      private List<ACADVersion> m_enumFileVersion;

      /// <summary>
      /// File version option to export
      /// </summary>
      private ACADVersion m_exportFileVersion;
      #endregion

      #region Class Properties

      /// <summary>
      /// String collection of AutoCAD versions
      /// </summary>
      public ReadOnlyCollection<string> FileVersion => new ReadOnlyCollection<string>(m_fileVersion);

      /// <summary>
      /// Collection of Autodesk.Revit.DB.ACADVersion defined in Revit
      /// </summary>
      public ReadOnlyCollection<ACADVersion> EnumFileVersion => new ReadOnlyCollection<ACADVersion>(m_enumFileVersion);

      /// <summary>
      /// File version option to export
      /// </summary>
      public ACADVersion ExportFileVersion
      {
         get => m_exportFileVersion;
         set => m_exportFileVersion = value;
      }
      #endregion

      #region Class Member Methods
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="commandData">Revit command data</param>
      /// <param name="exportFormat">Format to export</param>
      public ExportSATData(ExternalCommandData commandData, ExportFormat exportFormat)
         : base(commandData, exportFormat)
      {
         Initialize();
      }

      /// <summary>
      /// Initialize the variables
      /// </summary>
      private void Initialize()
      {
         //AutoCAD versions
         m_fileVersion = new List<string>();
         m_enumFileVersion = new List<ACADVersion>();
         m_fileVersion.Add("AutoCAD 2013 DXF Files (*.sat)");
         m_enumFileVersion.Add(ACADVersion.R2013);
         m_fileVersion.Add("AutoCAD 2010 DXF Files (*.sat)");
         m_enumFileVersion.Add(ACADVersion.R2010);
         m_fileVersion.Add("AutoCAD 2007 DXF Files (*.sat)");
         m_enumFileVersion.Add(ACADVersion.R2007);

         var tmp = new StringBuilder();
         foreach (var version in m_fileVersion)
         {
            tmp.Append(version + "|*.sat|");
         }
         m_filter = tmp.ToString().TrimEnd('|');
         m_title = "Export SAT";
      }

      /// <summary>
      /// Collect the parameters and export
      /// </summary>
      /// <returns></returns>
      public override bool Export()
      {
         base.Export();

         //parameter : ViewSet views
         var views = new ViewSet();
         if (m_currentViewOnly)
         {
            views.Insert(m_activeDoc.ActiveView);
         }
         else
         {
            views = m_selectViewsData.SelectedViews;
         }

         ICollection<ElementId> viewIds = new List<ElementId>();
         foreach (View view in views)
         {
            viewIds.Add(view.Id);
         }

         //parameter : DXFExportOptions dxfExportOptions
         var satExportOptions = new SATExportOptions();

         //Export
         var exported = m_activeDoc.Export(m_exportFolder, m_exportFileName, viewIds, satExportOptions);
         
         return exported;
      }
      #endregion
   }
}
