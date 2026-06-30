using System;
using System.Diagnostics;
using System.IO;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser
{
    public class SampleData
    {
        private static readonly string ThisFolderPath = SampleBrowserUtils.GetSourceFolder();

        public Type Type { get; }
        public string Namespace { get; }
        public string Name { get; }
        public bool IsCommand => typeof(IExternalCommand).IsAssignableFrom(Type);
        public bool IsApplication => typeof(IExternalApplication).IsAssignableFrom(Type);
        public string FolderPath { get; }
        public string ReadmePath { get; }
        public bool FolderPathExists => Directory.Exists(FolderPath);

        public SampleData(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Namespace = SampleBrowserUtils.NormalizeSampleNamespace(type.Namespace);
            Name = $"{Namespace}.{Type.Name}";
            FolderPath = ResolveFolderPath(type);

            if (!Directory.Exists(FolderPath))
            {
                Debug.WriteLine($"Could not find folder {FolderPath}");
                return;
            }

            ReadmePath = FindMarkdownDoc(FolderPath, Type.Name);
            if (ReadmePath == null)
                Debug.WriteLine($"Could not find a markdown doc for {Name} under {FolderPath}");
        }

        private static string FindMarkdownDoc(string startFolder, string className)
        {
            for (var dir = startFolder;
                 !string.IsNullOrEmpty(dir) && dir.StartsWith(ThisFolderPath, StringComparison.OrdinalIgnoreCase);
                 dir = Path.GetDirectoryName(dir))
            {
                foreach (var md in Directory.GetFiles(dir, "*.md"))
                {
                    if (Path.GetFileName(md).StartsWith("_", StringComparison.Ordinal))
                        continue;

                    var text = File.ReadAllText(md);
                    if (text.Contains("**Class**") && text.Contains($"| `{className}` |"))
                        return md;
                }
            }

            return null;
        }

        private static string ResolveFolderPath(Type type)
        {
            var ns = SampleBrowserUtils.NormalizeSampleNamespace(type.Namespace);
            var defaultPath = Path.Combine(ThisFolderPath, ns.Replace(".", "\\"));
            if (Directory.Exists(defaultPath))
                return defaultPath;

            if (type.Namespace != null && type.Namespace.StartsWith("BuildingCoder"))
            {
                var suffix = type.Name.StartsWith("Cmd", StringComparison.Ordinal)
                    ? type.Name.Substring(3)
                    : type.Name;
                var tbcPath = Path.Combine(ThisFolderPath, "TBC_" + suffix);
                if (Directory.Exists(tbcPath))
                    return tbcPath;
            }

            if (type.Namespace != null && type.Namespace.StartsWith("Ara3D.Bowerbird.RevitSamples"))
            {
                var suffix = type.Name.StartsWith("Cmd", StringComparison.Ordinal)
                    ? type.Name.Substring(3)
                    : type.Name;
                var bbPath = Path.Combine(ThisFolderPath, "BB_" + suffix);
                if (Directory.Exists(bbPath))
                    return bbPath;
            }

            if (type.Namespace != null && type.Namespace.StartsWith("ExcelExporterImporter"))
            {
                var folder = type.Name switch
                {
                    "CmdExcelExport" => "B1_ExcelExport",
                    "CmdExcelImport" => "B1_ExcelImport",
                    _ => null
                };
                if (folder != null)
                {
                    var b1Path = Path.Combine(ThisFolderPath, folder);
                    if (Directory.Exists(b1Path))
                        return b1Path;
                }
            }

            return defaultPath;
        }

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
    }
}
