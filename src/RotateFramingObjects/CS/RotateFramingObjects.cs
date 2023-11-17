// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.RotateFramingObjects.CS
{
    /// <summary>
    ///     Rotate the objects that were selected when the command was executed.
    ///     and allow the user input the amount, in degrees that the objects should be rotated.
    ///     the dialog contain option for the user to specify this value is absolute or relative.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class RotateFramingObjects : IExternalCommand
    {
        private const string AngleDefinitionName = "Cross-Section Rotation";
        private UIApplication m_revit; // application of Revit

        /// <summary>
        ///     receive change of Angle
        /// </summary>
        public double ReceiveRotationTextBox { get; set; }

        /// <summary>
        ///     is moving absolutely
        /// </summary>
        public bool IsAbsoluteChecked { get; set; }

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var revit = commandData.Application;

            m_revit = revit;
            var displayForm = new RotateFramingObjectsForm(this);
            displayForm.StartPosition = FormStartPosition.CenterParent;
            var selection = new ElementSet();
            foreach (var elementId in revit.ActiveUIDocument.Selection.GetElementIds())
            {
                selection.Insert(revit.ActiveUIDocument.Document.GetElement(elementId));
            }

            var isSingle = true; //selection is single object
            var isAllFamilyInstance = true; //all is not familyInstance

            // There must be beams, braces or columns selected
            if (selection.IsEmpty)
            {
                // nothing selected
                message = "Please select some beams, braces or columns.";
                return Result.Failed;
            }

            if (1 != selection.Size)
            {
                isSingle = false;
                try
                {
                    if (DialogResult.OK != displayForm.ShowDialog()) return Result.Cancelled;
                }
                catch (Exception)
                {
                    return Result.Failed;
                }
                //    return Autodesk.Revit.UI.Result.Succeeded;
                // more than one object selected            
            }

            // if the selected elements are familyInstances, try to get their existing rotation
            foreach (Element e in selection)
            {
                if (e is FamilyInstance familyComponent)
                {
                    switch (familyComponent.StructuralType)
                    {
                        case StructuralType.Beam:
                        case StructuralType.Brace:
                        {
                            // selection is a beam or brace
                            var returnValue = FindParameter(AngleDefinitionName, familyComponent);
                            displayForm.RotationTextBox.Text = returnValue;
                            break;
                        }
                        case StructuralType.Column:
                        {
                            // selection is a column
                            var columnLocation = familyComponent.Location;
                            var pointLocation = columnLocation as LocationPoint;
                            var temp = pointLocation.Rotation;
                            var output = Math.Round(temp * 180 / Math.PI, 3).ToString();
                            displayForm.RotationTextBox.Text = output;
                            break;
                        }
                        default:
                            // other familyInstance can not be rotated
                            message = "It is not a beam, brace or column.";
                            elements.Insert(familyComponent);
                            return Result.Failed;
                    }
                }
                else
                {
                    if (isSingle)
                    {
                        message = "It is not a FamilyInstance.";
                        elements.Insert(e);
                        return Result.Failed;
                    }

                    // there is some objects is not familyInstance
                    message = "They are not FamilyInstances";
                    elements.Insert(e);
                    isAllFamilyInstance = false;
                }
            }

            if (isSingle)
                try
                {
                    if (DialogResult.OK != displayForm.ShowDialog()) return Result.Cancelled;
                }
                catch (Exception)
                {
                    return Result.Failed;
                }

            return isAllFamilyInstance ? Result.Succeeded : Result.Failed;
        }

        /// <summary>
        ///     The function set value to rotation of the beams and braces
        ///     and rotate columns.
        /// </summary>
        public void RotateElement()
        {
            var transaction = new Transaction(m_revit.ActiveUIDocument.Document, "RotateElement");
            transaction.Start();
            try
            {
                var selection = new ElementSet();
                foreach (var elementId in m_revit.ActiveUIDocument.Selection.GetElementIds())
                {
                    selection.Insert(m_revit.ActiveUIDocument.Document.GetElement(elementId));
                }

                foreach (Element e in selection)
                {
                    if (!(e is FamilyInstance familyComponent))
                        //is not a familyInstance
                        continue;
                    switch (familyComponent.StructuralType)
                    {
                        // if be familyInstance,judge the types of familyInstance
                        case StructuralType.Beam:
                        case StructuralType.Brace:
                        {
                            // selection is a beam or Brace
                            var paraIterator = familyComponent.Parameters.ForwardIterator();
                            paraIterator.Reset();

                            while (paraIterator.MoveNext())
                            {
                                var para = paraIterator.Current;
                                var objectAttribute = para as Parameter;
                                //set generic property named "Cross-Section Rotation"                           
                                if (objectAttribute.Definition.Name.Equals(AngleDefinitionName))
                                {
                                    var originDegree = objectAttribute.AsDouble();
                                    var rotateDegree = ReceiveRotationTextBox * Math.PI / 180;
                                    if (!IsAbsoluteChecked)
                                        // absolute rotation
                                        rotateDegree += originDegree;
                                    objectAttribute.Set(rotateDegree);
                                    // relative rotation
                                }
                            }

                            break;
                        }
                        case StructuralType.Column:
                        {
                            // rotate a column
                            var columnLocation = familyComponent.Location;
                            // get the location object
                            var pointLocation = columnLocation as LocationPoint;
                            var insertPoint = pointLocation.Point;
                            // get the location point
                            var temp = pointLocation.Rotation;
                            //existing rotation
                            var directionPoint = new XYZ(0, 0, 1);
                            // define the vector of axis
                            var rotateAxis = Line.CreateUnbound(insertPoint, directionPoint);
                            var rotateDegree = ReceiveRotationTextBox * Math.PI / 180;
                            // rotate column by rotate method
                            if (IsAbsoluteChecked) rotateDegree -= temp;
                            var rotateResult = pointLocation.Rotate(rotateAxis, rotateDegree);
                            if (rotateResult == false) TaskDialog.Show("Revit", "Rotate Failed.");
                            break;
                        }
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", "Rotate failed! " + ex.Message);
                transaction.RollBack();
            }
        }

        /// <summary>
        ///     get the parameter value according given parameter name
        /// </summary>
        public string FindParameter(string parameterName, FamilyInstance familyInstanceName)
        {
            var i = familyInstanceName.Parameters.ForwardIterator();
            i.Reset();
            string valueOfParameter = null;
            var iMoreAttribute = i.MoveNext();
            while (iMoreAttribute)
            {
                var isFound = false;
                var o = i.Current;
                var familyAttribute = o as Parameter;
                if (familyAttribute.Definition.Name == parameterName)
                {
                    //find the parameter whose name is same to the given parameter name 
                    var st = familyAttribute.StorageType;
                    switch (st)
                    {
                        //get the storage type
                        case StorageType.Double:
                            if (parameterName.Equals(AngleDefinitionName))
                            {
                                //make conversion between degrees and radians
                                var temp = familyAttribute.AsDouble();
                                valueOfParameter = Math.Round(temp * 180 / Math.PI, 3).ToString();
                            }
                            else
                            {
                                valueOfParameter = familyAttribute.AsDouble().ToString();
                            }

                            break;
                        case StorageType.ElementId:
                            //get Autodesk.Revit.DB.ElementId as string 
                            valueOfParameter = familyAttribute.AsElementId().ToString();
                            break;
                        case StorageType.Integer:
                            //get Integer as string
                            valueOfParameter = familyAttribute.AsInteger().ToString();
                            break;
                        case StorageType.String:
                            //get string 
                            valueOfParameter = familyAttribute.AsString();
                            break;
                        case StorageType.None:
                            valueOfParameter = familyAttribute.AsValueString();
                            break;
                    }

                    isFound = true;
                }

                if (isFound) break;
                iMoreAttribute = i.MoveNext();
            }

            //return the value.
            return valueOfParameter;
        }
    }
}
