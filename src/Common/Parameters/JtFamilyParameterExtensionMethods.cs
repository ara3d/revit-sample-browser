// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Reflection;


namespace BuildingCoder
{
    public static class JtFamilyParameterExtensionMethods
    {
        public static bool IsShared(
            this FamilyParameter familyParameter)
        {
            var mi = familyParameter
                    .GetType()
                    .GetMethod("getParameter",
                        BindingFlags.Instance
                        | BindingFlags.NonPublic);

            if (null == mi)
                throw new InvalidOperationException(
                    "Could not find getParameter method");

            var parameter = mi.Invoke(familyParameter,
                new object[] { }) as Parameter;

            return parameter.IsShared;
        }
    }
}
