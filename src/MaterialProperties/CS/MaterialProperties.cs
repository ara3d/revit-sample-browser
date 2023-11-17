// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Data;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.MaterialProperties.CS
{
    /// <summary>
    ///     get the material physical properties of the selected beam, column or brace
    ///     get all material types and their sub types to the user and then change the material type of the selected beam to
    ///     the one chosen by the user
    ///     with a selected concrete beam, column or brace, change its unit weight to 145 P/ft3
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class MaterialProperties : IExternalCommand
    {
        private const double
            ToMetricUnitWeight = 0.010764; //coefficient of converting unit weight from public unit to metric unit 

        private const double
            ToMetricStress = 0.334554; //coefficient of converting stress from public unit to metric unit

        private const double
            ToImperialUnitWeight = 6.365827; //coefficient of converting unit weight from public unit to imperial unit

        private const double ChangedUnitWeight = 14.5; //the value of unit weight of selected component to be set

        private readonly Hashtable
            m_allMaterialMap = new Hashtable(); //hashtable contains all materials with index of their ElementId

        private Material m_cacheMaterial;
        private Parameter m_currentMaterial; //current material of selected beam, column or brace

        private UIApplication m_revit;
        private FamilyInstance m_selectedComponent; //selected beam, column or brace

        /// <summary>
        ///     get the material type of selected element
        /// </summary>
        public StructuralAssetClass CurrentType
        {
            get
            {
                var material = CurrentMaterial as Material;
                var materialId = ElementId.InvalidElementId;
                if (material != null) materialId = material.Id;
                if (materialId == ElementId.InvalidElementId) return StructuralAssetClass.Generic;
                var materialElem = (Material)m_allMaterialMap[materialId];
                return null == materialElem ? StructuralAssetClass.Generic : GetMaterialType(materialElem);
            }
        }

        /// <summary>
        ///     get the material attribute of selected element
        /// </summary>
        public object CurrentMaterial
        {
            get
            {
                m_cacheMaterial = GetCurrentMaterial();
                return m_cacheMaterial;
            }
        }

        /// <summary>
        ///     arraylist of all materials belonging to steel type
        /// </summary>
        public ArrayList SteelCollection { get; } = new ArrayList();

        /// <summary>
        ///     arraylist of all materials belonging to concrete type
        /// </summary>
        public ArrayList ConcreteCollection { get; } = new ArrayList();

        /// <summary>
        ///     three basic material types in Revit
        /// </summary>
        public ArrayList MaterialTypes
        {
            get
            {
                var typeAl = new ArrayList
                {
                    "Undefined",
                    "Basic",
                    "Generic",
                    "Metal",
                    "Concrete",
                    "Wood",
                    "Liquid",
                    "Gas",
                    "Plastic"
                };
                return typeAl;
            }
        }

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var revit = commandData.Application;
            m_revit = revit;
            if (!Init())
            {
                // there must be exactly one beam, column or brace selected
                TaskDialog.Show("Revit", "You should select only one beam, structural column or brace.");
                return Result.Failed;
            }

            var documentTransaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "Document");
            documentTransaction.Start();
            var displayForm = new MaterialPropertiesForm(this);
            try
            {
                displayForm.ShowDialog();
            }
            catch
            {
                TaskDialog.Show("Revit", "Sorry that your command failed.");
                return Result.Failed;
            }

            documentTransaction.Commit();
            return Result.Succeeded;
        }

        /// <summary>
        ///     get a datatable contains parameters' information of certain element
        /// </summary>
        /// <param name="o">Revit element</param>
        /// <param name="substanceKind">the material type of this element</param>
        /// <returns>datatable contains parameters' names and values</returns>
        public DataTable GetParameterTable(object o, StructuralAssetClass substanceKind)
        {
            //create an empty datatable
            var parameterTable = CreateTable();
            //if failed to convert object
            if (!(o is Material material)) return parameterTable;

            //- Behavior
            var temporaryAttribute =
                material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_BEHAVIOR); // hold each parameter
            switch (temporaryAttribute.AsInteger())
            {
                case 0:
                    AddDataRow(temporaryAttribute.Definition.Name, "Isotropic", parameterTable);
                    break;
                case 1:
                    AddDataRow(temporaryAttribute.Definition.Name, "Orthotropic", parameterTable);
                    break;
                default:
                    AddDataRow(temporaryAttribute.Definition.Name, "None", parameterTable);
                    break;
            }

            //- Young's Modulus
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD1);
            var temporaryValue = temporaryAttribute.AsValueString(); // hold each value
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD2);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD3);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);

            // - Poisson Modulus
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_POISSON_MOD1);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_POISSON_MOD2);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_POISSON_MOD3);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);

            // - Shear Modulus
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_MOD1);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_MOD2);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_MOD3);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);

            //- Thermal Expansion Coefficient
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_EXP_COEFF1);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_EXP_COEFF2);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_EXP_COEFF3);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);

            //- Unit Weight
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_UNIT_WEIGHT);
            temporaryValue = temporaryAttribute.AsValueString();
            AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);

            //- Bending Reinforcement
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_BENDING_REINFORCEMENT);
            if (null != temporaryAttribute)
            {
                temporaryValue = temporaryAttribute.AsValueString();
                AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            }

            //- Shear Reinforcement
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_REINFORCEMENT);
            if (null != temporaryAttribute)
            {
                temporaryValue = temporaryAttribute.AsValueString();
                AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            }

            //- Resistance Calc Strength
            temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_RESISTANCE_CALC_STRENGTH);
            if (null != temporaryAttribute)
            {
                temporaryValue = temporaryAttribute.AsValueString();
                AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
            }

            switch (substanceKind)
            {
                // For Steel only: 
                case StructuralAssetClass.Metal:
                    //- Minimum Yield Stress
                    temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_MINIMUM_YIELD_STRESS);
                    temporaryValue = temporaryAttribute.AsValueString();
                    AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);

                    //- Minimum Tensile Strength
                    temporaryAttribute =
                        material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_MINIMUM_TENSILE_STRENGTH);
                    temporaryValue = temporaryAttribute.AsValueString();
                    AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);

                    //- Reduction Factor
                    temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_REDUCTION_FACTOR);
                    temporaryValue = temporaryAttribute.AsValueString();
                    AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
                    break;
                // For Concrete only:
                case StructuralAssetClass.Concrete:
                    //- Concrete Compression     
                    temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_CONCRETE_COMPRESSION);
                    temporaryValue = temporaryAttribute.AsValueString();
                    AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);

                    //- Lightweight
                    temporaryAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_LIGHT_WEIGHT);
                    temporaryValue = temporaryAttribute.AsValueString();
                    AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);

                    //- Shear Strength Reduction
                    temporaryAttribute =
                        material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_STRENGTH_REDUCTION);
                    temporaryValue = temporaryAttribute.AsValueString();
                    AddDataRow(temporaryAttribute.Definition.Name, temporaryValue, parameterTable);
                    break;
            }

            return parameterTable;
        }

        /// <summary>
        ///     Update cache material
        /// </summary>
        /// <param name="obj">new material</param>
        public void UpdateMaterial(object obj)
        {
            if (null == obj) throw new ArgumentNullException();
            {
                m_cacheMaterial = obj as Material;
            }
        }

        /// <summary>
        ///     set the material of selected component
        /// </summary>
        public void SetMaterial()
        {
            if (null == m_cacheMaterial || null == m_currentMaterial) return;

            var identity = m_cacheMaterial.Id;
            m_currentMaterial.Set(identity);
        }

        /// <summary>
        ///     change unit weight of selected component to 14.50 kN/m3
        /// </summary>
        public bool ChangeUnitWeight()
        {
            var material = GetCurrentMaterial();
            if (material == null) return false;

            var weightPara = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_UNIT_WEIGHT);

            weightPara.Set(ChangedUnitWeight / ToMetricUnitWeight);

            return true;
        }

        /// <summary>
        ///     firstly, check whether only one beam, column or brace is selected
        ///     then initialize some member variables
        /// </summary>
        /// <returns>is the initialize successful</returns>
        private bool Init()
        {
            //selected 0 or more than 1 component
            if (m_revit.ActiveUIDocument.Selection.GetElementIds().Count != 1) return false;

            try
            {
                GetSelectedComponent();
                //selected component isn't beam, column or brace
                if (m_selectedComponent == null) return false;

                //initialize some member variables
                GetAllMaterial();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     get current material of selected component
        /// </summary>
        private Material GetCurrentMaterial()
        {
            if (null != m_cacheMaterial)
                return m_cacheMaterial;

            var identityValue = ElementId.InvalidElementId;
            if (m_currentMaterial != null)
                identityValue = m_currentMaterial.AsElementId(); //get the value of current material's ElementId
            //material has no value
            if (identityValue == ElementId.InvalidElementId) return null;
            var material = (Material)m_allMaterialMap[identityValue];

            return material;
        }

        /// <summary>
        ///     get selected beam, column or brace
        /// </summary>
        /// <returns></returns>
        private void GetSelectedComponent()
        {
            var componentCollection = new ElementSet();
            foreach (var elementId in m_revit.ActiveUIDocument.Selection.GetElementIds())
            {
                componentCollection.Insert(m_revit.ActiveUIDocument.Document.GetElement(elementId));
            }

            if (componentCollection.Size != 1) return;

            //if the selection is a beam, column or brace, find out its parameters for display
            foreach (var o in componentCollection)
            {
                if (!(o is FamilyInstance component)) continue;

                if (component.StructuralType == StructuralType.Beam
                    || component.StructuralType == StructuralType.Brace
                    || component.StructuralType == StructuralType.Column)
                    //get selected beam, column or brace
                    m_selectedComponent = component;

                //selection is a beam, column or brace, find out its parameters
                foreach (var p in component.Parameters)
                {
                    if (!(p is Parameter attribute)) continue;

                    var parameterName = attribute.Definition.Name;
                    // The "Beam Material" and "Column Material" family parameters have been replaced
                    // by the built-in parameter "Structural Material".
                    //if (parameterName == "Column Material" || parameterName == "Beam Material")
                    if (parameterName == "Structural Material")
                    {
                        //get current material of selected component
                        m_currentMaterial = attribute;
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     get all materials exist in current document
        /// </summary>
        /// <returns></returns>
        private void GetAllMaterial()
        {
            var collector = new FilteredElementCollector(m_revit.ActiveUIDocument.Document);
            var i = collector.OfClass(typeof(Material)).GetElementIterator();
            i.Reset();
            var moreValue = i.MoveNext();
            while (moreValue)
            {
                if (!(i.Current is Material material))
                {
                    moreValue = i.MoveNext();
                    continue;
                }

                //get the type of the material
                var materialType = GetMaterialType(material);

                //add materials to different ArrayList according to their types
                switch (materialType)
                {
                    case StructuralAssetClass.Metal:
                    {
                        SteelCollection.Add(new MaterialMap(material));
                        break;
                    }
                    case StructuralAssetClass.Concrete:
                    {
                        ConcreteCollection.Add(new MaterialMap(material));
                        break;
                    }
                }

                //map between materials and their elementId
                m_allMaterialMap.Add(material.Id, material);
                moreValue = i.MoveNext();
            }
        }

        /// <summary>
        ///     Create an empty table with parameter's name column and value column
        /// </summary>
        /// <returns></returns>
        private DataTable CreateTable()
        {
            // Create a new DataTable.
            var propDataTable = new DataTable("ParameterTable");

            // Create parameter column and add to the DataTable.
            var paraDataColumn = new DataColumn();
            paraDataColumn.DataType = Type.GetType("System.String");
            paraDataColumn.ColumnName = "Parameter";
            paraDataColumn.Caption = "Parameter";
            paraDataColumn.ReadOnly = true;
            // Add the column to the DataColumnCollection.
            propDataTable.Columns.Add(paraDataColumn);

            // Create value column and add to the DataTable.
            var valueDataColumn = new DataColumn();
            valueDataColumn.DataType = Type.GetType("System.String");
            valueDataColumn.ColumnName = "Value";
            valueDataColumn.Caption = "Value";
            valueDataColumn.ReadOnly = false;
            propDataTable.Columns.Add(valueDataColumn);

            return propDataTable;
        }

        /// <summary>
        ///     add one row to datatable of parameter
        /// </summary>
        /// <param name="parameterName">name of parameter</param>
        /// <param name="parameterValue">value of parameter</param>
        /// <param name="parameterTable">datatable to be added row</param>
        private void AddDataRow(string parameterName, string parameterValue, DataTable parameterTable)
        {
            var newRow = parameterTable.NewRow();
            newRow["Parameter"] = parameterName;
            newRow["Value"] = parameterValue;
            parameterTable.Rows.Add(newRow);
        }

        /// <summary>
        ///     Get the material type via giving material.
        ///     According to my knowledge, the material type can be retrieved by two ways now:
        ///     1. If the PropertySetElement exists, retrieve it by PHY_MATERIAL_PARAM_CLASS parameter. (via PropertySetElement
        ///     class)
        ///     2. If it's indenpendent, retrieve it by PHY_MATERIAL_PARAM_TYPE parameter(via Material class)
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private StructuralAssetClass GetMaterialType(Material material)
        {
            if (material.StructuralAssetId != ElementId.InvalidElementId)
            {
                var propElem =
                    m_revit.ActiveUIDocument.Document.GetElement(material.StructuralAssetId) as PropertySetElement;
                var propElemPara = propElem.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_CLASS);
                if (propElemPara != null) return (StructuralAssetClass)propElemPara.AsInteger();
            }

            return StructuralAssetClass.Undefined;
        }
    }

    /// <summary>
    ///     assistant class contains material and its name
    /// </summary>
    public class MaterialMap
    {
        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="material"></param>
        public MaterialMap(Material material)
        {
            MaterialName = material.Name;
            Material = material;
        }

        /// <summary>
        ///     Get the material name
        /// </summary>
        public string MaterialName { get; }

        /// <summary>
        ///     Get the material
        /// </summary>
        public Material Material { get; }
    }
}
