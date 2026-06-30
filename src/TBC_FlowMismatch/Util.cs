using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static bool IsDesirableSystemPredicate(MEPSystem s)
        {
            return s is MechanicalSystem || s is PipingSystem
                && !s.Name.Equals("unassigned")
                && 0 < s.Elements.Size;
        }

        internal static void ReportSystemMistmatch(
            StringBuilder sb,
            Element e,
            string cnctrSys,
            string cnntdSys)
        {
            var p = e.get_Parameter(
                BuiltInParameter.ALL_MODEL_MARK);

            sb.Append(string.Format("Family instance '{0}' "
                                    + "has a connector {{{1}}} that is connected to a "
                                    + "{{{2}}} system...\n\n",
                p.AsString(), cnctrSys, cnntdSys));
        }

        internal static void FindMismatch(Document doc)
        {
            var cnntdSys = "";
            var cnctrSys = "";

            var sb = new StringBuilder();

            var systems
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(MEPSystem));

            var desirableSystems = systems
                .Cast<MEPSystem>()
                .Where(IsDesirableSystemPredicate);

            foreach (var system in desirableSystems)
            {
                if (system.GetType() == typeof(PipingSystem))
                    cnntdSys = (system as PipingSystem).SystemType.ToString();
                if (system.GetType() == typeof(MechanicalSystem))
                    cnntdSys = (system as MechanicalSystem).SystemType.ToString();

                var mepSet = system.Elements;
                foreach (Element e in mepSet)
                {
                    var mepCS = MepSystemSearch.GetConnectors(e);

                    foreach (Connector elemConnector in mepCS)
                    {
                        if (elemConnector.Domain.Equals(
                            Domain.DomainPiping))
                        {
                            cnctrSys = elemConnector.PipeSystemType.ToString();

                            if (elemConnector.MEPSystem != null
                                && elemConnector.MEPSystem.Name.Equals(
                                    system.Name))
                            {
                                if (!elemConnector.PipeSystemType.Equals(
                                    (system as PipingSystem).SystemType))
                                    ReportSystemMistmatch(sb, e,
                                        cnctrSys, cnntdSys);
                            }
                        }

                        if (null != elemConnector.MEPSystem
                            && elemConnector.Domain.Equals(
                                Domain.DomainHvac))
                        {
                            cnctrSys = elemConnector.DuctSystemType.ToString();

                            if (elemConnector.MEPSystem.Name.Equals(
                                system.Name))
                            {
                                if (!elemConnector.DuctSystemType.Equals(
                                    (system as MechanicalSystem).SystemType))
                                    ReportSystemMistmatch(sb, e,
                                        cnctrSys, cnntdSys);
                            }
                        }
                    }
                }
            }

            TaskDialog.Show("Flow Mismatch", $"{sb}\n");
        }
    }
}
