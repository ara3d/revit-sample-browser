// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.FoundationSlab.CS
{
    /// <summary>
    ///     A class collecting all useful datas from revit API for UI.
    /// </summary>
    public class SlabData
    {
        // For finding elements and creating foundations slabs.
        private readonly UIApplication m_revit;
        // This set supplies all the regular slabs' datas for UI.
        private readonly List<RegularSlab> m_allBaseSlabList = new List<RegularSlab>();

        // A set of floors to find out all the regular slabs at the base of the building.
        private readonly List<Floor> m_floorList = new List<Floor>();

        // Foundation slab type for creating foundation slabs.
        private FloorType m_foundationSlabType;

        // A set of levels to find out the lowest level of the building.
        private readonly SortedList<double, Level> m_levelList = new SortedList<double, Level>();

        // A set of  the types of foundation slab.
        // This set supplies all the types of foundation slab for UI.
        private readonly List<FloorType> m_slabTypeList = new List<FloorType>();

        // A set of views to find out the regular slab's bounding box.
        private readonly List<View> m_viewList = new List<View>();

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="revit">An application object that contains data related to revit command.</param>
        public SlabData(UIApplication revit)
        {
            m_revit = revit;
            // Find out all useful elements.
            FindElements();
            // Get all base slabs. If no slab be found, throw an exception and return cancel.
            if (!GetAllBaseSlabs())
                throw new NullReferenceException("No planar slabs at the base of the building.");
        }

        /// <summary>
        ///     BaseSlabList property.
        ///     This property is for UI. It can be edited by user.
        /// </summary>
        public Collection<RegularSlab> BaseSlabList => new Collection<RegularSlab>(m_allBaseSlabList);

        /// <summary>
        ///     FoundationSlabTypeList property.
        ///     This property is for UI. It can not be edited by user.
        /// </summary>
        public ReadOnlyCollection<FloorType> FoundationSlabTypeList =>
            new ReadOnlyCollection<FloorType>(m_slabTypeList);

        /// <summary>
        ///     FoundationSlabType property.
        ///     This property gets value from UI to create foundation slabs.
        /// </summary>
        public object FoundationSlabType
        {
            set => m_foundationSlabType = value as FloorType;
        }

        /// <summary>
        ///     Check whether a regular slab is selected.
        /// </summary>
        /// <returns>The bool value suggest being selected or not.</returns>
        public bool CheckHaveSelected()
        {
            foreach (var slab in m_allBaseSlabList)
            {
                if (slab.Selected)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Change the Selected property for all regular slabs.
        /// </summary>
        /// <param name="value">The value for Selected property</param>
        public void ChangeAllSelected(bool value)
        {
            foreach (var slab in m_allBaseSlabList)
            {
                slab.Selected = value;
            }
        }

        /// <summary>
        ///     Create foundation slabs.
        /// </summary>
        /// <returns>The bool value suggest successful or not.</returns>
        public bool CreateFoundationSlabs()
        {
            // Create a foundation slab for each selected regular slab.
            foreach (var slab in m_allBaseSlabList)
            {
                if (!slab.Selected) continue;

                // Create a new slab.
                var t = new Transaction(m_revit.ActiveUIDocument.Document, Guid.NewGuid().GetHashCode().ToString());
                t.Start();

                var loop = new CurveLoop();
                foreach (Curve curve in slab.OctagonalProfile)
                {
                    loop.Append(curve);
                }

                var floorLoops = new List<CurveLoop> { loop };
                var foundationSlab = Floor.Create(m_revit.ActiveUIDocument.Document, floorLoops,
                    m_foundationSlabType.Id, m_levelList.Values[0].Id, true, null, 0.0);

                t.Commit();
                if (null == foundationSlab) return false;

                // Delete the regular slab.
                var t2 = new Transaction(m_revit.ActiveUIDocument.Document, Guid.NewGuid().GetHashCode().ToString());
                t2.Start();
                var deleteSlabId = slab.Id;
                m_revit.ActiveUIDocument.Document.Delete(deleteSlabId);
                t2.Commit();
            }

            return true;
        }

        /// <summary>
        ///     Find out all useful elements.
        /// </summary>
        private void FindElements()
        {
            IList<ElementFilter> filters = new List<ElementFilter>(4)
            {
                new ElementClassFilter(typeof(Level)),
                new ElementClassFilter(typeof(View)),
                new ElementClassFilter(typeof(Floor)),
                new ElementClassFilter(typeof(FloorType))
            };

            var orFilter = new LogicalOrFilter(filters);
            var collector = new FilteredElementCollector(m_revit.ActiveUIDocument.Document);
            var iterator = collector.WherePasses(orFilter).GetElementIterator();
            while (iterator.MoveNext())
            {
                switch (iterator.Current)
                {
                    // Find out all levels.
                    case Level level:
                        m_levelList.Add(level.Elevation, level);
                        continue;
                    // Find out all views.
                    case View view when !view.IsTemplate:
                        m_viewList.Add(view);
                        continue;
                    // Find out all floors.
                    case Floor floor:
                        m_floorList.Add(floor);
                        continue;
                }

                // Find out all foundation slab types.
                if (!(iterator.Current is FloorType floorType)) continue;
                if ("Structural Foundations" == floorType.Category.Name) m_slabTypeList.Add(floorType);
            }
        }

        /// <summary>
        ///     Get all base slabs.
        /// </summary>
        /// <returns>A bool value suggests successful or not.</returns>
        private bool GetAllBaseSlabs()
        {
            // No level, no slabs.
            if (0 == m_levelList.Count)
                return false;

            // Find out the lowest level's view for finding the bounding box of slab.
            View baseView = null;
            foreach (var view in m_viewList)
            {
                if (view.Name == m_levelList.Values[0].Name)
                    baseView = view;
            }

            if (null == baseView)
                return false;

            // Get all slabs at the base of the building.
            foreach (var floor in m_floorList)
            {
                if (floor.LevelId == m_levelList.Values[0].Id)
                {
                    var bbXyz = floor.get_BoundingBox(baseView); // Get the slab's bounding box.

                    // Check the floor. If the floor is planar, deal with it, otherwise, leap it.
                    if (!SampleBrowserUtils.IsPlanarFloor(bbXyz, floor, m_revit))
                        continue;

                    var floorProfile = SampleBrowserUtils.GetFloorProfile(floor, m_revit);
                    var regularSlab = new RegularSlab(floor, floorProfile, bbXyz); // Get a regular slab.
                    m_allBaseSlabList.Add(regularSlab); // Add regular slab to the set.
                }
            }

            // Getting regular slabs.
            return 0 != m_allBaseSlabList.Count;
        }
    }
}
