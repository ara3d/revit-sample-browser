// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
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
    public partial class DefaultFamilyTypes : Page, IDockablePaneProvider
    {
        public static readonly DockablePaneId PaneId = new(new Guid("{DF0F08C3-447C-4615-B9B9-4843D821012E}"));
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
                FamilyTypeRecord record = new()
                {
                    CategoryName = Enum.GetName(typeof(BuiltInCategory), cid)
                };

                FilteredElementCollector collector = new(m_document);
                collector = collector.OfClass(typeof(FamilySymbol));
                var query = from FamilySymbol et in collector
                                                  where et.IsValidDefaultFamilyType(new ElementId(cid))
                                                  select et;

                var defaultFamilyTypeId = m_document.GetDefaultFamilyTypeId(new ElementId(cid));

                List<DefaultFamilyTypeCandidate> defaultFamilyTypeCandidates = new();
                foreach (var t in query)
                {
                    DefaultFamilyTypeCandidate item = new()
                    {
                        Name = $"{t.FamilyName} - {t.Name}",
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
            FilteredElementCollector collector = new(document);
            collector = collector.OfClass(typeof(Family));
            var query = collector.ToElements();

            List<BuiltInCategory> categoryids = new()
            {
                // Architecture -> Build -> Component
                BuiltInCategory.OST_MatchModel,
                // Annotate -> Detail -> Component
                BuiltInCategory.OST_MatchDetail
            };

            foreach (Family t in query)
            {
                if (!categoryids.Contains(t.FamilyCategory.BuiltInCategory))
                    categoryids.Add(t.FamilyCategory.BuiltInCategory);
            }

            return categoryids;
        }

        private void DefaultFamilyTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.RemovedItems.Count == 1)
            {
                if (sender is not ComboBox cb)
                    return;

                if (e.AddedItems[0] is not DefaultFamilyTypeCandidate item)
                    return;

                m_handler.SetData(item.CateogryId, item.Id);
                m_event.Raise();
            }
        }
    }

    public class DefaultFamilyTypeCandidate
    {
        public string Name { get; set; }

        public ElementId Id { get; set; }

        public ElementId CateogryId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class FamilyTypeRecord
    {
        public string CategoryName { get; set; }

        public List<DefaultFamilyTypeCandidate> DefaultFamilyTypeCandidates { get; set; }

        public DefaultFamilyTypeCandidate DefaultFamilyType { get; set; }
    }

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
            using Transaction tran = new(revitApp.ActiveUIDocument.Document,
                       $"Set Default family type to {m_defaultTypeId}");
            tran.Start();
            revitApp.ActiveUIDocument.Document.SetDefaultFamilyTypeId(m_builtInCategory, m_defaultTypeId);
            tran.Commit();
        }

        public void SetData(ElementId categoryId, ElementId typeId)
        {
            m_builtInCategory = categoryId;
            m_defaultTypeId = typeId;
        }
    }
}
