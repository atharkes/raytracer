using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Geometry.Directions;

/// <summary> A 3-dimensional direction vector </summary>
public interface IDirection3 : IDirection<Vector3>, IEquatable<IDirection3> {
    /// <summary> The X-coordinate of the <see cref="IDirection3"/> </summary>
    Direction1 X => Vector.X;
    /// <summary> The Y-coordinate of the <see cref="IDirection3"/> </summary>
    Direction1 Y => Vector.Y;
    /// <summary> The Z-coordinate of the <see cref="IDirection3"/> </summary>
    Direction1 Z => Vector.Z;

    static Direction3 operator +(IDirection3 left, IDirection3 right) => left.Vector + right.Vector;
    static Direction3 operator -(IDirection3 direction) => -direction.Vector;
    static Direction3 operator -(IDirection3 left, IDirection3 right) => left.Vector - right.Vector;
    static Direction3 operator *(IDirection3 direction, Position1 scale) => direction.Vector * scale.Vector;
    static Direction3 operator *(IDirection3 direction, Position3 scale) => direction.Vector * scale.Vector;
    static Direction3 operator /(IDirection3 direction, Position1 scale) => direction.Vector / scale.Vector;
    static Direction3 operator /(Position1 scale, IDirection3 direction) => scale.Vector / direction.Vector;
    static Direction3 operator /(IDirection3 direction, Position3 scale) => direction.Vector / scale.Vector;

    static Vector1 Dot<TLeft, TRight>(TLeft left, TRight right) where TLeft : IDirection3 where TRight : IDirection3 => Vector3.Dot(left.Vector, right.Vector);
    static bool InSameClosedHemisphere<TLeft, TRight>(TLeft left, TRight right) where TLeft : IDirection3 where TRight : IDirection3 => Dot(left, right) >= 0;
    static bool InSameOpenHemisphere<TLeft, TRight>(TLeft left, TRight right) where TLeft : IDirection3 where TRight : IDirection3 => Dot(left, right) > 0;
    static Direction3 Abs<TDirection>(TDirection direction) where TDirection : IDirection3 => Vector3.Abs(direction.Vector);

    bool Equals(object? obj) => obj is IDirection3 direction && Equals(direction);
    bool IEquatable<IDirection3>.Equals(IDirection3? other) => Vector.Equals(other?.Vector);
    int GetHashCode() => Vector.GetHashCode();
    Normal3 Normalized() => new(Vector);
}
