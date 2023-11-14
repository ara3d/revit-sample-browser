// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.Creation.Document;

namespace Revit.SDK.Samples.TagBeam.CS
{
    /// <summary>
    ///     Tag beam data class.
    /// </summary>
    public class TagBeamData
    {
        /// <summary>
        ///     Selected beams
        /// </summary>
        private readonly List<FamilyInstance> m_beamList =
            new List<FamilyInstance>();

        /// <summary>
        ///     Tag types whose category is "Structural Framing Tags"
        /// </summary>
        private readonly List<FamilySymbolWrapper> m_categoryTagTypes =
            new List<FamilySymbolWrapper>();

        private Document m_docCreator; // document creation

        /// <summary>
        ///     Tag types whose category is "Material Tags"
        /// </summary>
        private readonly List<FamilySymbolWrapper> m_materialTagTypes =
            new List<FamilySymbolWrapper>();

        /// <summary>
        ///     Tag types whose category is "Multi-Category Tags"
        /// </summary>
        private readonly List<FamilySymbolWrapper> m_multiCategoryTagTypes =
            new List<FamilySymbolWrapper>();

        private readonly UIDocument m_revitDoc;

        //Required designer variable.
        private readonly View m_view; // current view

        /// <summary>
        ///     Initializes a new instance of TagBeamData.
        /// </summary>
        /// <param name="commandData">
        ///     An object that is passed to the external application
        ///     which contains data related to the command
        /// </param>
        public TagBeamData(ExternalCommandData commandData)
        {
            //Get beams selected 
            m_revitDoc = commandData.Application.ActiveUIDocument;
            m_docCreator = m_revitDoc.Document.Create;
            m_view = m_revitDoc.Document.ActiveView;

            var elementSet = new ElementSet();
            foreach (var elementId in m_revitDoc.Selection.GetElementIds())
                elementSet.Insert(m_revitDoc.Document.GetElement(elementId));
            var itor = elementSet.ForwardIterator();
            while (itor.MoveNext())
            {
                var familyInstance = itor.Current as FamilyInstance;
                if (familyInstance != null && familyInstance.StructuralType == StructuralType.Beam)
                    m_beamList.Add(familyInstance);
            }

            if (m_beamList.Count < 1) throw new ApplicationException("there is no beam selected");

            //Get the family symbols of tag in this document.
            var collector = new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document);
            var elements = collector.OfClass(typeof(Family)).ToElements();

            foreach (Family family in elements)
                if (family != null && family.GetFamilySymbolIds() != null)
                {
                    var ffs = new List<FamilySymbol>();
                    foreach (var elementId in family.GetFamilySymbolIds())
                        ffs.Add((FamilySymbol)commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
                    foreach (var tagSymbol in ffs)
                        try
                        {
                            if (tagSymbol != null)
                                switch (tagSymbol.Category.Name)
                                {
                                    case "Structural Framing Tags":
                                        m_categoryTagTypes.Add(new FamilySymbolWrapper(tagSymbol));
                                        continue;
                                    case "Material Tags":
                                        m_materialTagTypes.Add(new FamilySymbolWrapper(tagSymbol));
                                        continue;
                                    case "Multi-Category Tags":
                                        m_multiCategoryTagTypes.Add(new FamilySymbolWrapper(tagSymbol));
                                        continue;
                                    default:
                                        continue;
                                }
                        }
                        catch (Exception)
                        {
                        }
                }
        }

        /// <summary>
        ///     Tag families with specified mode
        /// </summary>
        /// <param name="mode">mode of tag families to get</param>
        /// <returns></returns>
        public List<FamilySymbolWrapper> this[TagMode mode]
        {
            get
            {
                switch (mode)
                {
                    case TagMode.TM_ADDBY_CATEGORY:
                        return m_categoryTagTypes;
                    case TagMode.TM_ADDBY_MATERIAL:
                        return m_materialTagTypes;
                    case TagMode.TM_ADDBY_MULTICATEGORY:
                        return m_multiCategoryTagTypes;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        ///     Tag the beam's start and end.
        /// </summary>
        /// <param name="tagMode">Mode of tag</param>
        /// <param name="tagSymbol">Tag symbol wrapper</param>
        /// <param name="leader">Whether the tag has leader</param>
        /// <param name="tagOrientation">Orientation of tag</param>
        public void CreateTag(TagMode tagMode,
            FamilySymbolWrapper tagSymbol, bool leader,
            TagOrientation tagOrientation)
        {
            foreach (var beam in m_beamList)
            {
                //Get the start point and end point of the selected beam.
                var location = beam.Location as LocationCurve;
                var curve = location.Curve;

                var t = new Transaction(m_revitDoc.Document);
                t.Start("Create new tag");
                //Create tag on the beam's start and end.
                var beamRef = new Reference(beam);
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
