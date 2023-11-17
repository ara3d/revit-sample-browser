using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser
{
    public static class Extensions
    {
        public static IEnumerable<T> GetFilteredElements<T>(this Document doc) where T : class
            => new FilteredElementCollector(doc).OfClass(typeof(T)).OfType<T>();

        public static T GetElement<T>(this Document doc, ElementId id) where T : class
            => doc.GetElement(id) as T;

        public static IEnumerable<T> GetElements<T>(this Document doc, IEnumerable<ElementId> ids) where T : class
            => ids.Select(doc.GetElement<T>).Where(e => e != null);
    }
}
