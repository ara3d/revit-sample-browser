using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser
{
    public static class Extensions
    {
        // For certain types, we need to use either the regular collector or a parent class. 
        // See: https://www.revitapidocs.com/2017/4b7fb6d7-cb9c-d556-56fc-003a0b8a51b7.htm
        // This looks slow, but it is only called once when starting the collector so it is O(1) compared to O(N) where N = number of elements. 
        public static FilteredElementCollector GetCollector(this FilteredElementCollector collector, Type t)
        {
            if (typeof(Material).IsAssignableFrom(t)) return collector;
            if (typeof(ConnectorElement).IsAssignableFrom(t)) return collector;
            if (typeof(HostedSweep).IsAssignableFrom(t)) return collector.OfClass(typeof(HostObject));
            if (typeof(SpatialElement).IsAssignableFrom(t)) return collector.OfClass(typeof(SpatialElement));
            if (typeof(SpatialElementTag).IsAssignableFrom(t)) return collector.OfClass(typeof(SpatialElementTag));
            if (typeof(CombinableElement).IsAssignableFrom(t)) return collector;
            if (typeof(Mullion).IsAssignableFrom(t)) return collector.OfClass(typeof(FamilyInstance));
            if (typeof(Panel).IsAssignableFrom(t)) return collector.OfClass(typeof(FamilyInstance));
            if (typeof(AnnotationSymbol).IsAssignableFrom(t)) return collector.OfClass(typeof(FamilySymbol));
            if (typeof(AreaReinforcementType).IsAssignableFrom(t)) return collector.OfClass(typeof(ElementType));
            if (typeof(PathReinforcementType).IsAssignableFrom(t)) return collector.OfClass(typeof(ElementType));
            if (typeof(AnnotationSymbolType).IsAssignableFrom(t)) return collector.OfClass(typeof(FamilyType));
            if (typeof(RoomTagType).IsAssignableFrom(t)) return collector.OfClass(typeof(FamilySymbol));
            if (typeof(SpaceTagType).IsAssignableFrom(t)) return collector.OfClass(typeof(FamilySymbol));
            if (typeof(TrussType).IsAssignableFrom(t)) return collector.OfClass(typeof(FamilySymbol));
            return collector.OfClass(t);
        }

        public static IEnumerable<T> GetFilteredElements<T>(this Document doc) where T : Element
            => new FilteredElementCollector(doc).GetCollector(typeof(T)).OfType<T>();

        public static T GetElement<T>(this Document doc, ElementId id) where T : class
            => doc.GetElement(id) as T;

        public static IEnumerable<T> GetElements<T>(this Document doc, IEnumerable<ElementId> ids) where T : class
            => ids.Select(doc.GetElement<T>).Where(e => e != null);

        public static View GetNamedView(this Document doc, string name)
            => doc.GetFilteredElements<View>().FirstOrDefault(v => v.Name == name);

    }
}
