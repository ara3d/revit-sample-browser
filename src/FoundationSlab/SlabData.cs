// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Ara3D.RevitSampleBrowser.FoundationSlab.CS
{
    public class SlabData
    {
        private readonly UIApplication m_revit;
        private readonly List<RegularSlab> m_allBaseSlabList = [];
        private readonly List<Floor> m_floorList = [];
        private FloorType m_foundationSlabType;
        private readonly SortedList<double, Level> m_levelList = [];
        private readonly List<FloorType> m_slabTypeList = [];
        private readonly List<View> m_viewList = [];

        public SlabData(UIApplication revit)
        {
            m_revit = revit;
            FindElements();
            if (!GetAllBaseSlabs())
                throw new NullReferenceException("No planar slabs at the base of the building.");
        }

        public Collection<RegularSlab> BaseSlabList => new(m_allBaseSlabList);

        public ReadOnlyCollection<FloorType> FoundationSlabTypeList =>
            new(m_slabTypeList);

        public object FoundationSlabType
        {
            set => m_foundationSlabType = value as FloorType;
        }

        public bool CheckHaveSelected()
        {
            foreach (var slab in m_allBaseSlabList)
            {
                if (slab.Selected)
                    return true;
            }

            return false;
        }

        public void ChangeAllSelected(bool value)
        {
            foreach (var slab in m_allBaseSlabList)
            {
                slab.Selected = value;
            }
        }

        public bool CreateFoundationSlabs()
        {
            foreach (var slab in m_allBaseSlabList)
            {
                if (!slab.Selected) continue;

                Transaction t = new(m_revit.ActiveUIDocument.Document, Guid.NewGuid().GetHashCode().ToString());
                t.Start();

                CurveLoop loop = new();
                foreach (Curve curve in slab.OctagonalProfile)
                {
                    loop.Append(curve);
                }

                List<CurveLoop> floorLoops = new()
                { loop };
                var foundationSlab = Floor.Create(m_revit.ActiveUIDocument.Document, floorLoops,
                    m_foundationSlabType.Id, m_levelList.Values[0].Id, true, null, 0.0);

                t.Commit();
                if (null == foundationSlab) return false;

                Transaction t2 = new(m_revit.ActiveUIDocument.Document, Guid.NewGuid().GetHashCode().ToString());
                t2.Start();
                var deleteSlabId = slab.Id;
                m_revit.ActiveUIDocument.Document.Delete(deleteSlabId);
                t2.Commit();
            }

            return true;
        }

        private void FindElements()
        {
            IList<ElementFilter> filters =
            [
                new ElementClassFilter(typeof(Level)),
                new ElementClassFilter(typeof(View)),
                new ElementClassFilter(typeof(Floor)),
                new ElementClassFilter(typeof(FloorType))
            ];

            LogicalOrFilter orFilter = new(filters);
            FilteredElementCollector collector = new(m_revit.ActiveUIDocument.Document);
            var iterator = collector.WherePasses(orFilter).GetElementIterator();
            while (iterator.MoveNext())
            {
                switch (iterator.Current)
                {
                    case Level level:
                        m_levelList.Add(level.Elevation, level);
                        continue;
                    case View view when !view.IsTemplate:
                        m_viewList.Add(view);
                        continue;
                    case Floor floor:
                        m_floorList.Add(floor);
                        continue;
                }

                if (iterator.Current is not FloorType floorType) continue;
                if ("Structural Foundations" == floorType.Category.Name) m_slabTypeList.Add(floorType);
            }
        }

        private bool GetAllBaseSlabs()
        {
            if (0 == m_levelList.Count)
                return false;

            View baseView = null;
            foreach (var view in m_viewList)
            {
                if (view.Name == m_levelList.Values[0].Name)
                    baseView = view;
            }

            if (null == baseView)
                return false;

            foreach (var floor in m_floorList)
            {
                if (floor.LevelId == m_levelList.Values[0].Id)
                {
                    var bbXyz = floor.get_BoundingBox(baseView);

                    if (!SampleBrowserUtils.IsPlanarFloor(bbXyz, floor, m_revit))
                        continue;

                    var floorProfile = SampleBrowserUtils.GetFloorProfile(floor, m_revit);
                    RegularSlab regularSlab = new(floor, floorProfile, bbXyz);
                    m_allBaseSlabList.Add(regularSlab);
                }
            }

            return 0 != m_allBaseSlabList.Count;
        }
    }
}
