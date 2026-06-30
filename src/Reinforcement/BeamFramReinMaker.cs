// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;
using RebarGeomHelper = Ara3D.RevitSampleBrowser.Common.Structural.RebarGeometry;
namespace Ara3D.RevitSampleBrowser.Reinforcement.CS
{
    public class BeamFramReinMaker : FramReinMaker
    {
        private readonly BeamGeometrySupport m_geometry; // The geometry support for beam rebar creation

        // The rebar type, hook type and spacing information


        public BeamFramReinMaker(ExternalCommandData commandData, FamilyInstance hostObject)
            : base(commandData, hostObject)
        {
            var geoOptions = commandData.Application.Application.Create.NewGeometryOptions();
            geoOptions.ComputeReferences = true;

            m_geometry = new BeamGeometrySupport(hostObject, geoOptions);
        }

        public RebarBarType TopEndRebarType { get; set; }

        public RebarBarType TopCenterRebarType { get; set; }

        public RebarBarType BottomRebarType { get; set; }

        public RebarBarType TransverseRebarType { get; set; }

        public double TransverseEndSpacing
        {
            get;
            set
            {
                if (0 > value) throw new Exception("Transverse end spacing should be above zero");
                field = value;
            }
        }

        public double TransverseCenterSpacing
        {
            get;
            set
            {
                if (0 > value) throw new Exception("Transverse center spacing should be above zero");
                field = value;
            }
        }

        public RebarHookType TopHookType { get; set; }

        public RebarHookType TransverseHookType { get; set; }

        protected override bool AssertData()
        {
            return base.AssertData();
        }

        protected override bool DisplayForm()
        {
            // Display BeamFramReinMakerForm for the user to input information 
            using (BeamFramReinMakerForm displayForm = new(this))
            {
                if (DialogResult.OK != displayForm.ShowDialog()) return false;
            }

            return base.DisplayForm();
        }

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

        public bool FillBottomBars()
        {
            // get the geometry information of the bottom rebar
            var geomInfo = m_geometry.GetBottomRebar();

            // create the rebar
            var rebar = PlaceRebars(BottomRebarType, null, null, geomInfo,
                RebarHookOrientation.Left, RebarHookOrientation.Left);
            return null != rebar;
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
        ///     Create the transverse rebars, according to the location of transverse rebars
        /// </summary>
        /// <param name="location">location of rebar which need to be created</param>
        /// <returns>the created rebar, return null if the creation is unsuccessful</returns>
        public Rebar FillTransverseBar(TransverseRebarLocation location)
        {
            // Get the geometry information which support rebar creation
            RebarGeometry geomInfo = new();
            switch (location)
            {
                case TransverseRebarLocation.Start: // start transverse rebar
                case TransverseRebarLocation.End: // end transverse rebar
                    geomInfo = m_geometry.GetTransverseRebar(location, TransverseEndSpacing);
                    break;
                case TransverseRebarLocation.Center: // center transverse rebar
                    geomInfo = m_geometry.GetTransverseRebar(location, TransverseCenterSpacing);
                    break;
            }

            var startHook = RebarHookOrientation.Right;
            var endHook = RebarHookOrientation.Left;
            if (!RebarGeomHelper.IsInRightDir(geomInfo.Normal))
            {
                startHook = RebarHookOrientation.Left;
                endHook = RebarHookOrientation.Right;
            }

            // create the rebar
            return PlaceRebars(TransverseRebarType, TransverseHookType, TransverseHookType,
                geomInfo, startHook, endHook);
        }

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
                var curveVec = XyzMath.SubXyz(rebarLine.GetEndPoint(1), rebarLine.GetEndPoint(0));
                return RebarGeomHelper.GetHookOrient(curveVec, normal, hookVec);
            }
            else // get the top end hook orient
            {
                var curveVec = XyzMath.SubXyz(rebarLine.GetEndPoint(0), rebarLine.GetEndPoint(1));
                return RebarGeomHelper.GetHookOrient(curveVec, normal, hookVec);
            }
        }

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
