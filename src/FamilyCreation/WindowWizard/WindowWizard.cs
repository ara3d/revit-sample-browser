// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public class WindowWizard
    {
        private readonly ExternalCommandData m_commandData;
        private WizardParameter m_para;
        private WindowCreation m_winCreator;

        public WindowWizard(ExternalCommandData commandData)
        {
            m_commandData = commandData;
        }

        public int RunWizard()
        {
            var result = 0;
            m_para = new WizardParameter
            {
                Template = "DoubleHung"
            };
            if (m_para.Template == "DoubleHung") m_winCreator = new DoubleHungWinCreation(m_para, m_commandData);
            using (WizardForm form = new(m_para))
            {
                result = form.ShowDialog() switch
                {
                    DialogResult.Cancel => 0,
                    DialogResult.OK => Creation() ? 1 : -1,
                    _ => -1,
                };
            }

            return result;
        }

        private bool Creation()
        {
            return m_winCreator.Creation();
        }
    }
}
