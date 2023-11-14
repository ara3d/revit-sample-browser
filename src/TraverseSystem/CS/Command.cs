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
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Element = Autodesk.Revit.DB.Element;

namespace Revit.SDK.Samples.TraverseSystem.CS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // Verify if the active document is null
                var activeDoc = commandData.Application.ActiveUIDocument;
                if (activeDoc == null)
                {
                    TaskDialog.Show("No Active Document", "There's no active document in Revit.", TaskDialogCommonButtons.Ok);
                    return Result.Failed;
                }

                // Verify the number of selected elements
                var selElements = new ElementSet();
                foreach (var elementId in activeDoc.Selection.GetElementIds())
                {
                   selElements.Insert(activeDoc.Document.GetElement(elementId));
                }
                if (selElements.Size != 1)
                {
                    message = "Please select ONLY one element from current project.";
                    return Result.Failed;
                }

                // Get the selected element
                Element selectedElement = null;
                foreach (Element element in selElements)
                {
                    selectedElement = element;
                    break;
                }

                // Get the expected mechanical or piping system from selected element
                // Some elements in a non-well-connected system may get lost when traversing 
                //the system in the direction of flow; and
                // flow direction of elements in a non-well-connected system may not be right, 
                // therefore the sample will only support well-connected system.
                var system = ExtractMechanicalOrPipingSystem(selectedElement);
                if (system == null)
                {
                    message = "The selected element does not belong to any well-connected mechanical or piping system. " +
                        "The sample will not support well-connected systems for the following reasons: " +
                        Environment.NewLine +
                        "- Some elements in a non-well-connected system may get lost when traversing the system in the " +
                        "direction of flow" + Environment.NewLine +
                        "- Flow direction of elements in a non-well-connected system may not be right";
                    return Result.Failed;
                }

                // Traverse the system and dump the traversal into an XML file
                var tree = new TraversalTree(activeDoc.Document, system);
                tree.Traverse();
                var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "traversal.xml");
                tree.DumpIntoXML(fileName);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        /// Get the mechanical or piping system from selected element
        /// </summary>
        /// <param name="selectedElement">Selected element</param>
        /// <returns>The extracted mechanical or piping system. Null if no expected system is found.</returns>
        private MEPSystem ExtractMechanicalOrPipingSystem(Element selectedElement)
        {
            MEPSystem system = null;

            if (selectedElement is MEPSystem)
            {
                if (selectedElement is MechanicalSystem || selectedElement is PipingSystem)
                {
                    system = selectedElement as MEPSystem;
                    return system;
                }
            }
            else // Selected element is not a system
            {
                var fi = selectedElement as FamilyInstance;
                //
                // If selected element is a family instance, iterate its connectors and get the expected system
                if (fi != null)
                {
                    var mepModel = fi.MEPModel;
                    ConnectorSet connectors = null;
                    try
                    {
                        connectors = mepModel.ConnectorManager.Connectors;
                    }
                    catch (Exception)
                    {
                        system = null;
                    }

                    system = ExtractSystemFromConnectors(connectors);
                }
                else
                {
                    //
                    // If selected element is a MEPCurve (e.g. pipe or duct), 
                    // iterate its connectors and get the expected system
                    var mepCurve = selectedElement as MEPCurve;
                    if (mepCurve != null)
                    {
                        var connectors = mepCurve.ConnectorManager.Connectors;
                        system = ExtractSystemFromConnectors(connectors);
                    }
                }
            }

            return system;
        }

        /// <summary>
        /// Get the mechanical or piping system from the connectors of selected element
        /// </summary>
        /// <param name="connectors">Connectors of selected element</param>
        /// <returns>The found mechanical or piping system</returns>
        static private MEPSystem ExtractSystemFromConnectors(ConnectorSet connectors)
        {
            MEPSystem system = null;

            if (connectors == null || connectors.Size == 0)
            {
                return null;
            }

            // Get well-connected mechanical or piping systems from each connector
            var systems = new List<MEPSystem>();
            foreach (Connector connector in connectors)
            {
                var tmpSystem = connector.MEPSystem;
                if (tmpSystem == null)
                {
                    continue;
                }

                var ms = tmpSystem as MechanicalSystem;
                if (ms != null)
                {
                    if (ms.IsWellConnected)
                    {
                        systems.Add(tmpSystem);
                    }
                }
                else
                {
                    var ps = tmpSystem as PipingSystem;
                    if (ps != null && ps.IsWellConnected)
                    {
                        systems.Add(tmpSystem);
                    }
                }
            }

            // If more than one system is found, get the system contains the most elements
            var countOfSystem = systems.Count;
            if (countOfSystem != 0)
            {
                var countOfElements = 0;
                foreach (var sys in systems)
                {
                    if (sys.Elements.Size > countOfElements)
                    {
                        system = sys;
                        countOfElements = sys.Elements.Size;
                    }
                }
            }

            return system;
        }
    }
}


