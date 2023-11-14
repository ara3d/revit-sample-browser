// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using Revit.SDK.Samples.ExternalResourceDBServer.CS;

namespace Revit.SDK.Samples.ExternalResourceUIServer.CS
{
    internal class SampleExternalResourceUIServer : IExternalResourceUIServer
    {
        private static readonly Guid m_myServerId = new Guid("E9B6C194-62DE-4134-900D-BA8DF7AD33FA");
        private static readonly Guid m_myDBServerId = new Guid("5F3CAA13-F073-4F93-BDC2-B7F4B806CDAF");

        // Methods that must be implemented by a server for any of Revit's external services
        /// <summary>
        ///     Return the Id of the server.
        /// </summary>
        public Guid GetServerId()
        {
            return m_myServerId;
        }

        /// <summary>
        ///     Return the Id of the service that the server belongs to.
        /// </summary>
        public ExternalServiceId GetServiceId()
        {
            return ExternalServices.BuiltInExternalServices.ExternalResourceUIService;
        }

        /// <summary>
        ///     Return the server's name.
        /// </summary>
        public string GetName()
        {
            return "SDK Sample ExtRes UI Server";
        }

        /// <summary>
        ///     Return the server's vendor Id.
        /// </summary>
        public string GetVendorId()
        {
            return "ADSK";
        }

        /// <summary>
        ///     Return the description of the server.
        /// </summary>
        public string GetDescription()
        {
            return "Simple UI server for the Revit SDK sample external resource server";
        }


        /// <summary>
        ///     Return the Id of the related DB server.
        /// </summary>
        public Guid GetDBServerId()
        {
            return m_myDBServerId;
        }


        /// <summary>
        ///     Reports the results of loads from the DB server (SampleExternalResourceServer).
        ///     This method should be implemented to provide UI to communicate success or failure
        ///     of a particular external resource load operation to the user.
        /// </summary>
        /// <param name="doc">
        ///     The Revit model into which the External Resource was loaded.
        /// </param>
        /// <param name="loadDataList">
        ///     Contains a list of ExternalResourceLoadData with results
        ///     for all external resources loaded by the DB server.  It is possible for the DB server
        ///     to have loaded more than one resource (for example, loading several linked files
        ///     when a host file is opened by the user).
        /// </param>
        public void HandleLoadResourceResults(Document doc, IList<ExternalResourceLoadData> loadDataList)
        {
            foreach (var data in loadDataList)
            {
                var resourceType = data.ExternalResourceType;

                // This message will be posted in a dialog box at the end of this method.
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
                        myMessage += "\n No new changes to load for link: " + resourcePath;
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
                        myMessage += "\n Version " + data.GetLoadContent().Version + " of keynote data \'" +
                                     resourcePath + "\' has been loaded successfully";
                        break;
                    }
                    case ExternalResourceLoadStatus.Success:
                    {
                        if (resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.RevitLink)
                        {
                            var resourcePath = data.GetExternalResourceReference().InSessionPath;
                            var ldrlc = (LinkLoadContent)data.GetLoadContent();
                            var destinationPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(ldrlc.GetLinkDataPath());
                            myMessage += "\n Version " + data.GetLoadContent().Version +
                                         " of the file: " + resourcePath +
                                         " has been downloaded into the cached folder: " + destinationPath +
                                         " for this Revit Link.";
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
                    // For Revit links, the UI server can also obtain a RevitLinkLoadResult which contains detailed
                    // information about the status of the attempt to load the local copy of the link into Revit.
                    var ldrlc = (LinkLoadContent)data.GetLoadContent();
                    var loadResult = ldrlc.GetLinkLoadResult();
                    if (loadResult != null)
                        myMessage += "\n LinkLoadResultType: " + loadResult.LoadResult.ToString("g");
                }

                MessageBox.Show(myMessage, "UI Server for SDK Sample External Resource Server");
            }
        }


        /// <summary>
        ///     Use this method to report any problems that occurred while the user was browsing for External Resources.
        ///     Revit will call this method each time the end user browses to a new folder location, or selects an item
        ///     and clicks Open.
        /// </summary>
        public void HandleBrowseResult(ExternalResourceUIBrowseResultType resultType, string browsingItemPath)
        {
            if (resultType == ExternalResourceUIBrowseResultType.Success)
                return;

            var resultString = resultType.ToString("g");

            // While executing its SetupBrowserData() method, the "DB server" - SampleExternalResourceServer - can store
            // detailed information about browse failures that occurred (user not logged in, network down, etc.).
            // Subsequently, when Revit calls this method, the details can be read from the DB server and reported to the user.
            var externalResourceService =
                ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.ExternalResourceService);
            if (externalResourceService == null)
            {
                MessageBox.Show("External Resource Service unexpectedly not found.");
                return;
            }

            if (!(externalResourceService.GetServer(GetDBServerId()) is SampleExternalResourceDBServer myDBServer))
            {
                MessageBox.Show("Cannot get SampleExternalResourceDBServer from ExternalResourceService.");
                return;
            }
            // ... Retrieve detailed failure information from SampleExternalResourceServer here.

            var message = $"The browse result for <{browsingItemPath}> was: <{resultString}>.";
            MessageBox.Show(message);
        }
    }
}
