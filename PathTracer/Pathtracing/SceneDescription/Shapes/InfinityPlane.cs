using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

namespace PathTracer.Pathtracing.SceneDescription.Shapes;

public readonly struct InfinityPlane : IShape, IEquatable<InfinityPlane> {
    public bool Volumetric => false;
    public float Volume => 0;
    public float SurfaceArea => float.PositiveInfinity;
    public AxisAlignedBox BoundingBox => new(Position3.MinValue, Position3.MaxValue);

    public static bool operator ==(InfinityPlane left, InfinityPlane right) => left.Equals(right);
    public static bool operator !=(InfinityPlane left, InfinityPlane right) => !(left == right);

    public override int GetHashCode() => 399527027;
    public override bool Equals(object? obj) => obj is InfinityPlane infPlane && Equals(infPlane);
    public bool Equals(InfinityPlane other) => true;

    public readonly ReadOnlySpan<Position1> IntersectDistances(IRay ray) => new Position1[] { Position1.MinValue, Position1.MaxValue };

    public bool OnSurface(Position3 position, float epsilon = 0.001F) => throw new NotImplementedException();

    public Normal3 OutwardsDirection(Position3 position) => SurfaceNormal(position);

    public Normal3 SurfaceNormal(Position3 position) => new(-((Direction3)position / Vector1.MaxValue));

    public Position3 SurfacePosition(Random random) => throw new NotImplementedException();

    public Position2 UVPosition(Position3 position) => throw new NotImplementedException("Directional information is required for uv mapping");

    public float DistanceToSurface(Position3 position) => float.PositiveInfinity;
}
