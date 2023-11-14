// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.ComponentModel;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.FrameBuilder.CS
{
    /// <summary>
    ///     for control PropertyGrid to show and modify parameters of column, beam or brace
    /// </summary>
    public class FrameTypeParameters
    {
        private readonly Parameter m_bDimension; // parameter named b
        private readonly Parameter m_hDimension; // parameter named h

        /// <summary>
        ///     constructor without parameter is forbidden
        /// </summary>
        private FrameTypeParameters()
        {
        }

        /// <summary>
        ///     constructor used only for object factory
        /// </summary>
        /// <param name="symbol">FamilySymbol object has parameters</param>
        private FrameTypeParameters(FamilySymbol symbol)
        {
            // iterate and initialize parameters
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

        /// <summary>
        ///     parameter h in parameter category Dimension
        /// </summary>
        [Category("Dimensions")]
        public double H
        {
            get => m_hDimension.AsDouble();
            set => m_hDimension.Set(value);
        }

        /// <summary>
        ///     parameter b in parameter category Dimension
        /// </summary>
        [Category("Dimensions")]
        public double B
        {
            get => m_bDimension.AsDouble();
            set => m_bDimension.Set(value);
        }

        /// <summary>
        ///     object factory to create FramingTypeParameters;
        ///     will return null if necessary Parameters can't be found
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static FrameTypeParameters CreateInstance(FamilySymbol symbol)
        {
            var result = new FrameTypeParameters(symbol);
            if (null == result.m_bDimension || null == result.m_hDimension) return null;
            return result;
        }
    }
}
