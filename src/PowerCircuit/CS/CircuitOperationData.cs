// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ara3D.RevitSampleBrowser.PowerCircuit.CS.Properties;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.PowerCircuit.CS
{
    /// <summary>
    ///     Data class which stores the information of electrical circuit operation
    /// </summary>
    public class CircuitOperationData
    {
        /// <summary>
        ///     Whether new circuit can be created with selected elements
        /// </summary>
        private bool m_canCreateCircuit;

        /// <summary>
        ///     Option of editing circuit
        /// </summary>
        private EditOption m_editOption;

        /// <summary>
        ///     All electrical system items which will be displayed in circuit selecting form
        /// </summary>
        private readonly List<ElectricalSystemItem> m_electricalSystemItems;

        /// <summary>
        ///     All electrical systems contain selected element
        /// </summary>
        private ISet<ElectricalSystem> m_electricalSystemSet;

        /// <summary>
        ///     Whether there is a circuit in selected elements
        /// </summary>
        private bool m_hasCircuit;

        /// <summary>
        ///     Whether the circuit in selected elements has panel
        /// </summary>
        private bool m_hasPanel;

        /// <summary>
        ///     Operation on selected elements
        /// </summary>
        private Operation m_operation;

        /// <summary>
        ///     Active document of Revit
        /// </summary>
        private readonly UIDocument m_revitDoc;

        /// <summary>
        ///     The electrical system chosen to operate
        /// </summary>
        private ElectricalSystem m_selectedElectricalSystem;

        /// <summary>
        ///     Selection of active document
        /// </summary>
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
            m_electricalSystemItems = new List<ElectricalSystemItem>();

            CollectConnectorInfo();
            CollectCircuitInfo();
        }

        /// <summary>
        ///     Operation type
        /// </summary>
        public Operation Operation
        {
            get => m_operation;

            set => m_operation = value;
        }

        /// <summary>
        ///     Get the information whether new circuit can be created
        /// </summary>
        public bool CanCreateCircuit => m_canCreateCircuit;

        /// <summary>
        ///     Get the value of whether there are circuits in selected elements
        /// </summary>
        public bool HasCircuit => m_hasCircuit;

        /// <summary>
        ///     Get the information whether the circuit in selected elements has panel
        /// </summary>
        public bool HasPanel => m_hasPanel;

        /// <summary>
        ///     Edit options
        /// </summary>
        public EditOption EditOption
        {
            get => m_editOption;
            set => m_editOption = value;
        }

        /// <summary>
        ///     All electrical system items which will be displayed in circuit selecting form
        /// </summary>
        public ReadOnlyCollection<ElectricalSystemItem> ElectricalSystemItems
        {
            get
            {
                foreach (var es in m_electricalSystemSet)
                {
                    var esi = new ElectricalSystemItem(es);
                    m_electricalSystemItems.Add(esi);
                }

                return new ReadOnlyCollection<ElectricalSystemItem>(m_electricalSystemItems);
            }
        }

        /// <summary>
        ///     Number of electrical systems contain selected elements
        /// </summary>
        public int ElectricalSystemCount => m_electricalSystemSet.Count;

        /// <summary>
        ///     Verify if all selected elements have unused connectors
        /// </summary>
        private void CollectConnectorInfo()
        {
            m_canCreateCircuit = true;
            // Flag to check if all selected elements are lighting devices
            var allLightingDevices = true;

            foreach (var elementId in m_selection.GetElementIds())
            {
                var element = m_revitDoc.Document.GetElement(elementId);
                if (!(element is FamilyInstance fi))
                {
                    m_canCreateCircuit = false;
                    return;
                }

                if (!string.Equals(fi.Category.Name, "Lighting Devices")) allLightingDevices = false;

                // Verify if the family instance has usable connectors
                if (!VerifyUnusedConnectors(fi))
                {
                    m_canCreateCircuit = false;
                    return;
                }
            }

            if (allLightingDevices) m_canCreateCircuit = false;
        }

        /// <summary>
        ///     Verify if the family instance has usable connectors
        /// </summary>
        /// <param name="fi">The family instance to be verified</param>
        /// <returns>
        ///     True if the family instance has usable connecotors,
        ///     otherwise false
        /// </returns>
        private static bool VerifyUnusedConnectors(FamilyInstance fi)
        {
            var hasUnusedElectricalConnector = false;
            try
            {
                var mepModel = fi.MEPModel;
                if (null == mepModel) return hasUnusedElectricalConnector;

                var cm = mepModel.ConnectorManager;
                var unusedConnectors = cm.UnusedConnectors;
                if (null == unusedConnectors || unusedConnectors.IsEmpty) return hasUnusedElectricalConnector;

                foreach (Connector connector in unusedConnectors)
                    if (connector.Domain == Domain.DomainElectrical)
                    {
                        hasUnusedElectricalConnector = true;
                        break;
                    }
            }
            catch (Exception)
            {
                return hasUnusedElectricalConnector;
            }

            return hasUnusedElectricalConnector;
        }

        /// <summary>
        ///     Get common circuits contain all selected elements
        /// </summary>
        private void CollectCircuitInfo()
        {
            //bool isElectricalSystem = false;

            var bInitilzedElectricalSystemSet = false;

            //
            // Get common circuits of selected elements
            //
            ElectricalSystem tempElectricalSystem = null;
            foreach (var elementId in m_selection.GetElementIds())
            {
                var element = m_revitDoc.Document.GetElement(elementId);
                MEPModel mepModel;

                if (element is FamilyInstance fi && (mepModel = fi.MEPModel) != null)
                {
                    //
                    // If the element is a family instance and its MEP model is not null,
                    // retrieve its circuits
                    // Then compare the circuits with circuits of other selected elements 
                    // to get the common ones
                    //
                    // Get all electrical systems
                    var ess = mepModel.GetElectricalSystems();
                    if (null == ess)
                    {
                        m_hasCircuit = false;
                        m_hasPanel = false;
                        return;
                    }

                    // Remove systems which are not power circuits
                    foreach (var es in ess)
                        if (es.SystemType != ElectricalSystemType.PowerCircuit)
                            ess.Remove(es);

                    if (ess.Count == 0)
                    {
                        m_hasCircuit = false;
                        m_hasPanel = false;
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
                        if (!ess.Contains(es))
                            m_electricalSystemSet.Remove(es);

                    if (m_electricalSystemSet.Count == 0)
                    {
                        m_hasCircuit = false;
                        m_hasPanel = false;
                        return;
                    }
                }
                else if ((tempElectricalSystem = element as ElectricalSystem) != null)
                {
                    //
                    // If the element is an electrical system, verify if it is a power circuit
                    // If not, compare with circuits of other selected elements
                    // to get the common ones
                    //
                    //verify if it is a power circuit
                    if (tempElectricalSystem.SystemType != ElectricalSystemType.PowerCircuit)
                    {
                        m_hasCircuit = false;
                        m_hasPanel = false;
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
                        m_hasCircuit = false;
                        m_hasPanel = false;
                        return;
                    }

                    m_electricalSystemSet.Clear();
                    m_electricalSystemSet.Add(tempElectricalSystem);
                }
                else
                {
                    m_hasCircuit = false;
                    m_hasPanel = false;
                    return;
                }
            }

            // Verify if there is any common power circuit
            if (m_electricalSystemSet.Count != 0)
            {
                m_hasCircuit = true;
                if (m_electricalSystemSet.Count == 1)
                    foreach (var es in m_electricalSystemSet)
                    {
                        m_selectedElectricalSystem = es;
                        break;
                    }

                foreach (var es in m_electricalSystemSet)
                    if (!string.IsNullOrEmpty(es.PanelName))
                    {
                        m_hasPanel = true;
                        break;
                    }
            }
        }

        /// <summary>
        ///     Dispatch operations
        /// </summary>
        public void Operate()
        {
            var transaction = new Transaction(m_revitDoc.Document, m_operation.ToString());
            transaction.Start();
            switch (m_operation)
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
            if (m_operation != Operation.CreateCircuit) SelectCurrentCircuit();
        }

        /// <summary>
        ///     Create a power circuit with selected elements
        /// </summary>
        public void CreatePowerCircuit()
        {
            var selectionElementId = new List<ElementId>();
            var elements = new ElementSet();
            foreach (var elementId in m_selection.GetElementIds())
                elements.Insert(m_revitDoc.Document.GetElement(elementId));

            foreach (Element e in elements) selectionElementId.Add(e.Id);

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
                ShowErrorMessage("FailedToCreateCircuit");
            }
        }

        /// <summary>
        ///     Dispatch operations of editing circuit
        /// </summary>
        public void EditCircuit()
        {
            switch (m_editOption)
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

        /// <summary>
        ///     Add an element to circuit
        /// </summary>
        public void AddElementToCircuit()
        {
            // Clear selection before selecting the panel
            m_selection.GetElementIds().Clear();
            // Interact with UI to select a panel
            if (m_revitDoc.Selection.PickObject(ObjectType.Element) == null) return;

            //
            // Verify if the selected element can be added to the circuit
            //

            // Get selected element
            Element selectedElement = null;
            foreach (var elementId in m_selection.GetElementIds())
            {
                var element = m_revitDoc.Document.GetElement(elementId);
                selectedElement = element;
            }

            // Get the MEP model of selected element
            MEPModel mepModel = null;
            if (!(selectedElement is FamilyInstance fi) || null == (mepModel = fi.MEPModel))
            {
                ShowErrorMessage("SelectElectricalComponent");
                return;
            }

            // Verify if the element has usable connector 
            if (!VerifyUnusedConnectors(fi))
            {
                ShowErrorMessage("NoUsableConnector");
                return;
            }

            if (IsElementBelongsToCircuit(mepModel, m_selectedElectricalSystem))
            {
                ShowErrorMessage("ElementInCircuit");
                return;
            }

            try
            {
                var es = new ElementSet();
                foreach (var elementId in m_selection.GetElementIds())
                    es.Insert(m_revitDoc.Document.GetElement(elementId));
                if (!m_selectedElectricalSystem.AddToCircuit(es)) ShowErrorMessage("FailedToAddElement");
            }
            catch (Exception)
            {
                ShowErrorMessage("FailedToAddElement");
            }
        }

        /// <summary>
        ///     Remove an element from selected circuit
        /// </summary>
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
            if (!(selectedElement is FamilyInstance fi) || null == (mepModel = fi.MEPModel))
            {
                ShowErrorMessage("SelectElectricalComponent");
                return;
            }

            // Check whether the selected element belongs to the circuit
            if (!IsElementBelongsToCircuit(mepModel, m_selectedElectricalSystem))
            {
                ShowErrorMessage("ElementNotInCircuit");
                return;
            }

            try
            {
                // Remove the selected element from circuit
                var es = new ElementSet();
                foreach (var elementId in m_revitDoc.Selection.GetElementIds())
                    es.Insert(m_revitDoc.Document.GetElement(elementId));
                m_selectedElectricalSystem.RemoveFromCircuit(es);
            }
            catch (Exception)
            {
                ShowErrorMessage("FailedToRemoveElement");
            }
        }

        private static bool IsElementBelongsToCircuit(MEPModel mepModel,
            ElectricalSystem selectedElectricalSystem)
        {
            var ess = mepModel.GetElectricalSystems();
            return null != ess && ess.Contains(selectedElectricalSystem);
        }

        /// <summary>
        ///     Select a panel for selected circuit
        /// </summary>
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
                ShowErrorMessage("FailedToSelectPanel");
            }
        }

        /// <summary>
        ///     Disconnect panel for selected circuit
        /// </summary>
        public void DisconnectPanel()
        {
            try
            {
                m_selectedElectricalSystem.DisconnectPanel();
            }
            catch (Exception)
            {
                ShowErrorMessage("FailedToDisconnectPanel");
            }
        }

        /// <summary>
        ///     Get selected index from circuit selecting form and locate expected circuit
        /// </summary>
        /// <param name="index">Index of selected item in circuit selecting form</param>
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

        /// <summary>
        ///     Select created/modified/selected electrical system
        /// </summary>
        public void SelectCurrentCircuit()
        {
            m_selection.GetElementIds().Clear();
            m_selection.GetElementIds().Add(m_selectedElectricalSystem.Id);
        }

        /// <summary>
        ///     Get selected index from circuit selecting form and show the circuit in the center of
        ///     screen by moving the view.
        /// </summary>
        /// <param name="index">Index of selected item in circuit selecting form</param>
        public void ShowCircuit(int index)
        {
            var esi = m_electricalSystemItems[index];
            var ei = esi.Id;
            m_revitDoc.ShowElements(ei);
        }

        /// <summary>
        ///     Show message box with specified string
        /// </summary>
        /// <param name="message">specified string to show</param>
        private static void ShowErrorMessage(string message)
        {
            TaskDialog.Show(Resources.ResourceManager.GetString("OperationFailed"),
                Resources.ResourceManager.GetString(message), TaskDialogCommonButtons.Ok);
        }
    }
}
