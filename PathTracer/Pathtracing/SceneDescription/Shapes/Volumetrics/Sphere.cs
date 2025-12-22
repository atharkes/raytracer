using PathTracer.Geometry.Positions;

namespace PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;

/// <summary> A perfect sphere <see cref="IShape"/> </summary>
public readonly struct Sphere : ISphere, IEquatable<Sphere> {
    /// <summary> Position of the <see cref="Sphere"/> </summary>
    public Position3 Position { get; }
    /// <summary> The radius of the <see cref="Sphere"/> </summary>
    public float Radius { get; }

    /// <summary> Create a new <see cref="Sphere"/> </summary>
    /// <param name="position">The position of the <see cref="Sphere"/></param>
    /// <param name="radius">The radius of the <see cref="Sphere"/></param>
    /// <param name="material">The material of the <see cref="Sphere"/></param>
    public Sphere(Position3 position, float radius = 1) {
        Position = position;
        Radius = radius;
    }

    public static bool operator ==(Sphere left, Sphere right) => left.Equals(right);
    public static bool operator !=(Sphere left, Sphere right) => !(left == right);

    public override int GetHashCode() => HashCode.Combine(Position.GetHashCode(), Radius.GetHashCode());
    public override bool Equals(object? obj) => obj is Sphere sphere && Equals(sphere);
    public bool Equals(Sphere sphere) => Position.Equals(sphere.Position) && Radius.Equals(sphere.Radius);
}