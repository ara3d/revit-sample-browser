// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.FrameBuilder.CS
{
    /// <summary>
    ///     data manager take charge of FamilySymbol object in current document
    /// </summary>
    public class FrameTypesMgr
    {
        // map list pairs FamilySymbol object and its Name 
        private readonly Dictionary<string, FamilySymbol> m_symbolMaps;

        // list of FamilySymbol objects
        private readonly List<FamilySymbol> m_symbols;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="commandData"></param>
        public FrameTypesMgr(ExternalCommandData commandData)
        {
            CommandData = commandData;
            m_symbolMaps = new Dictionary<string, FamilySymbol>();
            m_symbols = new List<FamilySymbol>();
        }

        /// <summary>
        ///     constructor without parameters is forbidden
        /// </summary>
        private FrameTypesMgr()
        {
        }

        /// <summary>
        ///     command data pass from entry point
        /// </summary>
        public ExternalCommandData CommandData { get; }

        /// <summary>
        ///     size of FamilySymbol objects in current Revit document
        /// </summary>
        public int Size => m_symbolMaps.Count;

        /// <summary>
        ///     get list of FamilySymbol objects in current Revit document
        /// </summary>
        public ReadOnlyCollection<FamilySymbol> FramingSymbols => new ReadOnlyCollection<FamilySymbol>(m_symbols);

        /// <summary>
        ///     add one FamilySymbol object to the lists
        /// </summary>
        /// <param name="framingSymbol"></param>
        /// <returns></returns>
        public bool AddSymbol(FamilySymbol framingSymbol)
        {
            if (ContainsSymbolName(framingSymbol.Name)) return false;
            m_symbolMaps.Add(framingSymbol.Name, framingSymbol);
            m_symbols.Add(framingSymbol);
            return true;
        }

        /// delete one FamilySymbol both in Revit and lists here
        /// </summary>
        /// <param name="symbol">FamilySymbol to be deleted</param>
        /// <returns>successful to delete</returns>
        public bool DeleteSymbol(FamilySymbol symbol)
        {
            try
            {
                // remove from the lists
                m_symbolMaps.Remove(symbol.Name);
                m_symbols.Remove(symbol);
                // delete from Revit
                var ids = CommandData.Application.ActiveUIDocument.Document.Delete(symbol.Id) as List<ElementId>;
                if (ids.Count == 0) return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     duplicate one FamilySymbol and add to lists
        /// </summary>
        /// <param name="framingSymbol">FamilySymbol to be copied</param>
        /// <param name="symbolName">duplicate FamilySymbol's Name</param>
        /// <returns>new FamilySymbol</returns>
        public FamilySymbol DuplicateSymbol(ElementType framingSymbol, string symbolName)
        {
            // duplicate a FamilySymbol
            var symbol = framingSymbol.Duplicate(GenerateSymbolName(symbolName));
            var result = symbol as FamilySymbol;
            if (null != result)
            {
                // add to lists
                m_symbolMaps.Add(result.Name, result);
                m_symbols.Add(result);
            }

            return result;
        }

        /// <summary>
        ///     inquire whether the FamilySymbol's Name already exists in the list
        /// </summary>
        /// <param name="symbolName"></param>
        /// <returns></returns>
        public bool ContainsSymbolName(string symbolName)
        {
            return m_symbolMaps.ContainsKey(symbolName);
        }

        /// <summary>
        ///     generate a new FamilySymbol's Name according to given name
        /// </summary>
        /// <param name="symbolName">original name</param>
        /// <returns>generated name</returns>
        public string GenerateSymbolName(string symbolName)
        {
            var suffix = 2;
            var result = symbolName;
            while (ContainsSymbolName(result))
            {
                result = symbolName + " " + suffix;
                suffix++;
            }

            return result;
        }
    }
}
