using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static FamilyInstance
            SelectFamilyInstanceWithApparentLoad(
                UIDocument uidoc)
        {
            var electricalApparentLoadFactory
                = new ElectricalApparentLoadFactory();

            var selectionFilter
                = new FamilyInstanceWithApparentLoadSelectionFilter(
                    electricalApparentLoadFactory);

            try
            {
                return (FamilyInstance) uidoc.Document.GetElement(
                    uidoc.Selection.PickObject(ObjectType.Element,
                        selectionFilter));
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        internal class ElectricalApparentLoad
        {
            public ElectricalApparentLoad(
                ElectricalSystemType electricalSystemType,
                int connectorId,
                double apparentLoad)
            {
                ElectricalSystemType = electricalSystemType;
                ConnectorId = connectorId;
                ApparentLoad = apparentLoad;
            }

            public ElectricalSystemType ElectricalSystemType { get; }

            public int ConnectorId { get; }

            public double ApparentLoad { get; }

            public override string ToString()
            {
                return $"{ElectricalSystemType}: {ConnectorId} - {{ApparentLoad}} V*A";
            }
        }

        internal class ElectricalApparentLoadFactory
        {
            public IEnumerable<ElectricalApparentLoad>
                Create(FamilyInstance familyInstance)
            {
                return familyInstance.MEPModel
                    .ConnectorManager
                    .Connectors
                    .Cast<Connector>()
                    .Select(Create)
                    .Where(x => x != null);
            }

            private static ElectricalApparentLoad Create(
                Connector connector)
            {
                if (connector.Domain != Domain.DomainElectrical)
                    return null;

                var mepConnectorInfo
                    = connector.GetMEPConnectorInfo()
                        as MEPFamilyConnectorInfo;

                if (mepConnectorInfo
                    ?.GetConnectorParameterValue(
                        new ElementId(
                            BuiltInParameter.RBS_ELEC_APPARENT_LOAD)) is not DoubleParameterValue parameterValue)
                    return null;

                var load = UnitUtils.ConvertFromInternalUnits(
                    parameterValue.Value,
                    UnitTypeId.VoltAmperes);

                return new ElectricalApparentLoad(
                    connector.ElectricalSystemType,
                    connector.Id, load);
            }
        }

        internal class FamilyInstanceWithApparentLoadSelectionFilter
            : ISelectionFilter
        {
            private readonly ElectricalApparentLoadFactory
                electricalApparentLoadFactory;

            public FamilyInstanceWithApparentLoadSelectionFilter(
                ElectricalApparentLoadFactory
                    electricalApparentLoadFactory)
            {
                this.electricalApparentLoadFactory
                    = electricalApparentLoadFactory;
            }

            public bool AllowElement(Element elem)
            {
                if (elem is not FamilyInstance familyInstance)
                    return false;

                return electricalApparentLoadFactory
                    .Create(familyInstance)
                    .Any();
            }

            public bool AllowReference(Reference r, XYZ p)
            {
                return false;
            }
        }
    }
}
