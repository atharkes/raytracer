using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Geometry.Directions;

/// <summary> A 2-dimensional direction vector </summary>
public interface IDirection2 : IDirection<Vector2>, IEquatable<IDirection2> {
    /// <summary> The X-coordinate of the <see cref="IDirection2"/> </summary>
    Direction1 X => Vector.X;
    /// <summary> The Y-coordinate of the <see cref="IDirection2"/> </summary>
    Direction1 Y => Vector.Y;

    static Direction2 operator +(IDirection2 left, IDirection2 right) => left.Vector + right.Vector;
    static Direction2 operator -(IDirection2 direction) => -direction.Vector;
    static Direction2 operator -(IDirection2 left, IDirection2 right) => left.Vector - right.Vector;
    static Direction2 operator *(IDirection2 direction, Position1 scale) => direction.Vector * scale.Vector;
    static Direction2 operator *(IDirection2 direction, Position2 scale) => direction.Vector * scale.Vector;
    static Direction2 operator /(IDirection2 direction, Position1 scale) => direction.Vector / scale.Vector;
    static Direction2 operator /(Position1 scale, IDirection2 direction) => scale.Vector / direction.Vector;
    static Direction2 operator /(IDirection2 direction, Position2 scale) => direction.Vector / scale.Vector;

    static Direction1 Dot(IDirection2 left, IDirection2 right) => Vector2.Dot(left.Vector, right.Vector);
    static Direction2 Abs(IDirection2 direction) => Vector2.Abs(direction.Vector);

    bool Equals(object? obj) => obj is IDirection2 direction && Equals(direction);
    bool IEquatable<IDirection2>.Equals(IDirection2? other) => Vector.Equals(other?.Vector);
    int GetHashCode() => Vector.GetHashCode();

    bool Similar(IDirection2 other) => Dot(this, other) > Direction1.Zero;
    Normal2 Normalized() => new(Vector);
}
