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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PhysicalProp.CS
{
    /// <summary>
    /// Define a command to dump physical material properties of 
    /// a structural element such as column, beam or brace. 
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
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

                var selection = new ElementSet();
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

                var es = new ElementSet();
                foreach (var elementId in activeDoc.Selection.GetElementIds())
                {
                   es.Insert(activeDoc.Document.GetElement(elementId));
                }
                System.Collections.IEnumerator iter = es.ForwardIterator();
                iter.MoveNext();

                // we need verify the selected element is a family instance
                var famIns = iter.Current as FamilyInstance;
                if (famIns == null)
                {
                    TaskDialog.Show("Revit", "Not a type of FamilyInsance!");
                    res = Result.Failed;
                    return res;
                }

                // we need select a column instance
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

                // the PHY_MATERIAL_PARAM_TYPE built in parameter contains a number 
                // that represents the type of material
                var materialType =
                    materialElement.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_TYPE);

                var str = "Material type: " +
                          (materialType.AsInteger() == 0 ?
                              "Generic" : (materialType.AsInteger() == 1 ? "Concrete" : "Steel")) + "\r\n";

                // A material type of more than 0 : 0 = Generic, 1 = Concrete, 2 = Steel
                if (materialType.AsInteger() > 0)
                {
                    // Common to all types

                    // Young's Modulus
                    var youngsModulus = new double[3];

                    youngsModulus[0] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD1).AsDouble();
                    youngsModulus[1] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD2).AsDouble();
                    youngsModulus[2] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD3).AsDouble();
                    str = str + "Young's modulus: " + youngsModulus[0].ToString() +
                        "," + youngsModulus[1].ToString() + "," + youngsModulus[2].ToString() +
                        "\r\n";

                    // Poisson Modulus
                    var PoissonRatio = new double[3];

                    PoissonRatio[0] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_POISSON_MOD1).AsDouble();
                    PoissonRatio[1] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_POISSON_MOD2).AsDouble();
                    PoissonRatio[2] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_POISSON_MOD3).AsDouble();
                    str = str + "Poisson modulus: " + PoissonRatio[0].ToString() +
                        "," + PoissonRatio[1].ToString() + "," + PoissonRatio[2].ToString() +
                        "\r\n";

                    // Shear Modulus
                    var shearModulus = new double[3];

                    shearModulus[0] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_MOD1).AsDouble();
                    shearModulus[1] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_MOD2).AsDouble();
                    shearModulus[2] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_MOD3).AsDouble();
                    str = str + "Shear modulus: " + shearModulus[0].ToString() +
                        "," + shearModulus[1].ToString() + "," + shearModulus[2].ToString() + "\r\n";

                    // Thermal Expansion Coefficient
                    var thermalExpCoeff = new double[3];

                    thermalExpCoeff[0] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_EXP_COEFF1).AsDouble();
                    thermalExpCoeff[1] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_EXP_COEFF2).AsDouble();
                    thermalExpCoeff[2] = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_EXP_COEFF3).AsDouble();
                    str = str + "Thermal expansion coefficient: " + thermalExpCoeff[0].ToString() +
                        "," + thermalExpCoeff[1].ToString() + "," + thermalExpCoeff[2].ToString() +
                        "\r\n";

                    // Unit Weight
                    var unitWeight = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_UNIT_WEIGHT).AsDouble();
                    str = str + "Unit weight: " + unitWeight.ToString() + "\r\n";

                    // Behavior 0 = Isotropic, 1 = Orthotropic
                    var behaviour = materialElement.get_Parameter(
                        BuiltInParameter.PHY_MATERIAL_PARAM_BEHAVIOR).AsInteger();
                    str = str + "Behavior: " + behaviour.ToString() + "\r\n";

                    // Concrete Only
                    if (materialType.AsInteger() == 1)
                    {
                        // Concrete Compression
                        var concreteCompression = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_CONCRETE_COMPRESSION).AsDouble();
                        str = str + "Concrete compression: " + concreteCompression.ToString() + "\r\n";

                        // Lightweight
                        var lightWeight = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_LIGHT_WEIGHT).AsDouble();
                        str = str + "Lightweight: " + lightWeight.ToString() + "\r\n";

                        // Shear Strength Reduction
                        var shearStrengthReduction = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_SHEAR_STRENGTH_REDUCTION).AsDouble();
                        str = str + "Shear strength reduction: " + shearStrengthReduction.ToString() + "\r\n";
                    }
                    // Steel only
                    else if (materialType.AsInteger() == 2)
                    {
                        // Minimum Yield Stress
                        var minimumYieldStress = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_MINIMUM_YIELD_STRESS).AsDouble();
                        str = str + "Minimum yield stress: " + minimumYieldStress.ToString() + "\r\n";

                        // Minimum Tensile Strength
                        var minimumTensileStrength = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_MINIMUM_TENSILE_STRENGTH).AsDouble();
                        str = str + "Minimum tensile strength: " +
                            minimumTensileStrength.ToString() + "\r\n";

                        // Reduction Factor
                        var reductionFactor = materialElement.get_Parameter(
                            BuiltInParameter.PHY_MATERIAL_PARAM_REDUCTION_FACTOR).AsDouble();
                        str = str + "Reduction factor: " + reductionFactor.ToString() + "\r\n";
                    } // end of if/else materialType.Integer == 1
                } // end if materialType.Integer > 0

                TaskDialog.Show("Physical materials", str);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("PhysicalProp", ex.Message);
                res = Result.Failed;
            }
            finally
            {
            }

            return res;
        } // end command
    } // end class
}