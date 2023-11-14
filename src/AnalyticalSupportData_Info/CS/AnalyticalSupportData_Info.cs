// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System.Data;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.AnalyticalSupportData_Info.CS
{
    /// <summary>
    ///     get element's id and type information and its supported information.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private ExternalCommandData m_revit; // application of Revit

        /// <summary>
        ///     property to get private member variable m_elementInformation.
        /// </summary>
        public DataTable ElementInformation { get; private set; }

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            // Set currently executable application to private variable m_revit
            m_revit = revit;

            var selectedElements = new ElementSet();
            foreach (var elementId in m_revit.Application.ActiveUIDocument.Selection.GetElementIds())
                selectedElements.Insert(m_revit.Application.ActiveUIDocument.Document.GetElement(elementId));

            // get all the required information of selected elements and store them in a data table.
            ElementInformation = StoreInformationInDataTable(selectedElements);

            // show UI
            var displayForm = new AnalyticalSupportData_InfoForm(this);
            displayForm.ShowDialog();

            return Result.Succeeded;
        }

        /// <summary>
        ///     get all the required information of selected elements and store them in a data table
        /// </summary>
        /// <param name="selectedElements">
        ///     all selected elements in Revit main program
        /// </param>
        /// <returns>
        ///     a data table which store all the required information
        /// </returns>
        private DataTable StoreInformationInDataTable(ElementSet selectedElements)
        {
            var informationTable = CreatDataTable();

            foreach (Element element in selectedElements)
            {
                // Get  
                AnalyticalElement analyticalModel = null;
                var document = element.Document;
                var assocManager =
                    AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
                if (assocManager != null)
                {
                    var associatedElementId = assocManager.GetAssociatedElementId(element.Id);
                    if (associatedElementId != ElementId.InvalidElementId)
                    {
                        var associatedElement = document.GetElement(associatedElementId);
                        if (associatedElement != null && associatedElement is AnalyticalElement)
                            analyticalModel = associatedElement as AnalyticalElement;
                    }
                }

                if (null == analyticalModel) // skip no AnalyticalModel element
                    continue;

                var newRow = informationTable.NewRow();
                var idValue = element.Id.ToString(); // store element Id value             
                var typeName = ""; // store element type name
                var supportInformation = GetSupportInformation(analyticalModel); // store support information

                // get element type information
                switch (element.GetType().Name)
                {
                    case "WallFoundation":
                        var wallFound = element as WallFoundation;
                        var wallFootSymbol =
                            m_revit.Application.ActiveUIDocument.Document.GetElement(wallFound.GetTypeId()) as
                                ElementType; // get element Type
                        typeName = wallFootSymbol.Category.Name + ": " + wallFootSymbol.Name;
                        break;

                    case "FamilyInstance":
                        var familyInstance = element as FamilyInstance;
                        var symbol =
                            m_revit.Application.ActiveUIDocument.Document.GetElement(familyInstance.GetTypeId()) as
                                FamilySymbol;
                        typeName = symbol.Family.Name + ": " + symbol.Name;
                        break;

                    case "Floor":
                        var slab = element as Floor;
                        var slabType =
                            m_revit.Application.ActiveUIDocument.Document
                                .GetElement(slab.GetTypeId()) as FloorType; // get element type
                        typeName = slabType.Category.Name + ": " + slabType.Name;
                        break;

                    case "Wall":
                        var wall = element as Wall;
                        var wallType =
                            m_revit.Application.ActiveUIDocument.Document
                                .GetElement(wall.GetTypeId()) as WallType; // get element type
                        typeName = wallType.Kind + ": " + wallType.Name;
                        break;
                }

                // set the relative information of current element into the table.
                newRow["Id"] = idValue;
                newRow["Element Type"] = typeName;
                newRow["Support Type"] = supportInformation[0];
                newRow["Remark"] = supportInformation[1];
                informationTable.Rows.Add(newRow);
            }

            return informationTable;
        }

        /// <summary>
        ///     create a empty DataTable
        /// </summary>
        /// <returns></returns>
        private DataTable CreatDataTable()
        {
            // Create a new DataTable.
            var elementInformationTable = new DataTable("ElementInformationTable");

            // Create element id column and add to the DataTable.
            var idColumn = new DataColumn();
            idColumn.DataType = typeof(string);
            idColumn.ColumnName = "Id";
            idColumn.Caption = "Id";
            idColumn.ReadOnly = true;
            elementInformationTable.Columns.Add(idColumn);

            // Create element type column and add to the DataTable.
            var typeColumn = new DataColumn();
            typeColumn.DataType = typeof(string);
            typeColumn.ColumnName = "Element Type";
            typeColumn.Caption = "Element Type";
            typeColumn.ReadOnly = true;
            elementInformationTable.Columns.Add(typeColumn);

            // Create support column and add to the DataTable.
            var supportColumn = new DataColumn();
            supportColumn.DataType = typeof(string);
            supportColumn.ColumnName = "Support Type";
            supportColumn.Caption = "Support Type";
            supportColumn.ReadOnly = true;
            elementInformationTable.Columns.Add(supportColumn);

            // Create a column which can note others information
            var remarkColumn = new DataColumn();
            remarkColumn.DataType = typeof(string);
            remarkColumn.ColumnName = "Remark";
            remarkColumn.Caption = "Remark";
            remarkColumn.ReadOnly = true;
            elementInformationTable.Columns.Add(remarkColumn);

            return elementInformationTable;
        }

        /// <summary>
        ///     get element's support information
        /// </summary>
        /// <param name="analyticalModel"> element's analytical model</param>
        /// <returns></returns>
        private string[] GetSupportInformation(AnalyticalElement analyticalModel)
        {
            // supportInformation[0] store supportType
            // supportInformation[1] store other informations
            var supportInformations = new string[2] { "", "" };
            // "Supported" flag indicates if the Element is completely supported.
            // AnalyticalModel Support list keeps track of all supports.
            supportInformations[0] = "not supported";

            return supportInformations;
        }
    }
}
