// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace RevitMultiSample.PerformanceAdviserControl.CS
{
    /// <summary>
    ///     A class that implements IPerformanceAdviserRule.  This class implements several methods that will be
    ///     run automatically when PerformanceAdviser::ExecuteRules or ExecuteAllRules is called.
    /// </summary>
    public class FlippedDoorCheck : IPerformanceAdviserRule
    {
        /// <summary>
        ///     A short description of the rule
        /// </summary>
        private readonly string m_description;

        /// <summary>
        ///     The ID of the failure definition for our API-based door flip check rule
        /// </summary>
        private readonly FailureDefinitionId m_doorWarningId;

        /// <summary>
        ///     A list of all family instances in the document that have the FaceFlipped property set to true;
        /// </summary>
        private List<ElementId> m_flippedDoors;

        /// <summary>
        ///     A short name for the rule
        /// </summary>
        private readonly string m_name;

        /// <summary>
        ///     Set up rule name, description, and error handling
        /// </summary>
        public FlippedDoorCheck()
        {
            m_name = "Flipped Door Check";
            m_description = "An API-based rule to search for and return any doors that are face-flipped";
            m_doorWarningId = new FailureDefinitionId(new Guid("25570B8FD4AD42baBD78469ED60FB9A3"));
            FailureDefinition.CreateFailureDefinition(m_doorWarningId, FailureSeverity.Warning,
                "Some doors in this project are face-flipped.");
        }

        /// <summary>
        ///     The rule ID for this rule;
        /// </summary>
        public static PerformanceAdviserRuleId Id { get; } 
            = new PerformanceAdviserRuleId(new Guid("BC38854474284491BD03795675AC7386"));

        /// <summary>
        ///     Does some preliminary work before executing tests on elements.  In this case,
        ///     we instantiate a list of FamilyInstances representing all doors that are flipped.
        /// </summary>
        /// <param name="document">The document being checked</param>
        public void InitCheck(Document document)
        {
            if (m_flippedDoors == null)
                m_flippedDoors = new List<ElementId>();
            else
                m_flippedDoors.Clear();
        }

        /// <summary>
        ///     This method does most of the work of the IPerformanceAdviserRule implementation.
        ///     It is called by PerformanceAdviser.
        ///     It examines the element passed to it (which was previously filtered by the filter
        ///     returned by GetElementFilter() (see below)).  After checking to make sure that the
        ///     element is an instance, it checks the FacingFlipped property of the element.
        ///     If it is flipped, it adds the instance to a list to be used later.
        /// </summary>
        /// <param name="document">The active document</param>
        /// <param name="element">The current element being checked</param>
        public void ExecuteElementCheck(Document document, Element element)
        {
            if (element is FamilyInstance doorCurrent)
            {
                if (doorCurrent.FacingFlipped)
                    m_flippedDoors.Add(doorCurrent.Id);
            }
        }

        /// <summary>
        ///     This method is called by PerformanceAdviser after all elements in document
        ///     matching the ElementFilter from GetElementFilter() are checked by ExecuteElementCheck().
        ///     This method checks to see if there are any elements (door instances, in this case) in the
        ///     m_FlippedDoor instance member.  If there are, it iterates through that list and displays
        ///     the instance name and door tag of each item.
        /// </summary>
        /// <param name="document">The active document</param>
        public void FinalizeCheck(Document document)
        {
            if (m_flippedDoors.Count == 0)
            {
                Debug.WriteLine("No doors were flipped.  Test passed.");
            }

            else
            {
                //Pass the element IDs of the flipped doors to the revit failure reporting APIs.
                var fm = new FailureMessage(m_doorWarningId);
                fm.SetFailingElements(m_flippedDoors);
                var failureReportingTransaction = new Transaction(document, "Failure reporting transaction");
                failureReportingTransaction.Start();
                PerformanceAdviser.GetPerformanceAdviser().PostWarning(fm);
                failureReportingTransaction.Commit();
                m_flippedDoors.Clear();
            }
        }

        /// <summary>
        ///     Gets the description of the rule
        /// </summary>
        /// <returns>The rule description</returns>
        public string GetDescription()
        {
            return m_description;
        }

        /// <summary>
        ///     This method supplies an element filter to reduce the number of elements that PerformanceAdviser
        ///     will pass to GetElementCheck().  In this case, we are filtering for door elements.
        /// </summary>
        /// <param name="document">The document being checked</param>
        /// <returns>A door element filter</returns>
        public ElementFilter GetElementFilter(Document document)
        {
            return new ElementCategoryFilter(BuiltInCategory.OST_Doors);
        }

        /// <summary>
        ///     Gets the name of the rule
        /// </summary>
        /// <returns>The rule name</returns>
        public string GetName()
        {
            return m_name;
        }

        /// <summary>
        ///     Returns true if this rule will iterate through elements and check them, false otherwise
        /// </summary>
        /// <returns>True</returns>
        public bool WillCheckElements()
        {
            return true;
        }

        /// <summary>
        ///     This method is used by PerformanceAdviser to get the
        ///     ID of the rule. It returns a global static field to make sharing the ID in different places
        ///     in the application easier.
        /// </summary>
        /// <returns>The Rule ID of this rule</returns>
        public PerformanceAdviserRuleId GetRuleId()
        {
            return Id;
        }
    }
}
