// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;

namespace Ara3D.RevitSampleBrowser.PhysicalProp.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class DumpMaterialPhysicalParameters : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var res = Result.Succeeded;
            try
            {
                var activeDoc = commandData.Application.ActiveUIDocument;

                Material materialElement = null;

                ElementSet selection = new();
                foreach (var elementId in activeDoc.Selection.GetElementIds())
                {
                    selection.Insert(activeDoc.Document.GetElement(elementId));
                }

                if (selection.Size != 1)
                {
                    message = "Please select only one element.";
                    res = Result.Failed;
                    return res;
                }

                ElementSet es = new();
                foreach (var elementId in activeDoc.Selection.GetElementIds())
                {
                    es.Insert(activeDoc.Document.GetElement(elementId));
                }

                IEnumerator iter = es.ForwardIterator();
                iter.MoveNext();

                if (iter.Current is not FamilyInstance famIns)
                {
                    TaskDialog.Show("Revit", "Not a type of FamilyInsance!");
                    res = Result.Failed;
                    return res;
                }

                foreach (Parameter p in famIns.Parameters)
                {
                    var parName = p.Definition.Name;
                    // The "Beam Material" and "Column Material" family parameters have been replaced
                    // by the built-in parameter "Structural Material".
                    //if (parName == "Column Material" || parName == "Beam Material")
                    if (parName == "Structural Material")
                    {
                        var elemId = p.AsElementId();
                        materialElement = activeDoc.Document.GetElement(elemId) as Material;
                        break;
                    }
                }

                if (materialElement == null)
                {
                    TaskDialog.Show("Revit", "Not a column!");
                    res = Result.Failed;
                    return res;
                }

                var materialType =
                    materialElement.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_TYPE);

                var str =
                    $"Material type: {(materialType.AsInteger() == 0 ? "Generic" : materialType.AsInteger() == 1 ? "Concrete" : "Steel")}\r\n";

                if (materialType.AsInteger() > 0)
                {
                    var youngsModulus = new double[3];

                    youngsModulus[0] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD1).AsDouble();
                    youngsModulus[1] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD2).AsDouble();
                    youngsModulus[2] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD3).AsDouble();
                    str = $"{str}Young's modulus: {youngsModulus[0]},{youngsModulus[1]},{youngsModulus[2]}\r\n";

                    var poissonRatio = new double[3];

                    poissonRatio[0] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_POISSON_MOD1).AsDouble();
                    poissonRatio[1] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_POISSON_MOD2).AsDouble();
                    poissonRatio[2] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_POISSON_MOD3).AsDouble();
                    str = $"{str}Poisson modulus: {poissonRatio[0]},{poissonRatio[1]},{poissonRatio[2]}\r\n";

                    var shearModulus = new double[3];

                    shearModulus[0] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_MOD1).AsDouble();
                    shearModulus[1] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_MOD2).AsDouble();
                    shearModulus[2] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_MOD3).AsDouble();
                    str = $"{str}Shear modulus: {shearModulus[0]},{shearModulus[1]},{shearModulus[2]}\r\n";

                    var thermalExpCoeff = new double[3];

                    thermalExpCoeff[0] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_EXP_COEFF1).AsDouble();
                    thermalExpCoeff[1] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_EXP_COEFF2).AsDouble();
                    thermalExpCoeff[2] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_EXP_COEFF3).AsDouble();
                    str =
                        $"{str}Thermal expansion coefficient: {thermalExpCoeff[0]},{thermalExpCoeff[1]},{thermalExpCoeff[2]}\r\n";

                    var unitWeight = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_UNIT_WEIGHT).AsDouble();
                    str = $"{str}Unit weight: {unitWeight}\r\n";

                    var behaviour = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_BEHAVIOR).AsInteger();
                    str = $"{str}Behavior: {behaviour}\r\n";

                    if (materialType.AsInteger() == 1)
                    {
                        var concreteCompression = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_CONCRETE_COMPRESSION).AsDouble();
                        str = $"{str}Concrete compression: {concreteCompression}\r\n";

                        var lightWeight = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_LIGHT_WEIGHT).AsDouble();
                        str = $"{str}Lightweight: {lightWeight}\r\n";

                        var shearStrengthReduction = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_STRENGTH_REDUCTION).AsDouble();
                        str = $"{str}Shear strength reduction: {shearStrengthReduction}\r\n";
                    }
                    else if (materialType.AsInteger() == 2)
                    {
                        var minimumYieldStress = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_MINIMUM_YIELD_STRESS).AsDouble();
                        str = $"{str}Minimum yield stress: {minimumYieldStress}\r\n";

                        var minimumTensileStrength = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_MINIMUM_TENSILE_STRENGTH).AsDouble();
                        str = $"{str}Minimum tensile strength: {minimumTensileStrength}\r\n";

                        var reductionFactor = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_REDUCTION_FACTOR).AsDouble();
                        str = $"{str}Reduction factor: {reductionFactor}\r\n";
                    }
                }

                TaskDialog.Show("Physical materials", str);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("PhysicalProp", ex.Message);
                res = Result.Failed;
            }

            return res;
        }
    }
}
