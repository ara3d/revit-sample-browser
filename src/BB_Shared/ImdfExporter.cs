using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using FilePath = Ara3D.Utils.FilePath;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class ImdfExporter : Bowerbird.NamedCommand
    {
        public string Name => "IMDF Exporter";

        public IDictionary<long, Room> Rooms { get; set; }
        public IDictionary<long, Level> Levels { get; set; }
        public IDictionary<long, FamilyInstance> Doors { get; set; }

        public override void Execute(object arg)
        {
            if (arg is not UIApplication uiapp)
                return;
            var doc = uiapp.ActiveUIDocument.Document;

            Rooms = doc.GetRooms().ToDictionary();
            Levels = doc.GetLevels().ToDictionary();
            Doors = doc.GetDoors().ToDictionary();

            var geoJsonDoc = Rooms.Values.ToGeoJson();
            LayoutFile.WriteJson(geoJsonDoc);
        }

        public static FilePath LayoutFile
            => new(@"C:\Users\cdigg\AppData\Local\Temp\imdf.geojson");
    }
}
