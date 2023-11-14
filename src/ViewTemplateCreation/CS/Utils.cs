// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Windows.Forms;

namespace Revit.SDK.Samples.ViewTemplateCreation.CS
{
    /// <summary>
    ///     Utils class contains useful methods and members for using in whole project.
    /// </summary>
    public class Utils
    {
        /// <summary>
        ///     Contains a name of this sample
        /// </summary>
        public const string SampleName = "View Template Creation sample";

        /// <summary>
        ///     Shows regular message box with warning icon and OK button
        /// </summary>
        public static void ShowWarningMessageBox(string message)
        {
            MessageBox.Show(
                message,
                SampleName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        ///     Shows regular message box with information icon and OK button
        /// </summary>
        public static void ShowInformationMessageBox(string message)
        {
            MessageBox.Show(
                message,
                SampleName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
