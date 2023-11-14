// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.RebarContainerAnyShapeType.CS
{
    /// <summary>
    ///     contain utility methods find or set certain parameter
    /// </summary>
    public class ParameterUtil
    {
        /// <summary>
        ///     find a parameter according to the parameter's name
        /// </summary>
        /// <param name="element">the host object of the parameter</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="value">the value of the parameter with integer type</param>
        /// <returns>if find the parameter return true</returns>
        public static bool SetParameter(Element element, string parameterName, int value)
        {
            var parameters = element.Parameters; //a set containing all of the parameters 
            //find a parameter according to the parameter's name
            var findParameter = FindParameter(parameters, parameterName);

            if (null == findParameter) return false;

            //judge whether the parameter is readonly before change its value
            if (!findParameter.IsReadOnly)
            {
                //judge whether the type of the value is the same as the parameter's
                var parameterType = findParameter.StorageType;
                if (StorageType.Integer != parameterType)
                    throw new Exception("The types of value and parameter are different!");
                findParameter.Set(value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     find a parameter according to the parameter's name
        /// </summary>
        /// <param name="element">the host object of the parameter</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="value">the value of the parameter with double type</param>
        /// <returns>if find the parameter return true</returns>
        public static bool SetParameter(Element element, string parameterName, double value)
        {
            var parameters = element.Parameters;
            var findParameter = FindParameter(parameters, parameterName);

            if (null == findParameter) return false;

            if (!findParameter.IsReadOnly)
            {
                var parameterType = findParameter.StorageType;
                if (StorageType.Double != parameterType)
                    throw new Exception("The types of value and parameter are different!");
                findParameter.Set(value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     find a parameter according to the parameter's name
        /// </summary>
        /// <param name="element">the host object of the parameter</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="value">the value of the parameter with string type</param>
        /// <returns>if find the parameter return true</returns>
        public static bool SetParameter(Element element, string parameterName, string value)
        {
            var parameters = element.Parameters;
            var findParameter = FindParameter(parameters, parameterName);

            if (null == findParameter) return false;

            if (!findParameter.IsReadOnly)
            {
                var parameterType = findParameter.StorageType;
                if (StorageType.String != parameterType)
                    throw new Exception("The types of value and parameter are different!");
                findParameter.Set(value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     find a parameter according to the parameter's name
        /// </summary>
        /// <param name="element">the host object of the parameter</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="value">the value of the parameter with Autodesk.Revit.DB.ElementId type</param>
        /// <returns>if find the parameter return true</returns>
        public static bool SetParameter(Element element, string parameterName, ref ElementId value)
        {
            var parameters = element.Parameters;
            var findParameter = FindParameter(parameters, parameterName);

            if (null == findParameter) return false;

            if (!findParameter.IsReadOnly)
            {
                var parameterType = findParameter.StorageType;
                if (StorageType.ElementId != parameterType)
                    throw new Exception("The types of value and parameter are different!");
                findParameter.Set(value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     set certain parameter of given element to int value
        /// </summary>
        /// <param name="element">given element</param>
        /// <param name="paraIndex">BuiltInParameter</param>
        /// <param name="value">the value of the parameter with integer type</param>
        /// <returns>if find the parameter return true</returns>
        public static bool SetParameter(Element element, BuiltInParameter paraIndex, int value)
        {
            //find a parameter according to the builtInParameter name
            var parameter = element.get_Parameter(paraIndex);
            if (null == parameter) return false;

            if (!parameter.IsReadOnly)
            {
                var parameterType = parameter.StorageType;
                if (StorageType.Integer != parameterType)
                    throw new Exception("The types of value and parameter are different!");
                parameter.Set(value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     find a parameter according to the parameter's name
        /// </summary>
        /// <param name="element">the host object of the parameter</param>
        /// <param name="paraIndex">parameter index</param>
        /// <param name="value">the value of the parameter with double type</param>
        /// <returns>if find the parameter return true</returns>
        public static bool SetParameter(Element element, BuiltInParameter paraIndex, double value)
        {
            var parameter = element.get_Parameter(paraIndex);
            if (null == parameter) return false;

            if (!parameter.IsReadOnly)
            {
                var parameterType = parameter.StorageType;
                if (StorageType.Double != parameterType)
                    throw new Exception("The types of value and parameter are different!");
                parameter.Set(value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     find a parameter according to the parameter's name
        /// </summary>
        /// <param name="element">the host object of the parameter</param>
        /// <param name="paraIndex">parameter index</param>
        /// <param name="value">the value of the parameter with string type</param>
        /// <returns>if find the parameter return true</returns>
        public static bool SetParameter(Element element, BuiltInParameter paraIndex, string value)
        {
            var parameter = element.get_Parameter(paraIndex);
            if (null == parameter) return false;

            if (!parameter.IsReadOnly)
            {
                var parameterType = parameter.StorageType;
                if (StorageType.String != parameterType)
                    throw new Exception("The types of value and parameter are different!");
                parameter.Set(value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     find a parameter according to the parameter's name
        /// </summary>
        /// <param name="element">the host object of the parameter</param>
        /// <param name="paraIndex">parameter index</param>
        /// <param name="value">the value of the parameter with Autodesk.Revit.DB.ElementId type</param>
        /// <returns>if find the parameter return true</returns>
        public static bool SetParameter(Element element,
            BuiltInParameter paraIndex, ref ElementId value)
        {
            var parameter = element.get_Parameter(paraIndex);
            if (null == parameter) return false;

            if (!parameter.IsReadOnly)
            {
                var parameterType = parameter.StorageType;
                if (StorageType.ElementId != parameterType)
                    throw new Exception("The types of value and parameter are different!");
                parameter.Set(value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     set null id to a parameter
        /// </summary>
        /// <param name="parameter">the parameter which wanted to change the value</param>
        /// <returns>if set parameter's value successful return true</returns>
        public static bool SetParaNullId(Parameter parameter)
        {
            var id = ElementId.InvalidElementId;

            if (!parameter.IsReadOnly)
            {
                parameter.Set(id);
                return true;
            }

            return false;
        }


        /// <summary>
        ///     find a parameter according to the parameter's name
        /// </summary>
        /// <param name="parameters">parameter set</param>
        /// <param name="name">parameter name</param>
        /// <returns>found parameter</returns>
        public static Parameter FindParameter(ParameterSet parameters, string name)
        {
            Parameter findParameter = null;

            foreach (Parameter parameter in parameters)
                if (parameter.Definition.Name == name)
                    findParameter = parameter;

            return findParameter;
        }
    }
}
