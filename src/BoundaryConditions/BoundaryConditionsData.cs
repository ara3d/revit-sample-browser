// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

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
        /// <summary>
        ///     construct function
        /// </summary>
        /// <param name="element"> host element</param>
        public BoundaryConditionsData(Element element)
        {
            // store the selected element and its BCs
            SetBcHostMap(element);
        }

        // the selected Element

        // store all the corresponding BCs of the current selected host element 
        // and use the BC Id value as the key

        // the object for which the grid in UI displays.

        /// <summary>
        ///     gets or sets the object for which the grid in UI displays.
        /// </summary>
        public BcProperties BcProperties { get; set; }

        /// <summary>
        ///     get current host element
        /// </summary>
        public Element HostElement { get; private set; }

        /// <summary>
        ///     get all the BCs correspond with current host
        /// </summary>
        public Dictionary<ElementId, Autodesk.Revit.DB.Structure.BoundaryConditions> BCs { get; } = new Dictionary<ElementId, Autodesk.Revit.DB.Structure.BoundaryConditions>();

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

        /// <summary>
        ///     Create a new Point BoundaryConditions Element.
        ///     All the parameter default as Fixed.
        /// </summary>
        /// <param name="hostElement">
        ///     structural element which provide the analytical line end reference
        /// </param>
        /// <returns> the created Point BoundaryConditions Element</returns>
        private Autodesk.Revit.DB.Structure.BoundaryConditions CreatePointBc(Element hostElement)
        {
            if (!(hostElement is FamilyInstance)) return null;

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

        /// <summary>
        ///     Create a new Line BoundaryConditions Element.
        ///     All the parameter default as Fixed.
        /// </summary>
        /// <param name="hostElement">structural element which provide the hostElementId</param>
        /// <returns>the created Point BoundaryConditions Element</returns>
        private Autodesk.Revit.DB.Structure.BoundaryConditions CreateLineBc(Element hostElement)
        {
            var createDoc = hostElement.Document.Create;
            var analyticalModel = GetAnalyticalElement(hostElement);
            return createDoc.NewLineBoundaryConditions(analyticalModel, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        /// <summary>
        ///     Create a new Area BoundaryConditions Element.
        ///     All the parameter default as Fixed.
        /// </summary>
        /// <param name="hostElement">structural element which provide the hostElementId</param>
        /// <returns>the created Point BoundaryConditions Element</returns>
        private Autodesk.Revit.DB.Structure.BoundaryConditions CreateAreaBc(Element hostElement)
        {
            return hostElement.Document.Create.NewAreaBoundaryConditions(GetAnalyticalElement(hostElement), 0, 0, 0, 0, 0, 0);
        }

        //A delegate for create boundary condition with different type
        private delegate Autodesk.Revit.DB.Structure.BoundaryConditions
            CreateBcHandler(Element hostElement);
    }
}
