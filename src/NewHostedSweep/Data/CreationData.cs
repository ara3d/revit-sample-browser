// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Creators;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data
{
    /// <summary>
    ///     This class contains the data for hosted sweep creation.
    /// </summary>
    public class CreationData
    {
        /// <summary>
        ///     Represents the method that will handle the EdgeAdded or EdgeRemoved events
        ///     of CreationData
        /// </summary>
        /// <param name="edge">Edge</param>
        public delegate void EdgeEventHandler(Edge edge);

        /// <summary>
        ///     Represents the method that will handle the SymbolChanged events
        ///     of CreationData
        /// </summary>
        /// <param name="sym">Symbol</param>
        public delegate void SymbolChangedEventHandler(ElementType sym);

        private readonly List<Edge> m_backUpEdges = [];

        private ElementType m_backUpSymbol;

        public CreationData(HostedSweepCreator creator)
        {
            Creator = creator;
        }

        /// <summary>
        ///     Creator contains the necessary data to fetch the edges and get the symbol.
        /// </summary>
        public HostedSweepCreator Creator { get; }

        public ElementType Symbol { get; set; }

        /// <summary>
        ///     Edges which contains references for HostedSweep creation.
        /// </summary>
        public List<Edge> EdgesForHostedSweep { get; } = [];

        public event EdgeEventHandler EdgeAdded;

        public event EdgeEventHandler EdgeRemoved;

        public event SymbolChangedEventHandler SymbolChanged;

        public void BackUp()
        {
            m_backUpSymbol = Symbol;
            m_backUpEdges.Clear();
            m_backUpEdges.AddRange(EdgesForHostedSweep);
        }

        public void Restore()
        {
            Symbol = m_backUpSymbol;
            EdgesForHostedSweep.Clear();
            EdgesForHostedSweep.AddRange(m_backUpEdges);
        }

        public void Update()
        {
            if (SymbolChanged != null && m_backUpSymbol != null &&
                m_backUpSymbol.Id != Symbol.Id)
                SymbolChanged(Symbol);

            if (EdgeRemoved != null)
                foreach (var edge in m_backUpEdges)
                {
                    if (EdgesForHostedSweep.IndexOf(edge) == -1)
                        EdgeRemoved(edge);
                }

            if (EdgeAdded != null)
                foreach (var edge in EdgesForHostedSweep)
                {
                    if (m_backUpEdges.IndexOf(edge) == -1)
                        EdgeAdded(edge);
                }
        }
    }
}
