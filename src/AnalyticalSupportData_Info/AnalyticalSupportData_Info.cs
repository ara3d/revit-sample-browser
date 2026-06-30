// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Data;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.AnalyticalSupportData_Info.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private ExternalCommandData m_revit; // application of Revit

        public DataTable ElementInformation { get; private set; }

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            m_revit = revit;

            var selectedElements = new ElementSet();
            foreach (var elementId in m_revit.Application.ActiveUIDocument.Selection.GetElementIds())
            {
                selectedElements.Insert(m_revit.Application.ActiveUIDocument.Document.GetElement(elementId));
            }

            ElementInformation = StoreInformationInDataTable(selectedElements);

            var displayForm = new AnalyticalSupportDataInfoForm(this);
            displayForm.ShowDialog();

            return Result.Succeeded;
        }

        private DataTable StoreInformationInDataTable(ElementSet selectedElements)
        {
            var informationTable = CreatDataTable();

            foreach (Element element in selectedElements)
            {
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
                        if (associatedElement != null && associatedElement is AnalyticalElement analyticalElement)
                            analyticalModel = analyticalElement;
                    }
                }

                if (null == analyticalModel) // skip no AnalyticalModel element
                    continue;

                var newRow = informationTable.NewRow();
                var idValue = element.Id.ToString(); // store element Id value             
                var typeName = ""; // store element type name
                var supportInformation = GetSupportInformation(analyticalModel); // store support information

                switch (element.GetType().Name)
                {
                    case "WallFoundation":
                        var wallFound = element as WallFoundation;
                        var wallFootSymbol =
                            m_revit.Application.ActiveUIDocument.Document.GetElement(wallFound.GetTypeId()) as
                                ElementType; // get element Type
                        typeName = $"{wallFootSymbol.Category.Name}: {wallFootSymbol.Name}";
                        break;

                    case "FamilyInstance":
                        var familyInstance = element as FamilyInstance;
                        var symbol =
                            m_revit.Application.ActiveUIDocument.Document.GetElement(familyInstance.GetTypeId()) as
                                FamilySymbol;
                        typeName = $"{symbol.Family.Name}: {symbol.Name}";
                        break;

                    case "Floor":
                        var slab = element as Floor;
                        var slabType =
                            m_revit.Application.ActiveUIDocument.Document
                                .GetElement(slab.GetTypeId()) as FloorType; // get element type
                        typeName = $"{slabType.Category.Name}: {slabType.Name}";
                        break;

                    case "Wall":
                        var wall = element as Wall;
                        var wallType =
                            m_revit.Application.ActiveUIDocument.Document
                                .GetElement(wall.GetTypeId()) as WallType; // get element type
                        typeName = $"{wallType.Kind}: {wallType.Name}";
                        break;
                }

                newRow["Id"] = idValue;
                newRow["Element Type"] = typeName;
                newRow["Support Type"] = supportInformation[0];
                newRow["Remark"] = supportInformation[1];
                informationTable.Rows.Add(newRow);
            }

            return informationTable;
        }

        private DataTable CreatDataTable()
        {
            var elementInformationTable = new DataTable("ElementInformationTable");

            var idColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Id",
                Caption = "Id",
                ReadOnly = true
            };
            elementInformationTable.Columns.Add(idColumn);

            var typeColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Element Type",
                Caption = "Element Type",
                ReadOnly = true
            };
            elementInformationTable.Columns.Add(typeColumn);

            // Create support column and add to the DataTable.
            var supportColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Support Type",
                Caption = "Support Type",
                ReadOnly = true
            };
            elementInformationTable.Columns.Add(supportColumn);

            var remarkColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Remark",
                Caption = "Remark",
                ReadOnly = true
            };
            elementInformationTable.Columns.Add(remarkColumn);

            return elementInformationTable;
        }

        private string[] GetSupportInformation(AnalyticalElement analyticalModel)
        {
            var supportInformations = new string[2] { "", "" };
            // AnalyticalModel Support list keeps track of all supports.
            supportInformations[0] = "not supported";

            return supportInformations;
        }
    }
}
