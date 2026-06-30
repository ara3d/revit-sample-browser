// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DoorSwing.CS
{
    public class DoorFamily
    {
        // Revit application
        private readonly UIApplication m_app;

        // opening value of one of this family's door which neither flipped nor mirrored.
        private string m_basalOpeningValue;

        // door family
        private readonly Family m_family;

        // the geometry of one of this family's door which neither flipped nor mirrored.
        private DoorGeometry m_geometry;

        // one door instance of this family.
        private readonly FamilyInstance m_oneInstance;

        public DoorFamily(Family doorFamily, UIApplication app)
        {
            m_app = app;
            m_family = doorFamily;
            // one door instance which belongs to this family and neither flipped nor mirrored.
            m_oneInstance = CreateOneInstanceWithThisFamily();
        }

        public string FamilyName => m_family.Name;

        public string BasalOpeningValue
        {
            get
            {
                if (string.IsNullOrEmpty(m_basalOpeningValue))
                {
                    var paramValue = DoorSwingResource.Undefined;

                    // get current opening value.  
                    var fss = new List<FamilySymbol>();
                    foreach (var elementId in m_family.GetFamilySymbolIds())
                    {
                        fss.Add((FamilySymbol)m_app.ActiveUIDocument.Document.GetElement(elementId));
                    }

                    var doorSymbol = fss[0];
                    paramValue = doorSymbol.ParametersMap.get_Item("BasalOpening").AsString();

                    // deal with invalid string.
                    if (!DoorSwingData.OpeningTypes.Contains(paramValue)) paramValue = DoorSwingResource.Undefined;

                    m_basalOpeningValue = paramValue;
                }

                return m_basalOpeningValue;
            }
            set => m_basalOpeningValue = value;
        }

        public DoorGeometry Geometry =>
            m_geometry ?? (m_geometry =
                // create one instance of DoorFamilyGeometry class.
                new DoorGeometry(m_oneInstance));

        public void UpdateOpeningFeature()
        {
            // get current Left/Right feature's value of this door family.
            var ffs = new List<FamilySymbol>();
            foreach (var elementId in m_family.GetFamilySymbolIds())
            {
                ffs.Add((FamilySymbol)m_app.ActiveUIDocument.Document.GetElement(elementId));
            }

            foreach (var doorSymbol in ffs)
            {
                if (doorSymbol.ParametersMap.Contains("BasalOpening"))
                {
                    var basalOpeningParam = doorSymbol.ParametersMap.get_Item("BasalOpening");
                    basalOpeningParam.Set(m_basalOpeningValue);
                }
            }
        }

        public void DeleteTempDoorInstance()
        {
            var doc = m_app.ActiveUIDocument.Document;
            var tempWall = m_oneInstance.Host;
            doc.Delete(m_oneInstance.Id); // delete temporarily created door instance with this family.
            doc.Delete(tempWall.Id);
        }

        private FamilyInstance CreateOneInstanceWithThisFamily()
        {
            var doc = m_app.ActiveUIDocument.Document;
            var creDoc = doc.Create;

            // get one level. A project has at least one level.
            var level = new FilteredElementCollector(doc).OfClass(typeof(Level)).FirstElement() as Level;

            // create one wall as door's host
            var wallCurve = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(100, 0, 0));
            var host = Wall.Create(doc, wallCurve, level.Id, false);
            doc.Regenerate();

            // door symbol.
            var ffs = new List<FamilySymbol>();
            foreach (var elementId in m_family.GetFamilySymbolIds())
            {
                ffs.Add((FamilySymbol)m_app.ActiveUIDocument.Document.GetElement(elementId));
            }

            var doorSymbol = ffs[0];

            var createdFamilyInstance = creDoc.NewFamilyInstance(new XYZ(0, 0, 0), doorSymbol, host, level,
                StructuralType.NonStructural);
            doc.Regenerate();

            return createdFamilyInstance;
        }
    }
}
