// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SvgExport by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/SvgExport

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using BoundarySegment = Autodesk.Revit.DB.BoundarySegment;

namespace Ara3D.RevitSampleBrowser.SvgExport.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        const int TargetSquareSize = 100;

        class RoomSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element e) => e is Room;

            public bool AllowReference(Reference r, XYZ p) => true;
        }

        static Result GetRoom(UIDocument uidoc, out Room room)
        {
            room = null;

            var doc = uidoc.Document;

            var rooms = new FilteredElementCollector(doc)
                .OfClass(typeof(SpatialElement));

            if (rooms.Count() == 1 && rooms.FirstElement() is Room)
            {
                room = rooms.FirstElement() as Room;
            }
            else
            {
                var sel = uidoc.Selection;

                foreach (var id in sel.GetElementIds())
                {
                    var e = doc.GetElement(id);
                    if (e is Room r)
                    {
                        room = r;
                        break;
                    }

                    if (e is RoomTag tag)
                    {
                        room = tag.Room;
                        break;
                    }
                }

                if (room == null)
                {
                    try
                    {
                        var r = sel.PickObject(
                            ObjectType.Element,
                            new RoomSelectionFilter(),
                            "Please select a room");

                        room = doc.GetElement(r.ElementId) as Room;
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                    {
                        return Result.Cancelled;
                    }
                }
            }

            return Result.Succeeded;
        }

        static string GetSvgPointFrom(XYZ p, XYZ pmid, double scale)
        {
            p -= pmid;
            p *= scale;
            var x = (int)(p.X + 0.5);
            var y = (int)(p.Y + 0.5);

            // Revit Y points up; SVG Y points down.
            y = -y;

            x += TargetSquareSize / 2;
            y += TargetSquareSize / 2;
            return x + " " + y;
        }

        static string GetSvgPathFrom(BoundingBoxXYZ bb, IList<BoundarySegment> loop)
        {
            var pmin = bb.Min;
            var pmax = bb.Max;
            var vsize = pmax - pmin;
            var pmid = pmin + 0.5 * vsize;
            var size = Math.Max(vsize.X, vsize.Y);
            var scale = TargetSquareSize / size;

            var s = new StringBuilder();
            var nSegments = loop.Count;

            XYZ p0 = null;
            XYZ p;
            XYZ q = null;

            foreach (var seg in loop)
            {
                var curve = seg.GetCurve();

                // Todo: handle non-linear curve.

                p = curve.GetEndPoint(0);

                Debug.Assert(null == q || q.IsAlmostEqualTo(p),
                    "expected last endpoint to equal current start point");

                q = curve.GetEndPoint(1);

                if (null == p0)
                {
                    p0 = p;
                    s.Append("M" + GetSvgPointFrom(p, pmid, scale));
                }

                s.Append("L" + GetSvgPointFrom(q, pmid, scale));
            }

            s.Append("Z");

            Debug.Assert(q.IsAlmostEqualTo(p0),
                "expected last endpoint to equal loop start point");

            return s.ToString();
        }

        static string BuildSvgDocument(string pathData)
        {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" width=""{TargetSquareSize}"" height=""{TargetSquareSize}"" viewBox=""0 0 {TargetSquareSize} {TargetSquareSize}"">
  <path d=""{pathData}"" fill=""none"" stroke=""black""/>
</svg>
";
        }

        static bool SaveSvg(string pathData, bool interactive, ref string message)
        {
            string filePath;

            if (interactive)
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Title = "Export room boundary to SVG";
                    sfd.Filter = "SVG files (*.svg)|*.svg";
                    sfd.RestoreDirectory = true;

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return false;

                    filePath = sfd.FileName;
                }
            }
            else
            {
                var folder = Environment.GetEnvironmentVariable("TEMP");
                filePath = Path.Combine(folder, "RoomBoundary.svg");
            }

            try
            {
                File.WriteAllText(filePath, BuildSvgDocument(pathData));
            }
            catch (IOException e)
            {
                message = $"Failed to write SVG file: {e.Message}";
                return false;
            }

            if (interactive)
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

            return true;
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;

            Room room;
            var rc = GetRoom(uidoc, out room);
            if (rc != Result.Succeeded)
                return rc;

            if (room == null)
            {
                message = "No room found or selected.";
                return Result.Failed;
            }

            var opt = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center
            };

            var loops = room.GetBoundarySegments(opt);
            if (loops == null || loops.Count == 0 || loops[0].Count == 0)
            {
                message = "Room has no boundary segments.";
                return Result.Failed;
            }

            var bb = room.get_BoundingBox(null);
            var pathData = GetSvgPathFrom(bb, loops[0]);

            var interactive = !app.IsJournalPlaying();
            if (!SaveSvg(pathData, interactive, ref message))
                return interactive ? Result.Cancelled : Result.Failed;

            return Result.Succeeded;
        }
    }
}
