// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Units;
using Autodesk.Revit.DB;
using System;
using System.ComponentModel;
using System.Windows.Forms;
namespace Ara3D.RevitSampleBrowser.BoundaryConditions.CS
{
    public enum BcType
    {
        Point = 0,

        Line,

        Area
    }

    public enum BcState
    {
        Fixed = 0,

        Pinned,

        Roller,

        User
    }

    public enum BcTranslationRotation
    {
        Fixed = 0,

        Released,

        Spring
    }

    // Restricts decorated members to applicable boundary-condition types.
    public sealed class BcTypeAttribute : Attribute
    {
        // Keep a variable internally ...

        public BcTypeAttribute(BcType[] bcTypes)
        {
            BcType = bcTypes;
        }

        public BcType[] BcType { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not BcTypeAttribute temp) return false;

            foreach (var t1 in temp.BcType)
            {
                foreach (var t2 in BcType)
                {
                    if (t1 == t2)
                        return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class BcProperties
    {
        // the boundary conditions of which the information will displayed in the UI grid
        private readonly Autodesk.Revit.DB.Structure.BoundaryConditions m_bC;

        public BcProperties(Autodesk.Revit.DB.Structure.BoundaryConditions bC)
        {
            m_bC = bC;
        }

        [Category("Structural Analysis")]
        [ReadOnly(true)]
        [BcType(new[] { BcType.Point, BcType.Line, BcType.Area })]
        public BcType BoundaryConditionsType
        {
            get => (BcType)GetParameterValue("Boundary Conditions Type", GroupTypeId.StructuralAnalysis);
            set => SetParameterValue("Boundary Conditions Type", GroupTypeId.StructuralAnalysis, value);
        }

        [Category("Structural Analysis")]
        [BcType(new[] { BcType.Point, BcType.Line, BcType.Area })]
        public BcState State
        {
            get => (BcState)GetParameterValue("State", GroupTypeId.StructuralAnalysis);
            // Point BC includes Fixed, Pinned, Roller and User four different State; 
            // Line BC includes Fixed, Pinned and User; 
            // while the Area BC includes Pinned and User.
            set
            {
                switch (BoundaryConditionsType)
                {
                    case BcType.Area:
                        {
                            if (value is not BcState.Fixed and not BcState.Roller)
                                SetParameterValue("State", GroupTypeId.StructuralAnalysis, value);
                            break;
                        }
                    case BcType.Line:
                        {
                            if (BcState.Roller != value)
                            {
                                SetParameterValue("State", GroupTypeId.StructuralAnalysis, value);
                                ;
                            }

                            break;
                        }
                    case BcType.Point:
                        SetParameterValue("State", GroupTypeId.StructuralAnalysis, value);
                        ;
                        break;
                }

                // other parameters do corresponding change when the State is changed.
                MatchBcValuesRule();
            }
        }

        [Description("This value can be edit under User state")]
        [Category("Translation in")]
        [BcType(new[] { BcType.Point, BcType.Line, BcType.Area })]
        public BcTranslationRotation XTranslation
        {
            get => (BcTranslationRotation)GetParameterValue("X Translation", GroupTypeId.TranslationIn);
            set
            {
                if (BcState.User == State) SetParameterValue("X Translation", GroupTypeId.TranslationIn, value);
            }
        }

        [Category("Translation in")]
        public double XTranslationSpringModulus
        {
            get => (double)GetParameterValue("X Spring Modulus", GroupTypeId.TranslationIn);
            set => SetParameterValue("X Spring Modulus", GroupTypeId.TranslationIn, value);
        }

        [Description("This value can be edit under User state")]
        [Category("Translation in")]
        [BcType(new[] { BcType.Point, BcType.Line, BcType.Area })]
        public BcTranslationRotation YTranslation
        {
            get => (BcTranslationRotation)GetParameterValue("Y Translation", GroupTypeId.TranslationIn);
            set
            {
                if (BcState.User == State) SetParameterValue("Y Translation", GroupTypeId.TranslationIn, value);
            }
        }

        [Category("Translation in")]
        public double YTranslationSpringModulus
        {
            get => (double)GetParameterValue("Y Spring Modulus", GroupTypeId.TranslationIn);
            set => SetParameterValue("Y Spring Modulus", GroupTypeId.TranslationIn, value);
        }

        [Description("This value can be edit under User state")]
        [Category("Translation in")]
        [BcType(new[] { BcType.Point, BcType.Line, BcType.Area })]
        public BcTranslationRotation ZTranslation
        {
            get => (BcTranslationRotation)GetParameterValue("Z Translation", GroupTypeId.TranslationIn);
            set
            {
                if (BcState.User == State) SetParameterValue("Z Translation", GroupTypeId.TranslationIn, value);
            }
        }

        [Category("Translation in")]
        public double ZTranslationSpringModulus
        {
            get => (double)GetParameterValue("Z Spring Modulus", GroupTypeId.TranslationIn);
            set => SetParameterValue("Z Spring Modulus", GroupTypeId.TranslationIn, value);
        }

        [Description("This value can be edit under User state")]
        [Category("Rotation about")]
        [BcType(new[] { BcType.Point, BcType.Line })]
        public BcTranslationRotation XRotation
        {
            get => (BcTranslationRotation)GetParameterValue("X Rotation", GroupTypeId.RotationAbout);
            set
            {
                if (BcState.User == State) SetParameterValue("X Rotation", GroupTypeId.RotationAbout, value);
            }
        }

        [Category("Rotation about")]
        public double XRotationSpringModulus
        {
            get => (double)GetParameterValue("X Spring Modulus", GroupTypeId.RotationAbout);
            set => SetParameterValue("X Spring Modulus", GroupTypeId.RotationAbout, value);
        }

        [Description("This value can be edit under User state")]
        [Category("Rotation about")]
        [BcType(new[] { BcType.Point })]
        public BcTranslationRotation YRotation
        {
            get => (BcTranslationRotation)GetParameterValue("Y Rotation", GroupTypeId.RotationAbout);
            set
            {
                if (BcState.User == State) SetParameterValue("Y Rotation", GroupTypeId.RotationAbout, value);
            }
        }

        [Category("Rotation about")]
        public double YRotationSpringModulus
        {
            get => (double)GetParameterValue("Y Spring Modulus", GroupTypeId.RotationAbout);
            set => SetParameterValue("Y Spring Modulus", GroupTypeId.RotationAbout, value);
        }

        [Description("This value can be edit under User state")]
        [Category("Rotation about")]
        [BcType(new[] { BcType.Point })]
        public BcTranslationRotation ZRotation
        {
            get => (BcTranslationRotation)GetParameterValue("Z Rotation", GroupTypeId.RotationAbout);
            set
            {
                if (BcState.User == State) SetParameterValue("Z Rotation", GroupTypeId.RotationAbout, value);
            }
        }

        [Category("Rotation about")]
        public double ZRotationSpringModulus
        {
            get =>
                (double)GetParameterValue("Z Spring Modulus",
                    GroupTypeId.RotationAbout);
            set => SetParameterValue("Z Spring Modulus", GroupTypeId.RotationAbout, value);
        }

        // Spring modulus must be positive; unit conversion depends on BC type and axis.
        public void SetSpringModulus(string gridItemLabel)
        {
            using SpringModulusForm springModulusForm = new();
            switch (BoundaryConditionsType)
            {
                // judge current conversion rule between the display value and inside value
                case BcType.Point when gridItemLabel.Contains("Translation"):
                    springModulusForm.Conversion = UnitConversion.UnitDictionary["PTSpringModulusConver"];
                    break;
                case BcType.Point:
                    {
                        if (gridItemLabel.Contains("Rotation"))
                            springModulusForm.Conversion = UnitConversion.UnitDictionary["PRSpringModulusConver"];
                        break;
                    }
                case BcType.Line when gridItemLabel.Contains("Translation"):
                    springModulusForm.Conversion = UnitConversion.UnitDictionary["LTSpringModulusConver"];
                    break;
                case BcType.Line:
                    {
                        if (gridItemLabel.Contains("Rotation"))
                            springModulusForm.Conversion = UnitConversion.UnitDictionary["LRSpringModulusConver"];
                        break;
                    }
                case BcType.Area when gridItemLabel.Contains("Translation"):
                    springModulusForm.Conversion = UnitConversion.UnitDictionary["ATSpringModulusConver"];
                    break;
            }

            switch (gridItemLabel)
            {
                // get the old value
                case "XTranslation":
                    springModulusForm.OldStringModulus = XTranslationSpringModulus;
                    break;
                case "YTranslation":
                    springModulusForm.OldStringModulus = YTranslationSpringModulus;
                    break;
                case "ZTranslation":
                    springModulusForm.OldStringModulus = ZTranslationSpringModulus;
                    break;
                case "XRotation":
                    springModulusForm.OldStringModulus = XRotationSpringModulus;
                    break;
                case "YRotation":
                    springModulusForm.OldStringModulus = YRotationSpringModulus;
                    break;
                case "ZRotation":
                    springModulusForm.OldStringModulus = ZRotationSpringModulus;
                    break;
            }

            // show the spring modulus dialog to ask a positive number
            var result = springModulusForm.ShowDialog();

            // set the user input number as the new value
            if (DialogResult.OK == result)
            {
                switch (gridItemLabel)
                {
                    case "XTranslation":
                        XTranslationSpringModulus = springModulusForm.StringModulus;
                        break;
                    case "YTranslation":
                        YTranslationSpringModulus = springModulusForm.StringModulus;
                        break;
                    case "ZTranslation":
                        ZTranslationSpringModulus = springModulusForm.StringModulus;
                        break;
                    case "XRotation":
                        XRotationSpringModulus = springModulusForm.StringModulus;
                        break;
                    case "YRotation":
                        YRotationSpringModulus = springModulusForm.StringModulus;
                        break;
                    case "ZRotation":
                        ZRotationSpringModulus = springModulusForm.StringModulus;
                        break;
                }
            }
        }

        protected object GetParameterValue(string parameterName, ForgeTypeId parameterGroup)
        {
            var parameters = m_bC.Parameters;
            foreach (Parameter parameter in parameters)
            {
                // find the parameter of which the name is the same as the param name
                if (parameterName != parameter.Definition.Name ||
                    parameterGroup != parameter.Definition.GetGroupTypeId())
                    continue;

                // get the parameter value
                return parameter.StorageType switch
                {
                    StorageType.Double => parameter.AsDouble(),
                    StorageType.Integer => parameter.AsInteger(),
                    StorageType.ElementId => parameter.AsElementId(),
                    StorageType.String => parameter.AsString(),
                    StorageType.None => parameter.AsValueString(),
                    _ => null,
                };
            }

            return null;
        }

        protected void SetParameterValue(string parameterName,
            ForgeTypeId parameterGroup,
            object value)
        {
            var parameters = m_bC.Parameters;
            foreach (Parameter parameter in parameters)
            {
                // find the parameter of which the name is the same as the param name
                if (parameterName != parameter.Definition.Name ||
                    parameterGroup != parameter.Definition.GetGroupTypeId())
                    continue;

                // get the parameter value
                switch (parameter.StorageType)
                {
                    case StorageType.Double:
                        parameter.Set((double)value);
                        break;
                    case StorageType.Integer:
                        parameter.Set((int)value);
                        break;
                    case StorageType.ElementId:
                        var id = (ElementId)value;
                        parameter.Set(id);
                        break;
                    case StorageType.String:
                        parameter.Set(value as string);
                        break;
                    case StorageType.None:
                        parameter.SetValueString(value as string);
                        break;
                }
            }
        }

        // Mirrors Revit UI rules (e.g. Fixed state forces all translation/rotation to Fixed).
        private void MatchBcValuesRule()
        {
            switch (State)
            {
                case BcState.Fixed:
                    XTranslation = BcTranslationRotation.Fixed;
                    YTranslation = BcTranslationRotation.Fixed;
                    ZTranslation = BcTranslationRotation.Fixed;
                    XRotation = BcTranslationRotation.Fixed;
                    YRotation = BcTranslationRotation.Fixed;
                    ZRotation = BcTranslationRotation.Fixed;
                    break;
                case BcState.Pinned:
                    XTranslation = BcTranslationRotation.Fixed;
                    YTranslation = BcTranslationRotation.Fixed;
                    ZTranslation = BcTranslationRotation.Fixed;
                    ZRotation = BcTranslationRotation.Released;
                    YRotation = BcTranslationRotation.Released;
                    ZRotation = BcTranslationRotation.Released;
                    break;
                case BcState.Roller:
                    XTranslation = BcTranslationRotation.Released;
                    YTranslation = BcTranslationRotation.Released;
                    ZTranslation = BcTranslationRotation.Fixed;
                    XRotation = BcTranslationRotation.Released;
                    YRotation = BcTranslationRotation.Released;
                    ZRotation = BcTranslationRotation.Released;
                    break;
            }
        }
    }
}
