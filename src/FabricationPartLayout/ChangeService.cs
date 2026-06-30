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

                var uidoc = commandData.Application.ActiveUIDocument;
                var collection = uidoc.Selection.GetElementIds();

                if (collection.Count > 0)
                {
                    // FabricationNetworkChangeService requires ISet<ElementId>.
                    ISet<ElementId> selIds = new HashSet<ElementId>();
                    foreach (var id in collection)
                    {
                        selIds.Add(id);
                    }

                    using (var tr = new Transaction(doc, "Change Service of Fabrication Parts"))
                    {
                        tr.Start();

                        var config = FabricationConfiguration.GetFabricationConfiguration(doc);

                        var allLoadedServices = config.GetAllLoadedServices();

                        var changeservice = new FabricationNetworkChangeService(doc);

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

                var uidoc = commandData.Application.ActiveUIDocument;
                var collection = uidoc.Selection.GetElementIds();

                if (collection.Count > 0)
                {
                    ISet<ElementId> selIds = new HashSet<ElementId>();
                    foreach (var id in collection)
                    {
                        selIds.Add(id);
                    }

                    using (var tr = new Transaction(doc, "Change Size of Fabrication Parts"))
                    {
                        tr.Start();

                        var config = FabricationConfiguration.GetFabricationConfiguration(doc);

                        var allLoadedServices = config.GetAllLoadedServices();

                        var sizeMappings = new HashSet<FabricationPartSizeMap>();
                        var mapping = new FabricationPartSizeMap("12x12", 1.0, 1.0, false,
                            ConnectorProfileType.Rectangular, allLoadedServices[0].ServiceId, 0)
                        {
                            MappedWidthDiameter = 1.5,
                            MappedDepth = 1.5
                        };
                        sizeMappings.Add(mapping);
                        var mapping1 = new FabricationPartSizeMap("18x18", 1.5, 1.5, false,
                            ConnectorProfileType.Rectangular, allLoadedServices[0].ServiceId, 0)
                        {
                            MappedWidthDiameter = 2.0,
                            MappedDepth = 2.0
                        };
                        sizeMappings.Add(mapping1);

                        var changesize = new FabricationNetworkChangeService(doc);

                        var result = changesize.ChangeSize(selIds, sizeMappings);

                        if (result != FabricationNetworkChangeServiceResult.Success)
                        {
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

                var uidoc = commandData.Application.ActiveUIDocument;
                var collection = uidoc.Selection.GetElementIds();

                if (collection.Count > 0)
                {
                    ISet<ElementId> selIds = new HashSet<ElementId>();
                    foreach (var id in collection)
                    {
                        selIds.Add(id);
                    }

                    using (var tr = new Transaction(doc, "Appply Change Service and Size of Fabrication Parts"))
                    {
                        tr.Start();

                        var config = FabricationConfiguration.GetFabricationConfiguration(doc);

                        var allLoadedServices = config.GetAllLoadedServices();

                        var applychange = new FabricationNetworkChangeService(doc);

                        applychange.SetSelection(selIds);
                        // Sample targets the second loaded service (ductwork exhaust) and round palette.
                        applychange.SetServiceId(allLoadedServices[1].ServiceId);
                        applychange.SetPaletteId(1);

                        var sizeMappings = applychange.GetMapOfAllSizesForStraights();
                        foreach (var sizemapping in sizeMappings)
                        {
                            if (sizemapping != null)
                            {
                                // Round profile: add 6" to each straight width/diameter.
                                var widthDia = sizemapping.WidthDiameter + 0.5;
                                sizemapping.MappedWidthDiameter = widthDia;
                            }
                        }

                        applychange.SetMapOfSizesForStraights(sizeMappings);

                        var inlineRevIds = new HashSet<ElementId>();
                        var inlineIds = applychange.GetInLinePartTypes();
                        for (var ii = inlineIds.Count() - 1; ii > -1; ii--)
                        {
                            var elemId = inlineIds.ElementAt(ii);
                            if (elemId != null)
                                inlineRevIds.Add(elemId);
                        }

                        // Reverse inline type order to exercise swap mapping.
                        IDictionary<ElementId, ElementId> swapinlineIds = new Dictionary<ElementId, ElementId>();
                        for (var ii = inlineIds.Count() - 1; ii > -1; ii--)
                        {
                            var elemId = inlineIds.ElementAt(ii);
                            var elemIdother = inlineRevIds.ElementAt(ii);
                            if (elemId != null && elemId != null)
                                swapinlineIds.Add(elemId, elemIdother);
                        }

                        applychange.SetMapOfInLinePartTypes(swapinlineIds);

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
