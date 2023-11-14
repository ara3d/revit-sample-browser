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

namespace Revit.SDK.Samples.CreateAirHandler.CS
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
        private static Application m_application;

        /// <summary>
        ///     The current document of the application
        /// </summary>
        private static Document m_document;

        /// <summary>
        ///     the radius of the arc profile
        /// </summary>
        private readonly double arcRadius = 0.17;

        /// <summary>
        ///     the height and width of the connector
        /// </summary>
        private readonly double[,] connectorDimensions = new double[2, 2]
        {
            { 3.58, 3.4 },
            { 3.59, 10.833 }
        };

        /// <summary>
        ///     the start and end offsets of the extrusion
        /// </summary>
        private readonly double[,] extrusionOffsets = new double[5, 2]
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
        private Extrusion[] extrusions;

        /// <summary>
        ///     the factory to creaate extrusions and connectors
        /// </summary>
        private FamilyItemFactory f;

        /// <summary>
        ///     the flow of the connector
        /// </summary>
        private readonly double flow = 547;

        /// <summary>
        ///     whether the extrusion is solid
        /// </summary>
        private readonly bool[] isSolid = new bool[5] { true, false, false, true, true };

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
        private readonly XYZ[,] profileData = new XYZ[5, 4]
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
        private readonly XYZ[,] sketchPlaneData = new XYZ[5, 2]
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
            m_application = commandData.Application.Application;
            m_document = commandData.Application.ActiveUIDocument.Document;
            f = m_document.FamilyCreate;
            extrusions = new Extrusion[5];
            m_combineElements = new CombinableElementArray();

            m_transaction = new Transaction(m_document, "External Tool");
            m_transaction.Start();

            if (m_document.OwnerFamily.FamilyCategory.Name !=
                m_document.Settings.Categories.get_Item(BuiltInCategory.OST_MechanicalEquipment)
                    .Name) // FamilyCategory.Name is not "Mechanical Equipment".
            {
                message = "Please make sure you opened a template of Mechanical Equipment.";
                return retRes;
            }

            try
            {
                CreateExtrusions();
                m_document.Regenerate();
                CreateConnectors();
                m_document.Regenerate();
                m_document.CombineElements(m_combineElements);
                m_document.Regenerate();
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
            var m_geoOptions = m_application.Create.NewGeometryOptions();
            m_geoOptions.View = m_document.ActiveView;
            m_geoOptions.ComputeReferences = true;

            // get the planar faces
            var m_planarFaces = new List<PlanarFace>();
            var geoElement = extrusion.get_Geometry(m_geoOptions);
            //foreach (GeometryObject geoObject in geoElement.Objects)
            var Objects = geoElement.GetEnumerator();
            while (Objects.MoveNext())
            {
                var geoObject = Objects.Current;

                var geoSolid = geoObject as Solid;
                if (null == geoSolid) continue;
                foreach (Face geoFace in geoSolid.Faces)
                    if (geoFace is PlanarFace face)
                        m_planarFaces.Add(face);
            }

            return m_planarFaces;
        }

        /// <summary>
        ///     create the extrusions of the air handler system
        /// </summary>
        private void CreateExtrusions()
        {
            var app = m_application.Create;
            CurveArray curves = null;
            CurveArrArray profile = null;
            Plane plane = null;
            SketchPlane sketchPlane = null;

            for (var i = 0; i <= 2; ++i)
            {
                // create the profile
                curves = app.NewCurveArray();
                curves.Append(Line.CreateBound(profileData[i, 0], profileData[i, 1]));
                curves.Append(Line.CreateBound(profileData[i, 1], profileData[i, 2]));
                curves.Append(Line.CreateBound(profileData[i, 2], profileData[i, 3]));
                curves.Append(Line.CreateBound(profileData[i, 3], profileData[i, 0]));
                profile = app.NewCurveArrArray();
                profile.Append(curves);

                // create the sketch plane
                plane = Plane.CreateByNormalAndOrigin(sketchPlaneData[i, 0], sketchPlaneData[i, 1]);
                sketchPlane = SketchPlane.Create(m_document, plane);

                // create the extrusion
                extrusions[i] = f.NewExtrusion(isSolid[i], profile, sketchPlane,
                    extrusionOffsets[i, 1]);
                extrusions[i].StartOffset = extrusionOffsets[i, 0];
                m_combineElements.Append(extrusions[i]);
            }

            for (var i = 3; i <= 4; ++i)
            {
                // create the profile
                profile = app.NewCurveArrArray();

                curves = app.NewCurveArray();
                plane = Plane.CreateByNormalAndOrigin(profileData[i, 0], profileData[i, 1]);
                curves.Append(Arc.Create(plane, arcRadius, 0, Math.PI * 2));
                profile.Append(curves);

                // create the sketch plane
                plane = Plane.CreateByNormalAndOrigin(sketchPlaneData[i, 0], sketchPlaneData[i, 1]);
                sketchPlane = SketchPlane.Create(m_document, plane);

                // create the extrusion
                extrusions[i] = f.NewExtrusion(isSolid[i], profile, sketchPlane,
                    extrusionOffsets[i, 1]);
                extrusions[i].StartOffset = extrusionOffsets[i, 0];
                m_combineElements.Append(extrusions[i]);
            }
        }

        /// <summary>
        ///     create the connectors on the extrusions
        /// </summary>
        private void CreateConnectors()
        {
            // get the planar faces of extrusion1
            var m_planarFaces = GetPlanarFaces(extrusions[1]);

            // create the Supply Air duct connector
            //DuctConnector connSupplyAir = f.NewDuctConnector(m_planarFaces[0].Reference,
            //    DuctSystemType.SupplyAir);
            var connSupplyAir = ConnectorElement.CreateDuctConnector(m_document, DuctSystemType.SupplyAir,
                ConnectorProfileType.Rectangular, m_planarFaces[0].Reference);
            var param = connSupplyAir.get_Parameter(BuiltInParameter.CONNECTOR_HEIGHT);
            param.Set(connectorDimensions[0, 0]);
            param = connSupplyAir.get_Parameter(BuiltInParameter.CONNECTOR_WIDTH);
            param.Set(connectorDimensions[0, 1]);
            param = connSupplyAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_DIRECTION_PARAM);
            param.Set(2);
            param = connSupplyAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_CONFIGURATION_PARAM);
            param.Set(1);
            param = connSupplyAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_PARAM);
            param.Set(flow);

            // get the planar faces of extrusion2
            m_planarFaces = GetPlanarFaces(extrusions[2]);

            // create the Return Air duct connector
            //DuctConnector connReturnAir = f.NewDuctConnector(m_planarFaces[0].Reference,
            //    DuctSystemType.ReturnAir);
            var connReturnAir = ConnectorElement.CreateDuctConnector(m_document, DuctSystemType.ReturnAir,
                ConnectorProfileType.Rectangular, m_planarFaces[0].Reference);
            param = connReturnAir.get_Parameter(BuiltInParameter.CONNECTOR_HEIGHT);
            param.Set(connectorDimensions[1, 0]);
            param = connReturnAir.get_Parameter(BuiltInParameter.CONNECTOR_WIDTH);
            param.Set(connectorDimensions[1, 1]);
            param = connReturnAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_DIRECTION_PARAM);
            param.Set(1);
            param =
                connReturnAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_CONFIGURATION_PARAM);
            param.Set(1);
            param = connReturnAir.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_PARAM);
            param.Set(flow);

            // get the planar faces of extrusion3
            m_planarFaces = GetPlanarFaces(extrusions[3]);

            // create the Hydronic Supply pipe connector
            //PipeConnector connSupplyHydronic = f.NewPipeConnector(m_planarFaces[0].Reference,
            //    PipeSystemType.SupplyHydronic);
            var connSupplyHydronic = ConnectorElement.CreatePipeConnector(m_document, PipeSystemType.SupplyHydronic,
                m_planarFaces[0].Reference);
            param = connSupplyHydronic.get_Parameter(BuiltInParameter.CONNECTOR_RADIUS);
            param.Set(arcRadius);
            param =
                connSupplyHydronic.get_Parameter(BuiltInParameter.RBS_PIPE_FLOW_DIRECTION_PARAM);
            param.Set(2);

            // get the planar faces of extrusion4
            m_planarFaces = GetPlanarFaces(extrusions[4]);

            // create the Hydronic Return pipe connector
            //PipeConnector connReturnHydronic = f.NewPipeConnector(m_planarFaces[0].Reference,
            //    PipeSystemType.ReturnHydronic);
            var connReturnHydronic = ConnectorElement.CreatePipeConnector(m_document, PipeSystemType.ReturnHydronic,
                m_planarFaces[0].Reference);
            param = connReturnHydronic.get_Parameter(BuiltInParameter.CONNECTOR_RADIUS);
            param.Set(arcRadius);
            param =
                connReturnHydronic.get_Parameter(BuiltInParameter.RBS_PIPE_FLOW_DIRECTION_PARAM);
            param.Set(1);
        }
    }
}
