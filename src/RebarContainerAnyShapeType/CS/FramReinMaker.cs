// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RebarContainerAnyShapeType.CS
{
    /// <summary>
    ///     The interface for the family instance reinforcement creation.
    ///     The main method is Run(), which used to create the reinforcement
    /// </summary>
    public interface IFrameReinMaker
    {
        /// <summary>
        ///     Main function of Maker interface
        /// </summary>
        /// <returns>indicate the result of run</returns>
        bool Run();
    }


    /// <summary>
    ///     The base class for family instance reinforcement creation.
    ///     It only implement the Run() method. which give the flow process for creation.
    /// </summary>
    public class FramReinMaker : IFrameReinMaker
    {
        /// <summary>
        ///     a list to store all the hook types
        /// </summary>
        protected List<RebarHookType> m_hookTypes = new List<RebarHookType>();

        /// <summary>
        ///     the family instance to places reinforcement on
        /// </summary>
        protected readonly FamilyInstance m_hostObject;

        protected List<RebarBarType> m_rebarBottomTypes = new List<RebarBarType>();
        protected List<RebarBarType> m_rebarTopCenterTypes = new List<RebarBarType>();

        protected List<RebarBarType> m_rebarTopEndTypes = new List<RebarBarType>();
        protected List<RebarBarType> m_rebarTransverseCenterTypes = new List<RebarBarType>();
        protected List<RebarBarType> m_rebarTransverseEndTypes = new List<RebarBarType>();
        protected List<RebarBarType> m_rebarTransverseTypes = new List<RebarBarType>();

        /// <summary>
        ///     a set to store all the reinforcement types
        /// </summary>
        protected List<RebarBarType> m_rebarVerticalTypes = new List<RebarBarType>();

        /// <summary>
        ///     the API create handle
        /// </summary>
        protected readonly Document m_revitDoc;

        protected List<RebarHookType> m_topHookTypes = new List<RebarHookType>();
        protected List<RebarHookType> m_transverseHookTypes = new List<RebarHookType>();

        /// <summary>
        ///     The constructor of FramReinMaker
        /// </summary>
        /// <param name="commandData">the ExternalCommandData reference</param>
        /// <param name="hostObject">the host family instance</param>
        protected FramReinMaker(ExternalCommandData commandData, FamilyInstance hostObject)
        {
            // Get and store reinforcement create handle and host family instance
            m_revitDoc = commandData.Application.ActiveUIDocument.Document;
            m_hostObject = hostObject;

            // Get all the rebar types in revit
            if (!GetRebarTypes(commandData)) throw new Exception("Can't get any rebar type from revit.");

            // Get all the rebar hook types in revit
            if (!GetHookTypes(commandData)) throw new Exception("Can't get any rebar hook type from revit.");
        }

        /// <summary>
        ///     Show all the reinforcement bar types in Revit
        /// </summary>
        public IList<RebarBarType> VerticalRebarTypes => m_rebarVerticalTypes;

        public IList<RebarBarType> TransverseCenterRebarTypes => m_rebarTransverseCenterTypes;

        public IList<RebarBarType> TransverseEndRebarTypes => m_rebarTransverseEndTypes;

        public IList<RebarBarType> TopEndRebarTypes => m_rebarTopEndTypes;

        public IList<RebarBarType> TopCenterRebarTypes => m_rebarTopCenterTypes;

        public IList<RebarBarType> BottomRebarTypes => m_rebarBottomTypes;

        public IList<RebarBarType> TransverseRebarTypes => m_rebarTransverseTypes;

        /// <summary>
        ///     Show all the rebar hook types in revit
        /// </summary>
        public IList<RebarHookType> HookTypes => m_hookTypes;

        public IList<RebarHookType> TopHookTypes => m_topHookTypes;

        public IList<RebarHookType> TransversHookTypes => m_transverseHookTypes;

        /// <summary>
        ///     Implement the Run() method of IFrameReinMaker interface.
        ///     Give the flew process of the reinforcement creation.
        /// </summary>
        /// <returns></returns>
        bool IFrameReinMaker.Run()
        {
            // First, check the data whether is right and enough.
            if (!AssertData()) return false;

            // Second, show a form to the user to collect creation information
            if (!DisplayForm()) return false;

            // At last, begin to create the reinforcement 
            return FillWithBars();
        }


        /// <summary>
        ///     This is a virtual method which used to check the data whether is right and enough.
        /// </summary>
        /// <returns>true if the the data is right and enough, otherwise false.</returns>
        protected virtual bool AssertData()
        {
            return true; // only return true
        }

        /// <summary>
        ///     This is a virtual method which used to collect creation information
        /// </summary>
        /// <returns>true if the informatin collection is successful, otherwise false</returns>
        protected virtual bool DisplayForm()
        {
            return true; // only return true
        }


        /// <summary>
        ///     This is a virtual method which used to create reinforcement.
        /// </summary>
        /// <returns>true if the creation is successful, otherwise false</returns>
        protected virtual bool FillWithBars()
        {
            return true; // only return true
        }


        /// <summary>
        ///     A wrap function which used to create the reinforcement.
        /// </summary>
        /// <param name="rebarType">The element of RebarBarType</param>
        /// <param name="startHook">The element of start RebarHookType</param>
        /// <param name="endHook">The element of end RebarHookType</param>
        /// <param name="geomInfo">The goemetry information of the rebar</param>
        /// <param name="startOrient">An Integer defines the orientation of the start hook</param>
        /// <param name="endOrient">An Integer defines the orientation of the end hook</param>
        /// <returns></returns>
        protected RebarContainerItem PlaceContainerItem(RebarContainer cont, RebarBarType rebarType,
            RebarHookType startHook, RebarHookType endHook,
            RebarGeometry geomInfo, RebarHookOrientation startOrient, RebarHookOrientation endOrient)
        {
            var normal = geomInfo.Normal; // the direction of reinforcement distribution
            var curves = geomInfo.Curves; // the shape of the reinforcement curves

            var item = cont.AppendItemFromCurves(RebarStyle.Standard, rebarType, startHook, endHook, normal, curves,
                startOrient, endOrient, false, true);
            if (2 < geomInfo.RebarNumber && 0 < geomInfo.RebarSpacing)
                item.SetLayoutAsNumberWithSpacing(geomInfo.RebarNumber, geomInfo.RebarSpacing, true, true, true);

            return item;
        }


        /// <summary>
        ///     get all the hook types in current project, and store in m_hookTypes data
        /// </summary>
        /// <param name="commandData">the ExternalCommandData reference</param>
        /// <returns>true if some hook types can be gotton, otherwise false</returns>
        private bool GetHookTypes(ExternalCommandData commandData)
        {
            // Initialize the m_hookTypes which used to store all hook types.
            var filteredElementCollector =
                new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document);
            filteredElementCollector.OfClass(typeof(RebarHookType));
            m_hookTypes = filteredElementCollector.Cast<RebarHookType>().ToList();
            m_topHookTypes = filteredElementCollector.Cast<RebarHookType>().ToList();
            m_transverseHookTypes = filteredElementCollector.Cast<RebarHookType>().ToList();

            // If no hook types in Revit return false, otherwise true
            return 0 == m_hookTypes.Count ? false : true;
        }

        /// <summary>
        ///     get all the rebar types in current project, and store in m_rebarTypes data
        /// </summary>
        /// <param name="commandData">the ExternalCommandData reference</param>
        /// <returns>true if some rebar types can be got, otherwise false</returns>
        private bool GetRebarTypes(ExternalCommandData commandData)
        {
            // Initialize the List<RebarBarType> which used to store all reinforcement bar types.
            // Get all reinforcement bar types in Revit and store them in proper List<RebarBarType>
            var filteredElementCollector =
                new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document);
            filteredElementCollector.OfClass(typeof(RebarBarType));
            m_rebarVerticalTypes = filteredElementCollector.Cast<RebarBarType>().ToList();
            m_rebarTransverseCenterTypes = filteredElementCollector.Cast<RebarBarType>().ToList();
            m_rebarTransverseEndTypes = filteredElementCollector.Cast<RebarBarType>().ToList();

            m_rebarTopEndTypes = filteredElementCollector.Cast<RebarBarType>().ToList();
            m_rebarTopCenterTypes = filteredElementCollector.Cast<RebarBarType>().ToList();
            m_rebarBottomTypes = filteredElementCollector.Cast<RebarBarType>().ToList();
            m_rebarTransverseTypes = filteredElementCollector.Cast<RebarBarType>().ToList();

            // If no reinforcement bar types in Revit return false, otherwise true
            return 0 == m_rebarVerticalTypes.Count ? false : true;
        }
    }
}
