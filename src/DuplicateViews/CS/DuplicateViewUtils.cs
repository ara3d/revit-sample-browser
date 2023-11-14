// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace RevitMultiSample.DuplicateViews.CS
{
    /// <summary>
    ///     Utilities that facilitate duplication of drafting views and schedules into another document.
    /// </summary>
    internal class DuplicateViewUtils
    {
        /// <summary>
        ///     Utility to duplicate schedules from one document to another.
        /// </summary>
        /// <param name="fromDocument">The source document.</param>
        /// <param name="views">The collection of schedule views.</param>
        /// <param name="toDocument">The target document.</param>
        public static void DuplicateSchedules(Document fromDocument,
            IEnumerable<ViewSchedule> views,
            Document toDocument)
        {
            // Use LINQ to convert to list of ElementIds for use in CopyElements() method
            var ids =
                views.AsEnumerable<View>().ToList().ConvertAll(ViewConvertToElementId);

            // Duplicate.  Pass false to make the function skip returning the map from source element to its copy
            DuplicateElementsAcrossDocuments(fromDocument, ids, toDocument, false);
        }

        /// <summary>
        ///     Utility to duplicate drafting views and their contents from one document to another.
        /// </summary>
        /// <param name="fromDocument">The source document.</param>
        /// <param name="views">The collection of drafting views.</param>
        /// <param name="toDocument">The target document.</param>
        /// <returns>The number of drafting elements created in the copied views.</returns>
        public static int DuplicateDraftingViews(Document fromDocument,
            IEnumerable<ViewDrafting> views,
            Document toDocument)
        {
            // Return value
            var numberOfDetailElements = 0;

            // Transaction group for all activities
            using (var tg = new TransactionGroup(toDocument,
                       "API - Duplication across documents with detailing"))
            {
                tg.Start();

                // Use LINQ to convert to list of ElementIds for use in CopyElements() method
                var ids =
                    views.AsEnumerable<View>().ToList().ConvertAll(ViewConvertToElementId);

                // Duplicate.  Pass true to get a map from source element to its copy
                var viewMap =
                    DuplicateElementsAcrossDocuments(fromDocument, ids, toDocument, true);

                // For each copied view, copy the contents
                foreach (var viewId in viewMap.Keys)
                {
                    var fromView = fromDocument.GetElement(viewId) as View;
                    var toView = toDocument.GetElement(viewMap[viewId]) as View;
                    numberOfDetailElements += DuplicateDetailingAcrossViews(fromView, toView);
                }

                tg.Assimilate();
            }

            return numberOfDetailElements;
        }

        /// <summary>
        ///     Duplicates a set of elements across documents.
        /// </summary>
        /// <param name="fromDocument">The source document.</param>
        /// <param name="elementIds">Collection of view ids.</param>
        /// <param name="toDocument">The target document.</param>
        /// <param name="findMatchingElements">
        ///     True to return a map of matching elements
        ///     (matched by Name).  False to skip creation of this map.
        /// </param>
        /// <returns>The map of matching elements, if findMatchingElements was true.</returns>
        private static Dictionary<ElementId, ElementId> DuplicateElementsAcrossDocuments(Document fromDocument,
            ICollection<ElementId> elementIds,
            Document toDocument,
            bool findMatchingElements)
        {
            // Return value
            var elementMap = new Dictionary<ElementId, ElementId>();

            ICollection<ElementId> copiedIds;
            using (var t1 = new Transaction(toDocument, "Duplicate elements"))
            {
                t1.Start();

                // Set options for copy-paste to hide the duplicate types dialog
                var options = new CopyPasteOptions();
                options.SetDuplicateTypeNamesHandler(new HideAndAcceptDuplicateTypeNamesHandler());

                // Copy the input elements.
                copiedIds =
                    ElementTransformUtils.CopyElements(fromDocument, elementIds, toDocument, Transform.Identity,
                        options);

                // Set failure handler to hide duplicate types warnings which may be posted.
                var failureOptions = t1.GetFailureHandlingOptions();
                failureOptions.SetFailuresPreprocessor(new HidePasteDuplicateTypesPreprocessor());
                t1.Commit(failureOptions);
            }

            // Find matching elements if required
            if (findMatchingElements)
            {
                // Build a map from name -> source element
                var nameToFromElementsMap = new Dictionary<string, ElementId>();

                foreach (var id in elementIds)
                {
                    var e = fromDocument.GetElement(id);
                    var name = e.Name;
                    if (!string.IsNullOrEmpty(name))
                        nameToFromElementsMap.Add(name, id);
                }

                // Build a map from name -> target element
                var nameToToElementsMap = new Dictionary<string, ElementId>();

                foreach (var id in copiedIds)
                {
                    var e = toDocument.GetElement(id);
                    var name = e.Name;
                    if (!string.IsNullOrEmpty(name))
                        nameToToElementsMap.Add(name, id);
                }

                // Merge to make source element -> target element map
                foreach (var name in nameToFromElementsMap.Keys)
                {
                    ElementId copiedId;
                    if (nameToToElementsMap.TryGetValue(name, out copiedId))
                        elementMap.Add(nameToFromElementsMap[name], copiedId);
                }
            }

            return elementMap;
        }

        /// <summary>
        ///     Copies all view-specific elements from a source view to a target view.
        /// </summary>
        /// <remarks>
        ///     The source and target views do not have to be in the same document.
        /// </remarks>
        /// <param name="fromView">The source view.</param>
        /// <param name="toView">The target view.</param>
        /// <returns>The number of new elements created during the copy operation.</returns>
        private static int DuplicateDetailingAcrossViews(View fromView,
            View toView)
        {
            // Collect view-specific elements in source view
            var collector = new FilteredElementCollector(fromView.Document, fromView.Id);

            // Skip elements which don't have a category.  In testing, this was
            // the revision table and the extents element, which should not be copied as they will
            // be automatically created for the copied view.
            collector.WherePasses(new ElementCategoryFilter(ElementId.InvalidElementId, true));

            // Get collection of elements to copy for CopyElements()
            var toCopy = collector.ToElementIds();

            // Return value
            var numberOfCopiedElements = 0;

            if (toCopy.Count > 0)
                using (var t2 = new Transaction(toView.Document, "Duplicate view detailing"))
                {
                    t2.Start();
                    // Set handler to skip the duplicate types dialog
                    var options = new CopyPasteOptions();
                    options.SetDuplicateTypeNamesHandler(new HideAndAcceptDuplicateTypeNamesHandler());

                    // Copy the elements using no transformation
                    var copiedElements =
                        ElementTransformUtils.CopyElements(fromView, toCopy, toView, Transform.Identity, options);
                    numberOfCopiedElements = copiedElements.Count;

                    // Set failure handler to skip any duplicate types warnings that are posted.
                    var failureOptions = t2.GetFailureHandlingOptions();
                    failureOptions.SetFailuresPreprocessor(new HidePasteDuplicateTypesPreprocessor());
                    t2.Commit(failureOptions);
                }

            return numberOfCopiedElements;
        }

        /// <summary>
        ///     Converter delegate for conversion of collections
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The view's id.</returns>
        private static ElementId ViewConvertToElementId(View view)
        {
            return view.Id;
        }
    }

    /// <summary>
    ///     A handler to accept duplicate types names created by the copy/paste operation.
    /// </summary>
    internal class HideAndAcceptDuplicateTypeNamesHandler : IDuplicateTypeNamesHandler
    {
        /// <summary>
        ///     Implementation of the IDuplicateTypeNameHandler
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            // Always use duplicate destination types when asked
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }

    /// <summary>
    ///     A failure preprocessor to hide the warning about duplicate types being pasted.
    /// </summary>
    internal class HidePasteDuplicateTypesPreprocessor : IFailuresPreprocessor
    {
        /// <summary>
        ///     Implementation of the IFailuresPreprocessor.
        /// </summary>
        /// <param name="failuresAccessor"></param>
        /// <returns></returns>
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            foreach (var failure in failuresAccessor.GetFailureMessages())
                // Delete any "Can't paste duplicate types.  Only non duplicate types will be pasted." warnings
                if (failure.GetFailureDefinitionId() == BuiltInFailures.CopyPasteFailures.CannotCopyDuplicates)
                    failuresAccessor.DeleteWarning(failure);

            // Handle any other errors interactively
            return FailureProcessingResult.Continue;
        }
    }
}
