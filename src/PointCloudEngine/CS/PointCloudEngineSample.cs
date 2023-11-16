// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.PointCloudEngine.CS
{
    /// <summary>
    ///     ExternalApplication used to register the point cloud engines managed by this sample.
    /// </summary>
    [Regeneration(RegenerationOption.Manual)]
    public class PointCloudTestApplication : IExternalApplication
    {
        /// <summary>
        ///     The implementation of IExternalApplication.OnStartup()
        /// </summary>
        /// <param name="application">The Revit application.</param>
        /// <returns>Result.Succeeded</returns>
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // Register point cloud engines for the sample.

                // Predefined engine (non-randomized)
                IPointCloudEngine engine = new BasicPointCloudEngine(PointCloudEngineType.Predefined);
                PointCloudEngineRegistry.RegisterPointCloudEngine("apipc", engine, false);

                // Predefined engine with randomized points at the cell borders
                engine = new BasicPointCloudEngine(PointCloudEngineType.RandomizedPoints);
                PointCloudEngineRegistry.RegisterPointCloudEngine("apipcr", engine, false);

                // XML-based point cloud definition
                engine = new BasicPointCloudEngine(PointCloudEngineType.FileBased);
                PointCloudEngineRegistry.RegisterPointCloudEngine("xml", engine, true);

                // Create user interface for accessing predefined point clouds
                var panel = application.CreateRibbonPanel("Point cloud testing");

                var assembly = Assembly.GetExecutingAssembly();

                panel.AddItem(new PushButtonData("AddPredefinedInstance",
                    "Add predefined instance",
                    assembly.Location,
                    "Ara3D.RevitSampleBrowser.CS.PointCloudEngine.AddPredefinedInstanceCommand"));
                panel.AddSeparator();

                panel.AddItem(new PushButtonData("AddRandomizedInstance",
                    "Add randomized instance",
                    assembly.Location,
                    "Ara3D.RevitSampleBrowser.CS.PointCloudEngine.AddRandomizedInstanceCommand"));
                panel.AddSeparator();

                panel.AddItem(new PushButtonData("AddTransformedInstance",
                    "Add randomized instance\nat transform",
                    assembly.Location,
                    "Ara3D.RevitSampleBrowser.CS.PointCloudEngine.AddTransformedInstanceCommand"));
                panel.AddSeparator();

                panel.AddItem(new PushButtonData("SerializePointCloud",
                    "Serialize point cloud (utility)",
                    assembly.Location,
                    "Ara3D.RevitSampleBrowser.CS.PointCloudEngine.SerializePredefinedPointCloud"));
            }
            catch (Exception e)
            {
                TaskDialog.Show("Exception from OnStartup", e.ToString());
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     The implementation of IExternalApplication.OnShutdown()
        /// </summary>
        /// <param name="application">The Revit application.</param>
        /// <returns>Result.Succeeded.</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     ExternalCommand to add a predefined point cloud.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class AddPredefinedInstanceCommand : AddInstanceCommandBase, IExternalCommand
    {
        /// <summary>
        ///     The implementation for IExternalCommand.Execute()
        /// </summary>
        /// <param name="commandData">The Revit command data.</param>
        /// <param name="message">The error message (ignored).</param>
        /// <param name="elements">The elements to display in the failure dialog (ignored).</param>
        /// <returns>Result.Succeeded</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            AddInstance(doc, "apipc", "", Transform.Identity);

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     ExternalCommand to a predefined point cloud with randomized points.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class AddRandomizedInstanceCommand : AddInstanceCommandBase, IExternalCommand
    {
        /// <summary>
        ///     The implementation for IExternalCommand.Execute()
        /// </summary>
        /// <param name="commandData">The Revit command data.</param>
        /// <param name="message">The error message (ignored).</param>
        /// <param name="elements">The elements to display in the failure dialog (ignored).</param>
        /// <returns>Result.Succeeded</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            AddInstance(doc, "apipcr", "", Transform.Identity);

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     ExternalCommand to add a predefined point cloud at a non-default transform.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class AddTransformedInstanceCommand : AddInstanceCommandBase, IExternalCommand
    {
        /// <summary>
        ///     The implementation for IExternalCommand.Execute()
        /// </summary>
        /// <param name="commandData">The Revit command data.</param>
        /// <param name="message">The error message (ignored).</param>
        /// <param name="elements">The elements to display in the failure dialog (ignored).</param>
        /// <returns>Result.Succeeded</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            var trf = Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI / 6.0, new XYZ(10, 5, 0));
            AddInstance(doc, "apipcr", "", trf);

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Base class for ExternalCommands used to add point cloud instances programmatically.
    /// </summary>
    public class AddInstanceCommandBase
    {
        /// <summary>
        ///     Adds a point cloud instance programmatically.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="engineType">The engine identifier string.</param>
        /// <param name="identifier">The identifier for the particular point cloud.</param>
        /// <param name="trf">The transform to apply to the new point cloud instance.</param>
        public void AddInstance(Document doc, string engineType, string identifier, Transform trf)
        {
            var t = new Transaction(doc, "Create PC instance");
            t.Start();
            var type = PointCloudType.Create(doc, engineType, identifier);
            PointCloudInstance.Create(doc, type.Id, trf);
            t.Commit();
        }
    }

    /// <summary>
    ///     Utility ExternalCommand to take a predefined point cloud and write the corresponding XML for it to disk.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    public class SerializePredefinedPointCloud : AddInstanceCommandBase, IExternalCommand
    {
        /// <summary>
        ///     The implementation for IExternalCommand.Execute()
        /// </summary>
        /// <param name="commandData">The Revit command data.</param>
        /// <param name="message">The error message (ignored).</param>
        /// <param name="elements">The elements to display in the failure dialog (ignored).</param>
        /// <returns>Result.Succeeded</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var cloud = new PredefinedPointCloud("dummy");

            var doc = new XDocument();
            var root = new XElement("PointCloud");
            cloud.SerializeObjectData(root);
            doc.Add(root);

            TextWriter writer = new StreamWriter(@"c:\serializedPC.xml");
            doc.WriteTo(new XmlTextWriter(writer));

            writer.Close();

            return Result.Succeeded;
        }
    }
}
