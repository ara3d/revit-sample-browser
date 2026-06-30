// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Ara3D.RevitSampleBrowser.PointCloudEngine.CS
{
    [Regeneration(RegenerationOption.Manual)]
    public class PointCloudTestApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                IPointCloudEngine engine = new BasicPointCloudEngine(PointCloudEngineType.Predefined);
                PointCloudEngineRegistry.RegisterPointCloudEngine("apipc", engine, false);

                engine = new BasicPointCloudEngine(PointCloudEngineType.RandomizedPoints);
                PointCloudEngineRegistry.RegisterPointCloudEngine("apipcr", engine, false);

                engine = new BasicPointCloudEngine(PointCloudEngineType.FileBased);
                PointCloudEngineRegistry.RegisterPointCloudEngine("xml", engine, true);

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

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class AddPredefinedInstanceCommand : AddInstanceCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            AddInstance(doc, "apipc", "", Transform.Identity);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class AddRandomizedInstanceCommand : AddInstanceCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            AddInstance(doc, "apipcr", "", Transform.Identity);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class AddTransformedInstanceCommand : AddInstanceCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.View.Document;

            var trf = Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI / 6.0, new XYZ(10, 5, 0));
            AddInstance(doc, "apipcr", "", trf);

            return Result.Succeeded;
        }
    }

    public class AddInstanceCommandBase
    {
        public void AddInstance(Document doc, string engineType, string identifier, Transform trf)
        {
            Transaction t = new(doc, "Create PC instance");
            t.Start();
            var type = PointCloudType.Create(doc, engineType, identifier);
            PointCloudInstance.Create(doc, type.Id, trf);
            t.Commit();
        }
    }

    [Transaction(TransactionMode.ReadOnly)]
    public class SerializePredefinedPointCloud : AddInstanceCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            PredefinedPointCloud cloud = new("dummy");

            XDocument doc = new();
            XElement root = new("PointCloud");
            cloud.SerializeObjectData(root);
            doc.Add(root);

            TextWriter writer = new StreamWriter(@"c:\serializedPC.xml");
            doc.WriteTo(new XmlTextWriter(writer));

            writer.Close();

            return Result.Succeeded;
        }
    }
}
