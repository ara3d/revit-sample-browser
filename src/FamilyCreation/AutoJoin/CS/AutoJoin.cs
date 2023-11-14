// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitMultiSample.AutoJoin.CS
{
    /// <summary>
    ///     Join all the overlapping generic forms in this document.
    /// </summary>
    public class AutoJoin
    {
        private readonly List<CombinableElement> m_elements; // this element list to combine geometry.

        /// <summary>
        ///     Constructor
        /// </summary>
        public AutoJoin()
        {
            m_elements = new List<CombinableElement>();
        }

        /// <summary>
        ///     Join geometry between overlapping solids.
        /// </summary>
        /// <param name="document">The active document</param>
        /// <returns>The number of geometry combination be joined in this document.</returns>
        public int Join(Document document)
        {
            var combinated = 0;

            // CombinableElement is of an element type that exists in the API, but not in Revit's native object model. 
            // We use a combination of GenericForm and GeomCombination elements instead to find all CombinableElement.
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
            // Added all solid forms in this document.

            while (1 < m_elements.Count)
            {
                var geomCombination = JoinOverlapping(m_elements, document);
                if (null == geomCombination) return combinated; //No overlapping.

                combinated++;
            }

            return combinated;
        }

        /// <summary>
        ///     Join the overlapped elements in the list.
        /// </summary>
        /// <param name="elements">the element list may includes overlapping.</param>
        /// <param name="document">the active document</param>
        /// <returns>the joined geometry combination, the joined elements is removed from the list.</returns>
        public GeomCombination JoinOverlapping(List<CombinableElement> elements, Document document)
        {
            var joinedElements = new CombinableElementArray();

            // try to find the first overlapping.
            foreach (var aElement in elements)
            {
                foreach (var xElement in elements)
                    if (IsOverlapped(aElement, xElement))
                    {
                        joinedElements.Append(aElement);
                        break;
                    }

                if (1 == joinedElements.Size)
                    break;
            }

            if (0 == joinedElements.Size) return null; //Can not find any overlapping.

            // try to find all elements overlapped the first element.
            foreach (var aElement in elements)
                if (IsOverlapped(aElement, joinedElements.get_Item(0)))
                    joinedElements.Append(aElement);

            var allCanJoin = new List<CombinableElement>();
            var isNew = false;
            do
            {
                allCanJoin.Clear();
                isNew = false;

                // try to find all elements overlapped joinedElements
                foreach (CombinableElement aElement in joinedElements)
                foreach (var xElement in elements)
                    if (IsOverlapped(aElement, xElement))
                        allCanJoin.Add(xElement);

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
            } while (isNew); // find all elements which overlapped with joined geometry combination.

            // removed the joined elements from the input list.
            foreach (CombinableElement aElement in joinedElements) elements.Remove(aElement);

            return document.CombineElements(joinedElements);
        }

        /// <summary>
        ///     Tell if the element A and B are overlapped.
        /// </summary>
        /// <param name="elementA">element A</param>
        /// <param name="elementB">element B</param>
        /// <returns>return true if A and B are overlapped, or else return false.</returns>
        public bool IsOverlapped(CombinableElement elementA, CombinableElement elementB)
        {
            if (elementA.Id == elementB.Id) return false;

            var geOptions = Command.SAppCreation.NewGeometryOptions();
            return Intersection.IsOverlapped(elementA.get_Geometry(geOptions), elementB.get_Geometry(geOptions));
        }
    }
}
