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

using System;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;

namespace RevitMultiSample.InCanvasControlAPI.CS
{
    /// <summary>
    ///     This class demonstrates using external server to handle click events on an in-canvas control.
    /// </summary>
    public class IssueSelectHandler : ITemporaryGraphicsHandler
    {
        /// <summary>
        ///     Gets the handler's description
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            return "Changes Issue marker visual upon marker click";
        }

        /// <summary>
        ///     Gets the hander's name
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return "Issue marker click event handler";
        }

        /// <summary>
        ///     Gets server's GUID
        /// </summary>
        /// <returns>External server GUID</returns>
        public Guid GetServerId()
        {
            return new Guid("81F91FC9-B6D5-4FD4-AB5B-04F307369A79");
        }

        /// <summary>
        ///     Gets service id this server should be registered on.
        /// </summary>
        /// <returns></returns>
        public ExternalServiceId GetServiceId()
        {
            return ExternalServices.BuiltInExternalServices.TemporaryGraphicsHandlerService;
        }

        /// <summary>
        ///     Gets vendor's name
        /// </summary>
        /// <returns></returns>
        public string GetVendorId()
        {
            return "ADSK";
        }

        /// <summary>
        ///     Handles the event of control being clicked
        /// </summary>
        /// <param name="data">
        ///     Data of the event. This only provides us with an index of the control clicked. The developer / API
        ///     user should make sense of each index himself.
        /// </param>
        public void OnClick(TemporaryGraphicsCommandData data)
        {
            IssueMarkerSelector.SelectMarker(data.Document, data.Index);
        }
    }
}
