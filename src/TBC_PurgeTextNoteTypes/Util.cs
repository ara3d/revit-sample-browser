#region Namespaces

using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PurgeTextNoteTypes sample.</summary>
    internal static partial class Util
    {
        public static ICollection<ElementId> GetUnusedTextNoteTypes(
            Document doc)
        {
            FilteredElementCollector collector
                = new(doc);

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
        public static ICollection<ElementId>
            GetUnusedTextNoteTypesExcluding(
                Document doc)
        {
            ICollection<ElementId> usedTextNotesTypeIds
                = [];

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
