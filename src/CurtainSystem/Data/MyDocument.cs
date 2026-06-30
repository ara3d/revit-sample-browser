// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.CurtainSystem.CS.CurtainSystem;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.CurtainSystem.CS.Data
{
    public class MyDocument
    {
        public delegate void FatalErrorHandler(string errorMsg);

        public delegate void MessageChangedHandler();

        // the message shown when there's a fatal error in the sample

        public MyDocument(ExternalCommandData commandData)
        {
            CommandData = commandData;
            UiDocument = CommandData.Application.ActiveUIDocument;
            Document = UiDocument.Document;

            // initialize the curtain system data
            SystemData = new SystemData(this);

            GetCurtainSystemType();
        }

        // object which contains reference of Revit Applicatio
        public ExternalCommandData CommandData { get; set; }

        // the active UI document of Revit

        public UIDocument UiDocument { get; }

        public Document Document { get; }

        // the data of the created curtain systems
        public SystemData SystemData { get; set; }

        // all the faces of  the parallelepiped mass
        public FaceArray MassFaceArray { get; set; }

        // the curtain system type of the active Revit document, used for curtain system creation
        public CurtainSystemType CurtainSystemType { get; set; }

        public string FatalErrorMsg
        {
            get;
            set
            {
                field = value;

                if (false == string.IsNullOrEmpty(field) &&
                    null != FatalErrorEvent)
                    FatalErrorEvent(field);
            }
        }

        public KeyValuePair<string /*msgText*/, bool /*is warningOrError*/> Message
        {
            get;
            set
            {
                field = value;
                MessageChanged?.Invoke();
            }
        }

        public event MessageChangedHandler MessageChanged;

        public event FatalErrorHandler FatalErrorEvent;

        private void GetCurtainSystemType()
        {
            FilteredElementCollector filteredElementCollector = new(Document);
            filteredElementCollector.OfClass(typeof(CurtainSystemType));
            CurtainSystemType = filteredElementCollector.FirstElement() as CurtainSystemType;
        }
    } // end of class
}
