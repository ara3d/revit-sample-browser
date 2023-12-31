// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.IO;
using System.Reflection;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.Utility
{
    public class FileUtility
    {
        private static string _smAssemblyPath;
        private static string _smAssemblyFullName;
        private static string _smAppResourcePath;

        public static string GetAssemblyPath()
        {
            if (string.IsNullOrEmpty(_smAssemblyPath))
                _smAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return _smAssemblyPath;
        }

        public static string GetAssemblyFullName()
        {
            if (string.IsNullOrEmpty(_smAssemblyFullName))
                _smAssemblyFullName = Assembly.GetExecutingAssembly().Location;
            return _smAssemblyFullName;
        }

        public static string GetApplicationResourcesPath()
        {
            if (string.IsNullOrEmpty(_smAppResourcePath))
                _smAppResourcePath = $"{GetAssemblyPath()}\\Resources\\";
            return _smAppResourcePath;
        }
    }
}
