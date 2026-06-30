// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public class WindowParameter
    {
        public WindowParameter(bool isMetric)
        {
            if (isMetric)
            {
                Type = "NewType";
                Height = 1000;
                Width = 500;
            }
            else
            {
                Type = "NewType";
                Height = 4.0;
                Width = 2.0;
            }
        }

        public WindowParameter(WindowParameter para)
        {
            if (string.IsNullOrEmpty(para.Type)) Type = "NewType";
            Type = $"{para.Type}1";
            Height = para.Height;
            Width = para.Width;
        }

        public string Type { set; get; } = string.Empty;

        public double Height { set; get; }

        public double Width { set; get; }
    }

    public class WizardParameter
    {
        public string Template = string.Empty;

        public ValidateWindowParameter Validator { get; set; } = new ValidateWindowParameter(10, 10);

        public List<string> FrameMaterials { set; get; } = [];

        public List<string> GlassMaterials { set; get; } = [];

        public string GlassMat { set; get; } = string.Empty;

        public string SashMat { set; get; } = string.Empty;

        public Hashtable WinParaTab { get; set; } = [];

        public WindowParameter CurrentPara { get; set; } = new WindowParameter(true);

        public string PathName { get; set; } = Path.GetTempPath();
    }

    public class DoubleHungWinPara : WindowParameter
    {
        public DoubleHungWinPara(bool isMetric)
            : base(isMetric)
        {
            if (isMetric)
            {
                Inset = 20;
                SillHeight = 800;
            }
            else
            {
                Inset = 0.05;
                SillHeight = 3;
            }
        }

        public DoubleHungWinPara(DoubleHungWinPara dbhungPara)
            : base(dbhungPara)
        {
            Inset = dbhungPara.Inset;
            SillHeight = dbhungPara.SillHeight;
        }

        public double Inset { set; get; }

        public double SillHeight { set; get; }
    }
}
