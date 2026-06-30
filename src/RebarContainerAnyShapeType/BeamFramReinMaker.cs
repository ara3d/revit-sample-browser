// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;
using RebarGeomHelper = Ara3D.RevitSampleBrowser.Common.Structural.RebarGeometry;
namespace Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS
{
    /// <summary>
    ///     The class derived from FramReinMaker shows how to create the reinforcement for a beam
    /// </summary>
    public class BeamFramReinMaker : FramReinMaker
    {
        private readonly BeamGeometrySupport m_geometry; // The geometry support for beam reinforcement creation

        // The reinforcement type, hook type and spacing information


        public BeamFramReinMaker(ExternalCommandData commandData, FamilyInstance hostObject)
            : base(commandData, hostObject)
        {
            //create new options for current project
            var geoOptions = commandData.Application.Application.Create.NewGeometryOptions();
            geoOptions.ComputeReferences = true;

            //create a BeamGeometrySupport instance.
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
            //create Rebar Container
            var conTypeId = RebarContainerType.CreateDefaultRebarContainerType(RevitDoc);
            var cont = RebarContainer.Create(RevitDoc, HostObject, conTypeId);

            // create the top items
            var flag = FillTopItems(cont);

            // create the bottom items
            flag = flag && FillBottomItems(cont);

            // create the transverse items
            flag = flag && FillTransverseItems(cont);

            return base.FillWithBars();
        }

        public bool FillBottomItems(RebarContainer cont)
        {
            // get the geometry information of the bottom reinforcement
            var geomInfo = m_geometry.GetBottomRebar();

            // create the container item
            var item = PlaceContainerItem(cont, BottomRebarType, null, null, geomInfo, RebarHookOrientation.Left,
                RebarHookOrientation.Left);
            return null != item;
        }

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

        public RebarContainerItem FillTransverseItem(RebarContainer cont, TransverseRebarLocation location)
        {
            // Get the geometry information which support reinforcement creation
            RebarGeometry geomInfo = new();
            switch (location)
            {
                case TransverseRebarLocation.Start: // start transverse reinforcement
                case TransverseRebarLocation.End: // end transverse reinforcement
                    geomInfo = m_geometry.GetTransverseRebar(location, TransverseEndSpacing);
                    break;
                case TransverseRebarLocation.Center: // center transverse reinforcement
                    geomInfo = m_geometry.GetTransverseRebar(location, TransverseCenterSpacing);
                    break;
            }

            var startHook = RebarHookOrientation.Left;
            var endHook = RebarHookOrientation.Left;
            if (!RebarGeomHelper.IsInRightDir(geomInfo.Normal))
            {
                startHook = RebarHookOrientation.Right;
                endHook = RebarHookOrientation.Right;
            }

            // create the container item
            return PlaceContainerItem(cont, TransverseRebarType, TransverseHookType, TransverseHookType, geomInfo,
                startHook, endHook);
        }

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
                var curveVec = XyzMath.SubXyz(rebarLine.GetEndPoint(0), rebarLine.GetEndPoint(1));
                return RebarGeomHelper.GetHookOrient(curveVec, normal, hookVec);
            }
            else // get the top end hook orient
            {
                var curveVec = XyzMath.SubXyz(rebarLine.GetEndPoint(0), rebarLine.GetEndPoint(1));
                return RebarGeomHelper.GetHookOrient(curveVec, normal, hookVec);
            }
        }

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
