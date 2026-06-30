// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.UI;

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

        private bool Creation()
        {
            return m_winCreator.Creation();
        }
    }
}
