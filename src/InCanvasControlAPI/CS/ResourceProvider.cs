// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System.IO;
using System.Reflection;

namespace Revit.SDK.Samples.InCanvasControlAPI.CS
{
    /// <summary>
    ///     Provider for string resources
    /// </summary>
    public class ResourceProvider
    {
        private static ResourceProvider provider;

        private ResourceProvider()
        {
            IssueImage = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "\\issue.bmp";
            SelectedIssueImage =
                new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "\\selected.bmp";
        }

        /// <summary>
        ///     Path to marker's bitmap for unselected issues
        /// </summary>
        public string IssueImage { get; }

        /// <summary>
        ///     Path to marker's bitmap for selected issues
        /// </summary>
        public string SelectedIssueImage { get; }

        /// <summary>
        ///     Gets the string resource provider
        /// </summary>
        /// <returns>Instance of the provider</returns>
        public static ResourceProvider GetInstance()
        {
            return provider ?? (provider = new ResourceProvider());
        }
    }
}
