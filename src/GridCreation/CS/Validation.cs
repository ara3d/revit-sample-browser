// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Resources;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.GridCreation.CS.Properties;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    /// <summary>
    ///     Class to validate input data before creating grids
    /// </summary>
    public static class Validation
    {
        // Get the resource contains strings
        private static readonly ResourceManager ResManager = Resources.ResourceManager;

        /// <summary>
        ///     Validate numbers in UI
        /// </summary>
        /// <param name="number1Ctrl">Control contains number information</param>
        /// <param name="number2Ctrl">Control contains another number information</param>
        /// <returns>Whether the numbers are validated</returns>
        public static bool ValidateNumbers(Control number1Ctrl, Control number2Ctrl)
        {
            if (!ValidateNumber(number1Ctrl) || !ValidateNumber(number2Ctrl)) return false;

            if (Convert.ToUInt32(number1Ctrl.Text) == 0 && Convert.ToUInt32(number2Ctrl.Text) == 0)
            {
                ShowWarningMessage(ResManager.GetString("NumbersCannotBeBothZero"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                number1Ctrl.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validate number value
        /// </summary>
        /// <param name="numberCtrl">Control contains number information</param>
        /// <returns>Whether the number value is validated</returns>
        public static bool ValidateNumber(Control numberCtrl)
        {
            if (!ValidateNotNull(numberCtrl, "Number")) return false;

            try
            {
                var number = Convert.ToUInt32(numberCtrl.Text);
                if (number > 200)
                {
                    ShowWarningMessage(ResManager.GetString("NumberBetween0And200"),
                        Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                    numberCtrl.Focus();
                    return false;
                }
            }
            catch (OverflowException)
            {
                ShowWarningMessage(ResManager.GetString("NumberBetween0And200"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                numberCtrl.Focus();
                return false;
            }
            catch (Exception)
            {
                ShowWarningMessage(ResManager.GetString("NumberFormatWrong"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                numberCtrl.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validate length value
        /// </summary>
        /// <param name="lengthCtrl">Control contains length information</param>
        /// <param name="typeName">Type of length</param>
        /// <param name="canBeZero">Whether the length can be zero</param>
        /// <returns>Whether the length value is validated</returns>
        public static bool ValidateLength(Control lengthCtrl, string typeName, bool canBeZero)
        {
            if (!ValidateNotNull(lengthCtrl, typeName)) return false;

            try
            {
                var length = Convert.ToDouble(lengthCtrl.Text);
                if (length <= 0 && !canBeZero)
                {
                    ShowWarningMessage(ResManager.GetString($"{typeName}CannotBeNegativeOrZero"),
                        Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                    lengthCtrl.Focus();
                    return false;
                }

                if (length < 0 && canBeZero)
                {
                    ShowWarningMessage(ResManager.GetString($"{typeName}CannotBeNegative"),
                        Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                    lengthCtrl.Focus();
                    return false;
                }
            }
            catch (Exception)
            {
                ShowWarningMessage(ResManager.GetString($"{typeName}FormatWrong"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                lengthCtrl.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validate coordinate value
        /// </summary>
        /// <param name="coordCtrl">Control contains coordinate information</param>
        /// <returns>Whether the coordinate value is validated</returns>
        public static bool ValidateCoord(Control coordCtrl)
        {
            if (!ValidateNotNull(coordCtrl, "Coordinate")) return false;

            try
            {
                Convert.ToDouble(coordCtrl.Text);
            }
            catch (Exception)
            {
                ShowWarningMessage(ResManager.GetString("CoordinateFormatWrong"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                coordCtrl.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validate start degree and end degree
        /// </summary>
        /// <param name="startDegree">Control contains start degree information</param>
        /// <param name="endDegree">Control contains end degree information</param>
        /// <returns>Whether the degree values are validated</returns>
        public static bool ValidateDegrees(Control startDegree, Control endDegree)
        {
            if (!ValidateDegree(startDegree) || !ValidateDegree(endDegree)) return false;

            if (Math.Abs(Convert.ToDouble(startDegree.Text) - Convert.ToDouble(endDegree.Text)) <= double.Epsilon)
            {
                ShowWarningMessage(ResManager.GetString("DegreesAreTooClose"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                startDegree.Focus();
                return false;
            }

            if (Convert.ToDouble(startDegree.Text) >= Convert.ToDouble(endDegree.Text))
            {
                ShowWarningMessage(ResManager.GetString("StartDegreeShouldBeLessThanEndDegree"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                startDegree.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validate degree value
        /// </summary>
        /// <param name="degreeCtrl">Control contains degree information</param>
        /// <returns>Whether the degree value is validated</returns>
        public static bool ValidateDegree(Control degreeCtrl)
        {
            if (!ValidateNotNull(degreeCtrl, "Degree")) return false;

            try
            {
                var startDegree = Convert.ToDouble(degreeCtrl.Text);
                if (startDegree < 0 || startDegree > 360)
                {
                    ShowWarningMessage(ResManager.GetString("DegreeWithin0To360"),
                        Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                    degreeCtrl.Focus();
                    return false;
                }
            }
            catch (Exception)
            {
                ShowWarningMessage(ResManager.GetString("DegreeFormatWrong"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                degreeCtrl.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validate label
        /// </summary>
        /// <param name="labelCtrl">Control contains label information</param>
        /// <param name="allLabels">List contains all labels in Revit document</param>
        /// <returns>Whether the label value is validated</returns>
        public static bool ValidateLabel(Control labelCtrl, ArrayList allLabels)
        {
            if (!ValidateNotNull(labelCtrl, "Label")) return false;

            var labelToBeValidated = labelCtrl.Text;
            foreach (string label in allLabels)
            {
                if (label == labelToBeValidated)
                {
                    ShowWarningMessage(ResManager.GetString("LabelExisted"),
                        Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                    labelCtrl.Focus();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Assure value is not null
        /// </summary>
        /// <param name="control">Control contains information needs to be checked</param>
        /// <param name="typeName">Type of information</param>
        /// <returns>Whether the value is not null</returns>
        public static bool ValidateNotNull(Control control, string typeName)
        {
            if (string.IsNullOrEmpty(control.Text.TrimStart(' ').TrimEnd(' ')))
            {
                ShowWarningMessage(ResManager.GetString($"{typeName}CannotBeNull"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                control.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Assure two labels are not same
        /// </summary>
        /// <param name="label1Ctrl">Control contains label information</param>
        /// <param name="label2Ctrl">Control contains label information</param>
        /// <returns>Whether the labels are same</returns>
        public static bool ValidateLabels(Control label1Ctrl, Control label2Ctrl)
        {
            if (label1Ctrl.Text.TrimStart(' ').TrimEnd(' ') == label2Ctrl.Text.TrimStart(' ').TrimEnd(' '))
            {
                ShowWarningMessage(ResManager.GetString("LabelsCannotBeSame"),
                    Resources.ResourceManager.GetString("FailureCaptionInvalidValue"));
                label1Ctrl.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Show a warning message box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="caption">title of message box</param>
        public static void ShowWarningMessage(string message, string caption)
        {
            TaskDialog.Show(caption, message, TaskDialogCommonButtons.Ok);
        }
    }
}
