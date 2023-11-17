// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.TraverseSystem.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
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
                    TaskDialog.Show("No Active Document", "There's no active document in Revit.",
                        TaskDialogCommonButtons.Ok);
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
                    message =
                        "The selected element does not belong to any well-connected mechanical or piping system. " +
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
                var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "traversal.xml");
                tree.DumpIntoXml(fileName);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Get the mechanical or piping system from selected element
        /// </summary>
        /// <param name="selectedElement">Selected element</param>
        /// <returns>The extracted mechanical or piping system. Null if no expected system is found.</returns>
        private MEPSystem ExtractMechanicalOrPipingSystem(Element selectedElement)
        {
            MEPSystem system = null;

            switch (selectedElement)
            {
                case MEPSystem element when element is MechanicalSystem || element is PipingSystem:
                    system = element;
                    return system;
                //
                // If selected element is a family instance, iterate its connectors and get the expected system
                case FamilyInstance fi:
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
                    break;
                }
                //
                // If selected element is a MEPCurve (e.g. pipe or duct), 
                // iterate its connectors and get the expected system
                case MEPCurve mepCurve:
                {
                    var connectors = mepCurve.ConnectorManager.Connectors;
                    system = ExtractSystemFromConnectors(connectors);
                    break;
                }
            }

            return system;
        }

        /// <summary>
        ///     Get the mechanical or piping system from the connectors of selected element
        /// </summary>
        /// <param name="connectors">Connectors of selected element</param>
        /// <returns>The found mechanical or piping system</returns>
        private static MEPSystem ExtractSystemFromConnectors(ConnectorSet connectors)
        {
            MEPSystem system = null;

            if (connectors == null || connectors.Size == 0) return null;

            // Get well-connected mechanical or piping systems from each connector
            var systems = new List<MEPSystem>();
            foreach (Connector connector in connectors)
            {
                var tmpSystem = connector.MEPSystem;
                switch (tmpSystem)
                {
                    case null:
                        continue;
                    case MechanicalSystem ms:
                    {
                        if (ms.IsWellConnected) systems.Add(tmpSystem);
                        break;
                    }
                    case PipingSystem ps when ps.IsWellConnected:
                        systems.Add(tmpSystem);
                        break;
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
