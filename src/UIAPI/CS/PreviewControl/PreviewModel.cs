// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ComboBox = System.Windows.Forms.ComboBox;
using Form = System.Windows.Forms.Form;
using RView = Autodesk.Revit.DB.View;
using RApplication = Autodesk.Revit.ApplicationServices.Application;

namespace Revit.SDK.Samples.UIAPI.CS
{
    public partial class PreviewModel : Form
    {
        private readonly RApplication m_application;

        private ElementId m_currentDbViewId;
        private Document m_dbDocument;
        private readonly UIApplication m_uiApplication;

        public PreviewModel(RApplication application, ElementId viewId)
        {
            InitializeComponent();
            m_application = application;
            m_uiApplication = new UIApplication(application);
            m_dbDocument = m_uiApplication.ActiveUIDocument.Document;

            UpdateDocumentList(m_dbDocument);
            UpdateViewsList(m_uiApplication.ActiveUIDocument.ActiveView.Id);
        }

        private void UpdateViewsList(ElementId viewId)
        {
            // fill the combobox with printable views <name + id>
            var collecotr = new FilteredElementCollector(m_dbDocument);
            collecotr.OfClass(typeof(RView));
            var secs = from Element f in collecotr where (f as RView).CanBePrinted select f as RView;
            _cbViews.Items.Clear();
            DbViewItem activeItem = null;
            foreach (var dbView in secs)
            {
                if (viewId == null || viewId == ElementId.InvalidElementId)
                {
                    activeItem = new DbViewItem(dbView, m_dbDocument);
                    viewId = dbView.Id;
                }

                if (dbView.Id == viewId)
                {
                    activeItem = new DbViewItem(dbView, m_dbDocument);
                    _cbViews.Items.Add(activeItem);
                }
                else
                {
                    _cbViews.Items.Add(new DbViewItem(dbView, m_dbDocument));
                }
            }

            _cbViews.SelectedItem = activeItem;
        }

        private void UpdateDocumentList(Document selectedDocument)
        {
            // fill the documents to the comboxbox _cbDocuments.
            DbDocumentItem activeItem = null;
            _cbDocuments.Items.Clear();
            var docIter = m_application.Documents.ForwardIterator();
            docIter.Reset();
            while (docIter.MoveNext())
            {
                var dbDoc = docIter.Current as Document;
                string documentName = null;
                DbDocumentItem item = null;
                if (dbDoc != null)
                {
                    if (dbDoc.IsFamilyDocument)
                    {
                        item = new DbDocumentItem(dbDoc.PathName, dbDoc);
                    }
                    else
                    {
                        var projName = dbDoc.ProjectInformation.Name;
                        if (string.IsNullOrEmpty(projName) || projName.ToLower().CompareTo("project name") == 0)
                        {
                            if (string.IsNullOrEmpty(dbDoc.PathName))
                                documentName = projName;
                            else
                                documentName = new FileInfo(dbDoc.PathName).Name;
                        }
                        else
                        {
                            documentName = projName;
                        }

                        item = new DbDocumentItem(documentName, dbDoc);
                    }

                    if (dbDoc.Equals(selectedDocument))
                    {
                        m_dbDocument = selectedDocument;
                        activeItem = item;
                    }

                    _cbDocuments.Items.Add(item);
                }
            }

            _cbDocuments.Items.Add(new DbDocumentItem());
            _cbDocuments.SelectedItem = activeItem;
        }

        private void cbViews_SelIdxChanged(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;

            if (!(cb?.SelectedItem is DbViewItem dbItem))
                return;

            //if (_currentDBViewId == null)
            //    return;

            //RView currentView = _dbDocument.get_Element(_currentDBViewId) as RView;
            //if(currentView == null)
            //    return;

            //if (dbItem.UniqueId.ToLower().CompareTo(currentView.UniqueId.ToLower()) != 0)
            //    return;

            var vc = _elementHostWPF.Child as PreviewControl;
            vc?.Dispose();
            _elementHostWPF.Child = new PreviewControl(m_dbDocument, dbItem.Id);
            m_currentDbViewId = dbItem.Id;
        }

        private void cbDocs_SelIdxChanged(object sender, EventArgs e)
        {
            var documentItem = _cbDocuments.SelectedItem as DbDocumentItem;
            if (documentItem.Document == m_dbDocument)
                return;

            if (documentItem.IsNull)
            {
                var ofd = new OpenFileDialog();
                ofd.DefaultExt = "rvt";
                ofd.Filter =
                    "Revit project files (*.rvt)|*.rvt|Revit family files (*.rfa)|*.rfa|Revit family template files (*.rft)|*.rft";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        m_dbDocument = m_application.OpenDocumentFile(ofd.FileName);
                    }
                    catch (Exception)
                    {
                    }

                    if (m_dbDocument != null)
                    {
                        UpdateDocumentList(m_dbDocument);
                        UpdateViewsList(null);
                    }
                }
                else
                {
                    // the combobox should show the current document item.
                    string documentName;
                    var projName = m_dbDocument.ProjectInformation.Name;
                    if (string.IsNullOrEmpty(projName) || projName.ToLower().CompareTo("project name") == 0)
                    {
                        if (string.IsNullOrEmpty(m_dbDocument.PathName))
                            documentName = projName;
                        else
                            documentName = new FileInfo(m_dbDocument.PathName).Name;
                    }
                    else
                    {
                        documentName = projName;
                    }

                    foreach (DbDocumentItem dbItem in _cbDocuments.Items)
                        if (dbItem.Name.ToLower().CompareTo(documentName.ToLower()) == 0)
                        {
                            _cbDocuments.SelectedItem = dbItem;
                            break;
                        }
                }
            }
            else
            {
                m_dbDocument = documentItem.Document;
                UpdateViewsList(null);
            }
        }
    }

    public class DbViewItem
    {
        private string m_name;
        private ElementId m_id;
        private string m_uniqueId;

        public DbViewItem(RView dbView, Document dbDoc)
        {
            var viewType = dbDoc.GetElement(dbView.GetTypeId()) as ElementType;
            Name = viewType.Name + " " + dbView.Name;
            Id = dbView.Id;
            UniqueId = dbView.UniqueId;
        }

        public string Name
        {
            get => m_name;
            set => m_name = value;
        }

        public ElementId Id
        {
            get => m_id;
            set => m_id = value;
        }

        public string UniqueId
        {
            get => m_uniqueId;
            set => m_uniqueId = value;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class DbDocumentItem
    {
        private bool m_isNull;
        private string m_name;
        private Document m_document;

        public DbDocumentItem(string name, Document doc)
        {
            Name = name;
            Document = doc;
            IsNull = false;
        }

        public DbDocumentItem()
        {
            IsNull = true;
        }

        public bool IsNull
        {
            get => m_isNull;
            set => m_isNull = value;
        }

        public string Name
        {
            get => m_name;
            set => m_name = value;
        }

        public Document Document
        {
            get => m_document;
            set => m_document = value;
        }

        public override string ToString()
        {
            return IsNull ? "<Open Document...>" : Name;
        }
    }
}
