// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace RevitMultiSample.FoundationSlab.CS
{
    /// <summary>
    ///     A class collecting all useful datas from revit API for UI.
    /// </summary>
    public class SlabData
    {
        private const double PlanarPrecision = 0.00033;

        // For finding elements and creating foundations slabs.
        public static UIApplication Revit;
        public static Application CreApp;

        // A set of regular slabs at the base of the building.
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
            Revit = revit;
            CreApp = Revit.Application.Create;
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
                if (slab.Selected)
                    return true;
            return false;
        }

        /// <summary>
        ///     Change the Selected property for all regular slabs.
        /// </summary>
        /// <param name="value">The value for Selected property</param>
        public void ChangeAllSelected(bool value)
        {
            foreach (var slab in m_allBaseSlabList) slab.Selected = value;
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
                var t = new Transaction(Revit.ActiveUIDocument.Document, Guid.NewGuid().GetHashCode().ToString());
                t.Start();

                var loop = new CurveLoop();
                foreach (Curve curve in slab.OctagonalProfile) loop.Append(curve);

                var floorLoops = new List<CurveLoop> { loop };
                var foundationSlab = Floor.Create(Revit.ActiveUIDocument.Document, floorLoops,
                    m_foundationSlabType.Id, m_levelList.Values[0].Id, true, null, 0.0);

                t.Commit();
                if (null == foundationSlab) return false;

                // Delete the regular slab.
                var t2 = new Transaction(Revit.ActiveUIDocument.Document, Guid.NewGuid().GetHashCode().ToString());
                t2.Start();
                var deleteSlabId = slab.Id;
                Revit.ActiveUIDocument.Document.Delete(deleteSlabId);
                t2.Commit();
            }

            return true;
        }

        /// <summary>
        ///     Find out all useful elements.
        /// </summary>
        private void FindElements()
        {
            IList<ElementFilter> filters = new List<ElementFilter>(4);
            filters.Add(new ElementClassFilter(typeof(Level)));
            filters.Add(new ElementClassFilter(typeof(View)));
            filters.Add(new ElementClassFilter(typeof(Floor)));
            filters.Add(new ElementClassFilter(typeof(FloorType)));

            var orFilter = new LogicalOrFilter(filters);
            var collector = new FilteredElementCollector(Revit.ActiveUIDocument.Document);
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
                if (view.Name == m_levelList.Values[0].Name)
                    baseView = view;
            if (null == baseView)
                return false;

            // Get all slabs at the base of the building.
            foreach (var floor in m_floorList)
                if (floor.LevelId == m_levelList.Values[0].Id)
                {
                    var bbXyz = floor.get_BoundingBox(baseView); // Get the slab's bounding box.

                    // Check the floor. If the floor is planar, deal with it, otherwise, leap it.
                    if (!IsPlanarFloor(bbXyz, floor))
                        continue;

                    var floorProfile = GetFloorProfile(floor); // Get the slab's profile.
                    var regularSlab = new RegularSlab(floor, floorProfile, bbXyz); // Get a regular slab.
                    m_allBaseSlabList.Add(regularSlab); // Add regular slab to the set.
                }

            // Getting regular slabs.
            return 0 != m_allBaseSlabList.Count;
        }

        /// <summary>
        ///     Check whether the floor is planar.
        /// </summary>
        /// <param name="bbXyz">The floor's bounding box.</param>
        /// <param name="floor">The floor object.</param>
        /// <returns>A bool value suggests the floor is planar or not.</returns>
        private static bool IsPlanarFloor(BoundingBoxXYZ bbXyz, Floor floor)
        {
            // Get floor thickness.
            var floorThickness = 0.0;
            var floorType = Revit.ActiveUIDocument.Document.GetElement(floor.GetTypeId()) as ElementType;
            var attribute = floorType.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM);
            if (null != attribute) floorThickness = attribute.AsDouble();

            // Get bounding box thickness.
            var boundThickness = Math.Abs(bbXyz.Max.Z - bbXyz.Min.Z);

            // Planar or not.
            return Math.Abs(boundThickness - floorThickness) < PlanarPrecision;
        }

        /// <summary>
        ///     Get a floor's profile.
        /// </summary>
        /// <param name="floor">The floor whose profile you want to get.</param>
        /// <returns>The profile of the floor.</returns>
        private CurveArray GetFloorProfile(Floor floor)
        {
            var floorProfile = new CurveArray();
            // Structural slab's profile can be found in it's analytical element.
            var document = floor.Document;
            AnalyticalPanel analyticalModel = null;
            var relManager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            if (relManager != null)
            {
                var associatedElementId = relManager.GetAssociatedElementId(floor.Id);
                if (associatedElementId != ElementId.InvalidElementId)
                {
                    var associatedElement = document.GetElement(associatedElementId);
                    if (associatedElement != null && associatedElement is AnalyticalPanel panel)
                        analyticalModel = panel;
                }
            }

            if (null != analyticalModel)
            {
                IList<Curve> curveList = analyticalModel.GetOuterContour().ToList();

                foreach (var curve in curveList)
                    floorProfile.Append(curve);

                return floorProfile;
            }

            // Nonstructural floor's profile can be formed through it's Geometry.
            var aOptions = Revit.Application.Create.NewGeometryOptions();
            var aElementOfGeometry = floor.get_Geometry(aOptions);
            //GeometryObjectArray geometryObjects = aElementOfGeometry.Objects;
            var objects = aElementOfGeometry.GetEnumerator();
            //foreach (GeometryObject o in geometryObjects)
            while (objects.MoveNext())
            {
                var o = objects.Current;

                var solid = o as Solid;
                if (null == solid)
                    continue;

                // Form the floor's profile through solid's edges.
                var edges = solid.Edges;
                for (var i = 0; i < edges.Size / 3; i++)
                {
                    var edge = edges.get_Item(i);
                    var xyzArray = edge.Tessellate() as List<XYZ>; // A set of points.
                    for (var j = 0; j < xyzArray.Count - 1; j++)
                    {
                        var startPoint = xyzArray[j];
                        var endPoint = xyzArray[j + 1];
                        var line = Line.CreateBound(startPoint, endPoint);

                        floorProfile.Append(line);
                    }
                }
            }

            return floorProfile;
        }
    }
}
