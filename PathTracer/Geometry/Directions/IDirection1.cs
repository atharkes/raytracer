using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;

namespace PathTracer.Geometry.Directions;

/// <summary> A 1-dimensional direction vector </summary>
public interface IDirection1 : IDirection<Vector1>, IComparable<IDirection1>, IEquatable<IDirection1> {
    static bool operator <=(IDirection1 left, IDirection1 right) => left.CompareTo(right) <= 0;
    static bool operator >=(IDirection1 left, IDirection1 right) => left.CompareTo(right) >= 0;
    static bool operator <(IDirection1 left, IDirection1 right) => left.CompareTo(right) < 0;
    static bool operator >(IDirection1 left, IDirection1 right) => left.CompareTo(right) > 0;

    static Direction1 operator +(IDirection1 left, IDirection1 right) => left.Vector + right.Vector;
    static Direction1 operator -(IDirection1 direction) => -direction.Vector;
    static Direction1 operator -(IDirection1 left, IDirection1 right) => left.Vector - right.Vector;
    static Direction1 operator *(IDirection1 direction, Position1 scale) => direction.Vector * scale.Vector;
    static Direction1 operator /(IDirection1 direction, Position1 scale) => direction.Vector / scale.Vector;

    static Position1 operator *(IDirection1 left, IDirection1 right) => left.Vector * right.Vector;
    static Position1 operator /(IDirection1 left, IDirection1 right) => left.Vector / right.Vector;

    static Direction1 Dot(IDirection1 left, IDirection1 right) => new(left.Vector * right.Vector);
    static Direction1 Abs(IDirection1 direction) => Vector1.Abs(direction.Vector);

    bool Equals(object? obj) => obj is IDirection1 direction && Equals(direction);
    bool IEquatable<IDirection1>.Equals(IDirection1? other) => Vector.Equals(other?.Vector);
    int GetHashCode() => Vector.GetHashCode();
    int IComparable<IDirection1>.CompareTo(IDirection1? other) => Vector.CompareTo(other?.Vector);

    bool Similar(IDirection1 other) => Dot(this, other) > Direction1.Zero;
    Normal1 Normalized() => new(Vector);
}
