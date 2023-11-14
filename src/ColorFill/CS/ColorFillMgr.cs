// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ColorFill.CS
{
    /// <summary>
    ///     This is a helper class to deal with the color fill
    /// </summary>
    public class ColorFillMgr
    {
        private readonly Document m_document;
        private readonly ExternalCommandData m_externalCommandData;
        private readonly FilteredElementCollector m_fec;
        private List<ElementId> m_fillPatternIds = new List<ElementId>();
        private readonly List<ElementId> m_levelIds = new List<ElementId>();
        private List<ElementId> m_parameterDefinitions = new List<ElementId>();
        private readonly ElementId m_schemeCategoryId;

        /// <summary>
        ///     Construction
        /// </summary>
        /// <param name="doc">Revit document that will be dealt with</param>
        /// <param name="commandData"></param>
        public ColorFillMgr(Document doc, ExternalCommandData commandData)
        {
            m_document = doc;
            m_fec = new FilteredElementCollector(m_document);
            m_schemeCategoryId = new ElementId(BuiltInCategory.OST_Rooms);
            m_externalCommandData = commandData;
        }

        /// <summary>
        ///     All room schemes from document
        /// </summary>
        public List<ColorFillScheme> RoomSchemes { get; private set; }

        /// <summary>
        ///     DB Views from the document
        /// </summary>
        public List<View> Views { get; private set; }

        /// <summary>
        ///     Get Data from document
        /// </summary>
        public void RetrieveData()
        {
            RoomSchemes = m_fec.OfClass(typeof(ColorFillScheme))
                .OfType<ColorFillScheme>()
                .Where(s => s.CategoryId == m_schemeCategoryId).ToList();
            m_fillPatternIds = new FilteredElementCollector(m_document)
                .OfClass(typeof(FillPatternElement))
                .OfType<FillPatternElement>()
                .Where(fp => fp.GetFillPattern().Target == FillPatternTarget.Drafting)
                .Select(f => f.Id)
                .ToList();
            //Select all floor views and elevation views.
            Views = new FilteredElementCollector(m_document)
                .OfClass(typeof(View))
                .OfType<View>().Where(v => !v.IsTemplate &&
                                           (v.ViewType == ViewType.FloorPlan || v.ViewType == ViewType.Elevation))
                .ToList();
        }

        /// <summary>
        ///     Duplicate a color fill scheme based on an existing one.
        /// </summary>
        /// <param name="scheme">The color fill which that is duplicated.</param>
        /// <param name="schemeName">Name for new color fill scheme.</param>
        /// <param name="schemeTitle">Title for new color fill scheme.</param>
        /// <returns></returns>
        public void DuplicateScheme(ColorFillScheme scheme, string schemeName, string schemeTitle)
        {
            var parameterId = new ElementId(BuiltInParameter.AREA_SCHEME_NAME);
            using (var tr = new Transaction(m_document))
            {
                tr.Start("CopyScheme");
                var newSchemeId = scheme.Duplicate(schemeName);
                var newScheme = scheme.Document.GetElement(newSchemeId) as ColorFillScheme;
                newScheme.Title = schemeTitle;
                m_parameterDefinitions = newScheme.GetSupportedParameterIds().ToList();
                if (m_parameterDefinitions.Contains(parameterId))
                    newScheme.ParameterDefinition = parameterId;
                tr.Commit();
            }
        }

        /// <summary>
        ///     Create color legend on view with the specific color fill scheme
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="view"></param>
        public void CreateAndPlaceLegend(ColorFillScheme scheme, View view)
        {
            using (var transaction = new Transaction(m_document))
            {
                transaction.Start("Create legend");
                var origin = view.Origin.Add(view.UpDirection.Multiply(20));

                if (view.CanApplyColorFillScheme(m_schemeCategoryId, scheme.Id))
                {
                    view.SetColorFillSchemeId(m_schemeCategoryId, scheme.Id);
                    var legend = ColorFillLegend.Create(m_document, view.Id, m_schemeCategoryId, origin);
                    legend.Height = legend.Height / 2;
                    transaction.Commit();
                }
                else
                {
                    throw new Exception("The scheme can not be applied on the view.");
                }
            }
        }

        /// <summary>
        ///     Make modify to existing color fill scheme
        /// </summary>
        /// <param name="scheme"></param>
        public void ModifyByValueScheme(ColorFillScheme scheme)
        {
            var entries = scheme.GetEntries().ToList();
            var newEntries = new List<ColorFillSchemeEntry>();
            var storageType = entries[0].StorageType;
            var random = new Random();
            var seed = random.Next(0, 256);
            foreach (var entry in entries)
            {
                seed++;
                var newEntry = CreateEntry(scheme, storageType, entry.FillPatternId, GenerateRandomColor(seed));
                switch (storageType)
                {
                    case StorageType.Double:
                        newEntry.SetDoubleValue(entry.GetDoubleValue());
                        break;
                    case StorageType.Integer:
                        newEntry.SetIntegerValue(entry.GetIntegerValue());
                        break;
                    case StorageType.String:
                        newEntry.SetStringValue(entry.GetStringValue());
                        break;
                    case StorageType.ElementId:
                        newEntry.SetElementIdValue(entry.GetElementIdValue());
                        break;
                }

                newEntries.Add(newEntry);
            }

            using (var tr = new Transaction(m_document))
            {
                tr.Start("update entries");
                scheme.SetEntries(newEntries);
                tr.Commit();
            }

            m_externalCommandData.Application.ActiveUIDocument.RefreshActiveView();
        }

        private Color GenerateRandomColor(int seed)
        {
            var r = new Random(seed);
            var red = byte.Parse(r.Next(0, 256).ToString());
            var green = byte.Parse(r.Next(0, 256).ToString());
            var blue = byte.Parse(r.Next(0, 256).ToString());
            var randomColor = new Color(red, green, blue);
            return randomColor;
        }

        private ColorFillSchemeEntry CreateEntry(ColorFillScheme scheme, StorageType type, ElementId fillPatternId,
            Color color)
        {
            var entries = scheme.GetEntries();
            ColorFillSchemeEntry lastEntry = null;
            if (entries.Count > 0)
                lastEntry = entries.Last();

            var entry = new ColorFillSchemeEntry(type);
            entry.FillPatternId = fillPatternId;

            switch (type)
            {
                case StorageType.Double:
                    double doubleValue = 0;
                    if (lastEntry != null)
                        doubleValue = lastEntry.GetDoubleValue() + 20.00;
                    entry.SetDoubleValue(doubleValue);
                    break;
                case StorageType.String:
                    var strValue = $"New entry {entries.Count}";
                    entry.SetStringValue(strValue);
                    break;
                case StorageType.Integer:
                    var intValue = 0;
                    if (lastEntry != null)
                        intValue = lastEntry.GetIntegerValue() + 20;
                    entry.SetIntegerValue(intValue);
                    break;
                case StorageType.ElementId:
                    var level = new FilteredElementCollector(m_document)
                        .OfClass(typeof(Level))
                        .Where(lv => !m_levelIds.Contains(lv.Id) && lv.Name != "Level 1")
                        .FirstOrDefault();
                    m_levelIds.Add(level.Id);
                    entry.SetElementIdValue(level.Id);
                    break;
                default:
                    throw new Exception("The type is not correct!");
            }

            return entry;
        }
    }
}
