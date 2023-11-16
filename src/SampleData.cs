using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser
{
    public class SampleData
    {
        public Type Type { get; }
        public string Namespace { get; } 
        public string Name { get; }
        public bool IsCommand => typeof(IExternalCommand).IsAssignableFrom(Type);
        public bool IsApplication => typeof(IExternalApplication).IsAssignableFrom(Type);
        public string FolderPath { get; }
        public string ReadmePath { get; }
        public bool FolderPathExists => Directory.Exists(FolderPath);

        public void Activate(ExternalCommandData commandData, UIControlledApplication uiControlledApplication)
        {
            if (IsCommand)
            {
                var command = Activator.CreateInstance(Type) as IExternalCommand;
                var message = "";
                var elementSet = new ElementSet();
                command?.Execute(commandData, ref message, elementSet);
            }
            else if (IsApplication)
            {
                var app = Activator.CreateInstance(Type) as IExternalApplication;
                app?.OnStartup(uiControlledApplication);
            }
        }

        public SampleData(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Namespace = type.Namespace ?? "";
            var prefix = "Ara3D.RevitSampleBrowser.";
            if (Namespace.StartsWith(prefix))
                Namespace = Namespace.Substring(prefix.Length);
            var suffix = ".CS";
            if (Namespace.EndsWith(suffix))
                Namespace = Namespace.Substring(0, Namespace.Length - suffix.Length);
            Name = Namespace + "." + Type.Name;

            var folderRelPath = Namespace.Replace(".", "\\");
            FolderPath = ThisFolderPath + "\\" + folderRelPath + @"\CS";
            if (!Directory.Exists(FolderPath))
            {
                FolderPath = ThisFolderPath + "\\" + folderRelPath + "\\..";
            }
            if (Directory.Exists(FolderPath))
            {
                ReadmePath = Directory.GetFiles(FolderPath, "*.rtf").FirstOrDefault();
                if (ReadmePath == null)
                {
                    Debug.WriteLine($"Could not find any rtf files in {FolderPath}");
                }
            }
            else
            {
                Debug.WriteLine($"Could not find folder {FolderPath}");
            }
        }

        private static readonly string ThisFolderPath = GetThisFolder();

        /// <summary>
        /// Fancy C# trick for getting the source file path. The "callerFilePath" parameter
        /// is automatically supplied by the compiler. 
        /// </summary>
        private static string GetThisFolder([CallerFilePath] string callerFilePath = null)
        {
            return new FileInfo(callerFilePath ?? "").DirectoryName;
        }
    }
}