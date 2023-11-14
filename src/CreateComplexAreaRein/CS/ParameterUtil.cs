// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;

namespace RevitMultiSample.CreateComplexAreaRein.CS
{
    /// <summary>
    ///     enum of AreaReinforcement's parameter Layout Rules
    /// </summary>
    public enum LayoutRules
    {
        FixedNumber = 2,
        MaximumSpacing = 3
    }

    /// <summary>
    ///     enum of AreaReinforcementCurve's parameter Hook Orientation
    /// </summary>
    public enum HookOrientation
    {
        Up = 0,
        Down = 2
    }

    /// <summary>
    ///     contain utility methods find or set certain parameter
    /// </summary>
    public class ParameterUtil
    {
        /// <summary>
        ///     find certain parameter in a set
        /// </summary>
        /// <param name="paras"></param>
        /// <param name="name">find by name</param>
        /// <returns>found parameter</returns>
        public static Parameter FindParaByName(ParameterSet paras, string name)
        {
            Parameter findPara = null;

            foreach (Parameter para in paras)
                if (para.Definition.Name == name)
                    findPara = para;

            return findPara;
        }

        /// <summary>
        ///     set certain parameter of given element to int value
        /// </summary>
        /// <param name="elem">given element</param>
        /// <param name="paraIndex">BuiltInParameter</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetParaInt(Element elem, BuiltInParameter paraIndex, int value)
        {
            var para = elem.get_Parameter(paraIndex);
            if (null == para) return false;

            para.Set(value);
            return true;
        }
    }
}
