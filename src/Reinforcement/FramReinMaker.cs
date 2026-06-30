// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.Reinforcement.CS
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
        private List<RebarHookType> m_hookTypes = [];

        private readonly FamilyInstance m_hostObject;

        private List<RebarBarType> m_rebarTypes = [];

        private readonly Document m_revitDoc;

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
        ///     Show all the rebar types in revit
        /// </summary>
        public IList<RebarBarType> RebarTypes => m_rebarTypes;

        /// <summary>
        ///     Show all the rebar hook types in revit
        /// </summary>
        public IList<RebarHookType> HookTypes => m_hookTypes;

        bool IFrameReinMaker.Run()
        {
            // First, check the data whether is right and enough.
            if (!AssertData()) return false;

            // Second, show a form to the user to collect creation information
            if (!DisplayForm()) return false;

            // At last, begin to create the reinforcement rebars
            return FillWithBars();
        }

        protected virtual bool AssertData()
        {
            return true; // only return true
        }

        protected virtual bool DisplayForm()
        {
            return true; // only return true
        }

        protected virtual bool FillWithBars()
        {
            return true; // only return true
        }

        protected static void SetRebarSpaceAndNumber(Rebar bar, int number, double spacing)
        {
            // Asset the parameter is valid
            if (null == bar || 2 > number || 0 > spacing) return;

            // Change the rebar number and spacing properties
            bar.GetShapeDrivenAccessor().SetLayoutAsNumberWithSpacing(number, spacing, true, true, true);
        }

        protected Rebar PlaceRebars(RebarBarType rebarType, RebarHookType startHook,
            RebarHookType endHook, RebarGeometry geomInfo,
            RebarHookOrientation startOrient, RebarHookOrientation endOrient)
        {
            var normal = geomInfo.Normal; // the direction of rebar distribution
            var curves = geomInfo.Curves; // the shape of the rebar curves

            var createdRebar = Rebar.CreateFromCurves(m_revitDoc, RebarStyle.Standard, rebarType, startHook, endHook,
                m_hostObject, normal, curves,
                startOrient, endOrient, false, true);

            if (null == createdRebar) // Assert the creation is successful
                return null;

            // Change the rebar number and spacing properties to the user wanted
            SetRebarSpaceAndNumber(createdRebar, geomInfo.RebarNumber, geomInfo.RebarSpacing);
            return createdRebar;
        }

        private bool GetHookTypes(ExternalCommandData commandData)
        {
            // Initialize the m_hookTypes which used to store all hook types.
            FilteredElementCollector filteredElementCollector =
                new(commandData.Application.ActiveUIDocument.Document);
            filteredElementCollector.OfClass(typeof(RebarHookType));
            m_hookTypes = filteredElementCollector.Cast<RebarHookType>().ToList();

            // If no hook types in revit return false, otherwise true
            return 0 != m_hookTypes.Count;
        }

        private bool GetRebarTypes(ExternalCommandData commandData)
        {
            // Initialize the m_rebarTypes which used to store all rebar types.
            // Get all rebar types in revit and add them in m_rebarTypes
            FilteredElementCollector filteredElementCollector =
                new(commandData.Application.ActiveUIDocument.Document);
            filteredElementCollector.OfClass(typeof(RebarBarType));
            m_rebarTypes = filteredElementCollector.Cast<RebarBarType>().ToList();

            // If no rebar types in revit return false, otherwise true
            return 0 != m_rebarTypes.Count;
        }
    }
}
