// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.AreaReinCurve.CS
{
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
        public static bool SetParaInt(Element elem, string paraName, int value)
        {
            var paras = elem.Parameters;
            var findPara = FindParaByName(paras, paraName);

            if (null == findPara) return false;

            if (!findPara.IsReadOnly)
            {
                findPara.Set(value);
                return true;
            }

            return false;
        }

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

            if (!para.IsReadOnly)
            {
                para.Set(value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     set certain parameter of given element to int value
        /// </summary>
        /// <param name="elem">given element</param>
        /// <param name="paraIndex">BuiltInParameter</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetParaNullId(Parameter para)
        {
            var id = ElementId.InvalidElementId;

            if (!para.IsReadOnly)
            {
                para.Set(id);
                return true;
            }

            return false;
        }
    }
}
