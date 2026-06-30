// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS
{
    /// <summary>
    ///     The class derived form FramReinMaker shows how to create the reinforcement for a column
    /// </summary>
    public class ColumnFramReinMaker : FramReinMaker
    {
        private readonly ColumnGeometrySupport m_geometry; // The geometry support for column reinforcement creation

        public ColumnFramReinMaker(ExternalCommandData commandData, FamilyInstance hostObject)
            : base(commandData, hostObject)
        {
            //create a new options for current project
            var geoOptions = commandData.Application.Application.Create.NewGeometryOptions();
            geoOptions.ComputeReferences = true;

            //create a ColumnGeometrySupport instance 
            m_geometry = new ColumnGeometrySupport(hostObject, geoOptions);
        }

        public RebarBarType TransverseEndType { get; set; }

        public RebarBarType TransverseCenterType { get; set; }

        public RebarBarType VerticalRebarType { get; set; }

        public double TransverseEndSpacing
        {
            get;
            set
            {
                if (0 > value) // spacing data must be above 0
                    throw new Exception("Transverse end spacing should be above zero");
                field = value;
            }
        }

        public double TransverseCenterSpacing
        {
            get;
            set
            {
                if (0 > value) // spacing data must be above 0
                    throw new Exception("Transverse center spacing should be above zero");
                field = value;
            }
        }

        public int VerticalRebarNumber
        {
            get;
            set
            {
                if (4 > value) // vertical reinforcement number must be above 3
                    throw new Exception("The minimum of vertical reinforcement number should be 4.");
                field = value;
            }
        }

        public RebarHookType TransverseHookType { get; set; }

        protected override bool AssertData()
        {
            return base.AssertData();
        }

        protected override bool DisplayForm()
        {
            // Display ColumnFramReinMakerForm for the user input information 
            using (ColumnFramReinMakerForm displayForm = new(this))
            {
                if (DialogResult.OK != displayForm.ShowDialog()) return false;
            }

            return base.DisplayForm();
        }

        protected override bool FillWithBars()
        {
            //create Rebar Container
            var conTypeId = RebarContainerType.CreateDefaultRebarContainerType(RevitDoc);
            var hostId = HostObject.Id;
            var host = RevitDoc.GetElement(hostId);
            if (null != host)
            {
                var cont = RebarContainer.Create(RevitDoc, host, conTypeId);
                var flag = FillTransverseItems(cont);
                flag = flag && FillVerticalItems(cont);
            }

            return base.FillWithBars();
        }

        public bool FillTransverseItems(RebarContainer cont)
        {
            // create all kinds of transverse reinforcement according to the TransverseRebarLocation
            foreach (TransverseRebarLocation location in Enum.GetValues(typeof(TransverseRebarLocation)))
            {
                var item = FillTransverseItem(cont, location);
                //judge whether the transverse reinforcement creation is successful
                if (null == item) return false;
            }

            return true;
        }

        public RebarContainerItem FillTransverseItem(RebarContainer cont, TransverseRebarLocation location)
        {
            // Get the geometry information which support reinforcement creation
            RebarGeometry geomInfo = new();
            RebarBarType barType = null;
            switch (location)
            {
                case TransverseRebarLocation.Start: // start transverse reinforcement
                case TransverseRebarLocation.End: // end transverse reinforcement
                    geomInfo = m_geometry.GetTransverseRebar(location, TransverseEndSpacing);
                    barType = TransverseEndType;
                    break;
                case TransverseRebarLocation.Center: // center transverse reinforcement   
                    geomInfo = m_geometry.GetTransverseRebar(location, TransverseCenterSpacing);
                    barType = TransverseCenterType;
                    break;
            }

            // create the container item
            return PlaceContainerItem(cont, barType, TransverseHookType, TransverseHookType, geomInfo,
                RebarHookOrientation.Left, RebarHookOrientation.Left);
        }

        public RebarContainerItem FillVerticalItem(RebarContainer cont, VerticalRebarLocation location)
        {
            //calculate the reinforcement number in different location
            var rebarNubmer = VerticalRebarNumber / 4;
            switch (location)
            {
                case VerticalRebarLocation.East: // the east vertical reinforcement
                    if (0 < VerticalRebarNumber % 4) rebarNubmer++;
                    break;
                case VerticalRebarLocation.North: // the north vertical reinforcement
                    if (2 < VerticalRebarNumber % 4) rebarNubmer++;
                    break;
                case VerticalRebarLocation.West: // the west vertical reinforcement
                    if (1 < VerticalRebarNumber % 4) rebarNubmer++;
                    break;
                case VerticalRebarLocation.South: // the south vertical reinforcement
                    break;
            }

            // get the geometry information for reinforcement creation
            var geomInfo = m_geometry.GetVerticalRebar(location, rebarNubmer);

            // create the container item
            return PlaceContainerItem(cont, VerticalRebarType, null, null, geomInfo, RebarHookOrientation.Right,
                RebarHookOrientation.Right);
        }

        private bool FillVerticalItems(RebarContainer cont)
        {
            // create all kinds of vertical reinforcement according to the VerticalRebarLocation
            foreach (VerticalRebarLocation location in Enum.GetValues(typeof(VerticalRebarLocation)))
            {
                var item = FillVerticalItem(cont, location);
                //judge whether the vertical reinforcement creation is successful
                if (null == item) return false;
            }

            return true;
        }
    }
}
