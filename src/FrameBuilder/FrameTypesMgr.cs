// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    public class FrameTypesMgr
    {
        private readonly Dictionary<string, FamilySymbol> m_symbolMaps;
        private readonly List<FamilySymbol> m_symbols;

        public FrameTypesMgr(ExternalCommandData commandData)
        {
            CommandData = commandData;
            m_symbolMaps = new Dictionary<string, FamilySymbol>();
            m_symbols = new List<FamilySymbol>();
        }

        private FrameTypesMgr()
        {
        }

        public ExternalCommandData CommandData { get; }

        public int Size => m_symbolMaps.Count;

        public ReadOnlyCollection<FamilySymbol> FramingSymbols => new ReadOnlyCollection<FamilySymbol>(m_symbols);

        public bool AddSymbol(FamilySymbol framingSymbol)
        {
            if (ContainsSymbolName(framingSymbol.Name)) return false;
            m_symbolMaps.Add(framingSymbol.Name, framingSymbol);
            m_symbols.Add(framingSymbol);
            return true;
        }

        public bool DeleteSymbol(FamilySymbol symbol)
        {
            try
            {
                m_symbolMaps.Remove(symbol.Name);
                m_symbols.Remove(symbol);
                var ids = CommandData.Application.ActiveUIDocument.Document.Delete(symbol.Id) as List<ElementId>;
                if (ids.Count == 0) return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public FamilySymbol DuplicateSymbol(ElementType framingSymbol, string symbolName)
        {
            var symbol = framingSymbol.Duplicate(GenerateSymbolName(symbolName));
            var result = symbol as FamilySymbol;
            if (null != result)
            {
                m_symbolMaps.Add(result.Name, result);
                m_symbols.Add(result);
            }

            return result;
        }

        public bool ContainsSymbolName(string symbolName)
        {
            return m_symbolMaps.ContainsKey(symbolName);
        }

        public string GenerateSymbolName(string symbolName)
        {
            var suffix = 2;
            var result = symbolName;
            while (ContainsSymbolName(result))
            {
                result = $"{symbolName} {suffix}";
                suffix++;
            }

            return result;
        }
    }
}
