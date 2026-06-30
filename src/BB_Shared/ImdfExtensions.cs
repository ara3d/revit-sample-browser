using Autodesk.Revit.DB.Architecture;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Bowerbird.RevitSamples;

public static class ImdfExtensions
{
    public static GeoJsonPolygon BoundaryAsGeoJson(this Room room)
    {
        return room.GetBoundaryLoops().Select(loop => loop.Select(pt => pt.ToAra3D())).ToGeoJson();
    }

    public static ImdfUnit ToImdf(this Room room)
    {
        return ImdfUnit.Create(
                room.Id.ToString(),
                room.BoundaryAsGeoJson(),
                new ImdfUnitProperties
                {
                    name = room.Name,
                    alt_name = room.Number,
                    level_elevation = room.Level?.Elevation,
                    level_id = room.LevelId.ToString(),
                    area = room.Area,
                    perimeter = room.Perimeter
                });
    }

    public static GeoJsonDocument ToGeoJson(this IEnumerable<Room> rooms)
    {
        GeoJsonDocument r = new();
        r.features.Add(rooms.Select(r => r.ToImdf()));
        return r;
    }
}