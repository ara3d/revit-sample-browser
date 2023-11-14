// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RebarContainerAnyShapeType.CS
{
    /// <summary>
    ///     The class derived from FramReinMaker shows how to create the reinforcement for a beam
    /// </summary>
    public class BeamFramReinMaker : FramReinMaker
    {
        private readonly BeamGeometrySupport m_geometry; // The geometry support for beam reinforcement creation
        private double m_transverseCenterSpacing; //the spacing value of center transverse reinforcement

        // The reinforcement type, hook type and spacing information

        private double m_transverseEndSpacing; //the spacing value of end transverse reinforcement


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
        ///     get and set the type of the end reinforcement in the top of beam
        /// </summary>
        public RebarBarType TopEndRebarType { get; set; }

        /// <summary>
        ///     get and set the type of the center reinforcement in the top of beam
        /// </summary>
        public RebarBarType TopCenterRebarType { get; set; }

        /// <summary>
        ///     get and set the type of the reinforcement in the bottom of beam
        /// </summary>
        public RebarBarType BottomRebarType { get; set; }

        /// <summary>
        ///     get and set the type of the transverse reinforcement
        /// </summary>
        public RebarBarType TransverseRebarType { get; set; }

        /// <summary>
        ///     get and set the spacing value of end transverse reinforcement
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
        ///     get and set the spacing value of center transverse reinforcement
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
        ///     get and set the hook type of top end reinforcement
        /// </summary>
        public RebarHookType TopHookType { get; set; }

        /// <summary>
        ///     get and set the hook type of transverse reinforcement
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
            //create Rebar Container
            var conTypeId = RebarContainerType.CreateDefaultRebarContainerType(m_revitDoc);
            var cont = RebarContainer.Create(m_revitDoc, m_hostObject, conTypeId);

            // create the top items
            var flag = FillTopItems(cont);

            // create the bottom items
            flag = flag && FillBottomItems(cont);

            // create the transverse items
            flag = flag && FillTransverseItems(cont);

            return base.FillWithBars();
        }


        /// <summary>
        ///     Create the reinforcement at the bottom of beam
        /// </summary>
        /// <returns>true if the creation is successful, otherwise false</returns>
        public bool FillBottomItems(RebarContainer cont)
        {
            // get the geometry information of the bottom reinforcement
            var geomInfo = m_geometry.GetBottomRebar();

            // create the container item
            var item = PlaceContainerItem(cont, BottomRebarType, null, null, geomInfo, RebarHookOrientation.Left,
                RebarHookOrientation.Left);
            return null != item;
        }

        /// <summary>
        ///     Create the transverse reinforcement
        /// </summary>
        /// <returns>true if the creation is successful, otherwise false</returns>
        public bool FillTransverseItems(RebarContainer cont)
        {
            // create all kinds of transverse reinforcement according to the TransverseRebarLocation
            foreach (TransverseRebarLocation location in Enum.GetValues(
                         typeof(TransverseRebarLocation)))
            {
                var item = FillTransverseItem(cont, location);
                //judge whether the transverse reinforcement creation is successful
                if (null == item) return false;
            }

            return true;
        }

        /// <summary>
        ///     Create the transverse reinforcement, according to the location of transverse bars
        /// </summary>
        /// <param name="location">location of rebar which need to be created</param>
        /// <returns>the created container item, return null if the creation is unsuccessful</returns>
        public RebarContainerItem FillTransverseItem(RebarContainer cont, TransverseRebarLocation location)
        {
            // Get the geometry information which support reinforcement creation
            var geomInfo = new RebarGeometry();
            switch (location)
            {
                case TransverseRebarLocation.Start: // start transverse reinforcement
                case TransverseRebarLocation.End: // end transverse reinforcement
                    geomInfo = m_geometry.GetTransverseRebar(location, m_transverseEndSpacing);
                    break;
                case TransverseRebarLocation.Center: // center transverse reinforcement
                    geomInfo = m_geometry.GetTransverseRebar(location, m_transverseCenterSpacing);
                    break;
            }

            var startHook = RebarHookOrientation.Left;
            var endHook = RebarHookOrientation.Left;
            if (!GeomUtil.IsInRightDir(geomInfo.Normal))
            {
                startHook = RebarHookOrientation.Right;
                endHook = RebarHookOrientation.Right;
            }

            // create the container item
            return PlaceContainerItem(cont, TransverseRebarType, TransverseHookType, TransverseHookType, geomInfo,
                startHook, endHook);
        }

        /// <summary>
        ///     Get the hook orient of the top reinforcement
        /// </summary>
        /// <param name="geomInfo">the rebar geometry support information</param>
        /// <param name="location">the location of top rebar</param>
        /// <returns>the hook orient of the top hook</returns>
        private RebarHookOrientation GetTopHookOrient(RebarGeometry geomInfo, TopRebarLocation location)
        {
            // Top center rebar doesn't need hook.
            if (TopRebarLocation.Center == location)
                throw new Exception("Center top reinforcement doesn't have any hook.");

            // Get the hook direction, reinforcement normal and reinforcement line
            var hookVec = m_geometry.GetDownDirection();
            var normal = geomInfo.Normal;
            var rebarLine = geomInfo.Curves[0] as Line;

            // get the top start hook orient
            if (TopRebarLocation.Start == location)
            {
                var curveVec = GeomUtil.SubXYZ(rebarLine.GetEndPoint(0), rebarLine.GetEndPoint(1));
                return GeomUtil.GetHookOrient(curveVec, normal, hookVec);
            }
            else // get the top end hook orient
            {
                var curveVec = GeomUtil.SubXYZ(rebarLine.GetEndPoint(0), rebarLine.GetEndPoint(1));
                return GeomUtil.GetHookOrient(curveVec, normal, hookVec);
            }
        }


        /// <summary>
        ///     Create the reinforcement at the top of beam
        /// </summary>
        /// <returns>true if the creation is successful, otherwise false</returns>
        private bool FillTopItems(RebarContainer cont)
        {
            // create all kinds of top reinforcement according to the TopRebarLocation
            foreach (TopRebarLocation location in Enum.GetValues(typeof(TopRebarLocation)))
            {
                var item = FillTopItem(cont, location);
                //judge whether the top reinforcement creation is successful
                if (null == item) return false;
            }

            return true;
        }

        /// <summary>
        ///     Create the reinforcement at the top of beam, according to the top reinforcement location
        /// </summary>
        /// <param name="location">location of rebar which need to be created</param>
        /// <returns>the created reinforcement, return null if the creation is unsuccessful</returns>
        private RebarContainerItem FillTopItem(RebarContainer cont, TopRebarLocation location)
        {
            //get the geometry information of the reinforcement
            var geomInfo = m_geometry.GetTopRebar(location);

            RebarHookType startHookType = null; //the start hook type of the reinforcement
            RebarHookType endHookType = null; // the end hook type of the reinforcement
            RebarBarType rebarType = null; // the reinforcement type 
            var startOrient = RebarHookOrientation.Right; // the start hook orient
            var endOrient = RebarHookOrientation.Left; // the end hook orient

            // decide the reinforcement type, hook type and hook orient according to location
            switch (location)
            {
                case TopRebarLocation.Start:
                    startHookType = TopHookType; // start hook type
                    rebarType = TopEndRebarType; // reinforcement type
                    startOrient = GetTopHookOrient(geomInfo, location); // start hook orient
                    break;
                case TopRebarLocation.Center:
                    rebarType = TopCenterRebarType; // reinforcement type
                    break;
                case TopRebarLocation.End:
                    endHookType = TopHookType; // end hook type
                    rebarType = TopEndRebarType; // reinforcement type
                    endOrient = GetTopHookOrient(geomInfo, location); // end hook orient
                    break;
            }

            // create the container item
            return PlaceContainerItem(cont, rebarType, startHookType, endHookType, geomInfo, startOrient, endOrient);
        }
    }
}
