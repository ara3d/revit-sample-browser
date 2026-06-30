#region Header

//
// CmdRemoveDwfLinks.cs - Remove DWF links
//
// Copyright (C) 2012-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdRemoveDwfLinks : IExternalCommand
    {
        #region MiroReloadLinks test code

        private void MiroReloadLinks(IList<RevitLinkType> fecLinkTypes)
        {
            foreach (var typeLink in fecLinkTypes)
            {
                if (!typeLink.IsFromRevitServer())
                    continue;

                var er = typeLink.GetExternalFileReference();

                if (er == null)
                    continue;

                var mp = er.GetPath();

                var userVisiblePath = ModelPathUtils
                    .ConvertModelPathToUserVisiblePath(mp);

                if (!mp.ServerPath)
                    continue;

                if (er.GetLinkedFileStatus()
                    != LinkedFileStatus.NotFound)
                    continue;

                // Nested links cannot be reloaded.
                if (typeLink.IsNestedLink)
                    continue;

                //RevitLinkLoadResult res = null; // 2017
                LinkLoadResult res = null; // 2018

                try
                {
                    // Reload/Load fail for problematic Revit Server links; LoadFrom with the same path works.
                    var mpForReload = ModelPathUtils
                        .ConvertUserVisiblePathToModelPath(
                            userVisiblePath);

                    res = typeLink.LoadFrom(mpForReload,
                        new WorksetConfiguration());

                    Util.InfoMsg($"Result = {res.LoadResult}");
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            } // foreach typeLink
        }

        #endregion // MiroReloadLinks test code

        #region Andrea Tassera Reload Links Test Code

#if NEED_THIS_SAMPLE_CODE
    // https://forums.autodesk.com/t5/revit-api-forum/reload-revit-links-from/m-p/7722248
    public Result Execute1(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      // Get application and document objects

      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
      Document doc = uidoc.Document;

      // NO TRANSACTION NEEDS TO BE OPENED

      try
      {
        using( Transaction tx = new Transaction( doc ) )
        {
          // Collect files linked in current project

          FilteredElementCollector collector = new FilteredElementCollector( doc );
          ICollection<Element> linkInstances = collector.OfClass( typeof( RevitLinkType ) ).ToElements();

          // Check which elements are loaded > to be used as filter

          List<bool> loaded = new List<bool>();
          foreach( RevitLinkType i in linkInstances )
          {
            loaded.Add( RevitLinkType.IsLoaded( doc, i.Id ) );
          }

          // Convert ICollection into a list of RevitLinkTypes
          int i1 = 0;

          List<RevitLinkType> revLinkType = new List<RevitLinkType>();
          foreach( RevitLinkType rli in linkInstances )
          {
            if( !loaded[i1++] )
            {
              revLinkType.Add( rli );
            }
          }

          // Put names of linked files into a list of strings
          int i2 = 0;

          List<string> linkNames = new List<string>();
          foreach( Element eli in linkInstances )
          {
            if( !loaded[i2++] )
            {
              linkNames.Add( eli.Name.Split( ' ' )[0] );
            }
          }

          // Prompt user with files selection dialog

          //Start:
          OpenFileDialog openFileDialog1 = new OpenFileDialog();
          openFileDialog1.InitialDirectory = ( @"P:\" );
          openFileDialog1.Filter = "RVT|*.rvt";
          openFileDialog1.Multiselect = true;
          openFileDialog1.RestoreDirectory = true;

          // If you select the files and hit OK (in the file browser)

          if( openFileDialog1.ShowDialog() == DialogResult.OK )
          {
            // Show which files (path + version) has been selected before linking them

            StringBuilder userSelectionWVersion = new StringBuilder();
            foreach( string fp in openFileDialog1.FileNames )
            {
              userSelectionWVersion.AppendLine(
                fp.ToString()
                + " which was created with " +
                BasicFileInfo.Extract( fp ).SavedInVersion.ToString().ToUpper() );
            }

            // Recap the user with his selection + Revit version of the file

            DialogResult linkCorrect = MessageBox.Show(
                          userSelectionWVersion.ToString(),
                          "You selected the files:",
                          MessageBoxButtons.OKCancel );

            // Put paths of files selected by user into a list

            if( linkCorrect == DialogResult.OK )
            {

              List<string> userSelectionNames = new List<string>();
              foreach( string fp in openFileDialog1.FileNames )
              {
                userSelectionNames.Add( fp.ToString() );
              }

              // Check which of the files that the user selected have the same name of the files linked in the project

              IEnumerable<string> elementsToReload = userSelectionNames.Where( a => linkNames.Exists( b => a.Contains( b ) ) );

              // Show which files need to be reloaded

              StringBuilder intersection = new StringBuilder();
              foreach( string fp in elementsToReload )
              {
                intersection.AppendLine( fp.ToString() );
              }
              DialogResult promptToLoad = MessageBox.Show( intersection.ToString(), "The following files need to be roloaded" );

              // Initialize + populate list of ModelPaths > path from where to reload

              List<ModelPath> modPaths = new List<ModelPath>();

              foreach( string fp in elementsToReload )
              {
                FileInfo filePath = new FileInfo( fp );
                ModelPath linkpath = ModelPathUtils.ConvertUserVisiblePathToModelPath( filePath.ToString() );
                modPaths.Add( linkpath );
              }

              // Zip together file (as RevitLinkType) and the corresponding path to be reloaded from > Reload

              foreach( var ab in revLinkType.Zip( modPaths, Tuple.Create ) )
              {
                ab.Item1.LoadFrom( ab.Item2, new WorksetConfiguration() );
              }
            }
            return Result.Succeeded;
          }
        }
      }
      catch( Exception ex )
      {
        // If something went wrong return Result.Failed

        DialogResult genericException = MessageBox.Show( ex.Message, "Oops there was problem!" );

        return Result.Failed;
      }
      return Result.Succeeded;
    }
#endif // NEED_THIS_SAMPLE_CODE

        #endregion // Andrea Tassera Reload Links Test Code

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            if (doc.IsFamilyDocument)
            {
                Util.ErrorMsg(
                    "This command requires an active document.");

                return Result.Failed;
            }

            var nDeleted = Util.RemoveDwfLinkUsingDelete(doc);

            Util.RemoveDwfLinkUsingExternalFileUtils(doc);

            return Result.Succeeded;
        }
    }
}