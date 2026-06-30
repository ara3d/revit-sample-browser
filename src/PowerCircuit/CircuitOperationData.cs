// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Mep;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Ara3D.RevitSampleBrowser.PowerCircuit.CS
{
    public class CircuitOperationData
    {
        private readonly List<ElectricalSystemItem> m_electricalSystemItems;

        private ISet<ElectricalSystem> m_electricalSystemSet;

        /// <summary>
        ///     Active document of Revit
        /// </summary>
        private readonly UIDocument m_revitDoc;

        private ElectricalSystem m_selectedElectricalSystem;

        private readonly Selection m_selection;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit's external commandData</param>
        public CircuitOperationData(ExternalCommandData commandData)
        {
            m_revitDoc = commandData.Application.ActiveUIDocument;
            m_selection = m_revitDoc.Selection;

            m_electricalSystemSet = new HashSet<ElectricalSystem>();
            m_electricalSystemItems = [];

            CollectConnectorInfo();
            CollectCircuitInfo();
        }

        public Operation Operation { get; set; }

        public bool CanCreateCircuit { get; private set; }

        public bool HasCircuit { get; private set; }

        public bool HasPanel { get; private set; }

        public EditOption EditOption { get; set; }

        public ReadOnlyCollection<ElectricalSystemItem> ElectricalSystemItems
        {
            get
            {
                foreach (var es in m_electricalSystemSet)
                {
                    ElectricalSystemItem esi = new(es);
                    m_electricalSystemItems.Add(esi);
                }

                return new ReadOnlyCollection<ElectricalSystemItem>(m_electricalSystemItems);
            }
        }

        public int ElectricalSystemCount => m_electricalSystemSet.Count;

        private void CollectConnectorInfo()
        {
            CanCreateCircuit = true;
            // Flag to check if all selected elements are lighting devices
            var allLightingDevices = true;

            foreach (var elementId in m_selection.GetElementIds())
            {
                var element = m_revitDoc.Document.GetElement(elementId);
                if (element is not FamilyInstance fi)
                {
                    CanCreateCircuit = false;
                    return;
                }

                if (!string.Equals(fi.Category.Name, "Lighting Devices")) allLightingDevices = false;

                // Verify if the family instance has usable connectors
                if (!ConnectorHelper.VerifyUnusedConnectors(fi))
                {
                    CanCreateCircuit = false;
                    return;
                }
            }

            if (allLightingDevices) CanCreateCircuit = false;
        }

        private void CollectCircuitInfo()
        {
            //bool isElectricalSystem = false;

            var bInitilzedElectricalSystemSet = false;

            // Get common circuits of selected elements
            foreach (var elementId in m_selection.GetElementIds())
            {
                var element = m_revitDoc.Document.GetElement(elementId);
                MEPModel mepModel;

                if (element is FamilyInstance fi && (mepModel = fi.MEPModel) != null)
                {
                    // If the element is a family instance and its MEP model is not null,
                    // retrieve its circuits
                    // Then compare the circuits with circuits of other selected elements 
                    // to get the common ones
                    // Get all electrical systems
                    var ess = mepModel.GetElectricalSystems();
                    if (null == ess)
                    {
                        HasCircuit = false;
                        HasPanel = false;
                        return;
                    }

                    // Remove systems which are not power circuits
                    foreach (var es in ess)
                    {
                        if (es.SystemType != ElectricalSystemType.PowerCircuit)
                            ess.Remove(es);
                    }

                    if (ess.Count == 0)
                    {
                        HasCircuit = false;
                        HasPanel = false;
                        return;
                    }

                    // If m_electricalSystemSet is not set before, set it
                    // otherwise compare the circuits with circuits of other selected elements
                    // to get the common ones
                    if (!bInitilzedElectricalSystemSet)
                    {
                        m_electricalSystemSet = ess;
                        bInitilzedElectricalSystemSet = true;
                        continue;
                    }

                    foreach (var es in m_electricalSystemSet)
                    {
                        if (!ess.Contains(es))
                            m_electricalSystemSet.Remove(es);
                    }

                    if (m_electricalSystemSet.Count == 0)
                    {
                        HasCircuit = false;
                        HasPanel = false;
                        return;
                    }
                }
                else if (element is ElectricalSystem tempElectricalSystem)
                {
                    // If the element is an electrical system, verify if it is a power circuit
                    // If not, compare with circuits of other selected elements
                    // to get the common ones
                    //verify if it is a power circuit
                    if (tempElectricalSystem.SystemType != ElectricalSystemType.PowerCircuit)
                    {
                        HasCircuit = false;
                        HasPanel = false;
                        return;
                    }

                    // If m_electricalSystemSet is not set before, set it
                    // otherwise compare with circuits of other selected elements
                    // to get the common ones 
                    if (!bInitilzedElectricalSystemSet)
                    {
                        m_electricalSystemSet.Add(tempElectricalSystem);
                        bInitilzedElectricalSystemSet = true;
                        continue;
                    }

                    if (!m_electricalSystemSet.Contains(tempElectricalSystem))
                    {
                        HasCircuit = false;
                        HasPanel = false;
                        return;
                    }

                    m_electricalSystemSet.Clear();
                    m_electricalSystemSet.Add(tempElectricalSystem);
                }
                else
                {
                    HasCircuit = false;
                    HasPanel = false;
                    return;
                }
            }

            // Verify if there is any common power circuit
            if (m_electricalSystemSet.Count != 0)
            {
                HasCircuit = true;
                if (m_electricalSystemSet.Count == 1)
                    foreach (var es in m_electricalSystemSet)
                    {
                        m_selectedElectricalSystem = es;
                        break;
                    }

                foreach (var es in m_electricalSystemSet)
                {
                    if (!string.IsNullOrEmpty(es.PanelName))
                    {
                        HasPanel = true;
                        break;
                    }
                }
            }
        }

        public void Operate()
        {
            Transaction transaction = new(m_revitDoc.Document, Operation.ToString());
            transaction.Start();
            switch (Operation)
            {
                case Operation.CreateCircuit:
                    CreatePowerCircuit();
                    break;
                case Operation.EditCircuit:
                    EditCircuit();
                    break;
                case Operation.SelectPanel:
                    SelectPanel();
                    break;
                case Operation.DisconnectPanel:
                    DisconnectPanel();
                    break;
            }

            transaction.Commit();

            // Select the modified circuit
            if (Operation != Operation.CreateCircuit) SelectCurrentCircuit();
        }

        public void CreatePowerCircuit()
        {
            List<ElementId> selectionElementId = new();
            ElementSet elements = new();
            foreach (var elementId in m_selection.GetElementIds())
            {
                elements.Insert(m_revitDoc.Document.GetElement(elementId));
            }

            foreach (Element e in elements)
            {
                selectionElementId.Add(e.Id);
            }

            try
            {
                // Creation
                var es = ElectricalSystem.Create(m_revitDoc.Document, selectionElementId,
                    ElectricalSystemType.PowerCircuit);

                // Select the newly created power system
                m_selection.GetElementIds().Clear();
                m_selection.GetElementIds().Add(es.Id);
            }
            catch (Exception)
            {
                DialogHelper.ShowErrorMessage("FailedToCreateCircuit");
            }
        }

        public void EditCircuit()
        {
            switch (EditOption)
            {
                case EditOption.Add:
                    AddElementToCircuit();
                    break;
                case EditOption.Remove:
                    RemoveElementFromCircuit();
                    break;
                case EditOption.SelectPanel:
                    SelectPanel();
                    break;
            }
        }

        public void AddElementToCircuit()
        {
            // Clear selection before selecting the panel
            m_selection.GetElementIds().Clear();
            // Interact with UI to select a panel
            if (m_revitDoc.Selection.PickObject(ObjectType.Element) == null) return;

            // Verify if the selected element can be added to the circuit

            // Get selected element
            Element selectedElement = null;
            foreach (var elementId in m_selection.GetElementIds())
            {
                var element = m_revitDoc.Document.GetElement(elementId);
                selectedElement = element;
            }

            // Get the MEP model of selected element
            MEPModel mepModel = null;
            if (selectedElement is not FamilyInstance fi || null == (mepModel = fi.MEPModel))
            {
                DialogHelper.ShowErrorMessage("SelectElectricalComponent");
                return;
            }

            // Verify if the element has usable connector 
            if (!ConnectorHelper.VerifyUnusedConnectors(fi))
            {
                DialogHelper.ShowErrorMessage("NoUsableConnector");
                return;
            }

            if (ConnectorHelper.IsElementBelongsToCircuit(mepModel, m_selectedElectricalSystem))
            {
                DialogHelper.ShowErrorMessage("ElementInCircuit");
                return;
            }

            try
            {
                ElementSet es = new();
                foreach (var elementId in m_selection.GetElementIds())
                {
                    es.Insert(m_revitDoc.Document.GetElement(elementId));
                }

                if (!m_selectedElectricalSystem.AddToCircuit(es)) DialogHelper.ShowErrorMessage("FailedToAddElement");
            }
            catch (Exception)
            {
                DialogHelper.ShowErrorMessage("FailedToAddElement");
            }
        }

        public void RemoveElementFromCircuit()
        {
            // Clear selection before selecting the panel
            m_selection.GetElementIds().Clear();
            // Interact with UI to select a panel
            if (m_revitDoc.Selection.PickObject(ObjectType.Element) == null) return;

            // Get the selected element
            Element selectedElement = null;
            foreach (var elementId in m_revitDoc.Selection.GetElementIds())
            {
                var element = m_revitDoc.Document.GetElement(elementId);
                selectedElement = element;
            }

            // Get the MEP model of selected element
            MEPModel mepModel = null;
            if (selectedElement is not FamilyInstance fi || null == (mepModel = fi.MEPModel))
            {
                DialogHelper.ShowErrorMessage("SelectElectricalComponent");
                return;
            }

            // Check whether the selected element belongs to the circuit
            if (!ConnectorHelper.IsElementBelongsToCircuit(mepModel, m_selectedElectricalSystem))
            {
                DialogHelper.ShowErrorMessage("ElementNotInCircuit");
                return;
            }

            try
            {
                // Remove the selected element from circuit
                ElementSet es = new();
                foreach (var elementId in m_revitDoc.Selection.GetElementIds())
                {
                    es.Insert(m_revitDoc.Document.GetElement(elementId));
                }

                m_selectedElectricalSystem.RemoveFromCircuit(es);
            }
            catch (Exception)
            {
                DialogHelper.ShowErrorMessage("FailedToRemoveElement");
            }
        }

        public void SelectPanel()
        {
            // Clear selection before selecting the panel
            m_selection.GetElementIds().Clear();

            // Interact with UI to select a panel
            if (m_revitDoc.Selection.PickObject(ObjectType.Element) == null) return;

            try
            {
                foreach (var elementId in m_selection.GetElementIds())
                {
                    var element = m_revitDoc.Document.GetElement(elementId);
                    if (element is FamilyInstance fi) m_selectedElectricalSystem.SelectPanel(fi);
                }
            }
            catch (Exception)
            {
                DialogHelper.ShowErrorMessage("FailedToSelectPanel");
            }
        }

        public void DisconnectPanel()
        {
            try
            {
                m_selectedElectricalSystem.DisconnectPanel();
            }
            catch (Exception)
            {
                DialogHelper.ShowErrorMessage("FailedToDisconnectPanel");
            }
        }

        public void SelectCircuit(int index)
        {
            // Locate ElectricalSystemItem by index
            var esi = m_electricalSystemItems[index];
            var ei = esi.Id;

            // Locate expected electrical system
            m_selectedElectricalSystem = m_revitDoc.Document.GetElement(ei) as ElectricalSystem;
            // Select the electrical system
            SelectCurrentCircuit();
        }

        public void SelectCurrentCircuit()
        {
            m_selection.GetElementIds().Clear();
            m_selection.GetElementIds().Add(m_selectedElectricalSystem.Id);
        }

        public void ShowCircuit(int index)
        {
            var esi = m_electricalSystemItems[index];
            var ei = esi.Id;
            m_revitDoc.ShowElements(ei);
        }

    }
}
