#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    internal class ElementInLinkSelectionFilter<T> : ISelectionFilter where T : Element
    {
        private readonly Document _doc;

        public ElementInLinkSelectionFilter(Document doc)
        {
            _doc = doc;
        }

        public Document LinkedDocument { get; private set; }

        public bool LastCheckedWasFromLink => null != LinkedDocument;

        public bool AllowElement(Element e)
        {
            return true;
        }

        public bool AllowReference(Reference r, XYZ p)
        {
            LinkedDocument = null;

            var e = _doc.GetElement(r);

            if (e is RevitLinkInstance li)
            {
                LinkedDocument = li.GetLinkDocument();

                e = LinkedDocument.GetElement(r.LinkedElementId);
            }

            return e is T;
        }
    }
}
