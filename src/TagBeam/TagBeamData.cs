// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Views;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using Document = Autodesk.Revit.Creation.Document;
namespace Ara3D.RevitSampleBrowser.TagBeam.CS
{
    public class TagBeamData
    {
        private readonly List<FamilyInstance> m_beamList;

        private readonly List<FamilySymbolWrapper> m_categoryTagTypes =
            [];

        private readonly Document m_docCreator; // document creation

        private readonly List<FamilySymbolWrapper> m_materialTagTypes =
            [];

        private readonly List<FamilySymbolWrapper> m_multiCategoryTagTypes =
            [];

        private readonly UIDocument m_revitDoc;

        //Required designer variable.
        private readonly View m_view; // current view

        public TagBeamData(ExternalCommandData commandData)
        {
            //Get beams selected 
            m_revitDoc = commandData.Application.ActiveUIDocument;
            m_docCreator = m_revitDoc.Document.Create;
            m_view = m_revitDoc.Document.ActiveView;

            m_beamList = SelectionHelper.GetSelectedBeams(m_revitDoc);
            if (m_beamList.Count < 1) throw new ApplicationException("there is no beam selected");

            //Get the family symbols of tag in this document.
            FilteredElementCollector collector = new(commandData.Application.ActiveUIDocument.Document);
            var elements = collector.OfClass(typeof(Family)).ToElements();

            foreach (Family family in elements)
            {
                if (family != null && family.GetFamilySymbolIds() != null)
                {
                    List<FamilySymbol> ffs = [];
                    foreach (var elementId in family.GetFamilySymbolIds())
                    {
                        ffs.Add((FamilySymbol)commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
                    }

                    foreach (var tagSymbol in ffs)
                        ViewHelper.AddTagSymbolByCategory(tagSymbol, m_categoryTagTypes, m_materialTagTypes,
                            m_multiCategoryTagTypes);
                }
            }
        }

        public List<FamilySymbolWrapper> this[TagMode mode] => mode switch
        {
            TagMode.TM_ADDBY_CATEGORY => m_categoryTagTypes,
            TagMode.TM_ADDBY_MATERIAL => m_materialTagTypes,
            TagMode.TM_ADDBY_MULTICATEGORY => m_multiCategoryTagTypes,
            _ => null,
        };

        public void CreateTag(TagMode tagMode,
            FamilySymbolWrapper tagSymbol, bool leader,
            TagOrientation tagOrientation)
        {
            foreach (var beam in m_beamList)
            {
                //Get the start point and end point of the selected beam.
                var location = beam.Location as LocationCurve;
                var curve = location.Curve;

                Transaction t = new(m_revitDoc.Document);
                t.Start("Create new tag");
                //Create tag on the beam's start and end.
                Reference beamRef = new(beam);
                var tag1 = IndependentTag.Create(m_revitDoc.Document,
                    m_view.Id, beamRef, leader, tagMode, tagOrientation, curve.GetEndPoint(0));
                var tag2 = IndependentTag.Create(m_revitDoc.Document,
                    m_view.Id, beamRef, leader, tagMode, tagOrientation, curve.GetEndPoint(1));

                //Change the tag's object Type.
                tag1.ChangeTypeId(tagSymbol.FamilySymbol.Id);
                tag2.ChangeTypeId(tagSymbol.FamilySymbol.Id);
                t.Commit();
            }
        }
    }
}
