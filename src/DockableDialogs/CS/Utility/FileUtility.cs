//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


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