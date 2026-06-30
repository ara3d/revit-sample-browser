// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.ExternalResourceServer.ExternalResourceDBServer.CS;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ExternalResourceServer.ExternalResourceUIServer.CS
{
    public class SampleExternalResourceUiServer : IExternalResourceUIServer
    {
        private static readonly Guid MyServerId = new Guid("E9B6C194-62DE-4134-900D-BA8DF7AD33FA");
        private static readonly Guid MyDbServerId = new Guid("5F3CAA13-F073-4F93-BDC2-B7F4B806CDAF");

        public Guid GetServerId()
        {
            return MyServerId;
        }

        public ExternalServiceId GetServiceId()
        {
            return ExternalServices.BuiltInExternalServices.ExternalResourceUIService;
        }

        public string GetName()
        {
            return "SDK Sample ExtRes UI Server";
        }

        public string GetVendorId()
        {
            return "ADSK";
        }

        public string GetDescription()
        {
            return "Simple UI server for the Revit SDK sample external resource server";
        }

        public Guid GetDBServerId()
        {
            return MyDbServerId;
        }

        /// <summary>Reports load results from the paired DB server to the user.</summary>
        public void HandleLoadResourceResults(Document doc, IList<ExternalResourceLoadData> loadDataList)
        {
            foreach (var data in loadDataList)
            {
                var resourceType = data.ExternalResourceType;

                var myMessage = string.Empty;

                var loadContext = data.GetLoadContext();
                data.GetExternalResourceReference();
                var currentlyLoadedRef = loadContext.GetCurrentlyLoadedReference();

                var loadType = loadContext.LoadOperationType;

                switch (loadType)
                {
                    case LoadOperationType.Automatic:
                        myMessage = "This is an Automatic load operation. ";
                        break;

                    case LoadOperationType.Explicit:
                        myMessage = "This is an Explicit load operation. ";
                        break;

                    default:
                        myMessage = "There is no load type information!! ";
                        break;
                }

                var bUnrecognizedStatus = false;
                switch (data.LoadStatus)
                {
                    case ExternalResourceLoadStatus.ResourceAlreadyCurrent when data.GetLoadContext().LoadOperationType == LoadOperationType.Explicit:
                    {
                        var resourcePath = currentlyLoadedRef.InSessionPath;
                        myMessage += $"\n No new changes to load for link: {resourcePath}";
                        break;
                    }
                    case ExternalResourceLoadStatus.ResourceAlreadyCurrent:
                        continue;
                    case ExternalResourceLoadStatus.Uninitialized:
                        myMessage += "\n The load status is uninitialized - this generally shouldn't happen";
                        break;
                    case ExternalResourceLoadStatus.Failure:
                        myMessage += "\n The load failed and the reason is unknown.";
                        break;
                    case ExternalResourceLoadStatus.Success when resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.KeynoteTable:
                    {
                        var resourcePath = data.GetExternalResourceReference().InSessionPath;
                        myMessage +=
                            $"\n Version {data.GetLoadContent().Version} of keynote data '{resourcePath}' has been loaded successfully";
                        break;
                    }
                    case ExternalResourceLoadStatus.Success:
                    {
                        if (resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.RevitLink)
                        {
                            var resourcePath = data.GetExternalResourceReference().InSessionPath;
                            var ldrlc = (LinkLoadContent)data.GetLoadContent();
                            var destinationPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(ldrlc.GetLinkDataPath());
                            myMessage +=
                                $"\n Version {data.GetLoadContent().Version} of the file: {resourcePath} has been downloaded into the cached folder: {destinationPath} for this Revit Link.";
                        }

                        break;
                    }
                    default:
                        myMessage += "Unrecognized external resource load status.";
                        bUnrecognizedStatus = true;
                        break;
                }

                if (!bUnrecognizedStatus &&
                    resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.RevitLink)
                {
                    // Revit links also expose LinkLoadResult with detailed load status.
                    var ldrlc = (LinkLoadContent)data.GetLoadContent();
                    var loadResult = ldrlc.GetLinkLoadResult();
                    if (loadResult != null)
                        myMessage += $"\n LinkLoadResultType: {loadResult.LoadResult:g}";
                }

                MessageBox.Show(myMessage, "UI Server for SDK Sample External Resource Server");
            }
        }

        /// <summary>Reports browse failures; detailed errors may be read from the DB server.</summary>
        public void HandleBrowseResult(ExternalResourceUIBrowseResultType resultType, string browsingItemPath)
        {
            if (resultType == ExternalResourceUIBrowseResultType.Success)
                return;

            var resultString = resultType.ToString("g");

            var externalResourceService =
                ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.ExternalResourceService);
            if (externalResourceService == null)
            {
                MessageBox.Show("External Resource Service unexpectedly not found.");
                return;
            }

            if (!(externalResourceService.GetServer(GetDBServerId()) is SampleExternalResourceDbServer myDbServer))
            {
                MessageBox.Show("Cannot get SampleExternalResourceDBServer from ExternalResourceService.");
                return;
            }

            var message = $"The browse result for <{browsingItemPath}> was: <{resultString}>.";
            MessageBox.Show(message);
        }
    }
}
