// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.DynamicModelUpdate.CS
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Command to setup the updater, register the triggers (on execute), and unregister it (on close the document)
    //

    [Transaction(TransactionMode.Manual)]
    public class AssociativeSectionUpdater : IExternalCommand
    {
        // application's private data
        private static SectionUpdater _sectionUpdater;

        private static readonly List<ElementId> IdsToWatch = new List<ElementId>();
        private static ElementId _oldSectionId = ElementId.InvalidElementId;
        private Document m_document;
        private UIDocument m_documentUi;
        private AddInId m_thisAppId;

        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                m_document = commandData.Application.ActiveUIDocument.Document;
                m_documentUi = commandData.Application.ActiveUIDocument;
                m_thisAppId = commandData.Application.ActiveAddInId;

                // creating and registering the updater for the document.
                if (_sectionUpdater == null)
                    using (var tran = new Transaction(m_document, "Register Section Updater"))
                    {
                        tran.Start();

                        _sectionUpdater = new SectionUpdater(m_thisAppId);
                        _sectionUpdater.Register(m_document);

                        tran.Commit();
                    }

                TaskDialog.Show("Message", "Please select a section view, then select a window.");

                ElementId modelId = null;
                Element sectionElement = null;
                try
                {
                    var referSection =
                        m_documentUi.Selection.PickObject(ObjectType.Element, "Please select a section view.");
                    if (referSection != null)
                    {
                        var sectionElem = m_document.GetElement(referSection);
                        if (sectionElem != null) sectionElement = sectionElem;
                    }

                    var referModel = m_documentUi.Selection.PickObject(ObjectType.Element,
                        "Please select a window to associated with the section view.");
                    if (referModel != null)
                    {
                        var model = m_document.GetElement(referModel);
                        if (model is FamilyInstance)
                            modelId = model.Id;
                    }
                }
                catch (OperationCanceledException)
                {
                    TaskDialog.Show("Message", "The selection has been canceled.");
                    return Result.Cancelled;
                }

                if (modelId == null)
                {
                    TaskDialog.Show("Error", "The model is supposed to be a window.\n The operation will be canceled.");
                    return Result.Cancelled;
                }

                // Find the real ViewSection for the selected section element.
                var name = sectionElement.Name;
                var collector = new FilteredElementCollector(m_document);
                collector.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Views));
                var viewElements = from element in collector
                    where element.Name == name
                    select element;

                var sectionViews = viewElements.ToList();
                if (sectionViews.Count == 0)
                {
                    TaskDialog.Show("Message",
                        "Cannot find the view name " + name + "\n The operation will be canceled.");
                    return Result.Failed;
                }

                var sectionId = sectionViews[0].Id;

                // Associated the section view to the window, and add a trigger for it.
                if (!IdsToWatch.Contains(modelId) || _oldSectionId != sectionId)
                {
                    IdsToWatch.Clear();
                    IdsToWatch.Add(modelId);
                    _oldSectionId = sectionId;
                    UpdaterRegistry.RemoveAllTriggers(_sectionUpdater.GetUpdaterId());
                    _sectionUpdater.AddTriggerForUpdater(m_document, IdsToWatch, sectionId, sectionElement);
                    TaskDialog.Show("Message",
                        "The ViewSection id: " + sectionId + " has been associated to the window id: " + modelId +
                        "\n You can try to move or modify the window to see how the updater works.");
                }
                else
                {
                    TaskDialog.Show("Message", "The model has been already associated to the ViewSection.");
                }

                m_document.DocumentClosing += UnregisterSectionUpdaterOnClose;

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Unregister the updater on Revit document close.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="args">The DocumentClosing event args.</param>
        private void UnregisterSectionUpdaterOnClose(object source, DocumentClosingEventArgs args)
        {
            IdsToWatch.Clear();
            _oldSectionId = ElementId.InvalidElementId;

            if (_sectionUpdater != null)
            {
                UpdaterRegistry.UnregisterUpdater(_sectionUpdater.GetUpdaterId());
                _sectionUpdater = null;
            }
        }
    }
}
