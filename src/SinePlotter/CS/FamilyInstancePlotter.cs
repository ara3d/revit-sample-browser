// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace RevitMultiSample.SinePlotter.CS
{
    /// <summary>
    ///     This class plots a number of instances of a given family object along a sine curve.
    /// </summary>
    internal class FamilyInstancePlotter
    {
        private readonly Document m_document;
        private readonly FamilySymbol m_familySymbol;

        /// <summary>
        ///     The constructor for the FamilyInstancePlotter Class.
        /// </summary>
        /// <param name="fs">A Revit family symbol.</param>
        /// <param name="doc">The active Revit document.</param>
        public FamilyInstancePlotter(FamilySymbol fs, Document doc)
        {
            m_familySymbol = fs;
            m_document = doc;
        }

        /// <summary>
        ///     Places a family instance at the given location.
        /// </summary>
        /// <param name="location">A point XYZ signifying the location for a family instance to be placed.</param>
        private void PlaceAtLocation(XYZ location)
        {
            var t = new Transaction(m_document, "Place family instance");
            t.Start();
            m_document.Create.NewFamilyInstance(location, m_familySymbol,
                StructuralType.NonStructural);
            t.Commit();
        }

        /// <summary>
        ///     Computes a sine curve taking into account input values for the sine curve period, amplitude and number
        ///     of circles and places a number of instances defined by the partitions input value of the same family
        ///     object along this curve.
        /// </summary>
        /// <param name="partitions">An integer value denoting the number of partitions per curve period.</param>
        /// <param name="period">
        ///     A double value denoting the period of the curve, i.e. how often the curve
        ///     goes a full repition around the unit circle.
        /// </param>
        /// <param name="amplitude">A double value denoting how far the curve gets away from the x-axis.</param>
        /// <param name="numOfCircles">A double value denoting the number of circles the curve makes.</param>
        public void PlaceInstancesOnCurve(int partitions, double period, double amplitude, double numOfCircles)
        {
            //Given the number of partitions compute the angle increment. 
            var theta = 2 * Math.PI / partitions;

            var transGroup = new TransactionGroup(m_document, "Place All Instances");
            transGroup.Start();
            //Calculates the sine of an angle. This function expects the values of the angle parameter to be
            //provided in radians (values from 0 to 6.28). Values are returned in the range -1 to 1. The theta 
            //increment will give us at every iteration the point on the curve x for which we need to evaluate y.
            for (var i = 0; i <= partitions * numOfCircles; i++)
            {
                var x = i * theta;
                //function used:  y = a*sin(b*x)
                var y = Math.Sin(period * x) * amplitude;
                var temp = new XYZ(x, y, 0);

                PlaceAtLocation(temp);
            }

            transGroup.Assimilate();
        }
    }
}
