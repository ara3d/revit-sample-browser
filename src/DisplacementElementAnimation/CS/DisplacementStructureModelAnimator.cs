// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace Revit.SDK.Samples.DisplacementElementAnimation.CS
{
    /// <summary>
    ///     A class that executes an animation of structural model elements using DisplacementElements.
    /// </summary>
    internal class DisplacementStructureModelAnimator
    {
        /// <summary>
        ///     The increment by which the displacement parameter is reduced during animation.
        /// </summary>
        private readonly double m_displacementIncrement = 0.05;

        /// <summary>
        ///     The displacement parameter (proceeds from 1 -> 0 during the animation)
        /// </summary>
        private double m_displacementParameter = 1.0;

        /// <summary>
        ///     The initial height for the initial displacement.
        /// </summary>
        private readonly double m_initialHeight = 100;

        /// <summary>
        ///     The maximum ratio of displacement in XY.
        /// </summary>
        private readonly double m_initialXyRatio = 1.25;

        /// <summary>
        ///     The index of the current parent element being animated.
        /// </summary>
        private int m_currentDisplacementIndex;

        /// <summary>
        ///     The current parent displacement element being animated.
        /// </summary>
        private DisplacementElement m_displacementElement;

        /// <summary>
        ///     The collection of top level displacement elements.
        /// </summary>
        private List<DisplacementElement> m_displacementElements;

        /// <summary>
        ///     The switch for using Idling event.
        /// </summary>
        private readonly bool m_isUsingIdling = true;

        /// <summary>
        ///     The timer that governs the automation.
        /// </summary>
        private Timer m_timer;

        /// <summary>
        ///     The model center.  Currently hardcoded.
        /// </summary>
        private readonly XYZ m_modelCenter = XYZ.Zero;

        /// <summary>
        ///     The number of milliseconds in between frames.
        /// </summary>
        private readonly int m_timerInterval = 60;

        /// <summary>
        ///     Signals that the timer has triggered.
        /// </summary>
        private bool m_timerTripped;

        /// <summary>
        ///     The application.
        /// </summary>
        private readonly UIApplication m_uiApplication;

        /// <summary>
        ///     Constructs an animator instance.
        /// </summary>
        /// <param name="uiApp">The application.</param>
        /// <param name="isUsingIdling">The switch for using Idling event.</param>
        public DisplacementStructureModelAnimator(UIApplication uiApp, bool isUsingIdling)
        {
            m_uiApplication = uiApp;
            m_isUsingIdling = isUsingIdling;
        }

        /// <summary>
        ///     Starts the animation by creating the needed displacement elements, and setting up the events to allow it proceed.
        /// </summary>
        public void StartAnimation()
        {
            // Instantiate or reset animation variables
            m_displacementElements = new List<DisplacementElement>();
            m_currentDisplacementIndex = 0;

            var uiDoc = m_uiApplication.ActiveUIDocument;
            var view = uiDoc.ActiveView;
            var doc = uiDoc.Document;

            // Get levels in order of elevation
            var collector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            var idGroupsInOrder = new List<ICollection<ElementId>>();
            IEnumerable<Level> levels = collector.Cast<Level>().OrderBy(lvl => lvl.Elevation);

            // Create lists of "elements on level" in ascending order:
            // * Foundation
            // * Framing (assumes framing consists of beams is placed to butt up to level above)
            // * Floors
            // * Walls
            // * Columns
            foreach (var level in levels)
            {
                AddInstancesOnLevelToIdGroupList(idGroupsInOrder, level, BuiltInCategory.OST_StructuralFoundation);
                AddInstancesOnReferenceLevelToIdGroupList(idGroupsInOrder, level,
                    BuiltInCategory.OST_StructuralFraming);
                AddInstancesOnLevelToIdGroupList(idGroupsInOrder, level, BuiltInCategory.OST_Floors);
                AddInstancesOnLevelToIdGroupList(idGroupsInOrder, level, BuiltInCategory.OST_Walls);
                AddInstancesOnLevelToIdGroupList(idGroupsInOrder, level, BuiltInCategory.OST_StructuralColumns);
            }

            // Initial setup of displacement elements for animation
            using (var t = new Transaction(doc, "Start animation"))
            {
                t.Start();
                foreach (var idGroups in idGroupsInOrder) BuildDisplacementElementGroup(doc, idGroups, view);
                if (m_displacementElements.Count == 0)
                {
                    t.RollBack();
                    return;
                }

                m_displacementElement = m_displacementElements[0];
                UnhideDisplacedElements();
                t.Commit();
            }

            m_displacementParameter = 1.0;

            if (m_isUsingIdling)
            {
                // Register idling for animation frames
                m_uiApplication.Idling += IdlingResponse;

                // Register timer for animation framews
                m_timer = new Timer();
                m_timer.Interval = m_timerInterval;
                m_timer.Elapsed += TimerElapsed;
                m_timer.Start();
            }
        }

        /// <summary>
        ///     Executes the next step on the animation.
        /// </summary>
        public void AnimateNextStep()
        {
            var groupFinished = false; // Is the current animation group finished?
            var allFinished = false; // Are all animation groups finished?
            if (m_displacementParameter <= 0)
            {
                m_displacementParameter = 1.0;
                groupFinished = true;
                m_currentDisplacementIndex++;
                if (m_currentDisplacementIndex == m_displacementElements.Count)
                {
                    allFinished = true;
                    if (m_isUsingIdling)
                    {
                        m_timer.Stop();
                        m_uiApplication.Idling -= IdlingResponse;
                    }
                }
            }

            // Execute transaction for next animation
            using (var t = new Transaction(m_uiApplication.ActiveUIDocument.Document,
                       groupFinished ? "Next animation group" : "Animation step"))
            {
                t.Start();
                if (groupFinished)
                {
                    // Delete displacement element (and children)
                    m_uiApplication.ActiveUIDocument.Document.Delete(m_displacementElement.Id);

                    // Increment to next group
                    m_displacementElement = allFinished ? null : m_displacementElements[m_currentDisplacementIndex];
                    UnhideDisplacedElements();
                }
                else
                {
                    ChangeDisplacementLocationForParent();
                    ChangeDisplacementLocationsForChildren();

                    // Decrement displacement parameter
                    m_displacementParameter -= m_displacementIncrement;
                }

                t.Commit();
            }
        }

        /// <summary>
        ///     Unhides the elements in the next displacement group.
        /// </summary>
        private void UnhideDisplacedElements()
        {
            if (m_displacementElement != null)
            {
                var view = m_displacementElement.Document.GetElement(m_displacementElement.OwnerViewId) as View;
                view.UnhideElements(m_displacementElement.GetDisplacedElementIdsFromAllChildren());
            }
        }

        /// <summary>
        ///     Find all instances matching category and level, and add to the collection of groups of sorted ids.
        /// </summary>
        /// <param name="idGroupsInOrder">The collection of groups of ids, sorted.</param>
        /// <param name="level">The level to match.</param>
        /// <param name="category">The category to match.</param>
        private static void AddInstancesOnLevelToIdGroupList(List<ICollection<ElementId>> idGroupsInOrder,
            Level level, BuiltInCategory category)
        {
            var collector = new FilteredElementCollector(level.Document);
            collector.WherePasses(new ElementLevelFilter(level.Id));
            collector.OfCategory(category);
            collector.WhereElementIsNotElementType();
            var idGroup = collector.ToElementIds();

            // Only add non-empty groups
            if (idGroup.Count > 0)
                idGroupsInOrder.Add(idGroup);
        }

        /// <summary>
        ///     Find all instances matching category and "Reference Level", and add to the collection of groups of sorted ids.
        /// </summary>
        /// <param name="idGroupsInOrder">The collection of groups of ids, sorted.</param>
        /// <param name="level">The level to match.</param>
        /// <param name="category">The category to match.</param>
        private static void AddInstancesOnReferenceLevelToIdGroupList(List<ICollection<ElementId>> idGroupsInOrder,
            Level level, BuiltInCategory category)
        {
            var collector = new FilteredElementCollector(level.Document);
            collector.OfCategory(category);
            collector.WhereElementIsNotElementType();

            // Use a parameter filter to match the Reference Level parameter
            var rule =
                ParameterFilterRuleFactory.CreateEqualsRule(
                    new ElementId(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM),
                    level.Id);
            collector.WherePasses(new ElementParameterFilter(rule));
            var idGroup = collector.ToElementIds();

            // Only add non-empty groups
            if (idGroup.Count > 0)
                idGroupsInOrder.Add(idGroup);
        }

        /// <summary>
        ///     Builds a group of displacement elements from a collection of element ids, and
        ///     sets the displacement to be uniform in Z but vary in XY based on location.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="ids">The collection of ids.</param>
        /// <param name="view">The view.</param>
        private void BuildDisplacementElementGroup(Document doc, ICollection<ElementId> ids, View view)
        {
            // The last element will be in the "parent" displacement element.  (At least one element
            // must remain assigned to the parent, so all child displacements will be relative to the 
            // parent's displacement.)
            var lastElement = doc.GetElement(ids.Last());
            var parentDisplacedLocation = GetDisplacementXyFor(lastElement, XYZ.Zero);
            parentDisplacedLocation = MoveToElevation(parentDisplacedLocation, m_initialHeight);

            // All elements are added to the parent displacement element. 
            var parent = DisplacementElement.Create(doc, ids, parentDisplacedLocation, view, null);
            m_displacementElements.Add(parent);

            var count = ids.Count;
            var childIds = new List<ElementId>();
            var idsList = ids.ToList();

            // Add all elements except the last one to child displacement elements
            for (var index = 0; index < count - 1; index++)
            {
                var childId = idsList[index];
                var e = doc.GetElement(childId);

                var displacedLocation = GetDisplacementXyFor(e, parentDisplacedLocation);

                // Setup id container for child DisplacementElement creation
                childIds.Clear();
                childIds.Add(childId);
                DisplacementElement.Create(doc, childIds, displacedLocation, view, parent);
            }

            view.HideElements(ids);
        }

        /// <summary>
        ///     Change the displacement location for the current parent element.
        /// </summary>
        private void ChangeDisplacementLocationForParent()
        {
            // Displacement includes displacement in Z
            var displacement = GetDisplacementXy(m_displacementElement);
            displacement = MoveToElevation(displacement, GetHeightDisplacementValue());
            m_displacementElement.SetRelativeDisplacement(displacement);
        }

        /// <summary>
        ///     Change the displacement location for children elements.
        /// </summary>
        private void ChangeDisplacementLocationsForChildren()
        {
            var subDisplacementElements = m_displacementElement.GetChildren();
            foreach (var element in subDisplacementElements)
            {
                // Displacement does not include change in Z
                var displacedLocation = GetDisplacementXy(element);

                element.SetRelativeDisplacement(displacedLocation);
            }
        }

        /// <summary>
        ///     Identifies if the displacement element is a parent or a child.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>True if the element is a child of another element.</returns>
        private static bool DisplacementElementIsChild(DisplacementElement element)
        {
            return element.ParentId != ElementId.InvalidElementId;
        }

        /// <summary>
        ///     Gets the XY displacement for a given element.
        /// </summary>
        /// <param name="element">The displacement element.</param>
        /// <returns>The displacement.</returns>
        private XYZ GetDisplacementXy(DisplacementElement element)
        {
            // If the element is a child, need to take into account the displacement of the parent.
            var displacementDueToParent = XYZ.Zero;
            if (DisplacementElementIsChild(element))
            {
                var parent = element.Document.GetElement(element.ParentId) as DisplacementElement;
                displacementDueToParent = parent.GetRelativeDisplacement();
            }

            // Assume one element per displacement, use that element to get the displacement needed.
            var id = element.GetDisplacedElementIds().First();
            var e = element.Document.GetElement(id);

            return GetDisplacementXyFor(e, displacementDueToParent);
        }

        /// <summary>
        ///     Gets the XY displacement for a given element relative to a parent displacement.
        /// </summary>
        /// <param name="e">The element.</param>
        /// <param name="displacementDueToParent">The parent element displacement.</param>
        /// <returns>The XY displacement.</returns>
        private XYZ GetDisplacementXyFor(Element e, XYZ displacementDueToParent)
        {
            var displacementDueToParentXy = MoveToElevationZero(displacementDueToParent);
            var location = GetNominalCenterLocation(e);
            var delta = location - m_modelCenter;
            var displacedLocation = delta * GetXyDisplacementRatio() - displacementDueToParentXy;

            return displacedLocation;
        }

        /// <summary>
        ///     Returns the current XY displacement ratio.
        /// </summary>
        /// <returns>The XY displacement ratio.</returns>
        private double GetXyDisplacementRatio()
        {
            if (m_displacementParameter == 1.0)
                return m_initialXyRatio;
            return m_initialXyRatio * 1 / Math.Pow(m_initialHeight - GetHeightDisplacementValue(), 0.75);
        }

        /// <summary>
        ///     Gets the current displcement in Z.
        /// </summary>
        /// <returns></returns>
        private double GetHeightDisplacementValue()
        {
            return m_initialHeight * m_displacementParameter;
        }

        /// <summary>
        ///     The timer callback for the animation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            m_timerTripped = true;
        }

        /// <summary>
        ///     The idling callback for the animation.
        /// </summary>
        /// <param name="senter"></param>
        /// <param name="e"></param>
        private void IdlingResponse(object senter, IdlingEventArgs e)
        {
            // Set to reraise idling immediately (unaffected by user activity)
            e.SetRaiseWithoutDelay();

            if (m_timerTripped)
            {
                m_timerTripped = false;
                AnimateNextStep();
            }
        }

        /// <summary>
        ///     Utility to move a point to the Z=0 projection
        /// </summary>
        /// <param name="location">The point.</param>
        /// <returns>The point at Z=0 projection.</returns>
        private static XYZ MoveToElevationZero(XYZ location)
        {
            return MoveToElevation(location, 0);
        }

        /// <summary>
        ///     Utility to move a point to an arbitrary Z projection
        /// </summary>
        /// <param name="location">The point.</param>
        /// <param name="z">The elevation to set for the point.</param>
        /// <returns>The point at Z=0 projection.</returns>
        private static XYZ MoveToElevation(XYZ location, double z)
        {
            return new XYZ(location.X, location.Y, z);
        }

        /// <summary>
        ///     Utility to get the "center" of the element based on location parameters, projected to Z=0.
        /// </summary>
        /// <param name="e">The element.</param>
        /// <returns>The center location.</returns>
        private static XYZ GetNominalCenterLocation(Element e)
        {
            switch (e.Location)
            {
                case LocationPoint lp:
                    return MoveToElevationZero(lp.Point);
                case LocationCurve lc:
                    return MoveToElevationZero(lc.Curve.Evaluate(0.5, true));
                default:
                    return XYZ.Zero;
            }
        }
    }
}
