using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static string GetCloudModelLocalCacheFilepath(
            Document doc,
            string version_number)
        {
            var title = doc.Title;
            var ext = Path.GetExtension(doc.PathName);
            string localRevitFile = null;

            if (doc.IsModelInCloud)
            {
                var modelPath = doc.GetCloudModelPath();
                var guid = modelPath.GetModelGUID().ToString();

                var folder = $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Autodesk\\Revit\\Autodesk Revit {version_number}\\CollaborationCache";

                var revitFile = guid + ext;

                var files = Directory
                    .GetFiles(folder, revitFile, SearchOption.AllDirectories)
                    .Where(c => !c.Contains("CentralCache"))
                    .ToArray();

                if (0 < files.Length)
                    localRevitFile = files[0];
                else
                    Debug.Print($"Unable to find local rvt for: {doc.PathName}");
            }
            else
            {
                localRevitFile = doc.PathName;
            }

            return localRevitFile;
        }
    }
}
