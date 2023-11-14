// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Reinforcement.CS
{
    /// <summary>
    ///     The class derived from FramReinMaker shows how to create the rebars for a beam
    /// </summary>
    public class BeamFramReinMaker : FramReinMaker
    {
        private readonly BeamGeometrySupport m_geometry; // The geometry support for beam rebar creation
        private double m_transverseCenterSpacing; //the spacing value of center transverse rebar

        // The rebar type, hook type and spacing information

        private double m_transverseEndSpacing; //the spacing value of end transverse rebar

        /// <summary>
        ///     Constructor of the BeamFramReinMaker
        /// </summary>
        /// <param name="commandData">the ExternalCommandData reference</param>
        /// <param name="hostObject">the host beam</param>
        public BeamFramReinMaker(ExternalCommandData commandData, FamilyInstance hostObject)
            : base(commandData, hostObject)
        {
            //create new options for current project
            var geoOptions = commandData.Application.Application.Create.NewGeometryOptions();
            geoOptions.ComputeReferences = true;

            //create a BeamGeometrySupport instance.
            m_geometry = new BeamGeometrySupport(hostObject, geoOptions);
        }

        /// <summary>
        ///     get and set the type of the end rebar in the top of beam
        /// </summary>
        public RebarBarType TopEndRebarType { get; set; }

        /// <summary>
        ///     get and set the type of the center rebar in the top of beam
        /// </summary>
        public RebarBarType TopCenterRebarType { get; set; }

        /// <summary>
        ///     get and set the type of the rebar in the bottom of beam
        /// </summary>
        public RebarBarType BottomRebarType { get; set; }

        /// <summary>
        ///     get and set the type of the transverse rebar
        /// </summary>
        public RebarBarType TransverseRebarType { get; set; }

        /// <summary>
        ///     get and set the spacing value of end transverse rebar
        /// </summary>
        public double TransverseEndSpacing
        {
            get => m_transverseEndSpacing;
            set
            {
                if (0 > value) throw new Exception("Transverse end spacing should be above zero");
                m_transverseEndSpacing = value;
            }
        }

        /// <summary>
        ///     get and set the spacing value of center transverse rebar
        /// </summary>
        public double TransverseCenterSpacing
        {
            get => m_transverseCenterSpacing;
            set
            {
                if (0 > value) throw new Exception("Transverse center spacing should be above zero");
                m_transverseCenterSpacing = value;
            }
        }

        /// <summary>
        ///     get and set the hook type of top end rebar
        /// </summary>
        public RebarHookType TopHookType { get; set; }

        /// <summary>
        ///     get and set the hook type of transverse rebar
        /// </summary>
        public RebarHookType TransverseHookType { get; set; }

        /// <summary>
        ///     Override method to do some further checks
        /// </summary>
        /// <returns>true if the the data is right and enough, otherwise false.</returns>
        protected override bool AssertData()
        {
            return base.AssertData();
        }

        /// <summary>
        ///     Display a form to collect the information for beam reinforcement creation
        /// </summary>
        /// <returns>true if the information collection is successful, otherwise false</returns>
        protected override bool DisplayForm()
        {
            // Display BeamFramReinMakerForm for the user to input information 
            using (var displayForm = new BeamFramReinMakerForm(this))
            {
                if (DialogResult.OK != displayForm.ShowDialog()) return false;
            }

            return base.DisplayForm();
        }

        /// <summary>
        ///     Override method to create rebar on the selected beam
        /// </summary>
        /// <returns>true if the creation is successful, otherwise false</returns>
        protected override bool FillWithBars()
        {
            // create the top rebars
            var flag = FillTopBars();

            // create the bottom rebars
            flag = flag && FillBottomBars();

            // create the transverse rebars
            flag = flag && FillTransverseBars();

            return base.FillWithBars();
        }

        /// <summary>
        ///     Create the rebar at the bottom of beam
        /// </summary>
        /// <returns>true if the creation is successful, otherwise false</returns>
        public bool FillBottomBars()
        {
            // get the geometry information of the bottom rebar
            var geomInfo = m_geometry.GetBottomRebar();

            // create the rebar
            var rebar = PlaceRebars(BottomRebarType, null, null, geomInfo,
                RebarHookOrientation.Left, RebarHookOrientation.Left);
            return null != rebar;
        }

        /// <summary>
        ///     Create the transverse rebars
        /// </summary>
        /// <returns>true if the creation is successful, otherwise false</returns>
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
        ///     Create the transverse rebars, according to the location of transverse rebars
        /// </summary>
        /// <param name="location">location of rebar which need to be created</param>
        /// <returns>the created rebar, return null if the creation is unsuccessful</returns>
        public Rebar FillTransverseBar(TransverseRebarLocation location)
        {
            // Get the geometry information which support rebar creation
            var geomInfo = new RebarGeometry();
            switch (location)
            {
                case TransverseRebarLocation.Start: // start transverse rebar
                case TransverseRebarLocation.End: // end transverse rebar
                    geomInfo = m_geometry.GetTransverseRebar(location, m_transverseEndSpacing);
                    break;
                case TransverseRebarLocation.Center: // center transverse rebar
                    geomInfo = m_geometry.GetTransverseRebar(location, m_transverseCenterSpacing);
                    break;
            }

            var startHook = RebarHookOrientation.Right;
            var endHook = RebarHookOrientation.Left;
            if (!GeomUtil.IsInRightDir(geomInfo.Normal))
            {
                startHook = RebarHookOrientation.Left;
                endHook = RebarHookOrientation.Right;
            }

            // create the rebar
            return PlaceRebars(TransverseRebarType, TransverseHookType, TransverseHookType,
                geomInfo, startHook, endHook);
        }

        /// <summary>
        ///     Get the hook orient of the top rebar
        /// </summary>
        /// <param name="geomInfo">the rebar geometry support information</param>
        /// <param name="location">the location of top rebar</param>
        /// <returns>the hook orient of the top hook</returns>
        private RebarHookOrientation GetTopHookOrient(RebarGeometry geomInfo, TopRebarLocation location)
        {
            // Top center rebar doesn't need hook.
            if (TopRebarLocation.Center == location) throw new Exception("Center top rebar doesn't have any hook.");

            // Get the hook direction, rebar normal and rebar line
            var hookVec = m_geometry.GetDownDirection();
            var normal = geomInfo.Normal;
            var rebarLine = geomInfo.Curves[0] as Line;

            // get the top start hook orient
            if (TopRebarLocation.Start == location)
            {
                var curveVec = GeomUtil.SubXYZ(rebarLine.GetEndPoint(1), rebarLine.GetEndPoint(0));
                return GeomUtil.GetHookOrient(curveVec, normal, hookVec);
            }
            else // get the top end hook orient
            {
                var curveVec = GeomUtil.SubXYZ(rebarLine.GetEndPoint(0), rebarLine.GetEndPoint(1));
                return GeomUtil.GetHookOrient(curveVec, normal, hookVec);
            }
        }

        /// <summary>
        ///     Create the rebar at the top of beam
        /// </summary>
        /// <returns>true if the creation is successful, otherwise false</returns>
        private bool FillTopBars()
        {
            // create all kinds of top rebars according to the TopRebarLocation
            foreach (TopRebarLocation location in Enum.GetValues(typeof(TopRebarLocation)))
            {
                var createdRebar = FillTopBar(location);
                //judge whether the top rebar creation is successful
                if (null == createdRebar) return false;
            }

            return true;
        }

        /// <summary>
        ///     Create the rebar at the top of beam, according to the top rebar location
        /// </summary>
        /// <param name="location">location of rebar which need to be created</param>
        /// <returns>the created rebar, return null if the creation is unsuccessful</returns>
        private Rebar FillTopBar(TopRebarLocation location)
        {
            //get the geometry information of the rebar
            var geomInfo = m_geometry.GetTopRebar(location);

            RebarHookType startHookType = null; //the start hook type of the rebar
            RebarHookType endHookType = null; // the end hook type of the rebar
            RebarBarType rebarType = null; // the rebar type 
            var startOrient = RebarHookOrientation.Right; // the start hook orient
            var endOrient = RebarHookOrientation.Left; // the end hook orient

            // decide the rebar type, hook type and hook orient according to location
            switch (location)
            {
                case TopRebarLocation.Start:
                    startHookType = TopHookType; // start hook type
                    rebarType = TopEndRebarType; // rebar type
                    startOrient = GetTopHookOrient(geomInfo, location); // start hook orient
                    break;
                case TopRebarLocation.Center:
                    rebarType = TopCenterRebarType; // rebar type
                    break;
                case TopRebarLocation.End:
                    endHookType = TopHookType; // end hook type
                    rebarType = TopEndRebarType; // rebar type
                    endOrient = GetTopHookOrient(geomInfo, location); // end hook orient
                    break;
            }

            // create the rebar
            return PlaceRebars(rebarType, startHookType, endHookType,
                geomInfo, startOrient, endOrient);
        }
    }
}
