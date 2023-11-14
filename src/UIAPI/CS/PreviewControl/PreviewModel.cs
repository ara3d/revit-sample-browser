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
        private readonly RApplication _application;


        private ElementId _currentDBViewId;
        private Document _dbDocument;
        private readonly UIApplication _uiApplication;

        public PreviewModel(RApplication application, ElementId viewId)
        {
            InitializeComponent();
            _application = application;
            _uiApplication = new UIApplication(application);
            _dbDocument = _uiApplication.ActiveUIDocument.Document;

            updateDocumentList(_dbDocument);
            updateViewsList(_uiApplication.ActiveUIDocument.ActiveView.Id);
        }

        private void updateViewsList(ElementId viewId)
        {
            // fill the combobox with printable views <name + id>
            var collecotr = new FilteredElementCollector(_dbDocument);
            collecotr.OfClass(typeof(RView));
            var secs = from Element f in collecotr where (f as RView).CanBePrinted select f as RView;
            _cbViews.Items.Clear();
            DBViewItem activeItem = null;
            foreach (var dbView in secs)
            {
                if (viewId == null || viewId == ElementId.InvalidElementId)
                {
                    activeItem = new DBViewItem(dbView, _dbDocument);
                    viewId = dbView.Id;
                }

                if (dbView.Id == viewId)
                {
                    activeItem = new DBViewItem(dbView, _dbDocument);
                    _cbViews.Items.Add(activeItem);
                }
                else
                {
                    _cbViews.Items.Add(new DBViewItem(dbView, _dbDocument));
                }
            }

            _cbViews.SelectedItem = activeItem;
        }

        private void updateDocumentList(Document selectedDocument)
        {
            // fill the documents to the comboxbox _cbDocuments.
            DBDocumentItem activeItem = null;
            _cbDocuments.Items.Clear();
            var docIter = _application.Documents.ForwardIterator();
            docIter.Reset();
            while (docIter.MoveNext())
            {
                var dbDoc = docIter.Current as Document;
                string documentName = null;
                DBDocumentItem item = null;
                if (dbDoc != null)
                {
                    if (dbDoc.IsFamilyDocument)
                    {
                        item = new DBDocumentItem(dbDoc.PathName, dbDoc);
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

                        item = new DBDocumentItem(documentName, dbDoc);
                    }

                    if (dbDoc.Equals(selectedDocument))
                    {
                        _dbDocument = selectedDocument;
                        activeItem = item;
                    }

                    _cbDocuments.Items.Add(item);
                }
            }

            _cbDocuments.Items.Add(new DBDocumentItem());
            _cbDocuments.SelectedItem = activeItem;
        }

        private void cbViews_SelIdxChanged(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb == null)
                return;

            var dbItem = cb.SelectedItem as DBViewItem;
            if (dbItem == null)
                return;

            //if (_currentDBViewId == null)
            //    return;

            //RView currentView = _dbDocument.get_Element(_currentDBViewId) as RView;
            //if(currentView == null)
            //    return;

            //if (dbItem.UniqueId.ToLower().CompareTo(currentView.UniqueId.ToLower()) != 0)
            //    return;

            var vc = _elementHostWPF.Child as PreviewControl;
            if (vc != null)
                vc.Dispose();
            _elementHostWPF.Child = new PreviewControl(_dbDocument, dbItem.Id);
            _currentDBViewId = dbItem.Id;
        }

        private void cbDocs_SelIdxChanged(object sender, EventArgs e)
        {
            var documentItem = _cbDocuments.SelectedItem as DBDocumentItem;
            if (documentItem.Document == _dbDocument)
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
                        _dbDocument = _application.OpenDocumentFile(ofd.FileName);
                    }
                    catch (Exception)
                    {
                    }

                    if (_dbDocument != null)
                    {
                        updateDocumentList(_dbDocument);
                        updateViewsList(null);
                    }
                }
                else
                {
                    // the combobox should show the current document item.
                    string documentName;
                    var projName = _dbDocument.ProjectInformation.Name;
                    if (string.IsNullOrEmpty(projName) || projName.ToLower().CompareTo("project name") == 0)
                    {
                        if (string.IsNullOrEmpty(_dbDocument.PathName))
                            documentName = projName;
                        else
                            documentName = new FileInfo(_dbDocument.PathName).Name;
                    }
                    else
                    {
                        documentName = projName;
                    }

                    foreach (DBDocumentItem dbItem in _cbDocuments.Items)
                        if (dbItem.Name.ToLower().CompareTo(documentName.ToLower()) == 0)
                        {
                            _cbDocuments.SelectedItem = dbItem;
                            break;
                        }
                }
            }
            else
            {
                _dbDocument = documentItem.Document;
                updateViewsList(null);
            }
        }
    }


    public class DBViewItem
    {
        public DBViewItem(RView dbView, Document dbDoc)
        {
            var viewType = dbDoc.GetElement(dbView.GetTypeId()) as ElementType;
            Name = viewType.Name + " " + dbView.Name;
            Id = dbView.Id;
            UniqueId = dbView.UniqueId;
        }

        public string Name { get; set; }

        public ElementId Id { get; set; }

        public string UniqueId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }


    public class DBDocumentItem
    {
        public DBDocumentItem(string name, Document doc)
        {
            Name = name;
            Document = doc;
            IsNull = false;
        }

        public DBDocumentItem()
        {
            IsNull = true;
        }

        public bool IsNull { get; set; }
        public string Name { get; set; }
        public Document Document { get; set; }

        public override string ToString()
        {
            if (IsNull)
                return "<Open Document...>";
            return Name;
        }
    }
}