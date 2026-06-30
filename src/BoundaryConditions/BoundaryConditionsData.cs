// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.BoundaryConditions.CS
{
    /// <summary>
    ///     user select a element. If the selected element has boundary conditions, display
    ///     its parameter values else create one.
    ///     this class prepare the needed data(the selected element type and its BC information)
    ///     and operate the Revit API
    /// </summary>
    public class BoundaryConditionsData
    {
        public BoundaryConditionsData(Element element)
        {
            // store the selected element and its BCs
            SetBcHostMap(element);
        }

        // the selected Element

        // store all the corresponding BCs of the current selected host element 
        // and use the BC Id value as the key

        // the object for which the grid in UI displays.

        public BcProperties BcProperties { get; set; }

        public Element HostElement { get; private set; }

        public Dictionary<ElementId, Autodesk.Revit.DB.Structure.BoundaryConditions> BCs { get; } = [];

        /// <summary>
        ///     According to the selected element create corresponding Boundary Conditions.
        ///     Add it into m_bCsDictionary.
        /// </summary>
        public bool CreateBoundaryConditions()
        {
            CreateBcHandler createBch = null;

            switch (HostElement)
            {
                // judge the type of the HostElement
                case FamilyInstance familyInstance:
                    {
                        var structuralType = familyInstance.StructuralType;

                        switch (structuralType)
                        {
                            // create Line BC for beam
                            case StructuralType.Beam:
                                createBch = CreateLineBc;
                                break;
                            case StructuralType.Brace:
                            case StructuralType.Column:
                            // create point BC for Column/brace
                            case StructuralType.Footing:
                                createBch = CreatePointBc;
                                break;
                        }
                        break;
                    }
                case Wall _:
                    // create line BC for wall
                    createBch = CreateLineBc;
                    break;
                case Floor _:
                    // create area BC for Floor
                    createBch = CreateAreaBc;
                    break;
                case WallFoundation _:
                    // create line BC for WallFoundation
                    createBch = CreateLineBc;
                    break;
            }

            // begin create
            Autodesk.Revit.DB.Structure.BoundaryConditions newBc = null;
            try
            {
                newBc = createBch(HostElement);
                if (null == newBc) return false;
            }
            catch (Exception)
            {
                return false;
            }

            // add the created Boundary Conditions into m_bCsDictionary
            BCs.Add(newBc.Id, newBc);
            return true;
        }

        /// <summary>
        ///     store the selected element and its corresponding BCs
        /// </summary>
        /// <param name="element"> use selected element in Revit UI(the host element)</param>
        private void SetBcHostMap(Element element)
        {
            // set the Host element with current selected element
            HostElement = element;
            // retrieve the Document in which the Element resides.
            var doc = element.Document;

            var boundaryConditions = doc.GetElements<Autodesk.Revit.DB.Structure.BoundaryConditions>();
            foreach (var bC in boundaryConditions)
            {
                if (HostElement.Id == bC.HostElementId)
                    BCs.Add(bC.Id, bC);
            }
        }

        private AnalyticalElement GetAnalyticalElement(Element element)
        {
            var document = element.Document;
            var assocManager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            return assocManager == null
                ? null : document.GetElement<AnalyticalElement>(element.Id);
        }

        private Autodesk.Revit.DB.Structure.BoundaryConditions CreatePointBc(Element hostElement)
        {
            if (hostElement is not FamilyInstance) return null;

            var analyticalModel = GetAnalyticalElement(hostElement);

            var refCurve = analyticalModel.GetCurve();
            if (null == refCurve)
                return null;

            var endReference =
                analyticalModel.GetReference(
                    new AnalyticalModelSelector(refCurve, AnalyticalCurveSelector.EndPoint));

            var createDoc = hostElement.Document.Create;

            return createDoc.NewPointBoundaryConditions(endReference, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        private Autodesk.Revit.DB.Structure.BoundaryConditions CreateLineBc(Element hostElement)
        {
            var createDoc = hostElement.Document.Create;
            var analyticalModel = GetAnalyticalElement(hostElement);
            return createDoc.NewLineBoundaryConditions(analyticalModel, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        private Autodesk.Revit.DB.Structure.BoundaryConditions CreateAreaBc(Element hostElement)
        {
            return hostElement.Document.Create.NewAreaBoundaryConditions(GetAnalyticalElement(hostElement), 0, 0, 0, 0, 0, 0);
        }

        //A delegate for create boundary condition with different type
        private delegate Autodesk.Revit.DB.Structure.BoundaryConditions
            CreateBcHandler(Element hostElement);
    }
}
