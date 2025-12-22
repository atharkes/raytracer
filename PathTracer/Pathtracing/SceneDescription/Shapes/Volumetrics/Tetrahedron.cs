using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

public readonly struct Tetrahedron : IVolumetricShape {
    public float Volume => throw new NotImplementedException();
    public float SurfaceArea => throw new NotImplementedException();
    public AxisAlignedBox BoundingBox => new(vertices);

    private readonly Position3[] vertices;
    private readonly Triangle[] triangles;

    public static Tetrahedron Regular(Position3 position, float size) {
        var v1 = position + new Direction3(-size / (float)Math.Sqrt(3), 0, size);
        var v2 = position + new Direction3(-size / (float)Math.Sqrt(3), 0, -size);
        var v3 = position + new Direction3(size * 2f / (float)Math.Sqrt(3f), 0, 0);
        var v4 = position + new Direction3(0, size * 4f / (float)Math.Sqrt(6f), 0);
        return new Tetrahedron(v1, v2, v3, v4);
    }

    public Tetrahedron(Position3 v1, Position3 v2, Position3 v3, Position3 v4) {
        vertices = new Position3[] { v1, v2, v3, v4 };
        triangles = new Triangle[] {
            new(v1, v2, v3),
            new(v1, v4, v2),
            new(v2, v4, v3),
            new(v3, v4, v1),
        };
    }

    public IEnumerable<Position1> IntersectDistances(IRay ray) {
        foreach (var triangle in triangles) {
            if (triangle.IntersectDistance(ray) is Position1 distance) {
                yield return distance;
            }
        }
    }

    public bool OnSurface(Position3 position, float epsilon = 0.0001F) => triangles.Any(t => t.OnSurface(position, epsilon));

    public float DistanceToSurface(Position3 position) => triangles.Min(t => t.DistanceToSurface(position));

    public Normal3 OutwardsDirection(Position3 position) {
        var closestTriangle = triangles.MinBy(t => t.DistanceToSurface(position));
        return (closestTriangle as IShape).OutwardsDirection(position);
    }

    public Normal3 SurfaceNormal(Position3 position) => OutwardsDirection(position);

    public Position3 SurfacePosition(Random random) => throw new NotImplementedException();

    public Position2 UVPosition(Position3 position) => throw new NotImplementedException();
}
