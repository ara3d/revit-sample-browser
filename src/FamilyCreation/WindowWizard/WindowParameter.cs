// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public class WindowParameter
    {
        private double m_height;
        private string m_type = string.Empty;
        private double m_width;

        public WindowParameter(bool isMetric)
        {
            if (isMetric)
            {
                m_type = "NewType";
                m_height = 1000;
                m_width = 500;
            }
            else
            {
                m_type = "NewType";
                m_height = 4.0;
                m_width = 2.0;
            }
        }

        public WindowParameter(WindowParameter para)
        {
            if (string.IsNullOrEmpty(para.m_type)) m_type = "NewType";
            m_type = $"{para.Type}1";
            m_height = para.Height;
            m_width = para.Width;
        }

        public string Type
        {
            set => m_type = value;
            get => m_type;
        }

        public double Height
        {
            set => m_height = value;
            get => m_height;
        }

        public double Width
        {
            set => m_width = value;
            get => m_width;
        }
    }

    public class WizardParameter
    {
        private WindowParameter m_curPara = new WindowParameter(true);
        private List<string> m_frameMats = new List<string>();
        private string m_glassMat = string.Empty;
        private List<string> m_glassMats = new List<string>();
        private string m_pathName = Path.GetTempPath();
        private string m_sashMat = string.Empty;

        public string Template = string.Empty;

        private ValidateWindowParameter m_validator = new ValidateWindowParameter(10, 10);
        private Hashtable m_winParas = new Hashtable();

        public ValidateWindowParameter Validator
        {
            get => m_validator;
            set => m_validator = value;
        }

        public List<string> FrameMaterials
        {
            set => m_frameMats = value;
            get => m_frameMats;
        }

        public List<string> GlassMaterials
        {
            set => m_glassMats = value;
            get => m_glassMats;
        }

        public string GlassMat
        {
            set => m_glassMat = value;
            get => m_glassMat;
        }

        public string SashMat
        {
            set => m_sashMat = value;
            get => m_sashMat;
        }

        public Hashtable WinParaTab
        {
            get => m_winParas;
            set => m_winParas = value;
        }

        public WindowParameter CurrentPara
        {
            get => m_curPara;
            set => m_curPara = value;
        }

        public string PathName
        {
            get => m_pathName;
            set => m_pathName = value;
        }
    }

    public class DoubleHungWinPara : WindowParameter
    {
        private double m_inset;
        private double m_sillHeight;

        public DoubleHungWinPara(bool isMetric)
            : base(isMetric)
        {
            if (isMetric)
            {
                m_inset = 20;
                m_sillHeight = 800;
            }
            else
            {
                m_inset = 0.05;
                m_sillHeight = 3;
            }
        }

        public DoubleHungWinPara(DoubleHungWinPara dbhungPara)
            : base(dbhungPara)
        {
            m_inset = dbhungPara.Inset;
            m_sillHeight = dbhungPara.SillHeight;
        }

        public double Inset
        {
            set => m_inset = value;
            get => m_inset;
        }

        public double SillHeight
        {
            set => m_sillHeight = value;
            get => m_sillHeight;
        }
    }
}
