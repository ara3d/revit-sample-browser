// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.SlabProperties.CS
{
    /// <summary>
    ///     Get some properties of a slab , such as Level, Type name, Span direction,
    ///     Material name, Thickness, and Young Modulus for the slab's Material.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private const double Pi = 3.1415926535879;
        private const int Degree = 180;
        private const int ToMillimeter = 1000;
        private const double ToMetricThickness = 0.3048; // unit for changing inch to meter
        private const double ToMetricYoungmodulus = 304800.0;
        private Document m_document;

        private ElementSet m_slabComponent; // the selected Slab component
        private Floor m_slabFloor; // Floor 
        private CompoundStructureLayer m_slabLayer; // Structure Layer 
        private IList<CompoundStructureLayer> m_slabLayerCollection; // Structure Layer collection

        /// <summary>
        ///     Level property, read only.
        /// </summary>
        public string Level { get; private set; }

        /// <summary>
        ///     TypeName property, read only.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        ///     SpanDirection property, read only.
        /// </summary>
        public string SpanDirection { get; private set; }

        /// <summary>
        ///     NumberOfLayers property, read only.
        /// </summary>
        public int NumberOfLayers { get; private set; }

        /// <summary>
        ///     LayerThickness property, read only.
        /// </summary>
        public string LayerThickness { get; private set; }

        /// <summary>
        ///     LayerMaterialName property, read only.
        /// </summary>
        public string LayerMaterialName { get; private set; }

        /// <summary>
        ///     LayerYoungModulusX property, read only.
        /// </summary>
        public string LayerYoungModulusX { get; private set; }

        /// <summary>
        ///     LayerYoungModulusY property, read only.
        /// </summary>
        public string LayerYoungModulusY { get; private set; }

        /// <summary>
        ///     LayerYoungModulusZ property, read only.
        /// </summary>
        public string LayerYoungModulusZ { get; private set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var revit = commandData.Application;

            try
            {
                // function initialization and find out a slab's Level, Type name, and set the Span Direction properties.
                var isInitialization = Initialize(revit);
                if (false == isInitialization) return Result.Failed;

                // show a displayForm to display the properties of the slab
                var slabForm = new SlabPropertiesForm(this);
                if (DialogResult.OK != slabForm.ShowDialog()) return Result.Cancelled;
            }
            catch (Exception displayProblem)
            {
                TaskDialog.Show("Revit", displayProblem.ToString());
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     SetLayer method
        /// </summary>
        /// <param name="layerNumber">The layerNumber for the number of the layers</param>
        public void SetLayer(int layerNumber)
        {
            // Get each layer.
            // An individual layer can be accessed by Layers property and its thickness and material can then be reported.
            m_slabLayer = m_slabLayerCollection[layerNumber];

            // Get the Thickness property and change to the metric millimeter
            LayerThickness = $"{m_slabLayer.Width * ToMetricThickness * ToMillimeter} mm";

            // Get the Material name property
            if (ElementId.InvalidElementId != m_slabLayer.MaterialId)
            {
                var material = m_document.GetElement(m_slabLayer.MaterialId) as Material;
                LayerMaterialName = material.Name;
            }
            else
            {
                LayerMaterialName = "Null";
            }

            // The Young modulus can be found from the material by using the following generic parameters: 
            // PHY_MATERIAL_PARAM_YOUNG_MOD1, PHY_MATERIAL_PARAM_YOUNG_MOD2, PHY_MATERIAL_PARAM_YOUNG_MOD3
            if (ElementId.InvalidElementId != m_slabLayer.MaterialId)
            {
                var material = m_document.GetElement(m_slabLayer.MaterialId) as Material;
                var youngModuleAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD1);
                if (null != youngModuleAttribute)
                    LayerYoungModulusX =
                        $"{(youngModuleAttribute.AsDouble() / ToMetricYoungmodulus):F2} MPa";
                youngModuleAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD2);
                if (null != youngModuleAttribute)
                    LayerYoungModulusY =
                        $"{(youngModuleAttribute.AsDouble() / ToMetricYoungmodulus):F2} MPa";
                youngModuleAttribute = material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_YOUNG_MOD3);
                if (null != youngModuleAttribute)
                    LayerYoungModulusZ =
                        $"{(youngModuleAttribute.AsDouble() / ToMetricYoungmodulus):F2} MPa";
            }
            else
            {
                LayerYoungModulusX = "Null";
                LayerYoungModulusY = "Null";
                LayerYoungModulusZ = "Null";
            }
        }

        /// <summary>
        ///     Initialization and find out a slab's Level, Type name, and set the Span Direction properties.
        /// </summary>
        /// <param name="revit">The revit object for the active instance of Autodesk Revit.</param>
        /// <returns>A value that signifies if your initialization was successful for true or failed for false.</returns>
        private bool Initialize(UIApplication revit)
        {
            m_slabComponent = new ElementSet();
            foreach (var elementId in revit.ActiveUIDocument.Selection.GetElementIds())
            {
                m_slabComponent.Insert(revit.ActiveUIDocument.Document.GetElement(elementId));
            }

            m_document = revit.ActiveUIDocument.Document;

            // There must be exactly one slab selected
            if (m_slabComponent.IsEmpty)
            {
                // nothing selected
                TaskDialog.Show("Revit", "Please select a slab.");
                return false;
            }

            if (1 != m_slabComponent.Size)
            {
                // too many things selected
                TaskDialog.Show("Revit", "Please select only one slab.");
                return false;
            }

            foreach (Element e in m_slabComponent)
            {
                // If the element isn't a slab, give the message and return failure. 
                // Else find out its Level, Type name, and set the Span Direction properties. 
                if ("Autodesk.Revit.DB.Floor" != e.GetType().ToString())
                {
                    TaskDialog.Show("Revit", "A slab should be selected.");
                    return false;
                }

                // Change the element type to floor type
                m_slabFloor = e as Floor;

                // Get the layer information from the type object by using the CompoundStructure property
                // The Layers property is then used to retrieve all the layers
                m_slabLayerCollection = m_slabFloor.FloorType.GetCompoundStructure().GetLayers();
                NumberOfLayers = m_slabLayerCollection.Count;

                // Get the Level property by the floor's Level property
                Level = (m_document.GetElement(m_slabFloor.LevelId) as Level).Name;

                // Get the Type name property by the floor's FloorType property
                TypeName = m_slabFloor.FloorType.Name;

                // The span direction can be found using generic parameter access 
                // using the built in parameter FLOOR_PARAM_SPAN_DIRECTION
                var spanDirectionAttribute = m_slabFloor.get_Parameter(BuiltInParameter.FLOOR_PARAM_SPAN_DIRECTION);
                if (null != spanDirectionAttribute)
                    // Set the Span Direction property
                    SetSpanDirection(spanDirectionAttribute.AsDouble());
            }

            return true;
        }

        /// <summary>
        ///     Set SpanDirection property to the class private member
        ///     Because of the property retrieved from the parameter uses radian for unit, we should change it to degree.
        /// </summary>
        /// <param name="spanDirection">The value of span direction property</param>
        private void SetSpanDirection(double spanDirection)
        {
            var spanDirectionDegree =
                // Change "radian" to "degree".
                spanDirection / Pi * Degree;

            // If the absolute value very small, we consider it to be zero
            if (Math.Abs(spanDirectionDegree) < 1E-12) spanDirectionDegree = 0.0;

            // The precision is 0.01, and unit is "degree".
            SpanDirection = spanDirectionDegree.ToString("F2");
        }
    }
}
