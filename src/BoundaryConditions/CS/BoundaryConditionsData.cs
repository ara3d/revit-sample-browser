// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.BoundaryConditions.CS
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
            SetBCHostMap(element);
        }

        // the selected Element

        // store all the corresponding BCs of the current selected host element 
        // and use the BC Id value as the key

        // the object for which the grid in UI displays.

        /// <summary>
        ///     gets or sets the object for which the grid in UI displays.
        /// </summary>
        public BCProperties BCProperties { get; set; }

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
            CreateBCHandler createBCH = null;

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
                            createBCH = CreateLineBC;
                            break;
                        case StructuralType.Brace:
                        case StructuralType.Column:
                        // create point BC for Column/brace
                        case StructuralType.Footing:
                            createBCH = CreatePointBC;
                            break;
                    }
                    break;
                }
                case Wall _:
                    // create line BC for wall
                    createBCH = CreateLineBC;
                    break;
                case Floor _:
                    // create area BC for Floor
                    createBCH = CreateAreaBC;
                    break;
                case WallFoundation _:
                    // create line BC for WallFoundation
                    createBCH = CreateLineBC;
                    break;
            }

            // begin create
            Autodesk.Revit.DB.Structure.BoundaryConditions NewBC = null;
            try
            {
                NewBC = createBCH(HostElement);
                if (null == NewBC) return false;
            }
            catch (Exception)
            {
                return false;
            }

            // add the created Boundary Conditions into m_bCsDictionary
            BCs.Add(NewBC.Id, NewBC);
            return true;
        }

        /// <summary>
        ///     store the selected element and its corresponding BCs
        /// </summary>
        /// <param name="element"> use selected element in Revit UI(the host element)</param>
        private void SetBCHostMap(Element element)
        {
            // set the Host element with current selected element
            HostElement = element;
            // retrieve the Document in which the Element resides.
            var doc = element.Document;

            var boundaryConditions = from elem in
                    new FilteredElementCollector(doc).OfClass(typeof(Autodesk.Revit.DB.Structure.BoundaryConditions))
                        .ToElements()
                let bC = elem as Autodesk.Revit.DB.Structure.BoundaryConditions
                where bC != null && HostElement.Id == bC.HostElementId
                select bC;
            foreach (var bC in boundaryConditions) BCs.Add(bC.Id, bC);
        }

        private AnalyticalElement GetAnalyticalElement(Element element)
        {
            AnalyticalElement analyticalModel = null;
            var document = element.Document;
            var assocManager =
                AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            if (assocManager != null)
            {
                var associatedElementId = assocManager.GetAssociatedElementId(element.Id);
                if (associatedElementId != ElementId.InvalidElementId)
                {
                    var associatedElement = document.GetElement(associatedElementId);
                    if (associatedElement != null && associatedElement is AnalyticalElement analyticalElement)
                        analyticalModel = analyticalElement;
                }
            }

            return analyticalModel;
        }

        /// <summary>
        ///     Create a new Point BoundaryConditions Element.
        ///     All the parameter default as Fixed.
        /// </summary>
        /// <param name="hostElement">
        ///     structural element which provide the analytical line end reference
        /// </param>
        /// <returns> the created Point BoundaryConditions Element</returns>
        private Autodesk.Revit.DB.Structure.BoundaryConditions CreatePointBC(Element hostElement)
        {
            if (!(hostElement is FamilyInstance)) return null;

            var analyticalModel = GetAnalyticalElement(hostElement);
            Reference endReference = null;

            var refCurve = analyticalModel.GetCurve();
            if (null != refCurve)
                endReference =
                    analyticalModel.GetReference(
                        new AnalyticalModelSelector(refCurve, AnalyticalCurveSelector.EndPoint));
            else
                return null;

            var createDoc = hostElement.Document.Create;

            // invoke Document.NewPointBoundaryConditions Method 
            var createdBC =
                createDoc.NewPointBoundaryConditions(endReference, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            return createdBC;
        }

        /// <summary>
        ///     Create a new Line BoundaryConditions Element.
        ///     All the parameter default as Fixed.
        /// </summary>
        /// <param name="hostElement">structural element which provide the hostElementId</param>
        /// <returns>the created Point BoundaryConditions Element</returns>
        private Autodesk.Revit.DB.Structure.BoundaryConditions CreateLineBC(Element hostElement)
        {
            var createDoc = hostElement.Document.Create;
            // invoke Document.NewLineBoundaryConditions Method
            var analyticalModel = GetAnalyticalElement(hostElement);
            var createdBC =
                createDoc.NewLineBoundaryConditions(analyticalModel, 0, 0, 0, 0, 0, 0, 0, 0);
            return createdBC;
        }

        /// <summary>
        ///     Create a new Area BoundaryConditions Element.
        ///     All the parameter default as Fixed.
        /// </summary>
        /// <param name="hostElement">structural element which provide the hostElementId</param>
        /// <returns>the created Point BoundaryConditions Element</returns>
        private Autodesk.Revit.DB.Structure.BoundaryConditions CreateAreaBC(Element hostElement)
        {
            var createDoc = hostElement.Document.Create;

            // invoke Document.NewAreaBoundaryConditions Method
            var createdBC =
                createDoc.NewAreaBoundaryConditions(GetAnalyticalElement(hostElement), 0, 0, 0, 0, 0, 0);
            return createdBC;
        }

        //A delegate for create boundary condition with different type
        private delegate Autodesk.Revit.DB.Structure.BoundaryConditions
            CreateBCHandler(Element HostElement);
    }
}
