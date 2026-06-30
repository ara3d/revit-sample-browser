// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.Events.SelectionChanged.CS
{
    public static class SelectionChangedEventArgsExtension
    {
        public static string GetInfo(this SelectionChangedEventArgs args, bool showId = false)
        {
            var sb = new StringBuilder();
            sb.AppendLine();

            var doc = args.GetDocument();
            sb.AppendLine($"[Event] {EventLoggingHelper.GetRevitUiEventName(args.GetType())}: {EventLoggingHelper.TitleNoExt(doc.Title)}");

            var refs = args.GetReferences();
            sb.AppendLine($"Selection Count:{refs.Count}");

            foreach (var aRef in refs)
            {
                switch (aRef.ElementReferenceType)
                {
                    case ElementReferenceType.REFERENCE_TYPE_NONE:
                    {
                        sb.Append("The reference is to an element.");
                        if (showId)
                        {
                            sb.AppendFormat(" ElementId:{0}.", aRef.ElementId);
                            if (aRef.LinkedElementId != ElementId.InvalidElementId)
                                sb.AppendFormat("LinkedElementId:{0}", aRef.LinkedElementId);
                        }

                        break;
                    }
                    case ElementReferenceType.REFERENCE_TYPE_LINEAR:
                    {
                        sb.Append("The reference is to a curve or edge.");
                        if (showId) sb.AppendFormat(" ElementId:{0}.", aRef.ElementId);
                        break;
                    }
                    case ElementReferenceType.REFERENCE_TYPE_SURFACE:
                    {
                        sb.Append("The reference is to a face or face region.");
                        if (showId) sb.AppendFormat(" ElementId:{0}.", aRef.ElementId);
                        break;
                    }
                    case ElementReferenceType.REFERENCE_TYPE_FOREIGN:
                    {
                        sb.Append("The reference is to geometry or elements in linked Revit file.");
                        if (showId) sb.AppendFormat(" LinkedElementId:{0}.", aRef.LinkedElementId);
                        break;
                    }
                    case ElementReferenceType.REFERENCE_TYPE_INSTANCE:
                    {
                        sb.Append("The reference is to an instance of a symbol.");
                        if (showId) sb.AppendFormat(" ElementId:{0}.", aRef.ElementId);
                        break;
                    }
                    case ElementReferenceType.REFERENCE_TYPE_CUT_EDGE:
                    {
                        sb.Append("The reference is to a face that was cut.");
                        if (showId) sb.AppendFormat(" ElementId:{0}.", aRef.ElementId);
                        break;
                    }
                    case ElementReferenceType.REFERENCE_TYPE_MESH:
                    {
                        sb.Append("The reference is to a mesh.");
                        if (showId) sb.AppendFormat(" ElementId:{0}.", aRef.ElementId);
                        break;
                    }
                    case ElementReferenceType.REFERENCE_TYPE_SUBELEMENT:
                    {
                        sb.Append("The reference is to a subelement.");
                        if (showId) sb.AppendFormat(" ElementId:{0}.", aRef.ElementId);
                        break;
                    }
                    default:
                    {
                        sb.Append("Unknown reference type.");
                        break;
                    }
                }

                var elem = doc.GetElement(aRef.ElementId);
                if (elem != null) sb.AppendFormat(" Name:{0}.", elem.Name);

                if (aRef.LinkedElementId != ElementId.InvalidElementId && elem is RevitLinkInstance instance)
                {
                    var linkedElem = instance.GetLinkDocument().GetElement(aRef.LinkedElementId);
                    if (linkedElem != null) sb.AppendFormat("Linked Element Name:{0}.", linkedElem.Name);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
