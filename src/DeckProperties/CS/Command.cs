//
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
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DeckProperties.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private DeckPropertyForm m_displayForm;
        private Document m_document;

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // Get the application of revit
            var revit = commandData.Application;
            m_document = revit.ActiveUIDocument.Document;

            try
            {
                var elementSet = new ElementSet();
                foreach (var elementId in revit.ActiveUIDocument.Selection.GetElementIds())
                    elementSet.Insert(revit.ActiveUIDocument.Document.GetElement(elementId));
                if (elementSet.IsEmpty)
                {
                    TaskDialog.Show("Select", "Please select one floor or slab at least.");
                    return Result.Cancelled;
                }

                using (m_displayForm = new DeckPropertyForm())
                {
                    var floorList = new List<Floor>();
                    foreach (var elementId in revit.ActiveUIDocument.Selection.GetElementIds())
                    {
                        var element = revit.ActiveUIDocument.Document.GetElement(elementId);
                        var floor = element as Floor;
                        if (floor != null) floorList.Add(floor);
                    }

                    if (floorList.Count <= 0)
                    {
                        TaskDialog.Show("Select", "Please select one floor or slab at least.");
                        return Result.Cancelled;
                    }

                    foreach (var floor in floorList) DumpSlab(floor);
                    m_displayForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                // If any error, store error information in message and return failed
                message = ex.Message;
                return Result.Failed;
            }

            // If everything goes well, return succeeded.
            return Result.Succeeded;
        }

        /// <summary>
        ///     Dump the properties of interest for the slab passed as a parameter
        /// </summary>
        /// <param name="slab"></param>
        private void DumpSlab(Floor slab)
        {
            m_displayForm.WriteLine("Dumping Slab" + slab.Id);

            var slabType = slab.FloorType;

            if (slabType != null)
                foreach (var layer in slabType.GetCompoundStructure().GetLayers())
                    if (layer.Function == MaterialFunctionAssignment.StructuralDeck)
                        DumbDeck(layer);
                    else
                        DumpLayer(layer);

            m_displayForm.WriteLine(" ");
        }

        /// <summary>
        ///     Dump properties specific to a decking layer
        /// </summary>
        /// <param name="deck"></param>
        private void DumbDeck(CompoundStructureLayer deck)
        {
            m_displayForm.WriteLine("Dumping Deck");

            if (deck.MaterialId != ElementId.InvalidElementId)
            {
                // get the deck material object. In this sample all we need to display is the
                // name, but other properties are readily available from the material object.
                var deckMaterial = m_document.GetElement(deck.MaterialId) as Material;
                m_displayForm.WriteLine("Deck Material = " + deckMaterial.Name);
            }

            if (deck.DeckProfileId != ElementId.InvalidElementId)
            {
                // the deck profile is actually a family symbol from a family of profiles
                var deckProfile = m_document.GetElement(deck.DeckProfileId) as FamilySymbol;

                // firstly display the full name as the user would see it in the user interface
                // this is done in the format Family.Name and then Symbol.Name
                m_displayForm.WriteLine("Deck Profile = "
                                        + deckProfile.Family.Name + " : " + deckProfile.Name);

                // the symbol object also contains parameters that describe how the deck is
                // specified. From these parameters an external application can generate
                // identical decking for analysis purposes
                DumpParameters(deckProfile);
            }
        }

        /// <summary>
        ///     A generic parameter display method that displays all the parameters of an element
        /// </summary>
        /// <param name="element"></param>
        private void DumpParameters(Element element)
        {
            foreach (Parameter parameter in element.Parameters)
            {
                var value = "";
                switch (parameter.StorageType)
                {
                    case StorageType.Double:
                        value = parameter.AsDouble().ToString();
                        break;
                    case StorageType.ElementId:
                        value = parameter.AsElementId().ToString();
                        break;
                    case StorageType.String:
                        value = parameter.AsString();
                        break;
                    case StorageType.Integer:
                        value = parameter.AsInteger().ToString();
                        break;
                }

                m_displayForm.WriteLine(parameter.Definition.Name + " = " + value);
            }
        }

        /// <summary>
        ///     for non deck layers this method is called and it displays minimal information
        ///     about the layer
        /// </summary>
        /// <param name="layer"></param>
        private void DumpLayer(CompoundStructureLayer layer)
        {
            // Display the name of the material. More detailed material properties can
            // be found form the material object
            m_displayForm.WriteLine("Dumping Layer");
            var material = m_document.GetElement(layer.MaterialId) as Material;
            if (material != null) m_displayForm.WriteLine("Layer material = " + material.Name);

            // display the thickness of the layer in inches.
            m_displayForm.WriteLine("Layer Thickness = " + layer.Width);
        }
    }
}