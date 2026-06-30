// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.ComponentModel;

namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    public class FrameTypeParameters
    {
        private readonly Parameter m_bDimension;
        private readonly Parameter m_hDimension;

        private FrameTypeParameters()
        {
        }

        private FrameTypeParameters(FamilySymbol symbol)
        {
            foreach (Parameter para in symbol.Parameters)
            {
                switch (para.Definition.Name)
                {
                    case "h":
                        m_hDimension = para;
                        continue;
                    case "b":
                        m_bDimension = para;
                        break;
                }
            }
        }

        [Category("Dimensions")]
        public double H
        {
            get => m_hDimension.AsDouble();
            set => m_hDimension.Set(value);
        }

        [Category("Dimensions")]
        public double B
        {
            get => m_bDimension.AsDouble();
            set => m_bDimension.Set(value);
        }

        // Returns null when the symbol lacks "h" and "b" dimension parameters.
        public static FrameTypeParameters CreateInstance(FamilySymbol symbol)
        {
            FrameTypeParameters result = new(symbol);
            return null == result.m_bDimension || null == result.m_hDimension ? null : result;
        }
    }
}
