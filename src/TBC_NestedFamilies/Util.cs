using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NestedFamilies sample.</summary>
    internal static partial class Util
    {
        public static IEnumerable<Family>
            GetFilteredNestedFamilyDefinitions(
                string familyFileNameFilter,
                Document familyDocument,
                bool caseSensitiveFiltering)
        {
            ValidateFamilyDocument(familyDocument);

            FilteredElementCollector collector
                = new(familyDocument);

            collector.OfClass(typeof(Family));

            var familiesMatching =
                from f in collector
                where NestedFamilyFilterMatches(f.Name, familyFileNameFilter, caseSensitiveFiltering)
                select f;

            return familiesMatching.Cast<Family>();
        }
        public static List<FamilyInstance>
            GetFilteredNestedFamilyInstances(
                string familyFileNameFilter,
                string typeNameFilter,
                Document familyDocument,
                bool caseSensitiveFiltering)
        {
            ValidateFamilyDocument(familyDocument);

            List<FamilyInstance> oResult
                = [];

            FamilyInstance oFamilyInstanceCandidate;
            FamilySymbol oFamilySymbolCandidate;

            List<Family> oMatchingNestedFamilies
                = [];

            List<FamilyInstance> oAllFamilyInstances
                = [];

            var bFamilyFileNameFilterExists = true;
            var bTypeNameFilterExists = true;

            if (string.IsNullOrEmpty(familyFileNameFilter)) bFamilyFileNameFilterExists = false;

            if (string.IsNullOrEmpty(typeNameFilter)) bTypeNameFilterExists = false;

            ElementClassFilter fFamilyClass = new(typeof(Family));
            ElementClassFilter fFamInstClass = new(typeof(FamilyInstance));
            LogicalOrFilter f = new(fFamilyClass, fFamInstClass);
            FilteredElementCollector collector = new(familyDocument);
            collector.WherePasses(f);

            foreach (var e in collector)
                if (e is Family oNestedFamilyFileCandidate)
                {
                    if (!bFamilyFileNameFilterExists
                        || NestedFamilyFilterMatches(oNestedFamilyFileCandidate.Name,
                            familyFileNameFilter, caseSensitiveFiltering))

                        oMatchingNestedFamilies.Add(oNestedFamilyFileCandidate);
                }
                else
                {
                    oFamilyInstanceCandidate
                        = e as FamilyInstance;

                    if (oFamilyInstanceCandidate != null)
                        oAllFamilyInstances.Add(oFamilyInstanceCandidate);
                }

            foreach (var oMatchingNestedFamilyFile
                    in oMatchingNestedFamilies)
                for (var iCounter = oAllFamilyInstances.Count - 1;
                    iCounter >= 0;
                    iCounter--)
                {
                    oFamilyInstanceCandidate
                        = oAllFamilyInstances[iCounter];

                    var id = oFamilyInstanceCandidate.GetTypeId();
                    oFamilySymbolCandidate = familyDocument.GetElement(id)
                        as FamilySymbol;

                    if (oFamilySymbolCandidate.Family.UniqueId
                        == oMatchingNestedFamilyFile.UniqueId)
                    {
                        if (!bTypeNameFilterExists
                            || NestedFamilyFilterMatches(oFamilyInstanceCandidate.Name,
                                typeNameFilter, caseSensitiveFiltering))
                            oResult.Add(oFamilyInstanceCandidate);

                        oAllFamilyInstances.RemoveAt(iCounter);
                    }
                }

            return oResult;
        }
        public static Parameter GetNestedFamilyParameter(
            FamilyInstance nestedFamilyInstance,
            string parameterName)
        {
            if (nestedFamilyInstance == null)
                throw new ArgumentNullException(
                    "nestedFamilyInstance");

            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException(
                    "parameterName");

            Parameter oResult = null;

            Debug.Assert(2 > nestedFamilyInstance.GetParameters(parameterName).Count,
                "ascertain that there are not more than one parameter of the given name");

            oResult = nestedFamilyInstance.LookupParameter(parameterName);

            if (oResult == null)
            {
                Debug.Assert(2 > nestedFamilyInstance.Symbol.GetParameters(parameterName).Count,
                    "ascertain that there are not more than one parameter of the given name");

                oResult = nestedFamilyInstance.Symbol.LookupParameter(parameterName);
            }

            return oResult;
        }
        public static void
            LinkNestedFamilyParameterToHostFamilyParameter(
                Document hostFamilyDocument,
                FamilyInstance nestedFamilyInstance,
                string nestedFamilyParameterName,
                string hostFamilyParameterNameToLink)
        {
            ValidateFamilyDocument(hostFamilyDocument);

            if (nestedFamilyInstance == null)
                throw new ArgumentNullException(
                    "nestedFamilyInstance");

            if (string.IsNullOrEmpty(nestedFamilyParameterName))
                throw new ArgumentNullException(
                    "nestedFamilyParameterName");

            if (string.IsNullOrEmpty(hostFamilyParameterNameToLink))
                throw new ArgumentNullException(
                    "hostFamilyParameterNameToLink");

            var oNestedFamilyParameter
                = GetNestedFamilyParameter(nestedFamilyInstance,
                    nestedFamilyParameterName);

            if (oNestedFamilyParameter == null)
                throw new Exception($"Parameter '{nestedFamilyParameterName}' was not found on the nested family '{nestedFamilyInstance.Symbol.Name}'");

            var oHostFamilyParameter
                = hostFamilyDocument.FamilyManager.get_Parameter(
                    hostFamilyParameterNameToLink);

            if (oHostFamilyParameter == null)
                throw new Exception($"Parameter '{hostFamilyParameterNameToLink}' was not found on the host family.");

            hostFamilyDocument.FamilyManager
                .AssociateElementParameterToFamilyParameter(
                    oNestedFamilyParameter, oHostFamilyParameter);
        }

        private static bool NestedFamilyFilterMatches(
            string nameToCheck,
            string filter,
            bool caseSensitiveComparison)
        {
            if (string.IsNullOrEmpty(nameToCheck))
                return false;

            if (string.IsNullOrEmpty(filter))
                return true;

            if (!caseSensitiveComparison)
            {
                nameToCheck = nameToCheck.ToUpper();
                filter = filter.ToUpper();
            }

            return nameToCheck.Contains(filter);
        }

        private static void ValidateFamilyDocument(
            Document document)
        {
            if (null == document) throw new ArgumentNullException("document");

            if (!document.IsFamilyDocument)
                throw new ArgumentOutOfRangeException(
                    "The document provided is not a Family Document.");
        }
    }
}
