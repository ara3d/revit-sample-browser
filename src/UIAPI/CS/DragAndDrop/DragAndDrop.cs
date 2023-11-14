// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace RevitMultiSample.UIAPI.CS
{
    public partial class FurnitureFamilyDragAndDropForm : Form
    {
        private static FurnitureFamilyDragAndDropForm _sForm;

        private readonly Document m_document;

        /// <summary>
        ///     Construct and populate the form.
        /// </summary>
        /// <param name="document"></param>
        private FurnitureFamilyDragAndDropForm(Document document)
        {
            InitializeComponent();
            m_document = document;

            UpdateFamilyFileList();
        }

        public static BuiltInCategory FamilyCategory => BuiltInCategory.OST_Furniture;

        public static FurnitureFamilyDragAndDropForm GetTheForm(Document document)
        {
            if (_sForm == null) _sForm = new FurnitureFamilyDragAndDropForm(document);
            _sForm.UpdateLoadedFamilies();
            return _sForm;
        }

        private void UpdateFamilyFileList()
        {
            // Visit each Revit library looking for Furniture families
            var libraryPaths = m_document.Application.GetLibraryPaths();
            foreach (var libraryPath in libraryPaths.Values)
            foreach (var directory in Directory.EnumerateDirectories(libraryPath, "*Furniture",
                         SearchOption.AllDirectories))
            foreach (var familyFile in Directory.EnumerateFiles(directory, "*.rfa", SearchOption.AllDirectories))
            {
                // Add each Furniture family to the listbox
                var fileName = Path.GetFileName(familyFile);
                var member = new FamilyListBoxMember(familyFile, fileName);
                listBox1.Items.Add(member);
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
                var item = new ListViewItem
                {
                    Tag = familySymbol.Id,
                    Text = familySymbol.Family.Name + "::" + familySymbol.Name
                };
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
                var selectedItem = listView1.SelectedItems.Cast<ListViewItem>().FirstOrDefault();

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
                var data = new List<string> { member.FullPath };
                UIApplication.DoDragDrop(data);
            }
        }

        private void FurnitureFamilyDragAndDropForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _sForm = null;
        }

        /// <summary>
        ///     Display class for the external family file listbox.
        /// </summary>
        private class FamilyListBoxMember
        {
            public readonly string FullPath;
            public readonly string Name;

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
    }

    /// <summary>
    ///     Custom handler for placement of loaded family types
    /// </summary>
    public class LoadedFamilyDropHandler : IDropHandler
    {
        public void Execute(UIDocument document, object data)
        {
            try
            {
                var familySymbolId = (ElementId)data;

                if (document.Document.GetElement(familySymbolId) is FamilySymbol symbol) document.PromptForFamilyInstancePlacement(symbol);
            }
            catch (OperationCanceledException)
            {
                // user canceled the operation
            }
        }
    }
}
