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

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.SDK.Samples.CurtainSystem.CS.CurtainSystem;

namespace Revit.SDK.Samples.CurtainSystem.CS.Data
{
    /// <summary>
    ///     maintains all the data used in the sample
    /// </summary>
    public class MyDocument
    {
        /// <summary>
        ///     occurs only when there's a fatal error
        ///     the delegate method to handle the fatal error event
        /// </summary>
        /// <param name="errorMsg"></param>
        public delegate void FatalErrorHandler(string errorMsg);

        /// <summary>
        ///     occurs only when the message was updated
        ///     the delegate method to handle the message update event
        /// </summary>
        public delegate void MessageChangedHandler();

        // the message shown when there's a fatal error in the sample
        private string m_fatalErrorMsg;

        // store the message of the sample
        private KeyValuePair<string /*msgText*/, bool /*is warningOrError*/> m_message;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="commandData">
        ///     object which contains reference of Revit Application
        /// </param>
        public MyDocument(ExternalCommandData commandData)
        {
            CommandData = commandData;
            UIDocument = CommandData.Application.ActiveUIDocument;
            Document = UIDocument.Document;

            // initialize the curtain system data
            SystemData = new SystemData(this);

            // get the curtain system type of the active Revit document
            GetCurtainSystemType();
        }

        // object which contains reference of Revit Applicatio
        /// <summary>
        ///     object which contains reference of Revit Applicatio
        /// </summary>
        public ExternalCommandData CommandData { get; set; }

        // the active UI document of Revit

        /// <summary>
        ///     the active document of Revit
        /// </summary>
        public UIDocument UIDocument { get; }

        public Document Document { get; }

        // the data of the created curtain systems
        /// <summary>
        ///     the data of the created curtain systems
        /// </summary>
        public SystemData SystemData { get; set; }

        // all the faces of  the parallelepiped mass
        /// <summary>
        ///     // all the faces of  the parallelepiped mass
        /// </summary>
        public FaceArray MassFaceArray { get; set; }

        // the curtain system type of the active Revit document, used for curtain system creation
        /// <summary>
        ///     the curtain system type of the active Revit document, used for curtain system creation
        /// </summary>
        public CurtainSystemType CurtainSystemType { get; set; }

        /// <summary>
        ///     the message shown when there's a fatal error in the sample
        /// </summary>
        public string FatalErrorMsg
        {
            get => m_fatalErrorMsg;
            set
            {
                m_fatalErrorMsg = value;

                if (false == string.IsNullOrEmpty(m_fatalErrorMsg) &&
                    null != FatalErrorEvent)
                    FatalErrorEvent(m_fatalErrorMsg);
            }
        }

        /// <summary>
        ///     store the message of the sample
        /// </summary>
        public KeyValuePair<string /*msgText*/, bool /*is warningOrError*/> Message
        {
            get => m_message;
            set
            {
                m_message = value;
                if (null != MessageChanged) MessageChanged();
            }
        }

        /// <summary>
        ///     the event triggered when message updated/changed
        /// </summary>
        public event MessageChangedHandler MessageChanged;

        /// <summary>
        ///     the event triggered when the sample meets a fatal error
        /// </summary>
        public event FatalErrorHandler FatalErrorEvent;

        /// <summary>
        ///     get the curtain system type from the active Revit document
        /// </summary>
        private void GetCurtainSystemType()
        {
            var filteredElementCollector = new FilteredElementCollector(Document);
            filteredElementCollector.OfClass(typeof(CurtainSystemType));
            CurtainSystemType = filteredElementCollector.FirstElement() as CurtainSystemType;
        }
    } // end of class
}