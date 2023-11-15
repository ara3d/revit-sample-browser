// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ChangeService : IExternalCommand
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
                    // FabricationNetworkChangeService needs an ISet<ElementId>
                    ISet<ElementId> selIds = new HashSet<ElementId>();
                    foreach (var id in collection) selIds.Add(id);

                    using (var tr = new Transaction(doc, "Change Service of Fabrication Parts"))
                    {
                        tr.Start();

                        var config = FabricationConfiguration.GetFabricationConfiguration(doc);

                        // Get all loaded fabrication services
                        var allLoadedServices = config.GetAllLoadedServices();

                        var changeservice = new FabricationNetworkChangeService(doc);

                        // Change the fabrication parts to the first loaded service and palette
                        var result = changeservice.ChangeService(selIds, allLoadedServices[0].ServiceId, 0);

                        if (result != FabricationNetworkChangeServiceResult.Success)
                        {
                            message = "There was a problem with the change service.";
                            return Result.Failed;
                        }

                        doc.Regenerate();

                        tr.Commit();
                    }

                    return Result.Succeeded;
                }

                // inform user they need to select at least one element
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ChangeSize : IExternalCommand
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
                    // FabricationNetworkChangeService needs an ISet<ElementId>
                    var selIds = new HashSet<ElementId>();
                    foreach (var id in collection) selIds.Add(id);

                    using (var tr = new Transaction(doc, "Change Size of Fabrication Parts"))
                    {
                        tr.Start();

                        var config = FabricationConfiguration.GetFabricationConfiguration(doc);

                        // Get all loaded fabrication services
                        var allLoadedServices = config.GetAllLoadedServices();

                        // Create a map of sizes to swap the current sizes to a new size
                        var sizeMappings = new HashSet<FabricationPartSizeMap>();
                        var mapping = new FabricationPartSizeMap("12x12", 1.0, 1.0, false,
                            ConnectorProfileType.Rectangular, allLoadedServices[0].ServiceId, 0);
                        mapping.MappedWidthDiameter = 1.5;
                        mapping.MappedDepth = 1.5;
                        sizeMappings.Add(mapping);
                        var mapping1 = new FabricationPartSizeMap("18x18", 1.5, 1.5, false,
                            ConnectorProfileType.Rectangular, allLoadedServices[0].ServiceId, 0);
                        mapping1.MappedWidthDiameter = 2.0;
                        mapping1.MappedDepth = 2.0;
                        sizeMappings.Add(mapping1);

                        var changesize = new FabricationNetworkChangeService(doc);

                        // Change the size of the fabrication parts in the selection to the new sizes
                        var result = changesize.ChangeSize(selIds, sizeMappings);

                        if (result != FabricationNetworkChangeServiceResult.Success)
                        {
                            // Get the collection of element identifiers for parts that had errors posted against them
                            ICollection<ElementId> errorIds = changesize.GetElementsThatFailed();
                            if (errorIds.Count > 0)
                            {
                                message = "There was a problem with the change size.";
                                return Result.Failed;
                            }
                        }

                        doc.Regenerate();

                        tr.Commit();
                    }

                    return Result.Succeeded;
                }

                // inform user they need to select at least one element
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ApplyChange : IExternalCommand
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
                    // FabricationNetworkChangeService needs an ISet<ElementId>
                    ISet<ElementId> selIds = new HashSet<ElementId>();
                    foreach (var id in collection) selIds.Add(id);

                    using (var tr = new Transaction(doc, "Appply Change Service and Size of Fabrication Parts"))
                    {
                        tr.Start();

                        var config = FabricationConfiguration.GetFabricationConfiguration(doc);

                        // Get all loaded fabrication services
                        var allLoadedServices = config.GetAllLoadedServices();

                        var applychange = new FabricationNetworkChangeService(doc);

                        // Set the selection of element identifiers to be changed
                        applychange.SetSelection(selIds);
                        // Set the service to the second service in the list (ductwork exhaust service)
                        applychange.SetServiceId(allLoadedServices[1].ServiceId);
                        // Set the palette to the second in the list (round)
                        applychange.SetPaletteId(1);

                        // Get the sizes of all the straights that was in the selection of elements that was added to FabricationNetworkChangeService
                        var sizeMappings = applychange.GetMapOfAllSizesForStraights();
                        foreach (var sizemapping in sizeMappings)
                            if (sizemapping != null)
                            {
                                // Testing round so ignoring the depth and adding 6" to the current size so all straights will be updated to a new size
                                var widthDia = sizemapping.WidthDiameter + 0.5;
                                sizemapping.MappedWidthDiameter = widthDia;
                            }

                        applychange.SetMapOfSizesForStraights(sizeMappings);

                        // Get the in-line element type identiers
                        var inlineRevIds = new HashSet<ElementId>();
                        var inlineIds = applychange.GetInLinePartTypes();
                        for (var ii = inlineIds.Count() - 1; ii > -1; ii--)
                        {
                            var elemId = inlineIds.ElementAt(ii);
                            if (elemId != null)
                                inlineRevIds.Add(elemId);
                        }

                        // Set the in-line element type identiers by swapping them out by reversing the order to keep it simple but still exercise the code
                        IDictionary<ElementId, ElementId> swapinlineIds = new Dictionary<ElementId, ElementId>();
                        for (var ii = inlineIds.Count() - 1; ii > -1; ii--)
                        {
                            var elemId = inlineIds.ElementAt(ii);
                            var elemIdother = inlineRevIds.ElementAt(ii);
                            if (elemId != null && elemId != null)
                                swapinlineIds.Add(elemId, elemIdother);
                        }

                        applychange.SetMapOfInLinePartTypes(swapinlineIds);

                        // Apply the changes
                        var result = applychange.ApplyChange();

                        if (result != FabricationNetworkChangeServiceResult.Success)
                        {
                            message = "There was a problem with the apply change.";
                            return Result.Failed;
                        }

                        doc.Regenerate();

                        tr.Commit();
                    }

                    return Result.Succeeded;
                }

                // inform user they need to select at least one element
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
