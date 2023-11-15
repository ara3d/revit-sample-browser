// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CreateDuctworkStiffener.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     The current document of the application
        /// </summary>
        private static Document _document;

        /// <summary>
        ///     The distance from ductwork start point to stiffener position
        ///     valid range: [0, m_ductwork.CenterlineLength]
        /// </summary>
        private double m_distanceToHostEnd;

        /// <summary>
        ///     The ductwork to host stiffeners
        /// </summary>
        private FabricationPart m_ductwork;

        /// <summary>
        ///     The type of the stiffeners
        /// </summary>
        private FamilySymbol m_stiffenerType;

        public Result Execute(ExternalCommandData commandData, ref string message,
            ElementSet elements)
        {
            _document = commandData.Application.ActiveUIDocument.Document;
            if (_document.IsFamilyDocument)
            {
                message = "Cannot create ductwork stiffener in family document";
                return Result.Failed;
            }

            //Get the ductwork in current project
            var ductCollector = new FilteredElementCollector(_document);
            ductCollector.OfCategory(BuiltInCategory.OST_FabricationDuctwork).OfClass(typeof(FabricationPart));
            if (ductCollector.GetElementCount() == 0)
            {
                message = "The document does not contain fabrication ductwork";
                return Result.Failed;
            }

            m_ductwork = ductCollector.FirstElement() as FabricationPart;

            //Get the ductwork stiffener type
            var stiffenerTypeCollector = new FilteredElementCollector(_document);
            stiffenerTypeCollector.OfCategory(BuiltInCategory.OST_FabricationDuctworkStiffeners)
                .OfClass(typeof(FamilySymbol));
            if (stiffenerTypeCollector.GetElementCount() == 0)
            {
                message = "The document does not contain stiffener family symbol";
                return Result.Failed;
            }

            var allStiffenerTypes = stiffenerTypeCollector.ToElements();
            var stiffenerTypeName = "Duct Stiffener - External Rectangular Angle Iron: L Angle";
            foreach (var element in allStiffenerTypes)
            {
                var f = element as FamilySymbol;
                var name = f.Family.Name + ": " + f.Name;
                if (name == stiffenerTypeName)
                {
                    m_stiffenerType = f;
                    break;
                }
            }

            if (m_stiffenerType == null)
            {
                message = "The stiffener type cannot be found in this document";
                return Result.Failed;
            }

            //Place the stiffener at ductwork middle point
            m_distanceToHostEnd = 0.5 * m_ductwork.CenterlineLength;

            try
            {
                using (var transaction = new Transaction(_document, "Sample_CreateDuctworkStiffener"))
                {
                    transaction.Start();
                    if (!m_stiffenerType.IsActive)
                    {
                        m_stiffenerType.Activate();
                        _document.Regenerate();
                    }

                    MEPSupportUtils.CreateDuctworkStiffener(_document, m_stiffenerType.Id, m_ductwork.Id,
                        m_distanceToHostEnd);
                    transaction.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
