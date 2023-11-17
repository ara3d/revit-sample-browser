// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ComboBox = System.Windows.Controls.ComboBox;
//
// (C) Copyright 2003-2019 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

namespace Ara3D.RevitSampleBrowser.GetSetDefaultTypes.CS
{
    /// <summary>
    ///     Interaction logic for DefaultFamilyTypes.xaml
    /// </summary>
    public partial class DefaultFamilyTypes : Page, IDockablePaneProvider
    {
        public static readonly DockablePaneId PaneId = new DockablePaneId(new Guid("{DF0F08C3-447C-4615-B9B9-4843D821012E}"));
        private Document m_document;

        private readonly ExternalEvent m_event;
        private readonly DefaultFamilyTypeCommandHandler m_handler;

        public DefaultFamilyTypes()
        {
            InitializeComponent();

            m_handler = new DefaultFamilyTypeCommandHandler();
            m_event = ExternalEvent.Create(m_handler);
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;

            data.InitialState = new DockablePaneState
            {
                DockPosition = DockPosition.Top
            };
        }

        /// <summary>
        ///     Sets document to the default family type pane.
        ///     It will get all valid default types for the category and fill the data grid.
        /// </summary>
        public void SetDocument(Document document)
        {
            if (m_document == document)
                return;

            m_document = document;

            _dataGrid_DefaultFamilyTypes.Items.Clear();

            var categories = GetAllFamilyCateogries(m_document);
            if (categories.Count < 1)
                return;

            foreach (var cid in categories)
            {
                var record = new FamilyTypeRecord
                {
                    CategoryName = Enum.GetName(typeof(BuiltInCategory), cid)
                };

                //RLog.WriteComment(String.Format("The valid default family type candidates of {0} are:", Enum.GetName(typeof(BuiltInCategory), cid)));
                var collector = new FilteredElementCollector(m_document);
                collector = collector.OfClass(typeof(FamilySymbol));
                var query = from FamilySymbol et in collector
                    where et.IsValidDefaultFamilyType(new ElementId(cid))
                    select et; // Linq query  

                var defaultFamilyTypeId = m_document.GetDefaultFamilyTypeId(new ElementId(cid));

                var defaultFamilyTypeCandidates = new List<DefaultFamilyTypeCandidate>();
                foreach (var t in query)
                {
                    var item = new DefaultFamilyTypeCandidate
                    {
                        Name = t.FamilyName + " - " + t.Name,
                        Id = t.Id,
                        CateogryId = new ElementId(cid)
                    };
                    defaultFamilyTypeCandidates.Add(item);
                    if (t.Id == defaultFamilyTypeId)
                        record.DefaultFamilyType = item;
                }

                record.DefaultFamilyTypeCandidates = defaultFamilyTypeCandidates;

                _dataGrid_DefaultFamilyTypes.Items.Add(record);
            }
        }

        private List<BuiltInCategory> GetAllFamilyCateogries(Document document)
        {
            var collector = new FilteredElementCollector(document);
            collector = collector.OfClass(typeof(Family));
            var query = collector.ToElements();

            var categoryids = new List<BuiltInCategory>
            {
                // The corresponding UI for OST_MatchModel is "Architecture->Build->Component"
                BuiltInCategory.OST_MatchModel,
                // The corresponding UI for OST_MatchModel is "Annotate->Detail->Component"
                BuiltInCategory.OST_MatchDetail
            };

            foreach (Family t in query)
            {
                if (!categoryids.Contains(t.FamilyCategory.BuiltInCategory))
                    categoryids.Add(t.FamilyCategory.BuiltInCategory);
            }

            return categoryids;
        }

        /// <summary>
        ///     Responses to the type selection changed.
        ///     It will set the selected type as default type.
        /// </summary>
        private void DefaultFamilyTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.RemovedItems.Count == 1)
            {
                if (!(sender is ComboBox cb))
                    return;

                if (!(e.AddedItems[0] is DefaultFamilyTypeCandidate item))
                    return;

                m_handler.SetData(item.CateogryId, item.Id);
                m_event.Raise();
            }
        }
    }

    /// <summary>
    ///     The default family type candidate.
    /// </summary>
    public class DefaultFamilyTypeCandidate
    {
        /// <summary>
        ///     The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The element id.
        /// </summary>
        public ElementId Id { get; set; }

        /// <summary>
        ///     The category id.
        /// </summary>
        public ElementId CateogryId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    ///     The element type record for the data grid.
    /// </summary>
    public class FamilyTypeRecord
    {
        /// <summary>
        ///     The category name.
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        ///     List of default family type candidates.
        /// </summary>
        public List<DefaultFamilyTypeCandidate> DefaultFamilyTypeCandidates { get; set; }

        /// <summary>
        ///     The current default family type.
        /// </summary>
        public DefaultFamilyTypeCandidate DefaultFamilyType { get; set; }
    }

    /// <summary>
    ///     The command handler to set current selection as default family type.
    /// </summary>
    public class DefaultFamilyTypeCommandHandler : IExternalEventHandler
    {
        private ElementId m_builtInCategory;
        private ElementId m_defaultTypeId;

        public string GetName()
        {
            return "Reset Default family type";
        }

        public void Execute(UIApplication revitApp)
        {
            using (var tran = new Transaction(revitApp.ActiveUIDocument.Document,
                       "Set Default family type to " + m_defaultTypeId))
            {
                tran.Start();
                revitApp.ActiveUIDocument.Document.SetDefaultFamilyTypeId(m_builtInCategory, m_defaultTypeId);
                tran.Commit();
            }
        }

        public void SetData(ElementId categoryId, ElementId typeId)
        {
            m_builtInCategory = categoryId;
            m_defaultTypeId = typeId;
        }
    } // class CommandHandler
}
