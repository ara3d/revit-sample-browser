#region Namespaces

using System.Diagnostics;
using System.IO;
using System.Text;
using Autodesk.Revit.ApplicationServices;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PartAtom sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Faster ExtractPartAtom reimplementation,
        ///     independent of Revit API, for standalone
        ///     external use. By Hеvard Dagsvik, Symetri.
        /// </summary>
        internal static string GetFamilyXmlData(
            string family_file_path)
        {
            var array = File.ReadAllBytes(family_file_path);

            var string_file = Encoding.UTF8.GetString(array);

            string xml_data = null;

            var start = string_file.IndexOf("<entry");

            if (start == -1)
            {
                Debug.Print($"XML start not detected: {family_file_path}");
            }
            else
            {
                var end = string_file.IndexOf("/entry>");

                if (end == -1)
                {
                    Debug.Print($"XML end not detected: {family_file_path}");
                }
                else
                {
                    end = end + 7;

                    var length = end - start;

                    if (length <= 0)
                        Debug.Print($"XML length is 0 or less: {family_file_path}");
                    else
                        xml_data = string_file.Substring(
                            start, length);
                }
            }

            return xml_data;
        }

        internal static void CreatePartAtomFile(
            Application app,
            string rfaFilePath,
            string partAtomFilePath)
        {
            app.ExtractPartAtomFromFamilyFile(
                rfaFilePath,
                partAtomFilePath);
        }
    }
}
