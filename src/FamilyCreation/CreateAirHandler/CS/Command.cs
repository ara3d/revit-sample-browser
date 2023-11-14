// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Document = Autodesk.Revit.DB.Document;

namespace RevitMultiSample.CreateAirHandler.CS
{
    /// <summary>
    ///     Create one air handler and add connectors.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     The revit application
        /// </summary>
        private static Application _application;

        /// <summary>
        ///     The current document of the application
        /// </summary>
        private static Document _document;

        /// <summary>
        ///     the radius of the arc profile
        /// </summary>
        private readonly double m_arcRadius = 0.17;

        /// <summary>
        ///     the height and width of the connector
        /// </summary>
        private readonly double[,] m_connectorDimensions = new double[2, 2]
        {
            { 3.58, 3.4 },
            { 3.59, 10.833 }
        };

        /// <summary>
        ///     the start and end offsets of the extrusion
        /// </summary>
        private readonly double[,] m_extrusionOffsets = new double[5, 2]
        {
            { -0.9, 6.77 },
            { 0, -0.18 },
            { 0, -0.08 },
            { 1, 1.15 },
            { 1, 1.15 }
        };

        /// <summary>
        ///     the extrusion array
        /// </summary>
        private Extrusion[] m_extrusions;

        /// <summary>
        ///     the factory to creaate extrusions and connectors
        /// </summary>
        private FamilyItemFactory m_f;

        /// <summary>
        ///     the flow of the connector
        /// </summary>
        private readonly double m_flow = 547;

        /// <summary>
        ///     whether the extrusion is solid
        /// </summary>
        private readonly bool[] m_isSolid = new bool[5] { true, false, false, true, true };

        /// <summary>
        ///     The list of all the elements to be combined in the air handler system
        /// </summary>
        private CombinableElementArray m_combineElements;

        /// <summary>
        ///     Transaction of ExternalCommand
        /// </summary>
        private Transaction m_transaction;

        /// <summary>
        ///     Data to create extrusions and connectors
        /// </summary>
        private readonly XYZ[,] m_profileData = new XYZ[5, 4]
        {
            // In Array 0 to 2, the data is the points that defines the edges of the profile
            {
                new XYZ(-17.28, -0.53, 0.9),
                new XYZ(-17.28, 11, 0.9),
                new XYZ(-0.57, 11, 0.9),
                new XYZ(-0.57, -0.53, 0.9)
            },
            {
                new XYZ(-0.57, 7, 6.58),
                new XYZ(-0.57, 7, 3),
                new XYZ(-0.57, 3.6, 3),
                new XYZ(-0.57, 3.6, 6.58)
            },
            {
                new XYZ(-17.28, -0.073, 7.17),
                new XYZ(-17.28, 10.76, 7.17),
                new XYZ(-17.28, 10.76, 3.58),
                new XYZ(-17.28, -0.073, 3.58)
            },
            // In Array 3 and 4, the data is the normal and origin of the plane of the arc profile
            {
                new XYZ(0, -1, 0),
                new XYZ(-9, 0.53, 7.17),
                null,
                null
            },
            {
                new XYZ(0, -1, 0),
                new XYZ(-8.24, 0.53, 0.67),
                null,
                null
            }
        };

        /// <summary>
        ///     the normal and origin of the sketch plane
        /// </summary>
        private readonly XYZ[,] m_sketchPlaneData = new XYZ[5, 2]
        {
            { new XYZ(0, 0, 1), new XYZ(0, 0, 0.9) },
            { new XYZ(1, 0, 0), new XYZ(-0.57, 0, 0) },
            { new XYZ(-1, 0, 0), new XYZ(-17.28, 0, 0) },
            { new XYZ(0, -1, 0), new XYZ(0, 0.53, 0) },
            { new XYZ(0, -1, 0), new XYZ(0, 0.53, 0) }
        };

        public Result Execute(ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            // set out default result to failure.
            var retRes = Result.Failed;
            _application = commandData.Application.Application;
            _document = commandData.Application.ActiveUIDocument.Document;
            m_f = _document.FamilyCreate;
            m_extrusions = new Extrusion[5];
            m_combineElements = new CombinableElementArray();

            m_transaction = new Transaction(_document, "External Tool");
            m_transaction.Start();

            if (_document.OwnerFamily.FamilyCategory.Name !=
                _document.Settings.Categories.get_Item(BuiltInCategory.OST_MechanicalEquipment)
                    .Name) // FamilyCategory.Name is not "Mechanical Equipment".
            {
                message = "Please make sure you opened a template of Mechanical Equipment.";
                return retRes;
            }

            try
            {
                CreateExtrusions();
                _document.Regenerate();
                CreateConnectors();
                _document.Regenerate();
                _document.CombineElements(m_combineElements);
                _document.Regenerate();
            }
            catch (Exception x)
            {
                m_transaction.RollBack();
                message = x.Message;
                return retRes;
            }

            m_transaction.Commit();

            retRes = Result.Succeeded;
            return retRes;
        }

        /// <summary>
        ///     get all planar faces of an extrusion
        /// </summary>
        /// <param name="extrusion">the extrusion to read</param>
        /// <returns>a list of all planar faces of the extrusion</returns>
        public List<PlanarFace> GetPlanarFaces(Extrusion extrusion)
        {
            // the option to get geometry elements
            var geoOptions = _application.Create.NewGeometryOptions();
            geoOptions.View = _document.ActiveView;
            geoOptions.ComputeReferences = true;

            // get the planar faces
            var planarFaces = new List<PlanarFace>();
            var geoElement = extrusion.get_Geometry(geoOptions);
            //foreach (GeometryObject geoObject in geoElement.Objects)
            var objects = geoElement.GetEnumerator();
            while (objects.MoveNext())
            {
                var geoObject = objects.Current;

                var geoSolid = geoObject as Solid;
                if (null == geoSolid) continue;
                foreach (Face geoFace in geoSolid.Faces)
                    if (geoFace is PlanarFace face)
                        planarFaces.Add(face);
            }

            return planarFaces;
        }

        /// <summary>
        ///     create the extrusions of the air handler system
        /// </summary>
        private void CreateExtrusions()
        {
            var app = _application.Create;
            CurveArray curves = null;
            CurveArrArray profile = null;
            Plane plane = null;
            SketchPlane sketchPlane = null;

            for (var i = 0; i <= 2; ++i)
            {
                // create the profile
                curves = app.NewCurveArray();
                curves.Append(Line.CreateBound(m_profileData[i, 0], m_profileData[i, 1]));
                curves.Append(Line.CreateBound(m_profileData[i, 1], m_profileData[i, 2]));
                curves.Append(Line.CreateBound(m_profileData[i, 2], m_profileData[i, 3]));
                curves.Append(Line.CreateBound(m_profileData[i, 3], m_profileData[i, 0]));
                profile = app.NewCurveArrArray();
                profile.Append(curves);

                // create the sketch plane
                plane = Plane.CreateByNormalAndOrigin(m_sketchPlaneData[i, 0], m_sketchPlaneData[i, 1]);
                sketchPlane = SketchPlane.Create(_document, plane);

                // create the extrusion
                m_extrusions[i] = m_f.NewExtrusion(m_isSolid[i], profile, sketchPlane,
                    m_extrusionOffsets[i, 1]);
                m_extrusions[i].StartOffset = m_extrusionOffsets[i, 0];
                m_combineElements.Append(m_extrusions[i]);
            }

            for (var i = 3; i <= 4; ++i)
            {
                // create the profile
                profile = app.NewCurveArrArray();

                curves = app.NewCurveArray();
                plane = Plane.CreateByNormalAndOrigin(m_profileData[i, 0], m_profileData[i, 1]);
                curves.Append(Arc.Create(plane, m_arcRadius, 0, Math.PI * 2));
                profile.Append(curves);

                // create the sketch plane
                plane = Plane.CreateByNormalAndOrigin(m_sketchPlaneData[i, 0], m_sketchPlaneData[i, 1]);
                sketchPlane = SketchPlane.Create(_document, plane);

                // create the extrusion
                m_extrusions[i] = m_f.NewExtrusion(m_isSolid[i], profile, sketchPlane,
                    m_extrusionOffsets[i, 1]);
                m_extrusions[i].StartOffset = m_extrusionOffsets[i, 0];
                m_combineElements.Append(m_extrusions[i]);
            }
        }

        /// <summary>
        ///     create the connectors on the extrusions
        /// </summary>
        private void CreateConnectors()
        {
            // get the planar faces of extrusion1
            var planarFaces = GetPlanarFaces(m_extrusions[1]);

            // create the Supply Air duct connector
            //DuctConnector connSupplyAir = f.NewDuctConnector(m_planarFaces[0].Reference,
            //    DuctSystemType.SupplyAir);
            var connSupplyAir = ConnectorElement.CreateDuctConnector(_document, DuctSystemType.SupplyAir,
                ConnectorProfileType.Rectangular, planarFaces[0].Reference);
            var param = connSupplyAir.get_Parameter(BuiltInParameter.CONNECTOR_HEIGHT);
            param.Set(m_connectorDimensions[0, 0]);
            param = connSupplyAir.get_Parameter(BuiltInParameter.CONNECTOR_WIDTH);
            param.Set(m_connectorDimensions[0, 1]);
            param = connSupplyAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_DIRECTION_PARAM);
            param.Set(2);
            param = connSupplyAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_CONFIGURATION_PARAM);
            param.Set(1);
            param = connSupplyAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_PARAM);
            param.Set(m_flow);

            // get the planar faces of extrusion2
            planarFaces = GetPlanarFaces(m_extrusions[2]);

            // create the Return Air duct connector
            //DuctConnector connReturnAir = f.NewDuctConnector(m_planarFaces[0].Reference,
            //    DuctSystemType.ReturnAir);
            var connReturnAir = ConnectorElement.CreateDuctConnector(_document, DuctSystemType.ReturnAir,
                ConnectorProfileType.Rectangular, planarFaces[0].Reference);
            param = connReturnAir.get_Parameter(BuiltInParameter.CONNECTOR_HEIGHT);
            param.Set(m_connectorDimensions[1, 0]);
            param = connReturnAir.get_Parameter(BuiltInParameter.CONNECTOR_WIDTH);
            param.Set(m_connectorDimensions[1, 1]);
            param = connReturnAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_DIRECTION_PARAM);
            param.Set(1);
            param =
                connReturnAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_CONFIGURATION_PARAM);
            param.Set(1);
            param = connReturnAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_PARAM);
            param.Set(m_flow);

            // get the planar faces of extrusion3
            planarFaces = GetPlanarFaces(m_extrusions[3]);

            // create the Hydronic Supply pipe connector
            //PipeConnector connSupplyHydronic = f.NewPipeConnector(m_planarFaces[0].Reference,
            //    PipeSystemType.SupplyHydronic);
            var connSupplyHydronic = ConnectorElement.CreatePipeConnector(_document, PipeSystemType.SupplyHydronic,
                planarFaces[0].Reference);
            param = connSupplyHydronic.get_Parameter(BuiltInParameter.CONNECTOR_RADIUS);
            param.Set(m_arcRadius);
            param =
                connSupplyHydronic.get_Parameter(BuiltInParameter.RBS_PIPE_FLOW_DIRECTION_PARAM);
            param.Set(2);

            // get the planar faces of extrusion4
            planarFaces = GetPlanarFaces(m_extrusions[4]);

            // create the Hydronic Return pipe connector
            //PipeConnector connReturnHydronic = f.NewPipeConnector(m_planarFaces[0].Reference,
            //    PipeSystemType.ReturnHydronic);
            var connReturnHydronic = ConnectorElement.CreatePipeConnector(_document, PipeSystemType.ReturnHydronic,
                planarFaces[0].Reference);
            param = connReturnHydronic.get_Parameter(BuiltInParameter.CONNECTOR_RADIUS);
            param.Set(m_arcRadius);
            param =
                connReturnHydronic.get_Parameter(BuiltInParameter.RBS_PIPE_FLOW_DIRECTION_PARAM);
            param.Set(1);
        }
    }
}
