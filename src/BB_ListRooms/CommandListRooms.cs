using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class CommandListRooms : NamedCommand
    {
        public override string Name => "List Rooms";

        public List<Room> Rooms;

        public override void Execute(object arg)
        {
            var doc = (arg as UIApplication)?.ActiveUIDocument?.Document;
            if (doc == null) return;
            Rooms = doc.GetRooms().ToList();

            DataTableBuilder builder = new(typeof(RoomData));
            builder.AddRows(Rooms.Select(r => r.GetRoomData()));
            DataTableForm form = new(builder);
            form.Show();
        }
    }
}