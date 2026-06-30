// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Reinforcement.CS
{
    public class ColumnFramReinMaker : FramReinMaker
    {
        private readonly ColumnGeometrySupport m_geometry; // The geometry support for column rebar creation

        public ColumnFramReinMaker(ExternalCommandData commandData, FamilyInstance hostObject)
            : base(commandData, hostObject)
        {
            var geoOptions = commandData.Application.Application.Create.NewGeometryOptions();
            geoOptions.ComputeReferences = true;

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
                if (4 > value) // vertical rebar number must be above 3
                    throw new Exception("The minimum of vertical rebar number shouble be four.");
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
            // create the transverse rebars
            var flag = FillTransverseBars();

            // create the vertical rebars
            flag = flag && FillVerticalBars();

            return base.FillWithBars();
        }

        public bool FillTransverseBars()
        {
            // create all kinds of transverse rebars according to the TransverseRebarLocation
            foreach (TransverseRebarLocation location in Enum.GetValues(
                         typeof(TransverseRebarLocation)))
            {
                var createdRebar = FillTransverseBar(location);
                //judge whether the transverse rebar creation is successful
                if (null == createdRebar) return false;
            }

            return true;
        }

        /// <summary>
        ///     Create the transverse rebars, according to the transverse rebar location
        /// </summary>
        /// <param name="location">location of rebar which need to be created</param>
        /// <returns>the created rebar, return null if the creation is unsuccessful</returns>
        public Rebar FillTransverseBar(TransverseRebarLocation location)
        {
            // Get the geometry information which support rebar creation
            RebarGeometry geomInfo = new();
            RebarBarType barType = null;
            switch (location)
            {
                case TransverseRebarLocation.Start: // start transverse rebar
                case TransverseRebarLocation.End: // end transverse rebar
                    geomInfo = m_geometry.GetTransverseRebar(location, TransverseEndSpacing);
                    barType = TransverseEndType;
                    break;
                case TransverseRebarLocation.Center: // center transverse rebar   
                    geomInfo = m_geometry.GetTransverseRebar(location, TransverseCenterSpacing);
                    barType = TransverseCenterType;
                    break;
            }

            // create the rebar
            return PlaceRebars(barType, TransverseHookType, TransverseHookType,
                geomInfo, RebarHookOrientation.Right, RebarHookOrientation.Left);
        }

        /// <summary>
        ///     Create the vertical rebar according the location
        /// </summary>
        /// <param name="location">location of rebar which need to be created</param>
        /// <returns>the created rebar, return null if the creation is unsuccessful</returns>
        public Rebar FillVerticalBar(VerticalRebarLocation location)
        {
            //calculate the rebar number in different location
            var rebarNubmer = VerticalRebarNumber / 4;
            switch (location)
            {
                case VerticalRebarLocation.East: // the east vertical rebar
                    if (0 < VerticalRebarNumber % 4) rebarNubmer++;
                    break;
                case VerticalRebarLocation.North: // the north vertical rebar
                    if (2 < VerticalRebarNumber % 4) rebarNubmer++;
                    break;
                case VerticalRebarLocation.West: // the west vertical rebar
                    if (1 < VerticalRebarNumber % 4) rebarNubmer++;
                    break;
                case VerticalRebarLocation.South: // the south vertical rebar
                    break;
            }

            // get the geometry information for rebar creation
            var geomInfo = m_geometry.GetVerticalRebar(location, rebarNubmer);

            // create the rebar
            return PlaceRebars(VerticalRebarType, null, null, geomInfo,
                RebarHookOrientation.Left, RebarHookOrientation.Left);
        }

        private bool FillVerticalBars()
        {
            // create all kinds of vertical rebars according to the VerticalRebarLocation
            foreach (VerticalRebarLocation location in Enum.GetValues(
                         typeof(VerticalRebarLocation)))
            {
                var createdRebar = FillVerticalBar(location);
                //judge whether the vertical rebar creation is successful
                if (null == createdRebar) return false;
            }

            return true;
        }
    }
}
