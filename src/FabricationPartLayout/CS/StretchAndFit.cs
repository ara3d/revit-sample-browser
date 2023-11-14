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
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand
    /// </summary>
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

                // check user selection
                var uidoc = commandData.Application.ActiveUIDocument;
                var collection = uidoc.Selection.GetElementIds();
                if (collection.Count > 0)
                {
                    var selIds = new List<ElementId>();
                    foreach (var id in collection)
                        selIds.Add(id);

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

                    using (var tr = new Transaction(doc, "Stretch and Fit"))
                    {
                        tr.Start();

                        var result = FabricationPart.StretchAndFit(doc, connFrom, toEnd, out _);
                        if (result != FabricationPartFitResult.Success)
                        {
                            message = result.ToString();
                            return Result.Failed;
                        }

                        doc.Regenerate();

                        tr.Commit();
                    }

                    return Result.Succeeded;
                }

                // inform user they need to select at least one element
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
            // must be a fabrication part
            var part = doc.GetElement(elementId) as FabricationPart;
            if (part == null)
                return null;

            // must not be a straight, hanger or tap
            if (part.IsAStraight() || part.IsATap() || part.IsAHanger())
                return null;

            // part must be connected at one end and have one unoccupied connector
            var numUnused = part.ConnectorManager.UnusedConnectors.Size;
            var numConns = part.ConnectorManager.Connectors.Size;

            if (numConns - numUnused != 1)
                return null;

            foreach (Connector conn in part.ConnectorManager.UnusedConnectors)
                // return the first unoccupied connector
                return conn;

            return null;
        }

        private Connector GetValidConnectorToStretchAndFitTo(Document doc, ElementId elementId)
        {
            // connect to another fabrication part - will work also with families.
            var part = doc.GetElement(elementId) as FabricationPart;
            if (part == null)
                return null;

            // must not be a fabrication part hanger
            if (part.IsAHanger())
                return null;

            foreach (Connector conn in part.ConnectorManager.UnusedConnectors)
                // return the first unoccupied connector
                return conn;

            return null;
        }
    }
}