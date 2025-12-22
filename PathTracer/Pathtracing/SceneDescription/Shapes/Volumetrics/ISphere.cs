using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Rays;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

/// <summary> A sphere </summary>
public interface ISphere : IVolumetricShape, IEquatable<ISphere> {
    /// <summary> Position of the <see cref="ISphere"/> </summary>
    Position3 Position { get; }
    /// <summary> The radius of the <see cref="ISphere"/> </summary>
    float Radius { get; }
    /// <summary> The volume of the <see cref="ISphere"/> </summary>
    float IShape.Volume => 4f / 3f * (float)Math.PI * Radius * Radius * Radius;
    /// <summary> The surface area of the <see cref="ISphere"/> </summary>
    float IShape.SurfaceArea => 4f * (float)Math.PI * Radius * Radius;
    /// <summary> The bounding box of the <see cref="ISphere"/> </summary>
    AxisAlignedBox IBoundable.BoundingBox => new(Position - new Direction3(Radius), Position + new Direction3(Radius));

    /// <summary> Convert the <see cref="ISphere"/> into an <see cref="int"/> hashcode </summary>
    /// <returns>The <see cref="int"/> hashcode of the <see cref="ISphere"/></returns>
    int GetHashCode() => HashCode.Combine(Position.GetHashCode(), Radius.GetHashCode());

    /// <summary> Check whether the <paramref name="obj"/> is equal to the <see cref="ISphere"/> </summary>
    /// <param name="obj">The <see cref="object"/></param>
    /// <returns>Whether the <paramref name="obj"/> is equal to the <see cref="ISphere"/></returns>
    bool Equals(object? obj) => obj is ISphere sphere && Equals(sphere);

    /// <summary> Check whether the <paramref name="sphere"/> is equal to the <see cref="ISphere"/> </summary>
    /// <param name="sphere">The other <see cref="ISphere"/></param>
    /// <returns>Whether the <paramref name="sphere"/> is equal to the <see cref="ISphere"/></returns>
    bool IEquatable<ISphere>.Equals(ISphere? sphere) => Position.Equals(sphere?.Position) && Radius.Equals(sphere?.Radius);

    /// <summary> Get a <paramref name="random"/> point on the surface of the <see cref="ISphere"/> </summary>
    /// <param name="random">The <see cref="Random"/> to decide the position on the surface</param>
    /// <returns>A <paramref name="random"/> point on the surface of the <see cref="ISphere"/></returns>
    Position3 IShape.SurfacePosition(Random random) {
        var z = 1 - 2 * random.NextDouble();
        var r = Math.Sqrt(Math.Max(0, 1 - z * z));
        var phi = 2 * Math.PI * random.NextDouble();
        Direction3 direction = new Normal3((float)(2 * Math.Cos(phi) * r), (float)(2 * Math.Sin(phi) * r), (float)z);
        return Position + direction * Radius;
    }

    /// <summary> Get the UV-position for a specified <paramref name="position"/> </summary>
    /// <param name="position">The surface position for which to get the UV-position</param>
    /// <returns>The UV-position for the <paramref name="position"/></returns>
    Position2 IShape.UVPosition(Position3 position) => throw new NotImplementedException();

    /// <summary> Get the distance to the surface of the <see cref="ISphere"/> from the specified <paramref name="position"/> </summary>
    /// <param name="position">The position to get the distance from the surface for</param>
    /// <returns>The distance to the surface of the <see cref="ISphere"/> from the specified <paramref name="position"/></returns>
    float IShape.DistanceToSurface(Position3 position) => (position - Position).Length - Radius;

    /// <summary> Check whether a <paramref name="position"/> is on the surface of the <see cref="ISphere"/> </summary>
    /// <param name="position">The position to check</param>
    /// <param name="epsilon">The epsilon to specify the precision</param>
    /// <returns>Whether the <paramref name="position"/> is on the surface of the <see cref="ISphere"/></returns>
    bool IShape.OnSurface(Position3 position, float epsilon) => (position - Position).Length.Equals(Radius, epsilon);

    /// <summary> Get the normal of the <see cref="Sphere"/> at a specified <paramref name="position"/> </summary>
    /// <param name="position">The surface point to get the normal for</param>
    /// <returns>The normal at the <paramref name="position"/></returns>
    Normal3 IShape.SurfaceNormal(Position3 position) => OutwardsDirection(position);

    /// <summary> Get the outwards direction for a specified <paramref name="position"/> </summary>
    /// <param name="position">The position to get the outwards direction from</param>
    /// <returns>The outwards direction at the specified <paramref name="position"/></returns>
    Normal3 IShape.OutwardsDirection(Position3 position) => (position - Position).Normalized();

    /// <summary> Intersect the <see cref="ISphere"/> with a <paramref name="ray"/> </summary>
    /// <param name="ray">The <see cref="IRay"/> to intersect the <see cref="ISphere"/> with</param>
    /// <returns>Whether and when the <see cref="IRay"/>intersects the <see cref="ISphere"/></returns>
    ReadOnlySpan<Position1> IIntersectable.IntersectDistances(IRay ray) {
        var rayOriginToSpherePosition = Position - ray.Origin;
        float sphereInDirectionOfRay = IDirection3.Dot(rayOriginToSpherePosition, ray.Direction);
        float rayNormalDistance = rayOriginToSpherePosition.LengthSquared - sphereInDirectionOfRay * sphereInDirectionOfRay;
        if (rayNormalDistance > Radius * Radius) return [];

        var raySphereDistance = (float)Math.Sqrt(Radius * Radius - rayNormalDistance);
        return new Position1[] { sphereInDirectionOfRay - raySphereDistance, sphereInDirectionOfRay + raySphereDistance };
    }
}
