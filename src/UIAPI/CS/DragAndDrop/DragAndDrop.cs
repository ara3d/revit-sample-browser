﻿//
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
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.UIAPI.CS
{
   public partial class FurnitureFamilyDragAndDropForm : System.Windows.Forms.Form
   {
      public static BuiltInCategory FamilyCategory => BuiltInCategory.OST_Furniture;

      private static FurnitureFamilyDragAndDropForm s_form;
      public static FurnitureFamilyDragAndDropForm GetTheForm(Document document)
      {
          if (s_form == null)
          {
              s_form = new FurnitureFamilyDragAndDropForm(document);
          }
          s_form.UpdateLoadedFamilies();
          return s_form;
      }

      /// <summary>
      /// Display class for the external family file listbox.
      /// </summary>
      private class FamilyListBoxMember
      {
         public string FullPath;
         public string Name;
         public FamilyListBoxMember(string fullPath, string name)
         {
            FullPath = fullPath;
            Name = name;
         }

         public override string ToString()
         {
            return Name;
         }
      }

      private Document m_document;

      /// <summary>
      /// Construct and populate the form.
      /// </summary>
      /// <param name="document"></param>
      private FurnitureFamilyDragAndDropForm(Document document)
      {
         InitializeComponent();
         m_document = document;

         UpdateFamilyFileList();
      }

      private void UpdateFamilyFileList()
      {
         // Visit each Revit library looking for Furniture families
         var libraryPaths = m_document.Application.GetLibraryPaths();
         foreach (var libraryPath in libraryPaths.Values)
         {
             foreach (var directory in Directory.EnumerateDirectories(libraryPath, "*Furniture", SearchOption.AllDirectories))
             {
                 foreach (var familyFile in Directory.EnumerateFiles(directory, "*.rfa", SearchOption.AllDirectories))
                 {
                     // Add each Furniture family to the listbox
                     var fileName = Path.GetFileName(familyFile);
                     var member = new FamilyListBoxMember(familyFile, fileName);
                     listBox1.Items.Add(member);
                 }
             }
         }
      }

      private void UpdateLoadedFamilies()
      {
         var collection = listView1.Items;
         collection.Clear();

         // Setup list view with loaded families
         var imageList = new ImageList();
         var size = new Size(50, 50);
         imageList.ImageSize = size;

         var collector = new FilteredElementCollector(m_document);
         collector.OfCategory(FamilyCategory);
         collector.OfClass(typeof(FamilySymbol));

         foreach (var familySymbol in collector.Cast<FamilySymbol>())
         {
            var item = new ListViewItem();
            item.Tag = familySymbol.Id;
            item.Text = familySymbol.Family.Name + "::" + familySymbol.Name;
            item.ToolTipText = "Drag to place instances of " + item.Text + " in the active document.";

            var bitmap = familySymbol.GetPreviewImage(size);

            if (bitmap != null)
            {
                imageList.Images.Add(bitmap);
                var index = imageList.Images.Count - 1;
                item.ImageIndex = index;
            }
            
            collection.Add(item);
         }

         listView1.LargeImageList = imageList;
      }

      // Drag action from list view
      private void listView_MouseMove(object sender, MouseEventArgs e)
      {
          if (MouseButtons == MouseButtons.Left)
          {
              var selectedItem = listView1.SelectedItems.Cast<ListViewItem>().FirstOrDefault<ListViewItem>();
              
              if (selectedItem != null)
              {
                  // Use custom Revit drag and drop behavior
                  var myhanlder = new LoadedFamilyDropHandler();
                  UIApplication.DoDragDrop(selectedItem.Tag, myhanlder);
              }
          }
      }

      // Drag action from list box
      private void listBox1_MouseMove(object sender, MouseEventArgs e)
      {
          if (MouseButtons == MouseButtons.Left)
          {
              var member = (FamilyListBoxMember)listBox1.SelectedItem;

              // Use standard Revit drag and drop behavior
              var data = new List<string>();
              data.Add(member.FullPath);
              UIApplication.DoDragDrop(data);
          }
      }

      private void FurnitureFamilyDragAndDropForm_FormClosed(object sender, FormClosedEventArgs e)
      {
          s_form = null;
      }
   }


   /// <summary>
   /// Custom handler for placement of loaded family types
   /// </summary>
   public class LoadedFamilyDropHandler : IDropHandler
   {
       public void Execute(UIDocument document, object data)
       {
         try
         {
            var familySymbolId = (ElementId)data;

            var symbol = document.Document.GetElement(familySymbolId) as FamilySymbol;

            if (symbol != null)
            {
               document.PromptForFamilyInstancePlacement(symbol);
            }
         }
         catch (Autodesk.Revit.Exceptions.OperationCanceledException)
         {
            // user canceled the operation
         }
       }
   }


}
