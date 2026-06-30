using Ara3D.Collections;
using Ara3D.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class RenderMesh
{
    private readonly RenderVertex[] _vertices;
    private readonly int[] _indices;

    public readonly Outline Bounds;

    public Span<RenderVertex> Vertices => _vertices;
    public Span<int> Indices => _indices;

    public int VertexCount => _vertices.Length;
    public int IndexCount => _indices.Length;

    public RenderMesh(RenderVertex[] vertices, int[] indices)
    {
        _vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
        _indices = indices ?? throw new ArgumentNullException(nameof(indices));
        Bounds = _computeBoundingBox();
        ApplyBoundingBoxGradient();
        Debug.Assert(_vertices.Length > 0, "Mesh must contain at least one vertex.");
    }

    public static RenderMesh Create(
        IReadOnlyList<Point3D> positions,
        IReadOnlyList<int>? indices,
        IReadOnlyList<Vector3>? normals = null,
        IReadOnlyList<Vector2>? uvs = null,
        IReadOnlyList<Color32>? colors = null)
    {
        if (positions is null) throw new ArgumentNullException(nameof(positions));
        if (positions.Count == 0) throw new ArgumentException("Empty meshes are not supported.", nameof(positions));
        if (indices is null) throw new ArgumentNullException(nameof(indices), "Triangle meshes require indices.");
        if (indices.Count == 0) throw new ArgumentException("Mesh indices cannot be empty.", nameof(indices));
        if (indices.Count % 3 != 0) throw new ArgumentException("Triangle indices must be a multiple of 3.", nameof(indices));

        var n = positions.Count;

        normals ??= Vector3.Default.Repeat(n);
        uvs ??= Vector2.Default.Repeat(n);
        colors ??= new Color32(128, 128, 128).Repeat(n);

        if (normals.Count != n) throw new InvalidOperationException($"Normals count {normals.Count} must match vertices count {n}.");
        if (uvs.Count != n) throw new InvalidOperationException($"UVs count {uvs.Count} must match vertices count {n}.");
        if (colors.Count != n) throw new InvalidOperationException($"Colors count {colors.Count} must match vertices count {n}.");

        var rv = new RenderVertex[n];
        for (var i = 0; i < n; i++)
            rv[i] = new RenderVertex(positions[i], normals[i], uvs[i], colors[i]);

        var idx = new int[indices.Count];
        for (var i = 0; i < idx.Length; i++)
            idx[i] = indices[i];

        RenderMesh rm = new(rv, idx);
        rm.ComputeSmoothedNormalsInPlace();

        return rm;
    }

    private Outline _computeBoundingBox()
    {
        var v = Vertices;
        Debug.Assert(v.Length > 0);

        double minX = v[0].PX; double minY = v[0].PY; double minZ = v[0].PZ;
        var maxX = minX; var maxY = minY; var maxZ = minZ;

        for (var i = 1; i < v.Length; i++)
        {
            double x = v[i].PX; double y = v[i].PY; double z = v[i].PZ;
            if (x < minX) minX = x; if (y < minY) minY = y; if (z < minZ) minZ = z;
            if (x > maxX) maxX = x; if (y > maxY) maxY = y; if (z > maxZ) maxZ = z;
        }

        const double eps = 1e-6;
        if (maxX - minX < eps) { maxX += eps; minX -= eps; }
        if (maxY - minY < eps) { maxY += eps; minY -= eps; }
        if (maxZ - minZ < eps) { maxZ += eps; minZ -= eps; }

        return new Outline(new XYZ(minX, minY, minZ), new XYZ(maxX, maxY, maxZ));
    }

    public void ComputeSmoothedNormalsInPlace(bool areaWeighted = true, float degenerateEps = 1e-10f)
    {
        Debug.Assert(_vertices.Length > 0);
        Debug.Assert(_indices.Length % 3 == 0);

        var vertexCount = _vertices.Length;
        var acc = new Vector3[vertexCount];

        for (var i = 0; i < _indices.Length; i += 3)
        {
            var i0 = _indices[i];
            var i1 = _indices[i + 1];
            var i2 = _indices[i + 2];

            if (i0 == i1 || i1 == i2 || i2 == i0)
                continue;

            var p0 = _vertices[i0].Position;
            var p1 = _vertices[i1].Position;
            var p2 = _vertices[i2].Position;

            var n = Vector3.Cross(p1 - p0, p2 - p0);
            var lenSq = n.LengthSquared();

            if (lenSq <= degenerateEps)
                continue;

            if (!areaWeighted)
                n *= 1.0f / MathF.Sqrt(lenSq);

            acc[i0] += n;
            acc[i1] += n;
            acc[i2] += n;
        }

        for (var i = 0; i < vertexCount; i++)
        {
            var n = acc[i];
            var lenSq = n.LengthSquared();

            n = lenSq > degenerateEps
                ? n * (1.0f / MathF.Sqrt(lenSq))
                : new Vector3(0, 0, 1);

            var v = _vertices[i];
            _vertices[i] = new RenderVertex(v.Position, n, v.UV, v.RGBA);
        }
    }

    public void ApplyBoundingBoxGradient(bool enhanceContrast = true)
    {
        Debug.Assert(_vertices.Length > 0);

        float minX = _vertices[0].PX, maxX = _vertices[0].PX;
        float minY = _vertices[0].PY, maxY = _vertices[0].PY;
        float minZ = _vertices[0].PZ, maxZ = _vertices[0].PZ;

        for (var i = 1; i < _vertices.Length; i++)
        {
            var v = _vertices[i];
            if (v.PX < minX) minX = v.PX;
            if (v.PX > maxX) maxX = v.PX;
            if (v.PY < minY) minY = v.PY;
            if (v.PY > maxY) maxY = v.PY;
            if (v.PZ < minZ) minZ = v.PZ;
            if (v.PZ > maxZ) maxZ = v.PZ;
        }

        var dx = maxX - minX;
        var dy = maxY - minY;
        var dz = maxZ - minZ;

        const float eps = 1e-12f;
        if (dx < eps) dx = 1;
        if (dy < eps) dy = 1;
        if (dz < eps) dz = 1;

        for (var i = 0; i < _vertices.Length; i++)
        {
            var v = _vertices[i];
            var nx = (v.PX - minX) / dx;
            var ny = (v.PY - minY) / dy;
            var nz = (v.PZ - minZ) / dz;

            if (enhanceContrast)
            {
                nx = MathF.Pow(nx, 0.8f);
                ny = MathF.Pow(ny, 0.8f);
                nz = MathF.Pow(nz, 0.8f);
            }

            var r = (byte)(Math.Clamp(nx, 0f, 1f) * 255f);
            var g = (byte)(Math.Clamp(ny, 0f, 1f) * 255f);
            var b = (byte)(Math.Clamp(nz, 0f, 1f) * 255f);
            _vertices[i] = new RenderVertex(v.Position, v.Normal, v.UV, new Color32(r, g, b, 255));
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct RenderVertex
{
    public RenderVertex(Vector3 position, Vector3 normal, Vector2 uv, Color32 color)
    {
        PX = position.X; PY = position.Y; PZ = position.Z;
        NX = normal.X; NY = normal.Y; NZ = normal.Z;
        U = uv.X; V = uv.Y;
        RGBA = color;
    }

    public Vector3 Position => new(PX, PY, PZ);
    public Vector3 Normal => new(NX, NY, NZ);
    public Vector2 UV => new(U, V);

    public readonly float PX, PY, PZ;
    public readonly float NX, NY, NZ;
    public readonly float U, V;
    public readonly Color32 RGBA;

    public VertexPositionNormalColored ToRevit()
    {
        return new(
                new XYZ(PX, PY, PZ),
                new XYZ(NX, NY, NZ),
                new ColorWithTransparency(RGBA.R, RGBA.G, RGBA.B, (byte)(255 - RGBA.A))
            );
    }
}
