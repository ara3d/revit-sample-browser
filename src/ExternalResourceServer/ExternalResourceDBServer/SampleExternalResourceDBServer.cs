// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.ExternalResourceServer.ExternalResourceDBServer.CS
{
    /// <summary>
    /// Sample external resource server providing keynote data and Revit links via a browsable folder tree.
    /// French/German keynotes come from a fictitious database; other locales and links use files under
    /// SampleResourceServerRoot next to this assembly.
    /// </summary>
    public class SampleExternalResourceDbServer : IExternalResourceServer
    {
        public SampleExternalResourceDbServer()
        {
        }

        private static string RootFolder
        {
            get
            {
                var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var rootFolderName = $"{assemblyFolder}\\SampleResourceServerRoot";
                var rootFolder = new DirectoryInfo(rootFolderName);
                if (!rootFolder.Exists)
                    rootFolder.Create();

                return rootFolderName;
            }
        }

        // Keys in ExternalResourceReference.GetReferenceInformation().
        private static string RefMapDbKeyEntry => "DBKey";

        private static string RefMapLinkPathEntry => "Path";

        private static string LocalLinkCacheFolder
        {
            get
            {
                var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var cacheRootFolderName = $"{assemblyFolder}\\SampleResourceServerLinkCache";
                var cacheRootFolder = new DirectoryInfo(cacheRootFolderName);
                if (!cacheRootFolder.Exists)
                    cacheRootFolder.Create();

                return cacheRootFolderName;
            }
        }

        // IExternalResourceServer must return ExternalResourceService.
        public ExternalServiceId GetServiceId()
        {
            return ExternalServices.BuiltInExternalServices.ExternalResourceService;
        }

        public Guid GetServerId()
        {
            return new Guid("5F3CAA13-F073-4F93-BDC2-B7F4B806CDAF");
        }

        public string GetName()
        {
            return "SDK Sample ExtRes Server";
        }

        public string GetVendorId()
        {
            return "ADSK";
        }

        public string GetDescription()
        {
            return "A Revit SDK sample external resource server which provides keynote data and Revit links.";
        }

        public string GetShortName()
        {
            return GetName();
        }

        public virtual string GetInformationLink()
        {
            return "http://www.autodesk.com";
        }

        public string GetIconPath()
        {
            return string.Empty;
        }

        public bool SupportsExternalResourceType(ExternalResourceType resourceType)
        {
            return resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.KeynoteTable
                   ||
                   resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.RevitLink;
        }

        public void SetupBrowserData(ExternalResourceBrowserData browserData)
        {
            var matchOptions = browserData.GetMatchOptions();

            var resourceType = matchOptions.ResourceType;

            var currentCulture = CultureInfo.CurrentCulture;
            var currentCultureName = currentCulture.ToString();

            // de-DE/fr-FR keynotes use a fictitious database tree; others use files under RootFolder.
            if (resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.KeynoteTable
                &&
                (currentCultureName == "de-DE" || currentCultureName == "fr-FR"))
                SetupKeynoteDatabaseBrowserData(browserData, currentCultureName);
            else
                SetupFileBasedBrowserData(browserData);
        }

        public bool IsResourceWellFormed(ExternalResourceReference extRef)
        {
            if (extRef.ServerId != GetServerId())
                return false;
            if (!extRef.HasValidDisplayPath())
                return false;

            // Database key (de-DE/fr-FR keynotes), link path, or keynote file path.
            var refMap = extRef.GetReferenceInformation();
            if (refMap.ContainsKey(RefMapDbKeyEntry))
                return ExternalResourceHelper.IsValidDbKey(refMap[RefMapDbKeyEntry]);
            if (refMap.ContainsKey(RefMapLinkPathEntry))
                return File.Exists(GetFullServerLinkFilePath(extRef));

            return File.Exists(GetFullServerKeynoteFilePath(extRef));
        }

        public virtual bool AreSameResources(IDictionary<string, string> referenceInformation1,
            IDictionary<string, string> referenceInformation2)
        {
            var same = true;
            if (referenceInformation1.Count != referenceInformation2.Count)
                same = false;
            else
                foreach (var key in referenceInformation1.Keys)
                {
                    if (!referenceInformation2.ContainsKey(key) ||
                        referenceInformation1[key] != referenceInformation2[key])
                    {
                        same = false;
                        break;
                    }
                }

            return same;
        }

        public string GetInSessionPath(ExternalResourceReference err, string savedPath)
        {
            return savedPath;
        }

        public void LoadResource(Guid loadRequestId, ExternalResourceType resourceType,
            ExternalResourceReference resourceReference, ExternalResourceLoadContext loadContext,
            ExternalResourceLoadContent loadContent)
        {
            loadContent.LoadStatus = ExternalResourceLoadStatus.Failure;

            if (loadRequestId == null)
                throw new ArgumentNullException(nameof(loadRequestId));
            ;
            if (resourceType == null)
                throw new ArgumentNullException(nameof(resourceType));
            ;
            if (resourceReference == null)
                throw new ArgumentNullException(nameof(resourceReference));
            ;
            if (loadContext == null)
                throw new ArgumentNullException(nameof(loadContext));
            ;
            if (loadContent == null)
                throw new ArgumentNullException(nameof(loadContent));
            ;
            if (!SupportsExternalResourceType(resourceType))
                throw new ArgumentOutOfRangeException(nameof(resourceType),
                    "The specified resource type is not supported by this server.");

            loadContent.Version = GetCurrentlyAvailableResourceVersion(resourceReference);

            if (resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.KeynoteTable)
                LoadKeynoteDataResource(resourceReference, loadContent);
            else
                LoadRevitLink(resourceReference, loadContent);
        }

        public ResourceVersionStatus GetResourceVersionStatus(ExternalResourceReference extRef)
        {
            // Determine whether currently loaded version is out of date, and return appropriate status.
            var currentlyLoadedVersion = extRef.Version;

            if (currentlyLoadedVersion == string.Empty)
                return ResourceVersionStatus.Unknown;

            return currentlyLoadedVersion == GetCurrentlyAvailableResourceVersion(extRef)
                ? ResourceVersionStatus.Current
                : ResourceVersionStatus.OutOfDate;
        }

        /// <summary>Registers Revit link extension callbacks; no extensions required for keynotes.</summary>
        public virtual void GetTypeSpecificServerOperations(ExternalResourceServerExtensions extensions)
        {
            var revitLinkOps = extensions.GetRevitLinkOperations();
            revitLinkOps.SetGetLocalPathForOpenCallback(new GetLinkPathForOpen());
            revitLinkOps.SetOnLocalLinkSharedCoordinatesSavedCallback(new LocalLinkSharedCoordinatesSaved());
        }

        private string GetCurrentlyAvailableResourceVersion(ExternalResourceReference extResRef)
        {
            var refMap = extResRef.GetReferenceInformation();
            if (refMap.ContainsKey(RefMapDbKeyEntry))
            {
                return ExternalResourceHelper.CurrentVersion;
            }

            if (refMap.ContainsKey(RefMapLinkPathEntry)) // ... OR it is a Revit link file
            {
                var serverLinkPath = GetFullServerLinkFilePath(extResRef);
                return GetFileVersion(serverLinkPath);
            }

            // ... OR it is a keynote file (non- French/German cases).
            var serverKeynoteFilePath = GetFullServerKeynoteFilePath(extResRef);
            return GetFileVersion(serverKeynoteFilePath);
        }

        private void SetupKeynoteDatabaseBrowserData(ExternalResourceBrowserData browserData, string currentCultureName)
        {
            var folderPath = browserData.FolderPath;

            switch (currentCultureName)
            {
                // Hard-coded de-DE/fr-FR folder tree; DBKey stored in each resource's reference map.
                case "de-DE" when folderPath == "/":
                    browserData.AddSubFolder("Unterordner1");
                    browserData.AddSubFolder("Unterordner2");
                    break;
                case "de-DE" when folderPath.EndsWith("/Unterordner1"):
                {
                    var refMap = new Dictionary<string, string>
                    {
                        [RefMapDbKeyEntry] = "1"
                    };
                    browserData.AddResource("Keynotes1_de-DE.txt", ExternalResourceHelper.CurrentVersion, refMap);
                    break;
                }
                case "de-DE":
                {
                    if (folderPath.EndsWith("/Unterordner2"))
                    {
                        var refMap = new Dictionary<string, string>
                        {
                            [RefMapDbKeyEntry] = "2"
                        };
                        browserData.AddResource("Keynotes2_de-DE.txt", ExternalResourceHelper.CurrentVersion, refMap);
                    }

                    break;
                }
                // Top-level
                case "fr-FR" when folderPath == "/":
                    browserData.AddSubFolder("Sous-dossier1");
                    browserData.AddSubFolder("Sous-dossier2");
                    break;
                case "fr-FR" when folderPath.EndsWith("/Sous-dossier1"):
                {
                    var refMap = new Dictionary<string, string>
                    {
                        [RefMapDbKeyEntry] = "3"
                    };
                    browserData.AddResource("Keynotes1_fr-FR.txt", ExternalResourceHelper.CurrentVersion, refMap);
                    break;
                }
                case "fr-FR":
                {
                    if (folderPath.EndsWith("/Sous-dossier2"))
                    {
                        var refMap = new Dictionary<string, string>
                        {
                            [RefMapDbKeyEntry] = "4"
                        };
                        browserData.AddResource("Keynotes2_fr-FR.txt", ExternalResourceHelper.CurrentVersion, refMap);
                    }

                    break;
                }
            }
        }

        // File-based resources use last-modified UTC as the version string.
        private string GetFileVersion(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var lastModifiedTime = fileInfo.LastWriteTimeUtc;
            var enUs = new CultureInfo("en-us");
            return lastModifiedTime.ToString(enUs);
        }

        private string GetFullServerKeynoteFilePath(ExternalResourceReference extRef)
        {
            var inSessionPath = extRef.InSessionPath;
            var serverName = GetName();
            // Prefer storing the relative path in referenceInformation (as done for links).
            var serverKeynoteFilePath = inSessionPath.Replace($"{serverName}://", $"{RootFolder}\\");
            return serverKeynoteFilePath;
        }

        private void SetupFileBasedBrowserData(ExternalResourceBrowserData browserData)
        {
            var matchOptions = browserData.GetMatchOptions();
            var resourceType = matchOptions.ResourceType;
            var filterPattern = resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.KeynoteTable
                ? "*.txt"
                : "*.rvt";

            var folderPath = browserData.FolderPath;
            var path = RootFolder + folderPath.Replace('/', '\\');
            if (Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);
                var subDirs = dir.GetDirectories();
                foreach (var subDir in subDirs)
                {
                    browserData.AddSubFolder(subDir.Name);
                }

                var subFiles = dir.GetFiles(filterPattern, SearchOption.TopDirectoryOnly);
                foreach (var file in subFiles)
                {
                    if (resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.KeynoteTable)
                    {
                        browserData.AddResource(file.Name, GetFileVersion(file.FullName));
                    }
                    else
                    {
                        var refMap = new Dictionary<string, string>
                        {
                            [RefMapLinkPathEntry] = $"{folderPath.TrimEnd('/')}/{file.Name}"
                        };
                        browserData.AddResource(file.Name, GetFileVersion(file.FullName), refMap);
                    }
                }
            }
            else
            {
                throw new ArgumentException("Path is invalid");
            }
        }

        private void LoadKeynoteDataResource(ExternalResourceReference resourceReference,
            ExternalResourceLoadContent loadContent)
        {
            var kdrlc = (KeyBasedTreeEntriesLoadContent)loadContent;
            if (kdrlc == null)
                throw new ArgumentException(
                    "Wrong type of ExternalResourceLoadContent (expecting a KeyBasedTreeEntriesLoadContent) for keynote data.",
                    nameof(loadContent));

            kdrlc.Reset();

            // Database key (de-DE/fr-FR) or file on disk.
            var refMap = resourceReference.GetReferenceInformation();
            if (refMap.ContainsKey(RefMapDbKeyEntry))
            {
                try
                {
                    ExternalResourceHelper.LoadKeynoteEntries(refMap[RefMapDbKeyEntry], ref kdrlc);
                    kdrlc.BuildEntries();
                    loadContent.LoadStatus = ExternalResourceLoadStatus.Success;
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (ArgumentNullException)
                {
                }
            }
            else
            {
                var serverKeynoteFilePath = GetFullServerKeynoteFilePath(resourceReference);
                var doesReadingSucceed = KeynoteEntries.LoadKeynoteEntriesFromFile(serverKeynoteFilePath, kdrlc);
                if (doesReadingSucceed)
                {
                    kdrlc.BuildEntries();
                    loadContent.LoadStatus = ExternalResourceLoadStatus.Success;
                }
            }
        }

        private void LoadRevitLink(ExternalResourceReference resourceReference, ExternalResourceLoadContent loadContent)
        {
            var linkLoadContent = (LinkLoadContent)loadContent;
            if (linkLoadContent == null)
                throw new ArgumentException(
                    "Wrong type of ExternalResourceLoadContent (expecting a LinkLoadContent) for Revit links.",
                    nameof(loadContent));

            try
            {
                // Copy server link to local cache; Revit opens the cached copy.
                var fullCachedPath = GetFullLinkCachedFilePath(resourceReference);
                var cacheFolder = Path.GetDirectoryName(fullCachedPath);
                if (!Directory.Exists(cacheFolder)) Directory.CreateDirectory(cacheFolder);
                var serverLinkPath = GetFullServerLinkFilePath(resourceReference);
                File.Copy(serverLinkPath, fullCachedPath, true);

                var linksPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fullCachedPath);
                linkLoadContent.SetLinkDataPath(linksPath);
                loadContent.LoadStatus = ExternalResourceLoadStatus.Success;
            }
            catch (Exception)
            {
            }
        }

        public static string GetFullServerLinkFilePath(ExternalResourceReference resource)
        {
            if (resource == null)
                return "";

            var refMap = resource.GetReferenceInformation();
            if (!refMap.ContainsKey(RefMapLinkPathEntry))
                return "";

            return RootFolder + resource.GetReferenceInformation()[RefMapLinkPathEntry].Replace("/", "\\");
        }

        /// <summary>
        /// Local cache path mirrors the server-relative link path (see GetFullServerLinkFilePath).
        /// </summary>
        public static string GetFullLinkCachedFilePath(ExternalResourceReference resource)
        {
            if (resource == null)
                return "";

            var refMap = resource.GetReferenceInformation();
            if (!refMap.ContainsKey(RefMapLinkPathEntry))
                return "";

            return LocalLinkCacheFolder + resource.GetReferenceInformation()[RefMapLinkPathEntry].Replace("/", "\\");
        }
    }
}
