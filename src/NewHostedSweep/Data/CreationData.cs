// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Creators;
using Autodesk.Revit.DB;

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

        private readonly List<Edge> m_backUpEdges = new List<Edge>();

        private ElementType m_backUpSymbol;

        /// <summary>
        ///     Creator contains the necessary data to fetch the edges and get the symbol.
        /// </summary>
        private readonly HostedSweepCreator m_creator;

        /// <summary>
        ///     Edges which contains references for HostedSweep creation.
        /// </summary>
        private readonly List<Edge> m_edgesForHostedSweep = new List<Edge>();

        private ElementType m_symbol;

        public CreationData(HostedSweepCreator creator)
        {
            m_creator = creator;
        }

        /// <summary>
        ///     Creator contains the necessary data to fetch the edges and get the symbol.
        /// </summary>
        public HostedSweepCreator Creator => m_creator;

        public ElementType Symbol
        {
            get => m_symbol;
            set => m_symbol = value;
        }

        /// <summary>
        ///     Edges which contains references for HostedSweep creation.
        /// </summary>
        public List<Edge> EdgesForHostedSweep => m_edgesForHostedSweep;

        public event EdgeEventHandler EdgeAdded;

        public event EdgeEventHandler EdgeRemoved;

        public event SymbolChangedEventHandler SymbolChanged;

        public void BackUp()
        {
            m_backUpSymbol = m_symbol;
            m_backUpEdges.Clear();
            m_backUpEdges.AddRange(m_edgesForHostedSweep);
        }

        public void Restore()
        {
            m_symbol = m_backUpSymbol;
            m_edgesForHostedSweep.Clear();
            m_edgesForHostedSweep.AddRange(m_backUpEdges);
        }

        public void Update()
        {
            if (SymbolChanged != null && m_backUpSymbol != null &&
                m_backUpSymbol.Id != m_symbol.Id)
                SymbolChanged(m_symbol);

            if (EdgeRemoved != null)
                foreach (var edge in m_backUpEdges)
                {
                    if (m_edgesForHostedSweep.IndexOf(edge) == -1)
                        EdgeRemoved(edge);
                }

            if (EdgeAdded != null)
                foreach (var edge in m_edgesForHostedSweep)
                {
                    if (m_backUpEdges.IndexOf(edge) == -1)
                        EdgeAdded(edge);
                }
        }
    }
}
