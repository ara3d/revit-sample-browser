// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

﻿#region Header

//
// MepSystemSearch.cs - traverse MEP system connectors
//
// Copyright (C) 2018-2020 by Geoff Overfield. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//
// Copied from https://github.com/geoffoverfield/RevitAPI_SystemSearch
//

#endregion // Header

#region Namespaces

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    public class MepSystemSearch
    {
        private const int MAX_LOOPS = 20;
        private readonly Document m_pDoc;
        private readonly Selection m_pSelectedElts;
        private int m_iLoops;
        private List<ElementId> m_lVistited, m_lSelectedElts;

        public MepSystemSearch(UIApplication pUIApp)
        {
            InitializeLists();
            m_pDoc = pUIApp.ActiveUIDocument.Document;
            m_pSelectedElts = pUIApp.ActiveUIDocument.Selection;
            m_lSelectedElts = m_pSelectedElts.GetElementIds().ToList();

            RunStepThroughElements();
        }

        public void InitializeLists()
        {
            m_lSelectedElts = new List<ElementId>();
            m_lVistited = new List<ElementId>();
        }

        public void RunStepThroughElements()
        {
            Element pPrev = null, pNext = null;
            foreach (var pId in m_lSelectedElts)
            {
                if (m_lVistited.Contains(pId)) continue;
                if (pNext == null)
                {
                    pPrev = m_pDoc.GetElement(pId);
                    //Initial Element ^^^^^
                    //Do what you want with the Element here... 
                    //you won't be seeing it again.
                    //Get data, etc.
                    m_lVistited.Add(pId);
                }
                else
                {
                    pPrev = pNext;
                }

                var pConnset = GetConnectors(pPrev);
                if (pConnset == null)
                    continue;

                foreach (Connector pConn in pConnset)
                {
                    pNext = GetNextConnectedElement(
                        pConn, pPrev, ref m_lVistited);
                    //Do what you want with the Elements here...
                    //Get data, etc.
                    //And make sure we mark it as visited 
                    //so we don't come back to it.
                    m_lVistited.Add(pNext.Id);
                }
                //This will run through every selected Element.  
                //If for some reason, we missed something, 
                //we make it recursive.
            }

            m_iLoops++;

            if (m_lVistited.Count < m_lSelectedElts.Count
                && m_iLoops < MAX_LOOPS)
                //We also don't want this infinitely spiraling, 
                //so we give it a max of 20 tries to look 
                //at everything.
                RunStepThroughElements();
        }

        public bool NextConnector(
            Connector pConn,
            Element pPrevElement,
            List<ElementId> lVisited)
        {
            return pConn.Owner == pPrevElement
                   || pConn.Owner.Id == pPrevElement.Id
                   || pConn.Domain != Domain.DomainPiping //Change Domain based on need
                   || lVisited.Contains(pConn.Owner.Id);
        }

        public Element GetNextConnectedElement(
            Connector pConn,
            Element pPrevElem,
            ref List<ElementId> lVisited)
        {
            foreach (Connector pRef in pConn.AllRefs)
            {
                if (NextConnector(pRef, pPrevElem, lVisited)) continue;
                return pRef.Owner;
            }

            return null;
        }

        public static ConnectorSet GetConnectors(Element e)
        {
            if (e is MEPCurve curve)
                return curve?
                    .ConnectorManager?
                    .Connectors;

            if (e is FamilyInstance instance)
                return instance?
                    .MEPModel?
                    .ConnectorManager?
                    .Connectors;

            // Add other statements to accomodate your 
            // needs based on different element types

            Debug.Assert(
                false,
                "expected all candidate connector provider "
                + "elements to be either family instances or "
                + "derived from MEPCurve");

            return null;
        }
    }
}