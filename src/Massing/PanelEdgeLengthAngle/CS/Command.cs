// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PanelEdgeLengthAngle.CS
{
    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     This class shows how to compute the length and angle data of curtain panels
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SetLengthAngleParams : IExternalCommand
    {
        /// <summary>
        ///     The Revit application instance
        /// </summary>
        private Application m_app;

        /// <summary>
        ///     The active Revit document
        /// </summary>
        private Document m_doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_app = commandData.Application.Application;
            m_doc = commandData.Application.ActiveUIDocument.Document;

            // step 1: get all the divided surfaces in the Revit document
            var dsList = GetElements<DividedSurface>();

            foreach (var ds in dsList)
            {
                // step 2: get the panel instances from the divided surface
                var fiList = GetFamilyInstances(ds);
                foreach (var inst in fiList)
                {
                    // step 3: compute the length and angle and set them to the parameters
                    var instParams = GetParams(inst);
                    var edges = GetEdges(inst);
                    SetParams(edges, instParams);
                }
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Get all the panel instances from a divided surface
        /// </summary>
        /// <param name="ds">The divided surface with some panels</param>
        /// <returns>A list containing all the panel instances</returns>
        private List<FamilyInstance> GetFamilyInstances(DividedSurface ds)
        {
            var fiList = new List<FamilyInstance>();

            for (var u = 0; u < ds.NumberOfUGridlines; ++u)
            for (var v = 0; v < ds.NumberOfVGridlines; ++v)
            {
                var gn = new GridNode(u, v);
                var familyInstance = ds.GetTileFamilyInstance(gn, 0);
                if (familyInstance != null) fiList.Add(familyInstance);
            }

            return fiList;
        }

        /// <summary>
        ///     Get all the edges from the given family instance
        /// </summary>
        /// <param name="familyInstance">The family instance with some edges</param>
        /// <returns>Edges of the family instance</returns>
        private EdgeArray GetEdges(FamilyInstance familyInstance)
        {
            var opt = m_app.Create.NewGeometryOptions();
            opt.ComputeReferences = true;
            var geomElem = familyInstance.get_Geometry(opt);
            //foreach (GeometryObject geomObject1 in geomElem.Objects)
            var objects = geomElem.GetEnumerator();
            while (objects.MoveNext())
            {
                var geomObject1 = objects.Current;

                Solid solid = null;
                switch (geomObject1)
                {
                    // partial panels
                    case Solid object1:
                    {
                        solid = object1;
                        if (null == solid)
                            continue;
                        break;
                    }
                    // non-partial panels
                    case GeometryInstance geomInst:
                    {
                        //foreach (Object geomObj in geomInst.SymbolGeometry.Objects)
                        var objects1 = geomInst.SymbolGeometry.GetEnumerator();
                        while (objects1.MoveNext())
                        {
                            object geomObj = objects1.Current;

                            solid = geomObj as Solid;
                            if (solid != null)
                                break;
                        }

                        break;
                    }
                }

                if (null == solid || // the solid can't be null
                    null == solid.Faces || 0 == solid.Faces.Size || // the solid must have 1 or more faces
                    null == solid.Faces.get_Item(0) || // the solid must have a NOT-null face
                    null == solid.Faces.get_Item(0).EdgeLoops ||
                    0 == solid.Faces.get_Item(0).EdgeLoops.Size) // the face must have some edges
                    continue;

                return solid.Faces.get_Item(0).EdgeLoops.get_Item(0);
            }

            return null;
        }

        /// <summary>
        ///     Compute the length and angle data of the edges, then update the parameters with these values
        /// </summary>
        /// <param name="edge_ar">The edges of the curtain panel</param>
        /// <param name="instParams">The parameters which records the length and angle data</param>
        private void SetParams(EdgeArray edgeAr, InstParameters instParams)
        {
            var length4 = 0d;
            var angle3 = 0d;
            var angle4 = 0d;
            var edge1 = edgeAr.get_Item(0);
            var edge2 = edgeAr.get_Item(1);
            var edge3 = edgeAr.get_Item(2);
            var length1 = edge1.ApproximateLength;
            var length2 = edge2.ApproximateLength;
            var length3 = edge3.ApproximateLength;
            var angle1 = AngleBetweenEdges(edge1, edge2);
            var angle2 = AngleBetweenEdges(edge2, edge3);

            if (edgeAr.Size == 3)
            {
                angle3 = AngleBetweenEdges(edge3, edge1);
            }
            else if (edgeAr.Size > 3)
            {
                var edge4 = edgeAr.get_Item(3);
                length4 = edge4.ApproximateLength;
                angle3 = AngleBetweenEdges(edge3, edge4);
                angle4 = AngleBetweenEdges(edge4, edge1);
            }

            instParams["Length1"].Set(length1);
            instParams["Length2"].Set(length2);
            instParams["Length3"].Set(length3);
            instParams["Length4"].Set(length4);
            instParams["Angle1"].Set(angle1);
            instParams["Angle2"].Set(angle2);
            instParams["Angle3"].Set(angle3);
            instParams["Angle4"].Set(angle4);
        }

        /// <summary>
        ///     Compute the angle between two edges
        /// </summary>
        /// <param name="edgeA">The 1st edge</param>
        /// <param name="edgeB">The 2nd edge</param>
        /// <returns>The angle of the 2 edges</returns>
        private double AngleBetweenEdges(Edge edgeA, Edge edgeB)
        {
            XYZ vectorA = null;
            XYZ vectorB = null;

            // find coincident vertices
            var a0 = edgeA.Evaluate(0);
            var a1 = edgeA.Evaluate(1);
            var b0 = edgeB.Evaluate(0);
            var b1 = edgeB.Evaluate(1);
            if (a0.IsAlmostEqualTo(b0))
            {
                vectorA = edgeA.ComputeDerivatives(0).BasisX.Normalize();
                vectorB = edgeA.ComputeDerivatives(0).BasisX.Normalize();
            }
            else if (a0.IsAlmostEqualTo(b1))
            {
                vectorA = edgeA.ComputeDerivatives(0).BasisX.Normalize();
                vectorB = edgeB.ComputeDerivatives(1).BasisX.Normalize();
            }
            else if (a1.IsAlmostEqualTo(b0))
            {
                vectorA = edgeA.ComputeDerivatives(1).BasisX.Normalize();
                vectorB = edgeB.ComputeDerivatives(0).BasisX.Normalize();
            }
            else if (a1.IsAlmostEqualTo(b1))
            {
                vectorA = edgeA.ComputeDerivatives(1).BasisX.Normalize();
                vectorB = edgeB.ComputeDerivatives(1).BasisX.Normalize();
            }

            if (a1.IsAlmostEqualTo(b0) || a0.IsAlmostEqualTo(b1)) vectorA = vectorA.Negate();

            if (null == vectorA || null == vectorB) return 0d;
            var angle = Math.Acos(vectorA.DotProduct(vectorB));
            return angle;
        }

        /// <summary>
        ///     Get all the parameters and store them into a list
        /// </summary>
        /// <param name="familyInstance">The instance of a curtain panel</param>
        /// <returns>A list containing all the required parameters</returns>
        private InstParameters GetParams(FamilyInstance familyInstance)
        {
            var iParams = new InstParameters();
            var l1 = familyInstance.LookupParameter("Length1");
            var l2 = familyInstance.LookupParameter("Length2");
            var l3 = familyInstance.LookupParameter("Length3");
            var l4 = familyInstance.LookupParameter("Length4");
            var a1 = familyInstance.LookupParameter("Angle1");
            var a2 = familyInstance.LookupParameter("Angle2");
            var a3 = familyInstance.LookupParameter("Angle3");
            var a4 = familyInstance.LookupParameter("Angle4");

            if (l1 == null || l2 == null || l3 == null || l4 == null || a1 == null || a2 == null || a3 == null ||
                a4 == null)
            {
                var errorstring = "Panel family: " + familyInstance.Id + " '" + familyInstance.Symbol.Family.Name +
                                  "' must have instance parameters Length1, Length2, Length3, Length4, Angle1, Angle2, Angle3, and Angle4";
                TaskDialog.Show("Revit", errorstring);
                //   throw new ArgumentException(errorstring);
            }

            iParams["Length1"] = l1;
            iParams["Length2"] = l2;
            iParams["Length3"] = l3;
            iParams["Length4"] = l4;
            iParams["Angle1"] = a1;
            iParams["Angle2"] = a2;
            iParams["Angle3"] = a3;
            iParams["Angle4"] = a4;

            return iParams;
        }

        protected List<T> GetElements<T>() where T : Element
        {
            var returns = new List<T>();
            var collector = new FilteredElementCollector(m_doc);
            ICollection<Element> founds = collector.OfClass(typeof(T)).ToElements();
            foreach (var elem in founds) returns.Add(elem as T);
            return returns;
        }
    }

    /// <summary>
    ///     This class contains a dictionary which stores the parameter and parameter name pairs
    /// </summary>
    internal class InstParameters
    {
        private readonly Dictionary<string, Parameter> m_parameters = new Dictionary<string, Parameter>(8);

        /// <summary>
        ///     Get/Set the parameter by its name
        /// </summary>
        /// <param name="index">the name of the parameter</param>
        /// <returns>The parameter which matches the name</returns>
        public Parameter this[string index]
        {
            get => m_parameters[index];
            set => m_parameters[index] = value;
        }
    }
}
