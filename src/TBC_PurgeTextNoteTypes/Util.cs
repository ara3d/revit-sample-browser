#region Namespaces

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PurgeTextNoteTypes sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return all unused text note types by collecting all
        ///     existing types in the document and removing the
        ///     ones that are used afterwards.
        /// </summary>
        public static ICollection<ElementId> GetUnusedTextNoteTypes(
            Document doc)
        {
            var collector
                = new FilteredElementCollector(doc);

            ICollection<ElementId> textNoteTypes
                = collector.OfClass(typeof(TextNoteType))
                    .ToElementIds()
                    .ToList();

            var textNotes
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(TextNote));

            foreach (TextNote textNote in textNotes)
            {
                textNoteTypes.Remove(
                    textNote.TextNoteType.Id);
            }

            return textNoteTypes;
        }

        /// <summary>
        ///     Return all unused text note types by first
        ///     determining all text note types in use and
        ///     then collecting all the others using an
        ///     exclusion filter.
        /// </summary>
        public static ICollection<ElementId>
            GetUnusedTextNoteTypesExcluding(
                Document doc)
        {
            ICollection<ElementId> usedTextNotesTypeIds
                = new Collection<ElementId>();

            var textNotes
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(TextNote));

            foreach (TextNote textNote in textNotes)
                usedTextNotesTypeIds.Add(
                    textNote.TextNoteType.Id);

            var unusedTypeCollector
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(TextNoteType));

            if (0 < usedTextNotesTypeIds.Count)
                unusedTypeCollector.Excluding(
                    usedTextNotesTypeIds);

            return unusedTypeCollector.ToElementIds();
        }
    }
}
