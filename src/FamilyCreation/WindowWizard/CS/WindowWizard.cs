// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace RevitMultiSample.WindowWizard.CS
{
    /// <summary>
    ///     The class is used to create window wizard form
    /// </summary>
    public class WindowWizard
    {
        /// <summary>
        ///     store the ExternalCommandData
        /// </summary>
        private readonly ExternalCommandData m_commandData;

        /// <summary>
        ///     store the WizardParameter
        /// </summary>
        private WizardParameter m_para;

        /// <summary>
        ///     store the WindowCreation
        /// </summary>
        private WindowCreation m_winCreator;

        /// <summary>
        ///     constructor of WindowWizard
        /// </summary>
        /// <param name="commandData">the ExternalCommandData parameter</param>
        public WindowWizard(ExternalCommandData commandData)
        {
            m_commandData = commandData;
        }

        /// <summary>
        ///     the method is used to show wizard form and do the creation
        /// </summary>
        /// <returns>the process result</returns>
        public int RunWizard()
        {
            var result = 0;
            m_para = new WizardParameter
            {
                Template = "DoubleHung"
            };
            if (m_para.Template == "DoubleHung") m_winCreator = new DoubleHungWinCreation(m_para, m_commandData);
            using (var form = new WizardForm(m_para))
            {
                switch (form.ShowDialog())
                {
                    case DialogResult.Cancel:
                        result = 0;
                        break;
                    case DialogResult.OK:
                        if (Creation())
                            result = 1;
                        else
                            result = -1;
                        break;
                    default:
                        result = -1;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        ///     The window creation process
        /// </summary>
        /// <returns>the result</returns>
        private bool Creation()
        {
            return m_winCreator.Creation();
        }
    }
}
