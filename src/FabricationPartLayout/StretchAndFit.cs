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
    public class StretchAndFit : IExternalCommand
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
                    List<ElementId> selIds = new();
                    foreach (var id in collection)
                    {
                        selIds.Add(id);
                    }

                    if (selIds.Count != 2)
                    {
                        message = "Select a fabrication part to stretch and fit from and an element to connect to.";
                        return Result.Cancelled;
                    }

                    var connFrom = GetValidConnectorToStretchAndFitFrom(doc, selIds.ElementAt(0));
                    var connTo = GetValidConnectorToStretchAndFitTo(doc, selIds.ElementAt(1));

                    var toEnd = FabricationPartRouteEnd.CreateFromConnector(connTo);

                    if (connFrom == null || connTo == null)
                    {
                        message = "Invalid fabrication parts to stretch and fit";
                        return Result.Cancelled;
                    }

                    using Transaction tr = new(doc, "Stretch and Fit");
                    tr.Start();

                    var result = FabricationPart.StretchAndFit(doc, connFrom, toEnd, out _);
                    if (result != FabricationPartFitResult.Success)
                    {
                        message = result.ToString();
                        return Result.Failed;
                    }

                    doc.Regenerate();

                    tr.Commit();

                    return Result.Succeeded;
                }

                message = "Select a fabrication part to stretch and fit from and an element to connect to.";

                return Result.Failed;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private Connector GetValidConnectorToStretchAndFitFrom(Document doc, ElementId elementId)
        {
            if (doc.GetElement(elementId) is not FabricationPart part)
                return null;

            // Straights, taps, and hangers cannot be stretched from.
            if (part.IsAStraight() || part.IsATap() || part.IsAHanger())
                return null;

            // Exactly one connected end and one free connector.
            var numUnused = part.ConnectorManager.UnusedConnectors.Size;
            var numConns = part.ConnectorManager.Connectors.Size;

            if (numConns - numUnused != 1)
                return null;

            foreach (Connector conn in part.ConnectorManager.UnusedConnectors)
            {
                return conn;
            }

            return null;
        }

        private Connector GetValidConnectorToStretchAndFitTo(Document doc, ElementId elementId)
        {
            if (doc.GetElement(elementId) is not FabricationPart part)
                return null;

            if (part.IsAHanger())
                return null;

            foreach (Connector conn in part.ConnectorManager.UnusedConnectors)
            {
                return conn;
            }

            return null;
        }
    }
}
