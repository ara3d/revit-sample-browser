// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.AutoJoin.CS
{
    public class AutoJoin
    {
        private readonly List<CombinableElement> m_elements;

        public AutoJoin()
        {
            m_elements = new List<CombinableElement>();
        }

        public int Join(Document document)
        {
            var combinated = 0;

            // CombinableElement exists in the API but not in Revit's native object model; collect GenericForm and GeomCombination instead.
            var filter = new LogicalOrFilter(
                new ElementClassFilter(typeof(GenericForm)),
                new ElementClassFilter(typeof(GeomCombination)));

            var itor = new FilteredElementCollector(document).WherePasses(filter).GetElementIterator();
            itor.Reset();
            while (itor.MoveNext())
            {
                if (itor.Current is GenericForm gf && !gf.IsSolid)
                    continue;

                if (!(itor.Current is CombinableElement ce))
                    continue;
                m_elements.Add(ce);
            }

            while (1 < m_elements.Count)
            {
                var geomCombination = JoinOverlapping(m_elements, document);
                if (null == geomCombination) return combinated;

                combinated++;
            }

            return combinated;
        }

        public GeomCombination JoinOverlapping(List<CombinableElement> elements, Document document)
        {
            var joinedElements = new CombinableElementArray();

            foreach (var aElement in elements)
            {
                foreach (var xElement in elements)
                {
                    if (IsOverlapped(aElement, xElement))
                    {
                        joinedElements.Append(aElement);
                        break;
                    }
                }

                if (1 == joinedElements.Size)
                    break;
            }

            if (0 == joinedElements.Size) return null;

            foreach (var aElement in elements)
            {
                if (IsOverlapped(aElement, joinedElements.get_Item(0)))
                    joinedElements.Append(aElement);
            }

            var allCanJoin = new List<CombinableElement>();
            var isNew = false;
            do
            {
                allCanJoin.Clear();
                isNew = false;

                foreach (CombinableElement aElement in joinedElements)
                {
                    foreach (var xElement in elements)
                    {
                        if (IsOverlapped(aElement, xElement))
                            allCanJoin.Add(xElement);
                    }
                }

                foreach (var aElement in allCanJoin)
                {
                    var isContained = false;

                    for (var ii = 0; ii < joinedElements.Size; ii++)
                        if (aElement.Id == joinedElements.get_Item(ii).Id)
                        {
                            isContained = true;
                            break;
                        }

                    if (!isContained)
                    {
                        isNew = true;
                        joinedElements.Append(aElement);
                    }
                }
            } while (isNew);

            foreach (CombinableElement aElement in joinedElements)
            {
                elements.Remove(aElement);
            }

            return document.CombineElements(joinedElements);
        }

        public bool IsOverlapped(CombinableElement elementA, CombinableElement elementB)
        {
            if (elementA.Id == elementB.Id) return false;

            var geOptions = Command.SAppCreation.NewGeometryOptions();
            return Intersection.IsOverlapped(elementA.get_Geometry(geOptions), elementB.get_Geometry(geOptions));
        }
    }
}
