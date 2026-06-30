using System.IO;
using System.Reflection;

namespace ExcelExporterImporter
{
    internal static class B1Paths
    {
        public static string AssemblyFolder =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        public static string SharedFolder => Path.Combine(AssemblyFolder, "B1_Shared");

        public static string ResourcePath(string fileName) =>
            Path.Combine(SharedFolder, "Resources", fileName);

        public static string ParametersSettingsPath =>
            Path.Combine(SharedFolder, "ParametersSettings.xml");
    }
}
