using Ara3D.Geometry;
using System.Collections.Generic;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class LevelStruct
    {
        public int Id;
        public string Name;
        public override string ToString()
        {
            return $"{Name} - {Id}";
        }
    }

    public class Polygon
    {
        public List<Vector3> Points = [];
    }

    public class RoomStruct
    {
        public int Id;
        public string Name;
        public int Level;
        public Bounds3D Bounds;
        public List<Polygon> Polygons = [];
        public override string ToString()
        {
            return $"{Name} - {Id}";
        }
    }

    public class DoorStruct
    {
        public int Id;
        public string Name;
        public int Level;
        public int FromRoom;
        public int ToRoom;
        public Bounds3D Bounds;
        public override string ToString()
        {
            return $"{Name} - {Id}";
        }
    }

    public class FloorPlanStruct
    {
        public Dictionary<int, RoomStruct> Rooms = [];
        public Dictionary<int, DoorStruct> Doors = [];
        public Dictionary<int, LevelStruct> Levels = [];
    }
}
