// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.Samples.DirectionCalculation
{
    /// <summary>
    ///     Implementation class for utilities to find south facing windows in a project.
    /// </summary>
    public class FindSouthFacingWindows : FindSouthFacingBase
    {
        /// <summary>
        ///     The implementation of the FindSouthFacingWindows command.
        /// </summary>
        /// <param name="useProjectLocationNorth">
        ///     true to use the active project location's north/south direction.
        ///     false to use the default coordinate system's north/south (Y-axis).
        /// </param>
        protected void Execute(bool useProjectLocationNorth)
        {
            var uiDoc = new UIDocument(Document);
            var selElements = new ElementSet();
            foreach (var elementId in uiDoc.Selection.GetElementIds())
                selElements.Insert(uiDoc.Document.GetElement(elementId));

            var windows = CollectWindows();
            foreach (var window in windows)
            {
                var exteriorDirection = GetWindowDirection(window);

                if (useProjectLocationNorth)
                    exteriorDirection = TransformByProjectLocation(exteriorDirection);

                var isSouthFacing = IsSouthFacing(exteriorDirection);
                if (isSouthFacing)
                    selElements.Insert(window);
            }

            // Select all windows which had the proper direction.
            var elemIdList = new List<ElementId>();
            foreach (Element element in selElements) elemIdList.Add(element.Id);
            uiDoc.Selection.SetElementIds(elemIdList);
        }

        /// <summary>
        ///     Finds all windows in the active document.
        /// </summary>
        /// <returns>An enumerable containing all windows.</returns>
        protected IEnumerable<FamilyInstance> CollectWindows()
        {
            // Windows are family instances whose category is correctly set.

            var familyInstanceFilter = new ElementClassFilter(typeof(FamilyInstance));
            var windowCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            var andFilter = new LogicalAndFilter(familyInstanceFilter, windowCategoryFilter);
            var collector = new FilteredElementCollector(Document);
            ICollection<Element> elementsToProcess = collector.WherePasses(andFilter).ToElements();

            // Convert to IEnumerable of FamilyInstance using LINQ
            var windows = from window in elementsToProcess.Cast<FamilyInstance>() select window;

            return windows;
        }

        /// <summary>
        ///     Obtains the facing direction of the window.
        /// </summary>
        /// <param name="wall">The window.</param>
        /// <returns>A normalized XYZ direction vector.</returns>
        protected XYZ GetWindowDirection(FamilyInstance window)
        {
            var options = new Options();

            // Extract the geometry of the window.
            var geomElem = window.get_Geometry(options);

            //foreach (GeometryObject geomObj in geomElem.Objects)
            var objects = geomElem.GetEnumerator();
            while (objects.MoveNext())
            {
                var geomObj = objects.Current;

                // We expect there to be one main Instance in each window.  Ignore the rest of the geometry.
                var instance = geomObj as GeometryInstance;
                if (instance != null)
                {
                    // Obtain the Instance's transform and the nominal facing direction (Y-direction).
                    var t = instance.Transform;
                    var facingDirection = t.BasisY;

                    // If the window is flipped in one direction, but not the other, the transform is left handed.  
                    // The Y direction needs to be reversed to obtain the facing direction.
                    if ((window.FacingFlipped && !window.HandFlipped) || (!window.FacingFlipped && window.HandFlipped))
                        facingDirection = -facingDirection;

                    // Because the need to perform this operation on instances is so common, 
                    // the Revit API exposes this calculation directly as the FacingOrientation property 
                    // as shown in GetWindowDirectionAlternate()

                    return facingDirection;
                }
            }

            return XYZ.BasisZ;
        }

        /// <summary>
        ///     Alternate way to obtain the facing direction for the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>A normalized XYZ direction vector.</returns>
        protected XYZ GetWindowDirectionAlternate(FamilyInstance window)
        {
            return window.FacingOrientation;
        }
    }
}
