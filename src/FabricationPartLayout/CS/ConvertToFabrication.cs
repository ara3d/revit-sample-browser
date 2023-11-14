//
// (C) Copyright 2003-2011 by Autodesk, Inc.
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
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Fabrication;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class ConvertToFabrication : IExternalCommand
   {
      public virtual Result Execute(ExternalCommandData commandData
          , ref string message, ElementSet elements)
      {
         try
         {
            var doc = commandData.Application.ActiveUIDocument.Document;

            // get the user selection
            var uidoc = commandData.Application.ActiveUIDocument;
            var collection = uidoc.Selection.GetElementIds();

            if (collection.Count > 0)
            {
               // DesignToFabrication needs an ISet<ElementId>
               ISet<ElementId> selIds = new HashSet<ElementId>();
               foreach (var id in collection)
               {
                  selIds.Add(id);
               }

               // Set the in-line element type identiers to be swapped during the conversion replacing the family with a similar fabrication part
               IDictionary<ElementId, ElementId> convertInLineIds = new Dictionary<ElementId, ElementId>();

               // Get all family symbols
               var familyFinder = new FilteredElementCollector(doc);
               var families = familyFinder.OfClass(typeof(FamilySymbol)).ToElements().ToList();

               // Get the family symbol for the damper we are going to convert
               var fsName = "Fire Damper - Rectangular - Simple";

               // The found family symbol
               FamilySymbol fsDamper = null;
               foreach (FamilySymbol family in families)
               {
                  if (family.FamilyName == fsName)
                  {
                     fsDamper = family;
                     break;
                  }
               }

               // If the damper was found try to find the matching fabrication part type
               if (fsDamper != null)
               {
                  // Get the element type identifier for the family symbol
                  var elemFamSymId = fsDamper.Id;

                  // Get all fabrication part types
                  var fabPartTypeFinder = new FilteredElementCollector(doc);
                  var fabPartTypes = fabPartTypeFinder.OfClass(typeof(FabricationPartType)).ToElements().ToList();

                  // Get the fabrication part type for the damper we are going to convert to
                  var fptName = "Rect FD - Flange";

                  // The found fabrication part type
                  FabricationPartType fptDamper = null;
                  foreach (FabricationPartType partType in fabPartTypes)
                  {
                     if (partType.FamilyName == fptName)
                     {
                        fptDamper = partType;
                        break;
                     }
                  }

                  // The damper was found create the mapping in between the family symbol and the matching fabrication part type
                  if (fptDamper != null)
                  {
                     // Get the element type identifier for the fabricaion part type
                     var elemFabPartTypeId = fptDamper.Id;

                     // Create the mapping for the family to the fabrication part
                     convertInLineIds.Add(elemFamSymId, elemFabPartTypeId);
                  }
               }

               using (var tr = new Transaction(doc, "Convert To Fabrication Parts"))
               {
                  tr.Start();

                  var config = FabricationConfiguration.GetFabricationConfiguration(doc);

                  // get all loaded fabrication services and attempt to convert the design elements 
                  // to the first loaded service
                  var allLoadedServices = config.GetAllLoadedServices();

                  var converter = new DesignToFabricationConverter(doc);

                  // If there is a mapping defined attempt to add it to the converter
                  if (convertInLineIds.Count() > 0)
                  {
                     // Set the mappings
                     var mappingResult = converter.SetMapForFamilySymbolToFabricationPartType(convertInLineIds);

                     if (mappingResult != DesignToFabricationMappingResult.Success)
                     {
                        if (mappingResult != DesignToFabricationMappingResult.Undefined)
                           message = "There was a problem with the conversion. The map contained no entries.";
                        else if (mappingResult != DesignToFabricationMappingResult.InvalidFamilySymbol)
                           message = "There was a problem with the conversion. There was an invalid Family symbol identifier or an identifier that did not exist in the mappings.";
                        else if (mappingResult != DesignToFabricationMappingResult.InvalidFabricationPartType)
                           message = "There was a problem with the conversion. There was an invalid Fabrication part type identifier or an identifier that did not exist in the mappings.";
                        else if (mappingResult != DesignToFabricationMappingResult.UnsupportedFamilySymbol)
                           message = "There was a problem with the conversion. Unsupported Family symbol it is expected to be either valve, strainer, damper, smoke detector, end cap, or other in line component.";
                        else if (mappingResult != DesignToFabricationMappingResult.UnsupportedFabricationPartType)
                           message = "There was a problem with the conversion. Unsupported Fabrication part type. It is expected to be either valve, strainer, damper, smoke detector, end cap, or other in line component.";
                        return Result.Failed;
                     }
                  }

                  var result = converter.Convert(selIds, allLoadedServices[0].ServiceId);

                  if (result != DesignToFabricationConverterResult.Success)
                  {
                     message = "There was a problem with the conversion.";
                     return Result.Failed;
                  }

                  doc.Regenerate();

                  tr.Commit();
               }

               return Result.Succeeded;
            }
            else
            {
               // inform user they need to select at least one element
               message = "Please select at least one element.";
            }

            return Result.Failed;
         }
         catch (Exception ex)
         {
            message = ex.Message;
            return Result.Failed;
         }
      }
   }
}
