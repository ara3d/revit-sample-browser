// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Generate add-in manifest on the fly

        // Shared by Joshua Lumley in
        // https://thebuildingcoder.typepad.com/blog/2021/02/addin-file-learning-python-and-ifcjs.html#comment-5276653852
        private static void GenerateAddInManifest(
            string dll_folder,
            string dll_name)
        {
            var sDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\Autodesk\\Revit\\Addins";

            var exists = Directory.Exists(sDir);

            if (!exists)
                Directory.CreateDirectory(sDir);

            XElement xElementAddIn = new("AddIn",
                new XAttribute("Type", "Application"));

            xElementAddIn.Add(new XElement("Name", dll_name));
            xElementAddIn.Add(new XElement("Assembly", $"{dll_folder}{dll_name}.dll"));
            xElementAddIn.Add(new XElement("AddInId", Guid.NewGuid().ToString()));
            xElementAddIn.Add(new XElement("FullClassName", $"{dll_name}.SettingUpRibbon"));
            xElementAddIn.Add(new XElement("VendorId", "01"));
            xElementAddIn.Add(new XElement("VendorDescription", "Joshua Lumley Secrets, twitter @joshnewzealand"));

            XElement xElementRevitAddIns = new("RevitAddIns");
            xElementRevitAddIns.Add(xElementAddIn);

            foreach (var d in Directory.GetDirectories(sDir))
            {
                var myString_ManifestPath = $"{d}\\{dll_name}.addin";

                var directories = d.Split(Path.DirectorySeparatorChar);

                if (int.TryParse(directories[directories.Count() - 1],
                    out var myInt_FromTextBox))
                {
                    // Install on version 2017 and above

                    if (myInt_FromTextBox >= 2017)
                    {
                        new XDocument(xElementRevitAddIns).Save(
                            myString_ManifestPath);
                    }
                    else
                    {
                        if (File.Exists(myString_ManifestPath)) File.Delete(myString_ManifestPath);
                    }
                }
            }
        }

        #endregion
    }
}
