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

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Revit.SDK.Samples.WindowWizard.CS
{
    /// <summary>
    ///     This class will deal with all parameters related to window creation
    /// </summary>
    public class WindowParameter
    {
        /// <summary>
        ///     store the height of opening
        /// </summary>
        private double m_height;

        /// <summary>
        ///     store the family type name
        /// </summary>
        private string m_type = string.Empty;

        /// <summary>
        ///     store the width of opening
        /// </summary>
        private double m_width;

        /// <summary>
        ///     constructor of WindowParameter
        /// </summary>
        /// <param name="isMetric">indicate whether the template is metric or imperial</param>
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

        /// <summary>
        ///     construcion of WindowParameter
        /// </summary>
        /// <param name="para">the WindowParameter</param>
        public WindowParameter(WindowParameter para)
        {
            if (string.IsNullOrEmpty(para.m_type)) m_type = "NewType";
            m_type = para.Type + "1";
            m_height = para.Height;
            m_width = para.Width;
        }

        /// <summary>
        ///     get/set the Type property
        /// </summary>
        public string Type
        {
            set => m_type = value;
            get => m_type;
        }

        /// <summary>
        ///     get/set the Height property
        /// </summary>
        public double Height
        {
            set => m_height = value;
            get => m_height;
        }

        /// <summary>
        ///     get/set the Width property
        /// </summary>
        public double Width
        {
            set => m_width = value;
            get => m_width;
        }
    }

    /// <summary>
    ///     This class is used to deal with wizard parameters
    /// </summary>
    public class WizardParameter
    {
        /// <summary>
        ///     store the current WindowParameter
        /// </summary>
        private WindowParameter m_curPara = new WindowParameter(true);

        /// <summary>
        ///     store the frame material list
        /// </summary>
        private List<string> m_frameMats = new List<string>();

        /// <summary>
        ///     store the glass material
        /// </summary>
        private string m_glassMat = string.Empty;

        /// <summary>
        ///     store the glass material list
        /// </summary>
        private List<string> m_GlassMats = new List<string>();

        /// <summary>
        ///     store the temp path
        /// </summary>
        private string m_pathName = Path.GetTempPath();

        /// <summary>
        ///     store the sash material
        /// </summary>
        private string m_sashMat = string.Empty;
        // ToDo add properties for them

        /// <summary>
        ///     store the template name
        /// </summary>
        public string m_template = string.Empty;

        /// <summary>
        ///     store the ValidateWindowParameter
        /// </summary>
        private ValidateWindowParameter m_validator = new ValidateWindowParameter(10, 10);

        /// <summary>
        ///     store the windowparameter hashtable
        /// </summary>
        private Hashtable m_winParas = new Hashtable();

        /// <summary>
        ///     get/set Validator property
        /// </summary>
        public ValidateWindowParameter Validator
        {
            get => m_validator;
            set => m_validator = value;
        }

        /// <summary>
        ///     get/set FrameMaterials property
        /// </summary>
        public List<string> FrameMaterials
        {
            set => m_frameMats = value;
            get => m_frameMats;
        }

        /// <summary>
        ///     get/set GlassMaterials property
        /// </summary>
        public List<string> GlassMaterials
        {
            set => m_GlassMats = value;
            get => m_GlassMats;
        }

        /// <summary>
        ///     get/set GlassMat property
        /// </summary>
        public string GlassMat
        {
            set => m_glassMat = value;
            get => m_glassMat;
        }

        /// <summary>
        ///     get/set SashMat property
        /// </summary>
        public string SashMat
        {
            set => m_sashMat = value;
            get => m_sashMat;
        }

        /// <summary>
        ///     get/set WinParaTab property
        /// </summary>
        public Hashtable WinParaTab
        {
            get => m_winParas;
            set => m_winParas = value;
        }

        /// <summary>
        ///     get/set CurrentPara property
        /// </summary>
        public WindowParameter CurrentPara
        {
            get => m_curPara;
            set => m_curPara = value;
        }

        /// <summary>
        ///     get/set PathName property
        /// </summary>
        public string PathName
        {
            get => m_pathName;
            set => m_pathName = value;
        }
    }

    /// <summary>
    ///     This class inherits from WindowParameter
    /// </summary>
    public class DoubleHungWinPara : WindowParameter
    {
        /// <summary>
        ///     store the m_inset
        /// </summary>
        private double m_inset;

        /// <summary>
        ///     store the m_sillHeight
        /// </summary>
        private double m_sillHeight;

        /// <summary>
        ///     constructor of DoubleHungWinPara
        /// </summary>
        /// <param name="isMetric">indicate whether the template is metric of imperial</param>
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

        /// <summary>
        ///     constructor of DoubleHungWinPara
        /// </summary>
        /// <param name="dbhungPara">DoubleHungWinPara</param>
        public DoubleHungWinPara(DoubleHungWinPara dbhungPara)
            : base(dbhungPara)
        {
            m_inset = dbhungPara.Inset;
            m_sillHeight = dbhungPara.SillHeight;
        }

        /// <summary>
        ///     set/get Inset property
        /// </summary>
        public double Inset
        {
            set => m_inset = value;
            get => m_inset;
        }

        /// <summary>
        ///     set/get SillHeight property
        /// </summary>
        public double SillHeight
        {
            set => m_sillHeight = value;
            get => m_sillHeight;
        }
    }
}