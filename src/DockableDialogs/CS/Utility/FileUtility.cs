// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System.IO;
using System.Reflection;

namespace Revit.SDK.Samples.DockableDialogs.CS
{
    public class FileUtility
    {
        private static string sm_assemblyPath;
        private static string sm_assemblyFullName;
        private static string sm_appResourcePath;


        public static string GetAssemblyPath()
        {
            if (string.IsNullOrEmpty(sm_assemblyPath))
                sm_assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return sm_assemblyPath;
        }

        public static string GetAssemblyFullName()
        {
            if (string.IsNullOrEmpty(sm_assemblyFullName))
                sm_assemblyFullName = Assembly.GetExecutingAssembly().Location;
            return sm_assemblyFullName;
        }

        public static string GetApplicationResourcesPath()
        {
            if (string.IsNullOrEmpty(sm_appResourcePath))
                sm_appResourcePath = GetAssemblyPath() + "\\Resources\\";
            return sm_appResourcePath;
        }
    }
}
