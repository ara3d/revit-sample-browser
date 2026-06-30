// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using System.IO;
using System.Reflection;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class AssemblyPathHelper
    {
        private static string _assemblyPath;
        private static string _assemblyFullName;
        private static string _appResourcePath;

        public static string GetAssemblyPath()
        {
            if (string.IsNullOrEmpty(_assemblyPath))
                _assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return _assemblyPath;
        }

        public static string GetAssemblyFullName()
        {
            if (string.IsNullOrEmpty(_assemblyFullName))
                _assemblyFullName = Assembly.GetExecutingAssembly().Location;
            return _assemblyFullName;
        }

        public static string GetApplicationResourcesPath()
        {
            if (string.IsNullOrEmpty(_appResourcePath))
                _appResourcePath = $"{GetAssemblyPath()}\\Resources\\";
            return _appResourcePath;
        }

        public static string ResolveDirectoryPath(string path)
        {
            return Path.IsPathRooted(path) ? path : Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
        }

    }
}