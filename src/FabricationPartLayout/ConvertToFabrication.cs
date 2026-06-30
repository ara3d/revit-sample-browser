// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ConvertToFabrication : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;

                var uidoc = commandData.Application.ActiveUIDocument;
                var collection = uidoc.Selection.GetElementIds();

                if (collection.Count > 0)
                {
                    // DesignToFabricationConverter.Convert requires ISet<ElementId>.
                    ISet<ElementId> selIds = new HashSet<ElementId>();
                    foreach (var id in collection)
                    {
                        selIds.Add(id);
                    }

                    IDictionary<ElementId, ElementId> convertInLineIds = new Dictionary<ElementId, ElementId>();

                    FilteredElementCollector familyFinder = new(doc);
                    var families = familyFinder.OfClass(typeof(FamilySymbol)).ToElements().ToList();

                    var fsName = "Fire Damper - Rectangular - Simple";

                    FamilySymbol fsDamper = null;
                    foreach (FamilySymbol family in families)
                    {
                        if (family.FamilyName == fsName)
                        {
                            fsDamper = family;
                            break;
                        }
                    }

                    if (fsDamper != null)
                    {
                        var elemFamSymId = fsDamper.Id;

                        FilteredElementCollector fabPartTypeFinder = new(doc);
                        var fabPartTypes = fabPartTypeFinder.OfClass(typeof(FabricationPartType)).ToElements().ToList();

                        var fptName = "Rect FD - Flange";

                        FabricationPartType fptDamper = null;
                        foreach (FabricationPartType partType in fabPartTypes)
                        {
                            if (partType.FamilyName == fptName)
                            {
                                fptDamper = partType;
                                break;
                            }
                        }

                        if (fptDamper != null)
                        {
                            var elemFabPartTypeId = fptDamper.Id;

                            convertInLineIds.Add(elemFamSymId, elemFabPartTypeId);
                        }
                    }

                    using Transaction tr = new(doc, "Convert To Fabrication Parts");
                    tr.Start();

                    var config = FabricationConfiguration.GetFabricationConfiguration(doc);

                    // Sample converts to the first loaded service.
                    var allLoadedServices = config.GetAllLoadedServices();

                    DesignToFabricationConverter converter = new(doc);

                    if (convertInLineIds.Count() > 0)
                    {
                        var mappingResult = converter.SetMapForFamilySymbolToFabricationPartType(convertInLineIds);

                        if (mappingResult != DesignToFabricationMappingResult.Success)
                        {
                            if (mappingResult != DesignToFabricationMappingResult.Undefined)
                                message = "There was a problem with the conversion. The map contained no entries.";
                            else if (mappingResult != DesignToFabricationMappingResult.InvalidFamilySymbol)
                                message =
                                    "There was a problem with the conversion. There was an invalid Family symbol identifier or an identifier that did not exist in the mappings.";
                            else if (mappingResult != DesignToFabricationMappingResult.InvalidFabricationPartType)
                                message =
                                    "There was a problem with the conversion. There was an invalid Fabrication part type identifier or an identifier that did not exist in the mappings.";
                            else if (mappingResult != DesignToFabricationMappingResult.UnsupportedFamilySymbol)
                                message =
                                    "There was a problem with the conversion. Unsupported Family symbol it is expected to be either valve, strainer, damper, smoke detector, end cap, or other in line component.";
                            else if (mappingResult !=
                                     DesignToFabricationMappingResult.UnsupportedFabricationPartType)
                                message =
                                    "There was a problem with the conversion. Unsupported Fabrication part type. It is expected to be either valve, strainer, damper, smoke detector, end cap, or other in line component.";
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

                    return Result.Succeeded;
                }

                message = "Please select at least one element.";

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
